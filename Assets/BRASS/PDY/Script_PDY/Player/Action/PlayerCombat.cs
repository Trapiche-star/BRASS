using UnityEngine;

namespace BRASS
{
    /// 단일 3타 공격 콤보 시스템을 제어하며 애니메이션 이벤트와 연동하여 공격 판정 및 이동 제한을 관리하는 클래스
    public class PlayerCombat : MonoBehaviour
    {
        #region Variables
        [SerializeField] private float comboInputWindow = 1.0f; // 다음 연계 공격 입력을 유효하게 인정하는 시간 범위
        [SerializeField] private PlayerAnimationController animationController; // 공격 애니메이션 재생 제어 컴포넌트
        [SerializeField] private PlayerState state; // 플레이어의 현재 상태 플래그 데이터 참조
        [SerializeField] private PlayerController playerController; // 외부 물리 이동 명령을 전달할 컨트롤러

        [SerializeField] private WeaponDamage currentWeapon; // 현재 장착 중인 무기의 데미지 판정 컴포넌트

        [Header("Combo Step Move")]
        [SerializeField] private float[] comboStepDistances = { 0.15f, 0.2f, 0.25f }; // 각 콤보 단계별 전진 이동 거리

        [Header("Bare Hand Attack (Gizmo)")]
        [SerializeField] private float bareAttackDamage = 10f; // 무기 미장착 시 적용되는 기본 공격 데미지
        [SerializeField] private float bareAttackDistance = 2.2f; // 맨손 공격이 도달하는 최대 수평 거리
        [SerializeField] private float bareAttackAngle = 45f; // 맨손 공격 판정이 발생하는 부채꼴 각도
        [SerializeField] private float bareAttackRadius = 2.2f; // 탐색을 위한 오버랩 스피어의 기본 반경
        [SerializeField] private LayerMask damageLayer; // 공격 판정을 수행할 대상 레이어 마스크

        private Vector3 cachedAttackDirection; // 공격 시작 시 확정된 전방 방향 벡터
        private int attackInputCount; // 현재까지 누적된 콤보 입력 횟수
        private float lastAttackInputTime; // 마지막으로 공격 버튼을 누른 시점의 시간
        private bool isAttackSequenceActive; // 현재 공격 시퀀스가 진행 중인지 여부
        #endregion

        #region Property
        public bool IsAttackSequenceActive => isAttackSequenceActive; // 외부에서 공격 시퀀스 진행 여부 참조
        #endregion

        #region Unity Event Method
        private void Awake()
        {
            if (state == null) state = GetComponentInParent<PlayerState>(); // 부모 객체에서 상태 데이터 참조 할당
            if (playerController == null) playerController = GetComponentInParent<PlayerController>(); // 부모 객체에서 컨트롤러 참조 할당
        }

        private void LateUpdate()
        {
            if (!isAttackSequenceActive) return; // 공격 시퀀스 중이 아니라면 회전 로직을 수행하지 않는다
            if (cachedAttackDirection == Vector3.zero) return; // 저장된 공격 방향이 유효하지 않으면 무시한다

            transform.root.rotation = Quaternion.LookRotation(cachedAttackDirection);   // 매 프레임 저장된 방향으로 회전 갱신
        }
        #endregion

        #region Custom Method
        // 외부 시스템으로부터 현재 활성화된 무기 정보를 갱신받음
        public void SetCurrentWeapon(WeaponDamage newWeapon)
        {
            currentWeapon = newWeapon; // 새로운 무기 데미지 컴포넌트를 캐싱한다
        }

        // 애니메이션 이벤트: 무기 또는 맨손의 공격 판정 활성화 시점
        public void OnAttackHitStart()
        {
            if (currentWeapon != null) // 장착된 무기가 있다면
            {
                currentWeapon.StartAttack(); // 무기 컴포넌트의 공격 판정을 시작한다
            }
            else // 무기가 없는 맨손 상태라면
            {
                DealBareHandAttack(); // 즉시 맨손 공격 판정을 수행한다
            }
        }

        // 애니메이션 이벤트: 모든 공격 판정 비활성화 시점
        public void OnAttackHitEnd()
        {
            if (currentWeapon != null) currentWeapon.StopAttack(); // 무기 컴포넌트의 판정을 정지시킨다
        }

        // 공격 버튼 입력 시 호출되는 메인 로직
        public void OnBasicAttackStarted()
        {
            float now = Time.time; // 현재 시스템 시간을 측정한다

            if (isAttackSequenceActive) // 이미 공격 시퀀스가 진행 중이라면
            {
                if (now - lastAttackInputTime <= comboInputWindow) // 콤보 유효 시간 이내에 재입력되었다면
                {
                    attackInputCount++; // 다음 콤보를 위해 입력 횟수를 증가시킨다
                    lastAttackInputTime = now; // 마지막 입력 시간을 갱신한다
                }
                return; // 추가 처리를 방지하기 위해 메서드를 나간다
            }

            state.IsMoving = false; // 공격을 시작하므로 이동 상태를 해제한다
            StartAttackSequence(now); // 새로운 공격 시퀀스를 시작한다
        }

        // 애니메이션 특정 시점에서 다음 콤보 진행 여부를 판단
        public bool OnComboSectionReached(int sectionIndex)
        {
            int requiredInput = sectionIndex + 1; // 다음 단계로 넘어가기 위해 필요한 최소 입력 수

            if (attackInputCount < requiredInput) // 필요한 입력 횟수를 채우지 못했다면
            {
                ForceEndAttack(); // 공격 시퀀스를 강제 종료한다
                return false;
            }

            if (Time.time - lastAttackInputTime > comboInputWindow) // 입력 유효 시간이 만료되었다면
            {
                ForceEndAttack(); // 공격 시퀀스를 강제 종료한다
                return false;
            }

            return true; // 모든 조건을 통과하면 다음 콤보 애니메이션 진행을 허용한다
        }

        // 애니메이션 이벤트: 전체 콤보 시퀀스가 끝났을 때 호출
        public void OnComboAnimationFinished()
        {
            ForceEndAttack(); // 공격 데이터를 초기화하고 제어권을 복구한다
        }

        // 외부(피격 등)로부터 공격 시퀀스 취소 요청 시 호출
        public void CancelAttack()
        {
            ForceEndAttack(); // 즉시 모든 공격 상태를 해제한다
        }

        // 공격 시퀀스를 초기화하고 애니메이션을 재생함
        private void StartAttackSequence(float now)
        {
            // 타겟이 있을 때만 방향 스냅 회전
            if (state.CurrentTarget != null)    // 타겟이 존재한다면
            {
                Vector3 toTarget = state.CurrentTarget.position - transform.root.position;      // 타겟까지의 벡터 계산

                toTarget.y = 0f;    // 수평 회전만 고려

                // 회전할 방향이 유효할 때만 회전 수행
                if (toTarget.sqrMagnitude > 0.001f) // 거리가 너무 가까우면 회전하지 않는다
                {
                    Quaternion targetRot = Quaternion.LookRotation(toTarget.normalized);    // 타겟 방향으로 회전 계산
                    transform.root.rotation = targetRot; // 즉시 타겟 방향으로 회전 적용 
                }

                state.IsEngagedWithTarget = true;   // 타겟과 교전 중 상태 설정
            }

            state.IsAttacking = true;   // 공격 상태 플래그 설정
            state.IsInputMovementLocked = true;     // 이동 입력 잠금 설정

            isAttackSequenceActive = true;   // 공격 시퀀스 활성화
            attackInputCount = 1;   // 첫 번째 공격 입력으로 초기화
            lastAttackInputTime = now;  // 마지막 입력 시간 기록

            CacheAttackDirection();          // 회전 후 방향 캐싱
            animationController?.PlayAttack();  // 공격 애니메이션 재생 명령 전달
        }

        // 모든 공격 관련 상태와 플래그를 초기값으로 복구
        private void ForceEndAttack()
        {
            isAttackSequenceActive = false; // 시퀀스 비활성화
            attackInputCount = 0; // 입력 횟수 초기화
            cachedAttackDirection = Vector3.zero; // 저장된 방향 벡터 초기화
            lastAttackInputTime = 0f; // 마지막 입력 시간 초기화

            if (state != null) // 상태 데이터가 유효하다면
            {
                state.IsAttacking = false; // 공격 상태 해제
                state.IsInputMovementLocked = false; // 이동 잠금 해제
            }

            animationController?.StopAttack(); // 애니메이션 시스템에 정지 신호 전달
        }

        // 현재 캐릭터의 전방 방향을 수평 벡터로 추출하여 저장
        private void CacheAttackDirection()
        {
            Vector3 forward = transform.root.forward; // ⭐ 루트 기준 전방 사용
            forward.y = 0f;

            cachedAttackDirection =
                forward.sqrMagnitude < 0.01f ? Vector3.zero : forward.normalized;
        }

        // 애니메이션 이벤트: 콤보 단계별 전진 이동 명령 수행
        public void ApplyComboStep(int comboIndex)
        {
            if (playerController == null) return; // 컨트롤러가 없으면 이동 처리를 수행하지 않는다
            if (comboIndex < 0 || comboIndex >= comboStepDistances.Length) return; // 인덱스 범위를 벗어나면 무시한다
            if (cachedAttackDirection == Vector3.zero) return; // 저장된 이동 방향이 없으면 처리를 중단한다

            Vector3 delta = cachedAttackDirection * comboStepDistances[comboIndex]; // 방향과 정의된 거리를 곱해 이동량 계산
            playerController.MoveExternal(delta); // 플레이어 컨트롤러를 통해 물리 이동 적용
        }

        // 맨손 상태일 때 전방의 적에게 구체/부채꼴 판정 데미지 적용
        private void DealBareHandAttack()
        {
            Vector3 origin = transform.root.position; // 판정 시작점을 발밑 위치로 설정
            Collider[] hits = Physics.OverlapSphere(origin, bareAttackRadius, damageLayer); // 주변 반경 내 적 콜라이더 수집

            foreach (Collider hit in hits) // 수집된 모든 개체 순회
            {
                if (hit.transform.root == transform.root) continue; // 자기 자신은 타격 대상에서 제외한다
                if (!IsInBareAttackArea(hit.transform.position)) continue; // 부채꼴 영역 밖에 있다면 무시한다

                IDamageable target = hit.GetComponentInParent<IDamageable>(); // 데미지 인터페이스 추출
                if (target == null) continue; // 인터페이스가 없는 대상이라면 다음으로 건너뛴다

                target.TakeDamage(bareAttackDamage); // 정의된 맨손 데미지를 입힌다
            }
        }

        // 대상 좌표가 플레이어 정면 부채꼴 판정 내에 있는지 확인
        private bool IsInBareAttackArea(Vector3 worldPos)
        {
            Vector3 toTarget = worldPos - transform.root.position; // 원점에서 대상까지의 벡터 계산
            toTarget.y = 0f; // 수평 판정만 고려

            if (toTarget.magnitude > bareAttackDistance) return false; // 최대 도달 거리를 초과하면 거짓 반환

            float angle = Vector3.Angle(transform.root.forward, toTarget); // 캐릭터 정면 방향과의 각도 비교
            return angle <= bareAttackAngle; // 각도가 허용치 이내라면 참을 반환한다
        }
        #endregion

#if UNITY_EDITOR
        // 에디터 뷰에서 맨손 공격 판정 영역을 시각화
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.cyan; // 기즈모 색상을 청록색으로 설정

            Transform root = transform.root;
            if (root == null) return; // 최상단 루트가 없으면 그리기를 포기한다

            Vector3 origin = root.position; // 원점 설정
            Vector3 left = Quaternion.Euler(0, -bareAttackAngle, 0) * root.forward; // 왼쪽 경계 벡터 연산
            Vector3 right = Quaternion.Euler(0, bareAttackAngle, 0) * root.forward; // 오른쪽 경계 벡터 연산

            Gizmos.DrawLine(origin, origin + left * bareAttackDistance); // 왼쪽 각도 라인
            Gizmos.DrawLine(origin, origin + right * bareAttackDistance); // 오른쪽 각도 라인
            Gizmos.DrawWireSphere(origin, bareAttackDistance); // 전체 사거리 가이드라인
        }
#endif
    }
}
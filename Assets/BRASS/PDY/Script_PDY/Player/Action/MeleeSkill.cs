using UnityEngine;
using System.Collections;

namespace BRASS
{
    /// <summary>
    /// 플레이어의 근접 스킬 입력을 처리하고 애니메이션 이벤트 타이밍에 맞춰 공격 판정을 수행하는 클래스
    /// </summary>
    public class MeleeSkill : MonoBehaviour
    {
        #region Variables
        [Header("References")]
        [SerializeField] private PlayerState state; // 플레이어의 현재 동작 상태 데이터 참조
        [SerializeField] private PlayerAnimationController animController; // 애니메이션 재생 제어 컴포넌트
        private Animator animator; // 실제 애니메이션 트리거를 전달할 컴포넌트

        [Header("Skill 3 Movement")]
        [SerializeField] private float skill3Speed = 12f; // 스킬 3 사용 시 전진 이동 속도
        private bool isSkill3Moving; // 스킬 3에 의한 강제 이동 활성화 여부

        [Header("Skill Damage - Bare Hand")]
        [SerializeField] private float bareSkill1Damage = 15f; // 맨손 스킬 1 데미지
        [SerializeField] private float bareSkill2Damage = 20f; // 맨손 스킬 2 데미지
        [SerializeField] private float bareSkill3Damage = 25f; // 맨손 스킬 3 데미지

        [Header("Skill Damage - BattleAxe")]
        [SerializeField] private float axeSkill1Damage = 40f; // 도끼 스킬 1 데미지
        [SerializeField] private float axeSkill2Damage = 60f; // 도끼 스킬 2 데미지
        [SerializeField] private float axeSkill3Damage = 80f; // 도끼 스킬 3 데미지

        [Header("Attack Hit Area")]
        [SerializeField] private float attackHitDistance = 2.5f; // 공격 판정이 유효한 최대 거리
        [SerializeField] private float attackHitAngle = 45f; // 공격 판정이 유효한 전방 시야 각도

        [Header("Hit Detection")]
        [SerializeField] private float skillRange = 2.5f; // 오버랩 스피어 탐색 반경
        [SerializeField] private LayerMask damageLayer; // 공격 대상을 필터링할 레이어 마스크
        #endregion

        #region Unity Event Method
        private void Awake()
        {
            if (state == null) state = GetComponentInParent<PlayerState>(); // 부모 객체에서 상태 스크립트 탐색 및 할당
            if (animController == null) animController = GetComponentInParent<PlayerAnimationController>(); // 부모에서 애니메이션 컨트롤러 탐색
            animator = animController.GetComponentInChildren<Animator>(); // 하위 오브젝트에서 애니메이터 컴포넌트 참조
        }

        private void Update()
        {
            UpdateSkill3Movement(); // 매 프레임 스킬 3의 물리적 전진 이동 업데이트
        }
        #endregion

        #region Custom Method
        // 스킬 1 실행 요청 및 초기 설정
        public void ExecuteSkill01()
        {
            if (!CanUseSkill()) return; // 사용 불가능한 상태라면 메서드를 종료한다
            BeginSkill(); // 공격 시작 상태 설정 및 타겟 조준
            animator.SetTrigger("Skill_1"); // 애니메이터의 스킬 1 트리거 활성화
        }

        // 스킬 2 실행 요청 및 초기 설정
        public void ExecuteSkill02()
        {
            if (!CanUseSkill()) return; // 현재 공격 중이거나 공중에 있으면 차단한다
            BeginSkill(); // 공격 시작 공통 로직 실행
            animator.SetTrigger("Skill_2"); // 애니메이터의 스킬 2 트리거 활성화
        }

        // 스킬 3 실행 요청 및 초기 설정
        public void ExecuteSkill03()
        {
            if (!CanUseSkill()) return; // 스킬 사용 조건을 만족하는지 검사한다
            BeginSkill(); // 공격 상태 플래그 갱신 및 회전 처리
            animator.SetTrigger("Skill_3"); // 애니메이터의 스킬 3 트리거 활성화
        }

        // 스킬 실행 시 호출되는 공통 상태 설정 로직
        private void BeginSkill()
        {
            state.IsAttacking = true; // 플레이어를 공격 상태로 전환한다
            state.IsInputMovementLocked = true; // 이동 입력을 잠궈 공격 동작을 방해하지 않게 한다
            FaceTargetIfExists(); // 타겟이 있다면 즉시 그 방향을 바라보게 한다
        }

        // 스킬 3의 물리적 전진 이동 처리
        private void UpdateSkill3Movement()
        {
            if (!isSkill3Moving) return; // 이동 플래그가 거짓이면 물리 처리를 수행하지 않는다

            CharacterController cc = GetComponentInParent<CharacterController>(); // 이동 처리를 위한 컨트롤러 참조
            if (cc == null) return; // 컨트롤러가 존재하지 않으면 이동을 취소한다

            Vector3 dir = transform.root.forward; // 루트의 전방 방향 벡터 획득
            dir.y = 0f; // 수직 이동을 방지하기 위해 Y축 제거
            cc.Move(dir.normalized * skill3Speed * Time.deltaTime); // 일정한 속도로 전방 이동 적용
        }

        // 애니메이션 이벤트: 스킬 3 이동 시작 시점
        public void OnSkill3MoveStart()
        {
            if (state != null && state.IsAttacking) isSkill3Moving = true; // 공격 중일 때만 전진 이동을 활성화한다
        }

        // 애니메이션 이벤트: 스킬 3 이동 종료 시점
        public void OnSkill3MoveEnd()
        {
            isSkill3Moving = false; // 전진 이동 플래그를 해제한다
        }

        // 애니메이션 이벤트: 스킬 동작이 완전히 종료될 때 호출
        public void OnSkillAttackEnd()
        {
            if (state == null) return; // 상태 참조가 없으면 종료 처리를 수행하지 않는다
            state.IsAttacking = false; // 공격 상태를 해제한다
            state.IsInputMovementLocked = false; // 잠겼던 이동 제어권을 다시 플레이어에게 돌려준다
        }

        // 애니메이션 이벤트: 스킬 1의 실질적인 공격 판정 시점
        public void OnSkill1Damage()
        {
            DealSkillDamage(GetSkillDamage(1)); // 스킬 1 데미지를 계산하여 타격 판정 실행
        }

        // 애니메이션 이벤트: 스킬 2의 실질적인 공격 판정 시점
        public void OnSkill2Damage()
        {
            DealSkillDamage(GetSkillDamage(2)); // 스킬 2 데미지를 계산하여 타격 판정 실행
        }

        // 애니메이션 이벤트: 스킬 3의 실질적인 공격 판정 시점
        public void OnSkill3Damage()
        {
            DealSkillDamage(GetSkillDamage(3)); // 스킬 3 데미지를 계산하여 타격 판정 실행
        }

        // 범위를 탐색하고 부채꼴 판정을 거쳐 대상에게 데미지 전달
        private void DealSkillDamage(float damage)
        {
            Vector3 origin = transform.root.position; // 플레이어 발밑 위치를 판정 원점으로 설정
            Collider[] hits = Physics.OverlapSphere(origin, skillRange, damageLayer); // 주변 반경 내 적들을 수집한다

            foreach (Collider hit in hits) // 탐지된 모든 콜라이더를 순회한다
            {
                if (hit.transform.root == transform.root) continue; // 자신은 타격 대상에서 제외한다
                if (!IsInAttackHitArea(hit.transform.position)) continue; // 부채꼴 범위 밖에 있다면 무시한다

                IDamageable target = hit.GetComponentInParent<IDamageable>(); // 타격 인터페이스 추출
                if (target == null) continue; // 인터페이스가 없는 객체는 건너뛴다

                target.TakeDamage(damage); // 최종 계산된 데미지를 전달한다
                StartCoroutine(HitStop(0.06f)); // 타격 시 일시적으로 멈추는 효과를 준다
            }
        }

        // 대상 좌표가 플레이어 전방의 부채꼴 판정 영역 안에 있는지 검사
        private bool IsInAttackHitArea(Vector3 worldPos)
        {
            Vector3 toTarget = worldPos - transform.root.position; // 대상까지의 방향 벡터 계산
            toTarget.y = 0f; // 수평 평면상에서의 판정만 수행한다

            if (toTarget.magnitude > attackHitDistance) return false; // 최대 사거리보다 멀면 거짓을 반환한다

            float angle = Vector3.Angle(transform.root.forward, toTarget); // 캐릭터 전방과 대상 사이의 각도 계산
            return angle <= attackHitAngle; // 각도가 허용 범위 이내인지 확인하여 반환한다
        }

        // 스킬 사용이 가능한 상태인지 논리 조건 검사
        private bool CanUseSkill()
        {
            return state != null && !state.IsAttacking && state.IsGrounded; // 공격 중이 아니고 땅에 있는 경우에만 참
        }

        // 타겟팅 시스템에 타겟이 있다면 해당 방향으로 즉시 회전
        private void FaceTargetIfExists()
        {
            if (state == null || state.CurrentTarget == null) return; // 타겟이 없으면 회전 로직을 생략한다

            Vector3 dir = state.CurrentTarget.position - transform.root.position; // 타겟을 향한 방향 벡터 추출
            dir.y = 0f; // Y축 회전만 고려한다

            if (dir.sqrMagnitude < 0.01f) return; // 거리가 너무 가까우면 회전하지 않는다
            transform.root.rotation = Quaternion.LookRotation(dir.normalized); // 타겟 방향으로 캐릭터를 회전시킨다
        }

        // 무기 종류와 스킬 인덱스에 따른 최종 데미지 값 반환
        private float GetSkillDamage(int skillIndex)
        {
            bool hasAxe = state != null && state.IsBattleAxeEquipped; // 도끼 장착 여부 확인

            if (hasAxe) // 도끼 장착 중일 때 스위치 문으로 데미지 결정
            {
                return skillIndex switch
                {
                    1 => axeSkill1Damage,
                    2 => axeSkill2Damage,
                    3 => axeSkill3Damage,
                    _ => 0f
                };
            }

            return skillIndex switch // 맨손 상태일 때 스위치 문으로 데미지 결정
            {
                1 => bareSkill1Damage,
                2 => bareSkill2Damage,
                3 => bareSkill3Damage,
                _ => 0f
            };
        }

        // 타격 시 역경직(Hit Stop) 연출을 위한 코루틴
        private IEnumerator HitStop(float duration)
        {
            state.IsInputMovementLocked = true; // 입력 잠금을 명시적으로 유지한다
            yield return new WaitForSeconds(duration); // 지정된 시간만큼 대기한다
            state.IsInputMovementLocked = false; // 대기 종료 후 제어권을 복구한다
        }

        // 에디터 뷰에서 공격 판정 영역을 시각화
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow; // 기즈모 색상을 노란색으로 설정

            Transform root = transform.root;
            if (root == null) return;

            Vector3 origin = root.position;
            Vector3 left = Quaternion.Euler(0, -attackHitAngle, 0) * root.forward; // 왼쪽 각도 라인 계산
            Vector3 right = Quaternion.Euler(0, attackHitAngle, 0) * root.forward; // 오른쪽 각도 라인 계산

            Gizmos.DrawLine(origin, origin + left * attackHitDistance); // 왼쪽 경계선 그리기
            Gizmos.DrawLine(origin, origin + right * attackHitDistance); // 오른쪽 경계선 그리기
            Gizmos.DrawWireSphere(origin, attackHitDistance); // 전체 사거리 구체 그리기
        }

        // 외부에서 스킬 시퀀스를 강제로 종료시킬 때 호출
        public void ForceEndAttack()
        {
            isSkill3Moving = false; // 이동 중이었다면 즉시 중단한다

            if (state != null)
            {
                state.IsAttacking = false; // 공격 플래그 해제
                state.IsInputMovementLocked = false; // 입력 잠금 해제
            }

            StopAllCoroutines(); // 진행 중인 모든 코루틴(역경직 등)을 멈춘다
        }
        #endregion
    }
}
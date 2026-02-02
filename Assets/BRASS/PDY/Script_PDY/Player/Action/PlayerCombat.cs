using UnityEngine;

namespace BRASS
{
    /// <summary>
    /// 공격 입력 타이밍과 애니메이션 이벤트를 기반으로
    /// 단일 3타 공격 콤보를 제어하고
    /// 공격 입력 순간부터 입력 기반 이동을 잠근다
    /// </summary>
    public class PlayerCombat : MonoBehaviour
    {
        #region Variables
        [SerializeField] private float comboInputWindow = 1.0f;        // 다음 콤보 입력을 허용하는 유효 시간 범위
        [SerializeField] private PlayerAnimationController animationController;        // 공격 애니메이션 재생 제어 컴포넌트
        [SerializeField] private PlayerController playerController;        // 콤보 스텝 이동 처리를 위한 컨트롤러 참조
        [SerializeField] private PlayerState state;        // 공격 중 입력 이동 잠금 상태를 제어하기 위한 상태 컨테이너
        [SerializeField] private WeaponDamage currentWeapon; // 무기 스크립트 참조 추가

        [Header("Combo Step Move")]
        [SerializeField] private float[] comboStepDistances = { 0.15f, 0.2f, 0.25f };        // 각 타수별 전진 거리 데이터

        [Header("Auto Approach - Basic Attack")]
        [SerializeField] private float basicAttackApproachDistance = 2.5f; // 기본 공격 진입 거리
        [SerializeField] private float basicAttackApproachAngle = 45f; // 기본 공격 진입 각도

        private Vector3 cachedAttackDirection;        // 공격 시작 시 고정되는 카메라 기준 정면 수평 방향
        private int attackInputCount;           // 현재 시퀀스 내 누적된 공격 입력 횟수
        private float lastAttackInputTime;        // 마지막 공격 입력 시각
        private bool isAttackSequenceActive;        // 공격 시퀀스 진행 여부
        private bool basicAttackPending;        // 범위 진입 후 공격 실행 대기

        #endregion

        #region Property
        public bool IsAttackSequenceActive => isAttackSequenceActive;
        // 현재 공격 시퀀스가 유효한 상태인지 외부에서 조회하기 위한 읽기 전용 프로퍼티
        #endregion

        #region Unity Event Method
        private void Awake()
        {
            if (state == null)
                state = GetComponentInParent<PlayerState>();
            // 부모 계층에서 PlayerState를 탐색하여 캐싱한다
        }

        private void LateUpdate()
        {
            // 기본 공격 대기 상태가 걸려있다면 실행을 시도한다
            TryExecutePendingBasicAttack();

            // 공격 시퀀스가 활성 상태라면 캐싱된 방향으로 모델을 회전시킨다
            if (!isAttackSequenceActive)
                return;

            // 방향 데이터가 유효하지 않다면 회전을 수행하지 않는다
            if (cachedAttackDirection == Vector3.zero)
                return;

            // 캐싱된 공격 방향으로 모델을 회전시킨다
            transform.rotation = Quaternion.LookRotation(cachedAttackDirection);
        }
        #endregion

        #region Custom Method

        // 외부(WeaponHandler)에서 소환된 무기를 등록해주기 위한 구멍
        public void SetCurrentWeapon(WeaponDamage newWeapon)
        {
            currentWeapon = newWeapon;

            // 디버그용 (제대로 연결됐는지 콘솔창에 확인)
            if (newWeapon != null)
                Debug.Log($"리모컨에 무기 등록 완료: {newWeapon.gameObject.name}");
            else
                Debug.Log("리모컨 무기 등록 해제");
        }

        // 공격 판정 시작 (애니메이션의 "휘두르는" 시점에 배치)
        public void OnAttackHitStart()
        {
            if (currentWeapon != null)
                currentWeapon.StartAttack();
        }

        // 공격 판정 종료 (애니메이션의 "휘두르기가 끝나는" 시점에 배치)
        public void OnAttackHitEnd()
        {
            if (currentWeapon != null)
                currentWeapon.StopAttack();
        }

        // 기본 공격 입력이 시작되었을 때 호출되어 공격 시퀀스를 개시한다
        public void OnBasicAttackStarted()
        {
            float now = Time.time;

            // 콤보 처리
            if (isAttackSequenceActive)
            {
                if (Time.time - lastAttackInputTime <= comboInputWindow)
                {
                    attackInputCount++;
                    lastAttackInputTime = Time.time;
                }
                return;
            }

            playerController?.CancelClickMove();
            state.IsMoving = false;

            // 타겟 없으면 허공 공격
            if (state.CurrentTarget == null)
            {
                StartAttackSequence(now);
                return;
            }

            // 전투 상태라도 거리/각도 체크는 무조건 한다
            if (IsInAttackStartArea(
                state.CurrentTarget,
                basicAttackApproachDistance,
                basicAttackApproachAngle))
            {
                state.IsEngagedWithTarget = true;
                StartAttackSequence(now);
                return;
            }

            // 사거리 밖이면 무조건 다시 자동 접근
            state.IsEngagedWithTarget = true;
            basicAttackPending = true;

            playerController.StartAutoApproach(
                state.CurrentTarget,
                basicAttackApproachDistance
            );
        }


        // 애니메이션 이벤트 지점에서 호출되어 다음 콤보 진행 가능 여부를 판단한다
        public bool OnComboSectionReached(int sectionIndex)
        {
            int requiredInput = sectionIndex + 1;
            // 해당 섹션에 필요한 입력 수 계산

            if (attackInputCount < requiredInput)
            {
                ForceEndAttack();
                // 입력 부족 시 공격 시퀀스를 종료한다

                return false;
            }

            if (Time.time - lastAttackInputTime > comboInputWindow)
            {
                ForceEndAttack();
                // 입력 유효 시간을 초과하면 시퀀스를 종료한다

                return false;
            }

            return true;
            // 다음 콤보 진행 허용
        }

        // 전체 콤보 애니메이션이 종료되었을 때 호출된다
        public void OnComboAnimationFinished()
        {
            ForceEndAttack();
            // 공격 시퀀스를 완전히 종료한다
        }

        // 애니메이션 이벤트 시점에서 콤보 타수별 전진 이동을 적용한다
        public void ApplyComboStep(int comboIndex)
        {
            if (cachedAttackDirection == Vector3.zero)
                return;
            // 공격 방향이 없으면 이동하지 않는다

            if (comboIndex < 0 || comboIndex >= comboStepDistances.Length)
                return;
            // 잘못된 인덱스는 무시한다

            Vector3 delta = cachedAttackDirection * comboStepDistances[comboIndex];
            // 타수별 이동 벡터 계산

            if (playerController != null)
                playerController.MoveExternal(delta);
            // 입력 이동 잠금과 무관하게 공격 연출 이동을 수행한다
        }

        // 외부 입력(점프/슬라이드 등)에 의해 공격을 강제 종료한다
        public void CancelAttack()
        {
            ForceEndAttack();
            basicAttackPending = false;
        }

        // 공격 시퀀스를 시작하는 내부 메서드
        private void StartAttackSequence(float now)
        {
            // 타겟이 있으면 타겟을 바라봄
            if (state != null && state.CurrentTarget != null)
            {
                Vector3 toTarget = state.CurrentTarget.position - transform.position;
                toTarget.y = 0f;

                if (toTarget.sqrMagnitude > 0.001f)
                    transform.rotation = Quaternion.LookRotation(toTarget.normalized);

                state.IsEngagedWithTarget = true;
            }

            state.IsAttacking = true;

            isAttackSequenceActive = true;
            attackInputCount = 1;
            lastAttackInputTime = now;

            CacheAttackDirection();
            animationController?.PlayAttack();
        }


        // 공격 시퀀스를 강제로 종료하고 상태를 초기화한다
        private void ForceEndAttack()
        {
            isAttackSequenceActive = false;
            // 시퀀스 비활성화

            attackInputCount = 0;
            // 입력 카운트 초기화

            cachedAttackDirection = Vector3.zero;
            // 방향 데이터 초기화

            lastAttackInputTime = 0f;
            // 시간 기록 초기화

            if (state != null)
            {
                state.IsAttacking = false;
                state.IsInputMovementLocked = false;
            }
            animationController?.StopAttack();
            basicAttackPending = false;
        }

        // 현재 카메라 기준 공격 방향을 수평 벡터로 캐싱한다
        private void CacheAttackDirection()
        {
            Vector3 forward = transform.forward;
            forward.y = 0f;

            cachedAttackDirection = forward.sqrMagnitude < 0.01f
                ? Vector3.zero
                : forward.normalized;
        }

        // 기본 공격 대기 상태가 걸려있다면 실행을 시도한다

        private void TryExecutePendingBasicAttack()
        {
            if (!basicAttackPending)
                return;

            if (state == null || playerController == null)
                return;

            if (state.CurrentTarget == null)
            {
                basicAttackPending = false;
                return;
            }

            Vector3 toTarget = state.CurrentTarget.position - transform.position;
            toTarget.y = 0f;

            // 거리만 확인 (각도 제거)
            if (toTarget.magnitude <= basicAttackApproachDistance)
            {
                basicAttackPending = false;

                // 타겟을 확실히 바라보게 만든다
                if (toTarget.sqrMagnitude > 0.001f)
                    transform.rotation = Quaternion.LookRotation(toTarget.normalized);

                state.IsEngagedWithTarget = true;
                StartAttackSequence(Time.time);
            }
        }

        // 대상이 지정된 거리와 각도 내에 있는지 검사한다
        private bool IsInAttackStartArea(Transform target, float distance, float angle) // 대상이 지정된 거리와 각도 내에 있는지 검사
        {
            // 거리 계산 및 각도 계산
            Vector3 toTarget = target.position - transform.position;    
            toTarget.y = 0f;

            // 만약 타겟이 지정된 거리보다 멀다면 false 반환
            if (toTarget.magnitude > distance)  // 거리 계산
                return false;   // 거리 내에 있지 않으면 false 반환

            // 만약 타겟이 지정된 각도보다 벗어난다면 false 반환
            float a = Vector3.Angle(transform.forward, toTarget);   // 플레이어 전방과 타겟 방향 사이의 각도 계산
            return a <= angle;  // 각도 내에 있으면 true 반환
        }        
        #endregion
    }
}

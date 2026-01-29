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
        [SerializeField] private float comboInputWindow = 1.0f;
        // 다음 콤보 입력을 허용하는 유효 시간 범위

        [SerializeField] private PlayerAnimationController animationController;
        // 공격 애니메이션 재생 제어 컴포넌트

        [SerializeField] private PlayerController playerController;
        // 콤보 스텝 이동 처리를 위한 컨트롤러 참조

        [SerializeField] private PlayerState state;
        // 공격 중 입력 이동 잠금 상태를 제어하기 위한 상태 컨테이너

        [Header("Combo Step Move")]
        [SerializeField] private float[] comboStepDistances = { 0.15f, 0.2f, 0.25f };
        // 각 타수별 전진 거리 데이터

        private Vector3 cachedAttackDirection;
        // 공격 시작 시 고정되는 카메라 기준 정면 수평 방향

        private int attackInputCount;
        // 현재 시퀀스 내 누적된 공격 입력 횟수

        private float lastAttackInputTime;
        // 마지막 공격 입력 시각

        private bool isAttackSequenceActive;
        // 공격 시퀀스 진행 여부
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
            if (!isAttackSequenceActive)
                return;
            // 공격 중이 아니면 회전 고정을 수행하지 않는다

            if (cachedAttackDirection == Vector3.zero)
                return;
            // 유효한 공격 방향이 없으면 회전하지 않는다

            transform.rotation = Quaternion.LookRotation(cachedAttackDirection);
            // 공격 시퀀스 동안 캐릭터 회전을 고정한다
        }
        #endregion

        #region Custom Method
        // 기본 공격 입력이 시작되었을 때 호출되어 공격 시퀀스를 개시한다
        public void OnBasicAttackStarted()
        {
            float now = Time.time;
            // 현재 게임 시간 기록

            if (!isAttackSequenceActive)
            {
                isAttackSequenceActive = true;
                // 공격 시퀀스 시작

                attackInputCount = 1;
                // 첫 타 입력 처리

                lastAttackInputTime = now;
                // 입력 시각 기록

                if (state != null)
                    state.IsInputMovementLocked = true;
                // 공격 입력 순간부터 입력 기반 이동을 즉시 잠근다

                CacheAttackDirection();
                // 공격 방향을 고정한다

                if (animationController != null)
                    animationController.PlayAttack();
                // 첫 공격 애니메이션 실행

                return;
            }

            if (now - lastAttackInputTime <= comboInputWindow)
            {
                attackInputCount++;
                // 콤보 연계를 위한 추가 입력 처리

                lastAttackInputTime = now;
                // 마지막 입력 시각 갱신
            }
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
                state.IsInputMovementLocked = false;
            // 공격 종료 시 입력 이동 잠금을 해제한다

            if (animationController != null)
                animationController.StopAttack();
            // 애니메이션을 대기 상태로 복귀시킨다
        }

        // 현재 카메라 기준 공격 방향을 수평 벡터로 캐싱한다
        private void CacheAttackDirection()
        {
            Camera cam = Camera.main;
            // 메인 카메라 참조

            if (cam == null)
            {
                cachedAttackDirection = Vector3.zero;
                return;
            }

            Vector3 forward = cam.transform.forward;
            forward.y = 0f;
            // 수평 방향만 사용한다

            cachedAttackDirection = forward.sqrMagnitude < 0.01f
                ? Vector3.zero
                : forward.normalized;
            // 유효한 방향만 정규화하여 저장한다
        }
        #endregion
    }
}

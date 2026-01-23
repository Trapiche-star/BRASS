using UnityEngine;

namespace BRASS
{
    /// <summary>
    /// 점프 입력을 처리하고
    /// 점프 횟수(JumpIndex)를 기준으로 2단 점프까지만 허용한다
    /// 1단 점프에는 짧은 입력 쿨타임을 적용하여 연타를 방지하고
    /// 2단 점프 이후에는 별도의 회복 쿨타임을 적용하여
    /// 착지 직후 애니메이션이 씹히는 현상을 방지한다
    /// </summary>
    public class PlayerJump : MonoBehaviour
    {
        #region Variables
        [SerializeField] private int maxJumpCount = 2;            // 허용되는 최대 점프 횟수 (1단 + 2단)
        [SerializeField] private float jumpForce = 5f;            // 점프 시 적용할 수직 속도

        [Header("Cooldown")]
        [SerializeField] private float jumpInputCooldown = 0.15f;
        // 1단 점프 포함 공통 입력 연타 방지용 쿨타임

        [SerializeField] private float secondJumpCooldown = 0.25f;
        // 2단 점프 사용 이후 적용되는 착지 애니메이션 보호용 쿨타임

        private PlayerController controller;                      // 수직 속도 적용을 담당하는 컨트롤러 참조
        private PlayerState state;                                 // 점프 단계 및 접지 상태를 관리하는 상태 컨테이너

        private float lastJumpInputTime;                           // 마지막 점프 입력이 처리된 시각
        private float secondJumpTime;                              // 마지막 2단 점프가 실행된 시각
        private bool isSecondJumpCooldownActive;                   // 2단 점프 쿨타임 활성 여부
        #endregion

        #region Unity Event Method
        private void Awake()
        {
            controller = GetComponentInParent<PlayerController>();
            // 부모 계층에서 PlayerController를 탐색하여 캐싱한다

            state = GetComponentInParent<PlayerState>();
            // 부모 계층에서 PlayerState를 탐색하여 캐싱한다
        }

        private void Update()
        {
            CheckSecondJumpCooldown();
            // 2단 점프 쿨타임 종료 여부를 감시한다
        }
        #endregion

        #region Custom Method
        // 점프 입력이 들어왔을 때 호출되어 점프 가능 여부를 판단한다
        public void TryJump()
        {
            if (state == null || controller == null) return;
            // 필수 참조가 없으면 점프 처리를 수행하지 않는다

            if (Time.time < lastJumpInputTime + jumpInputCooldown)
                return;
            // 공통 입력 쿨타임이 남아 있으면 연타 입력을 차단한다

            if (state.JumpIndex == 0 &&
                isSecondJumpCooldownActive &&
                Time.time < secondJumpTime + secondJumpCooldown)
                return;
            // 2단 점프 이후 착지 직후 보호 쿨타임이 남아 있으면 입력을 차단한다

            if (state.JumpIndex >= maxJumpCount) return;
            // 최대 점프 횟수를 초과한 경우 입력을 무시한다

            state.JumpIndex++;
            // 점프 단계 증가 (0 → 1 → 2)

            state.IsJumping = true;
            // 점프 펄스를 시작하여 Animator가 점프 상태를 인식하도록 한다

            controller.SetVerticalVelocity(jumpForce);
            // 실제 물리 점프를 수행한다

            lastJumpInputTime = Time.time;
            // 공통 입력 쿨타임 기준 시각을 기록한다

            if (state.JumpIndex == maxJumpCount)
            {
                secondJumpTime = Time.time;
                isSecondJumpCooldownActive = true;
                // 2단 점프가 실행된 시점에 보호 쿨타임을 활성화한다
            }
        }

        // 2단 점프 보호 쿨타임 종료 시점을 감지한다
        private void CheckSecondJumpCooldown()
        {
            if (!isSecondJumpCooldownActive) return;
            // 보호 쿨타임이 활성화되지 않았으면 검사하지 않는다

            if (Time.time >= secondJumpTime + secondJumpCooldown)
            {
                isSecondJumpCooldownActive = false;
                // 보호 쿨타임 상태를 종료한다

                Debug.Log("[Jump] 2단 점프 보호 쿨타임 종료");
                // 디버그용: 착지 직후 애니메이션 보호 구간 종료
            }
        }
        #endregion
    }
}

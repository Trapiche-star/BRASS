using UnityEngine;

namespace BRASS
{
    /// <summary>
    /// 플레이어의 점프 액션을 담당하며 1단·2단 점프 횟수와 점프 단계 상태를 관리한다
    /// </summary>
    public class PlayerJump : MonoBehaviour
    {
        #region Variables
        [SerializeField] private int maxJumpCount = 2;     // 최대 점프 가능 횟수
        [SerializeField] private float jumpForce = 5f;     // 점프 시 적용할 초기 상승 속도

        private int currentJumpCount;                      // 현재까지 사용한 점프 횟수

        private PlayerController controller;               // 수직 속도 적용을 담당
        private CharacterController characterController;   // 지면 판별을 담당
        private PlayerState state;                         // 점프 단계 및 이벤트 상태를 기록
        #endregion

        #region Unity Event Method
        private void Awake()
        {
            controller = GetComponent<PlayerController>();
            // PlayerController 참조를 캐싱한다

            characterController = GetComponent<CharacterController>();
            // CharacterController 참조를 캐싱한다

            state = GetComponent<PlayerState>();
            // PlayerState 참조를 캐싱한다
        }

        private void Update()
        {
            CheckLanding();
            // 매 프레임 착지 여부를 확인하여 점프 상태를 초기화한다
        }
        #endregion

        #region Custom Method
        // 점프 입력이 들어왔을 때 호출되어 점프 가능 여부를 판단하고 점프를 실행한다
        public void TryJump()
        {
            Debug.Log("TryJump called");

            if (currentJumpCount >= maxJumpCount) return;
            // 이미 최대 점프 횟수를 사용했다면 더 이상 점프하지 않는다

            controller.SetVerticalVelocity(jumpForce);
            // 수직 속도를 점프 힘으로 설정하여 상승을 시작한다

            currentJumpCount++;
            // 점프를 사용했으므로 점프 횟수를 증가시킨다

            state.JumpIndex = currentJumpCount;
            // 이번 점프가 1단인지 2단인지 점프 단계를 기록한다

            state.IsJumping = true;
            // 점프가 시작되었음을 애니메이션 컨트롤러에 알린다
        }

        // 캐릭터가 지면에 착지했는지 확인하고 점프 단계를 초기화한다
        private void CheckLanding()
        {
            if (!characterController.isGrounded) return;
            // 공중 상태라면 착지 처리를 하지 않는다

            if (currentJumpCount > 0)
            {
                currentJumpCount = 0;
                // 착지했으므로 점프 횟수를 초기화한다

                state.JumpIndex = 0;
                // 점프 단계를 초기 상태로 되돌린다
            }
        }
        #endregion
    }
}

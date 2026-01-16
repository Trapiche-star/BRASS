using UnityEngine;
using UnityEngine.InputSystem;

namespace BRASS
{
    /// MovementAnimation
    /// 이동 / 달리기 / 슬라이딩 상태만 판단하는 모듈
    public class MovementAnimation : MonoBehaviour
    {
        #region Variables
        [SerializeField] private PlayerInputHandler input;     // 입력 상태 참조
        [SerializeField] private PlayerController controller; // 플레이어 이동 로직 참조(자동 이동 판정용)

        private float currentSpeed;   // 현재 이동 속도
        private bool isMoving;        // 이동 중 여부
        private bool isFastRun;       // 빠른 달리기 여부
        private bool isSliding;       // 슬라이딩 여부
        #endregion

        #region Unity Event Method
        private void Awake()
        {
            if (input == null)
                input = GetComponent<PlayerInputHandler>(); // 만약 [입력 참조가 없으면] [자기 자신에서 탐색]

            if (controller == null)
                controller = GetComponent<PlayerController>(); // 만약 [컨트롤러 참조가 없으면] [자기 자신에서 탐색]
        }

        private void Update()
        {
            UpdateMoveState(); // 이동 상태 갱신
        }
        #endregion

        #region Custom Method
        // 입력 상태 기반 이동 / 달리기 / 슬라이딩 판정
        private void UpdateMoveState()
        {
            if (input == null) return;
            // 만약 [입력 참조가 없으면] [이 프레임 처리를 중단한다]

            // 1. 키보드(WASD) 이동 여부 판단
            bool hasKeyboardMove = input.IsKeyboardMove && input.MoveInput.sqrMagnitude > 0.01f;

            // 2. 자동(좌클릭) 이동 여부 판단
            //    PlayerController에서 자동 이동 중일 때 true를 반환하도록 되어 있음
            bool hasClickMove = controller != null && controller.IsAutoMoving;

            // 3. 최종 이동 판정
            //    WASD 이동이든, 좌클릭 자동 이동이든 하나라도 해당되면 "이동 중"
            isMoving = hasKeyboardMove || hasClickMove;

            // 4. 패스트런 판정 (Shift)
            isFastRun = Keyboard.current != null && Keyboard.current.leftShiftKey.isPressed;

            // 5. 슬라이딩 판정 (Space)
            isSliding = Keyboard.current != null && Keyboard.current.spaceKey.isPressed;

            // 6. 이동 속도(입력 크기 기반)
            //    자동 이동 시에는 입력이 0이므로, 필요하면 별도 계산 가능
            currentSpeed = input.MoveInput.magnitude;
        }
        #endregion

        #region Property
        public bool IsMoving => isMoving;             // 이동 중 여부
        public bool IsFastRun => isFastRun;           // 빠른 달리기 여부
        public bool IsSliding => isSliding;           // 슬라이딩 여부
        public float CurrentSpeed => currentSpeed;   // 현재 이동 속도
        #endregion
    }
}

using UnityEngine;
using UnityEngine.InputSystem;

namespace BRASS
{
    /// 플레이어 입력을 수집하여 이동·카메라·행동 로직에 전달하는 클래스
    public class PlayerInputHandler : MonoBehaviour
    {
        #region Variables
        [SerializeField] private PlayerInput playerInput; // Input Action Asset을 통해 입력을 수신
        #endregion

        #region Property
        public Vector2 MoveInput { get; private set; }     // WASD 이동 입력값
        public bool ClickMovePressed { get; private set; } // 좌클릭 이동 입력 여부
        public Vector2 LookInput { get; private set; }     // 마우스 델타 회전 입력값
        public Vector2 MousePosition { get; private set; } // 현재 마우스 스크린 좌표
        public bool RotatePressed { get; private set; }    // 우클릭 회전 입력 여부
        public float ZoomInput { get; private set; }       // 마우스 휠 줌 입력값
        public bool IsKeyboardMove { get; private set; }   // WASD 이동 중 여부
        public bool SlidePressed { get; private set; }     // 슬라이드 입력 여부 (Space)
        #endregion

        #region Unity Event Method
        private void Awake()
        {
            if (playerInput == null)
                playerInput = GetComponent<PlayerInput>();
            // 만약 [참조가 없으면] [PlayerInput 컴포넌트를 자동 탐색한다]
        }

        private void OnEnable()
        {
            if (playerInput == null) return;
            // 만약 [PlayerInput이 없으면] [입력 이벤트를 구독하지 않는다]

            playerInput.actions["Move"].performed += OnMove;
            playerInput.actions["Move"].canceled += OnMove;

            playerInput.actions["ClickMove"].performed += OnClickMove;
            playerInput.actions["ClickMove"].canceled += OnClickMove;

            playerInput.actions["Look"].performed += OnLook;
            playerInput.actions["Look"].canceled += OnLook;

            playerInput.actions["Rotate"].performed += OnRotate;
            playerInput.actions["Rotate"].canceled += OnRotate;

            playerInput.actions["Zoom"].performed += OnZoom;
            playerInput.actions["Zoom"].canceled += OnZoom;

            playerInput.actions["Sliding"].performed += OnSlide;
            playerInput.actions["Sliding"].canceled += OnSlide;
        }

        private void OnDisable()
        {
            if (playerInput == null) return;
            // 만약 [PlayerInput이 없으면] [이벤트 구독을 해제하지 않는다]

            playerInput.actions["Move"].performed -= OnMove;
            playerInput.actions["Move"].canceled -= OnMove;

            playerInput.actions["ClickMove"].performed -= OnClickMove;
            playerInput.actions["ClickMove"].canceled -= OnClickMove;

            playerInput.actions["Look"].performed -= OnLook;
            playerInput.actions["Look"].canceled -= OnLook;

            playerInput.actions["Rotate"].performed -= OnRotate;
            playerInput.actions["Rotate"].canceled -= OnRotate;

            playerInput.actions["Zoom"].performed -= OnZoom;
            playerInput.actions["Zoom"].canceled -= OnZoom;

            playerInput.actions["Sliding"].performed += OnSlide;
            playerInput.actions["Sliding"].canceled += OnSlide;
        }

        private void Update()
        {
            if (Mouse.current != null)
                MousePosition = Mouse.current.position.ReadValue();
            // 만약 [마우스가 존재하면] [현재 마우스 좌표를 갱신한다]
        }
        #endregion

        #region Custom Method
        // 휠 줌 입력을 초기화한다
        public void ClearZoom()
        {
            ZoomInput = 0f;
            // 만약 [줌 입력을 사용했으면] [다음 프레임 누적을 방지한다]
        }

        // WASD 이동 입력 수집
        private void OnMove(InputAction.CallbackContext context)
        {
            MoveInput = context.ReadValue<Vector2>();
            // 이동 입력값을 갱신한다

            IsKeyboardMove = MoveInput.sqrMagnitude > 0.01f;
            // 만약 [입력이 존재하면] [키보드 이동 중으로 판단한다]
        }

        // 좌클릭 이동 입력 수집
        private void OnClickMove(InputAction.CallbackContext context)
        {
            ClickMovePressed = context.ReadValueAsButton();
            // 만약 [버튼이 눌리면] [클릭 이동 입력으로 판단한다]
        }

        // 마우스 회전 입력 수집
        private void OnLook(InputAction.CallbackContext context)
        {
            LookInput = context.ReadValue<Vector2>();
            // 마우스 델타 회전 입력값을 갱신한다
        }

        // 우클릭 회전 입력 수집
        private void OnRotate(InputAction.CallbackContext context)
        {
            RotatePressed = context.ReadValueAsButton();
            // 우클릭 회전 입력 여부를 갱신한다
        }

        // 휠 줌 입력 수집
        private void OnZoom(InputAction.CallbackContext context)
        {
            ZoomInput = context.ReadValue<float>();
            // 마우스 휠 줌 입력값을 갱신한다
        }

        // 슬라이드 입력 수집
        private void OnSlide(InputAction.CallbackContext context)
        {
            SlidePressed = context.ReadValueAsButton();
            // 만약 [Space 입력이 들어오면] [슬라이드 입력 상태를 갱신한다]
        }
        #endregion
    }
}

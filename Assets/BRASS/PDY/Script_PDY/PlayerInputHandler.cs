using UnityEngine;
using UnityEngine.InputSystem;

namespace BRASS
{
    /// 플레이어 입력을 수집하여 이동·카메라·상호작용 로직에 전달하는 클래스
    public class PlayerInputHandler : MonoBehaviour
    {
        #region Variables

        [SerializeField] private PlayerInput playerInput; // Input Action Asset을 통해 입력을 수신

        #endregion

        #region Property

        public Vector2 MoveInput { get; private set; } // WASD 이동 입력값
        public Vector2 LookInput { get; private set; } // 마우스 델타 회전 입력값
        public Vector2 MousePosition { get; private set; } // 현재 마우스 스크린 좌표
        public bool ClickMovePressed { get; private set; } // 좌클릭 이동 트리거 상태
        public bool RotatePressed { get; private set; } // 우클릭 시점 회전 상태
        public float ZoomInput { get; private set; } // 마우스 휠 줌 입력값(Y축)       
        #endregion

        public void ClearZoom()
        {
            ZoomInput = 0f;
        }

        #region Unity Event Method

        private void Awake()
        {
            if (playerInput == null) playerInput = GetComponent<PlayerInput>();
            // 만약 [참조가 없으면] [같은 오브젝트에서 컴포넌트를 가져온다]
        }

        private void OnEnable()
        {
            if (playerInput == null) return;
            // 만약 [참조가 없으면] [입력 이벤트를 구독하지 않는다]

            playerInput.actions["Move"].performed += OnMove;
            playerInput.actions["Move"].canceled += OnMove;

            playerInput.actions["Look"].performed += OnLook;
            playerInput.actions["Look"].canceled += OnLook;

            playerInput.actions["ClickMove"].performed += OnClickMove;
            playerInput.actions["ClickMove"].canceled += OnClickMove;

            playerInput.actions["Rotate"].performed += OnRotate;
            playerInput.actions["Rotate"].canceled += OnRotate;

            playerInput.actions["Zoom"].performed += OnZoom;
            playerInput.actions["Zoom"].canceled += OnZoom;
        }

        private void OnDisable()
        {
            if (playerInput == null) return;
            // 만약 [참조가 없으면] [입력 이벤트를 해제하지 않는다]

            playerInput.actions["Move"].performed -= OnMove;
            playerInput.actions["Move"].canceled -= OnMove;

            playerInput.actions["Look"].performed -= OnLook;
            playerInput.actions["Look"].canceled -= OnLook;

            playerInput.actions["ClickMove"].performed -= OnClickMove;
            playerInput.actions["ClickMove"].canceled -= OnClickMove;

            playerInput.actions["Rotate"].performed -= OnRotate;
            playerInput.actions["Rotate"].canceled -= OnRotate;

            playerInput.actions["Zoom"].performed -= OnZoom;
            playerInput.actions["Zoom"].canceled -= OnZoom;
        }

        private void Update()
        {
            if (Mouse.current != null)
                MousePosition = Mouse.current.position.ReadValue();
            // 만약 [마우스가 있으면] [현재 마우스 좌표를 갱신한다]
        }

        #endregion

        #region Custom Method

        // 이동 입력 수집
        private void OnMove(InputAction.CallbackContext context)
            => MoveInput = context.ReadValue<Vector2>();

        // 회전 입력 수집(마우스 델타)
        private void OnLook(InputAction.CallbackContext context)
            => LookInput = context.ReadValue<Vector2>();

        // 좌클릭 이동 트리거 입력 수집
        private void OnClickMove(InputAction.CallbackContext context)
            => ClickMovePressed = context.ReadValueAsButton();

        // 우클릭 시점 회전 입력 수집
        private void OnRotate(InputAction.CallbackContext context)
            => RotatePressed = context.ReadValueAsButton();

        // 휠 줌 입력 수집(Vector2 중 Y축만 사용)
        private void OnZoom(InputAction.CallbackContext context)
        {
            ZoomInput = context.ReadValue<float>();           
        }        
        #endregion
    }
}

using UnityEngine;
using UnityEngine.InputSystem;

namespace BRASS
{
    /// 플레이어 입력을 수집하여 전달하는 핸들러 클래스
    public class PlayerInputHandler : MonoBehaviour
    {
        [SerializeField] private PlayerInput playerInput; // 입력을 수신할 Input Action Asset
        public Vector2 MoveInput { get; private set; } // WASD 이동 입력값
        public Vector2 LookInput { get; private set; } // 마우스 델타 회전값
        public Vector2 MousePosition { get; private set; } // 현재 마우스 스크린 좌표
        public bool ClickMovePressed { get; private set; } // 좌클릭 이동 트리거 상태
        public bool RotatePressed { get; private set; } // 우클릭 시점 회전 상태

        private void Awake()
        {
            if (playerInput == null) playerInput = GetComponent<PlayerInput>();
        }

        private void OnEnable()
        {
            if (playerInput == null) return; // 만약 [컴포넌트가 없으면] [구독하지 않는다]

            playerInput.actions["Move"].performed += OnMove;
            playerInput.actions["Move"].canceled += OnMove;
            playerInput.actions["Look"].performed += OnLook;
            playerInput.actions["Look"].canceled += OnLook;
            playerInput.actions["ClickMove"].performed += OnClickMove;
            playerInput.actions["ClickMove"].canceled += OnClickMove;
            playerInput.actions["Rotate"].performed += OnRotate;
            playerInput.actions["Rotate"].canceled += OnRotate;
        }

        private void OnDisable()
        {
            if (playerInput == null) return; // 만약 [컴포넌트가 없으면] [구독 해제한다]

            playerInput.actions["Move"].performed -= OnMove;
            playerInput.actions["Move"].canceled -= OnMove;
            playerInput.actions["Look"].performed -= OnLook;
            playerInput.actions["Look"].canceled -= OnLook;
            playerInput.actions["ClickMove"].performed -= OnClickMove;
            playerInput.actions["ClickMove"].canceled -= OnClickMove;
            playerInput.actions["Rotate"].performed -= OnRotate;
            playerInput.actions["Rotate"].canceled -= OnRotate;
        }

        private void Update()
        {
            if (Mouse.current != null) MousePosition = Mouse.current.position.ReadValue(); // 만약 [마우스가 존재하면] [위치 갱신]
        }

        private void OnMove(InputAction.CallbackContext context) => MoveInput = context.ReadValue<Vector2>();
        private void OnLook(InputAction.CallbackContext context) => LookInput = context.ReadValue<Vector2>();
        private void OnClickMove(InputAction.CallbackContext context) => ClickMovePressed = context.ReadValueAsButton();
        private void OnRotate(InputAction.CallbackContext context) => RotatePressed = context.ReadValueAsButton();
    }
}
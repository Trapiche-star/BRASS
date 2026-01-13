using UnityEngine;
using UnityEngine.InputSystem;

namespace BRASS
{
    /// 플레이어 입력만 수집하여 외부(PlayerController)에서 참조하도록 전달하는 클래스
    public class PlayerInputHandler : MonoBehaviour
    {
        #region Variables

        [SerializeField] private PlayerInput playerInput; // Input Action Asset을 통해 입력을 수신

        public Vector2 MoveInput { get; private set; }   // WASD 이동 입력
        public Vector2 LookInput { get; private set; }   // 마우스 이동량(회전용)
        public bool ClickMovePressed { get; private set; } // 좌클릭 자동 이동 트리거
        public bool RotatePressed { get; private set; }    // 우클릭 회전 모드 스위치

        #endregion

        #region Unity Event Method

        private void Awake()
        {
            if (playerInput == null)
                playerInput = GetComponent<PlayerInput>(); // 인스펙터 미할당 시 자동 참조
        }

        private void OnEnable()
        {
            if (playerInput == null) return; // PlayerInput이 없으면 입력을 처리하지 않는다

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
            if (playerInput == null) return; // PlayerInput이 없으면 구독 해제할 대상이 없다

            playerInput.actions["Move"].performed -= OnMove;
            playerInput.actions["Move"].canceled -= OnMove;

            playerInput.actions["Look"].performed -= OnLook;
            playerInput.actions["Look"].canceled -= OnLook;

            playerInput.actions["ClickMove"].performed -= OnClickMove;
            playerInput.actions["ClickMove"].canceled -= OnClickMove;

            playerInput.actions["Rotate"].performed -= OnRotate;
            playerInput.actions["Rotate"].canceled -= OnRotate;
        }

        #endregion

        #region Custom Method

        // 이동 입력(Vector2)을 읽어 저장한다
        private void OnMove(InputAction.CallbackContext context)
        {
            MoveInput = context.ReadValue<Vector2>(); // 현재 WASD 입력값을 저장
        }

        // 마우스 이동량(Vector2)을 읽어 저장한다
        private void OnLook(InputAction.CallbackContext context)
        {
            LookInput = context.ReadValue<Vector2>(); // 회전에 사용할 마우스 델타값 저장
        }

        // 좌클릭 상태를 버튼 값으로 저장한다
        private void OnClickMove(InputAction.CallbackContext context)
        {
            ClickMovePressed = context.ReadValueAsButton(); // 클릭 여부를 bool 값으로 저장
        }

        // 우클릭 상태를 버튼 값으로 저장한다
        private void OnRotate(InputAction.CallbackContext context)
        {
            RotatePressed = context.ReadValueAsButton(); // 회전 모드 활성 여부 저장
        }

        #endregion
    }
}

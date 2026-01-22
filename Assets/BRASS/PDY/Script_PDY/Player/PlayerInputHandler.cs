using UnityEngine;
using UnityEngine.InputSystem;

namespace BRASS
{
    /// <summary>
    /// 플레이어 입력을 수집하여 이동·카메라·행동 로직에 전달하는 클래스
    /// </summary>
    public class PlayerInputHandler : MonoBehaviour
    {
        #region Variables
        [SerializeField] private PlayerInput playerInput; // Input Action Asset을 통해 입력을 수신

        private PlayerCombat combat; // 기본공격 입력 전달 담당
        private PlayerJump jump;     // 점프 입력 전달 담당
        #endregion

        #region Property
        public Vector2 MoveInput { get; private set; }     // WASD 이동 입력값
        public bool ClickMovePressed { get; private set; } // 좌클릭 이동 입력 여부
        public Vector2 LookInput { get; private set; }     // 마우스 델타 회전 입력값
        public Vector2 MousePosition { get; private set; } // 현재 마우스 스크린 좌표
        public bool RotatePressed { get; private set; }    // 우클릭 회전 입력 여부
        public float ZoomInput { get; private set; }       // 마우스 휠 줌 입력값
        public bool IsKeyboardMove { get; private set; }   // WASD 이동 중 여부
        public bool SlidePressed { get; private set; }     // 슬라이드 입력 여부
        #endregion

        #region Unity Event Method
        private void Awake()
        {
            if (playerInput == null)
                playerInput = GetComponent<PlayerInput>();
            // PlayerInput 참조가 없으면 같은 오브젝트에서 자동으로 탐색한다

            combat = GetComponentInChildren<PlayerCombat>();
            // Player 하위 오브젝트에서 PlayerCombat을 탐색하여 캐싱한다

            jump = GetComponentInChildren<PlayerJump>();
            // Player 하위 계층에서 PlayerJump를 탐색하여 캐싱한다

            if (jump == null)
            {
                Debug.LogError("[PlayerInputHandler] PlayerJump not found in children.");
            }
            // PlayerJump가 존재하지 않을 경우 구조 오류를 명확히 로그로 알린다
        }

        private void OnEnable()
        {
            if (playerInput == null) return;
            // PlayerInput이 없으면 입력 이벤트를 구독하지 않는다

            playerInput.actions.FindActionMap("Player").Enable();
            // 이동·카메라·점프 입력이 포함된 Player 맵을 활성화한다

            playerInput.actions.FindActionMap("Attack").Enable();
            // 전투 입력이 포함된 Attack 맵을 활성화한다

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

            playerInput.actions["Jump"].performed += OnJump;            

            playerInput.actions["BasicAttack"].started += OnBasicAttackStarted;
            playerInput.actions["BasicAttack"].canceled += OnBasicAttackCanceled;
        }

        private void OnDisable()
        {
            if (playerInput == null) return;
            // PlayerInput이 없으면 이벤트 구독 해제를 수행하지 않는다

            playerInput.actions.FindActionMap("Player").Disable();
            // Player 맵을 비활성화하여 입력을 차단한다

            playerInput.actions.FindActionMap("Attack").Disable();
            // Attack 맵을 비활성화하여 입력을 차단한다

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

            playerInput.actions["Sliding"].performed -= OnSlide;
            playerInput.actions["Sliding"].canceled -= OnSlide;

            playerInput.actions["Jump"].performed -= OnJump;

            playerInput.actions["BasicAttack"].started -= OnBasicAttackStarted;
            playerInput.actions["BasicAttack"].canceled -= OnBasicAttackCanceled;
        }

        private void Update()
        {
            if (Mouse.current != null)
                MousePosition = Mouse.current.position.ReadValue();
            // 마우스가 존재하면 현재 스크린 좌표를 갱신한다
        }
        #endregion

        #region Custom Method
        // 휠 줌 입력을 초기화한다
        public void ClearZoom()
        {
            ZoomInput = 0f;
            // 줌 입력 누적을 방지하기 위해 값을 초기화한다
        }

        // WASD 이동 입력을 수집한다
        private void OnMove(InputAction.CallbackContext context)
        {
            MoveInput = context.ReadValue<Vector2>();
            // 이동 입력값을 갱신한다

            IsKeyboardMove = MoveInput.sqrMagnitude > 0.01f;
            // 입력이 존재하면 키보드 이동 중으로 판단한다
        }

        // 좌클릭 이동 입력을 수집한다
        private void OnClickMove(InputAction.CallbackContext context)
        {
            ClickMovePressed = context.ReadValueAsButton();
            // 클릭 입력 여부를 상태값으로 저장한다
        }

        // 마우스 회전 입력을 수집한다
        private void OnLook(InputAction.CallbackContext context)
        {
            LookInput = context.ReadValue<Vector2>();
            // 마우스 델타 회전 입력값을 갱신한다
        }

        // 우클릭 회전 입력을 수집한다
        private void OnRotate(InputAction.CallbackContext context)
        {
            RotatePressed = context.ReadValueAsButton();
            // 우클릭 회전 입력 여부를 갱신한다
        }

        // 휠 줌 입력을 수집한다
        private void OnZoom(InputAction.CallbackContext context)
        {
            ZoomInput = context.ReadValue<float>();
            // 마우스 휠 줌 입력값을 갱신한다
        }

        // 슬라이드 입력을 수집한다
        private void OnSlide(InputAction.CallbackContext context)
        {
            SlidePressed = context.ReadValueAsButton();
            // 슬라이드 입력 여부를 상태값으로 갱신한다
        }

        // 점프 입력이 시작되었을 때 PlayerJump로 전달한다
        private void OnJump(InputAction.CallbackContext context)
        {
            if (jump == null) return;
            // PlayerJump가 없으면 점프를 처리하지 않는다

            jump.TryJump();
            // 점프 시도를 PlayerJump에 요청한다
        }

        // 기본공격 입력 시작 처리
        private void OnBasicAttackStarted(InputAction.CallbackContext context)
        {
            if (combat == null) return;
            // PlayerCombat이 없으면 처리를 중단한다

            combat.OnBasicAttackStarted();
            // 기본공격 버튼이 눌렸음을 전달한다
        }

        // 기본공격 입력 종료 처리
        private void OnBasicAttackCanceled(InputAction.CallbackContext context)
        {
            if (combat == null) return;
            // PlayerCombat이 없으면 처리를 중단한다

            combat.OnBasicAttackCanceled();
            // 기본공격 버튼 해제를 전달한다
        }
        #endregion
    }
}

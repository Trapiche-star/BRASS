using UnityEngine;
using UnityEngine.InputSystem;

namespace BRASS
{
    /// 플레이어 입력을 수집하여 이동, 카메라, 행동 로직에 전달하는 클래스
    public class PlayerInputHandler : MonoBehaviour
    {
        #region Variables
        [SerializeField] private PlayerInput playerInput; // 인풋 액션 에셋을 통해 데이터를 수신하는 컴포넌트

        private PlayerCombat combat; // 기본 공격 입력을 처리할 전투 컴포넌트
        private PlayerJump jump; // 점프 기능을 실행할 컴포넌트
        private WeaponHandler weaponHandler; // 무기 장착 및 해제를 관리하는 핸들러
        #endregion

        #region Property
        public Vector2 MoveInput { get; private set; } // 방향키 또는 WASD 입력 값
        public bool ClickMovePressed { get; private set; } // 마우스 클릭 이동 버튼의 상태
        public Vector2 LookInput { get; private set; } // 마우스 델타(회전) 입력 값
        public Vector2 MousePosition { get; private set; } // 화면상의 마우스 좌표
        public bool RotatePressed { get; private set; } // 카메라 회전 활성화 키 상태
        public float ZoomInput { get; private set; } // 마우스 휠을 통한 줌 값
        public bool IsKeyboardMove { get; private set; } // 현재 키보드 입력을 통해 이동 중인지 여부
        public bool SlidePressed { get; private set; } // 슬라이딩 키 입력 상태
        #endregion

        #region Unity Event Methods
        private void Awake()
        {
            if (playerInput == null) // 할당되지 않았다면 컴포넌트에서 찾는다
                playerInput = GetComponent<PlayerInput>();

            // 모든 ActionMap을 명시적으로 비활성화
            playerInput.actions.Disable();

            // 자식 객체에서 필요한 기능적 컴포넌트들을 캐싱한다
            combat = GetComponentInChildren<PlayerCombat>();
            jump = GetComponentInChildren<PlayerJump>();
            weaponHandler = GetComponentInChildren<WeaponHandler>();

            if (jump == null) // 점프 컴포넌트 누락 시 경고
                Debug.LogError("[PlayerInputHandler] PlayerJump not found in children.");

            if (weaponHandler == null) // 무기 핸들러 누락 시 경고
                Debug.LogError("[PlayerInputHandler] WeaponHandler not found in children.");
        }

        private void OnEnable()
        {
            if (playerInput == null) return;

            // 필요한 ActionMap만 활성화
            playerInput.actions.FindActionMap("Player", true).Enable();
            playerInput.actions.FindActionMap("Attack", true).Enable();            

            // 각 입력 액션에 콜백 메서드를 등록한다
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
            playerInput.actions["WeaponSlot1"].performed += OnWeaponSlot1;
        }

        private void OnDisable()
        {
            if (playerInput == null) return;

            // 메모리 누수 방지를 위해 등록된 콜백을 모두 해제한다
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
            playerInput.actions["WeaponSlot1"].performed -= OnWeaponSlot1;

            // ActionMap 개별 Disable 대신 전체 Disable
            playerInput.actions.Disable();
        }

        private void Update()
        {
            // 매 프레임 마우스의 현재 좌표를 갱신한다
            if (Mouse.current != null)
                MousePosition = Mouse.current.position.ReadValue();
        }
        #endregion

        #region Custom Methods
        // 카메라 줌 입력 값을 0으로 초기화한다
        public void ClearZoom()
        {
            ZoomInput = 0f;
        }

        private void OnMove(InputAction.CallbackContext context)
        {
            MoveInput = context.ReadValue<Vector2>(); // 2D 이동 벡터를 읽는다
            IsKeyboardMove = MoveInput.sqrMagnitude > 0.01f; // 입력 세기가 유효한지 판단
        }

        private void OnClickMove(InputAction.CallbackContext context)
        {
            ClickMovePressed = context.ReadValueAsButton(); // 클릭 이동 버튼 눌림 상태 저장
        }

        private void OnLook(InputAction.CallbackContext context)
        {
            LookInput = context.ReadValue<Vector2>(); // 마우스 델타 값 저장
        }

        private void OnRotate(InputAction.CallbackContext context)
        {
            RotatePressed = context.ReadValueAsButton(); // 카메라 회전 모드 여부 저장
        }

        private void OnZoom(InputAction.CallbackContext context)
        {
            ZoomInput = context.ReadValue<float>(); // 마우스 휠 값 저장
        }

        private void OnSlide(InputAction.CallbackContext context)
        {
            SlidePressed = context.ReadValueAsButton(); // 슬라이딩/회피 상태 저장
        }

        private void OnJump(InputAction.CallbackContext context)
        {
            if (jump == null) return; // 점프 컴포넌트가 없다면 무시
            jump.TryJump(); // 실제 점프 로직 시도
        }

        private void OnBasicAttackStarted(InputAction.CallbackContext context)
        {
            if (combat == null) return; // 전투 컴포넌트가 없다면 무시
            combat.OnBasicAttackStarted(); // 공격 시퀀스 시작
        }

        private void OnBasicAttackCanceled(InputAction.CallbackContext context)
        {
            if (combat == null) return;
            // 필요 시 공격 중단 혹은 장전 로직 등을 추가 가능
        }

        // 특정 무기 슬롯 입력을 받아 핸들러에 전달함
        private void OnWeaponSlot1(InputAction.CallbackContext context)
        {
            if (!context.performed) // 수행 완료 시점이 아니라면 중단
                return;

            Debug.Log("WeaponSlot1 입력 수신"); // 무기 슬롯 1번 입력 확인

            if (weaponHandler != null) // 무기 핸들러가 존재할 때만 실행
                weaponHandler.ToggleWeaponByIndex(0);
        }
        #endregion
    }
}
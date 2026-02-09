using UnityEngine;
using UnityEngine.InputSystem;


namespace BRASS
{
    /// 플레이어 입력을 수집하여 이동과 전투 입력을 각 시스템으로 전달하는 입력 라우터
    public class PlayerInputHandler : MonoBehaviour
    {
        #region Variables

        [SerializeField] private PlayerInput playerInput; // Input System PlayerInput 컴포넌트 참조

        private RangeSkill rangeSkill;     // 원거리 스킬 처리 컴포넌트
        private PlayerCombat combat;        // 근접 기본 공격 처리 컴포넌트
        private RangeCombat rangeCombat;    // 원거리 기본 공격 처리 컴포넌트
        private MeleeSkill meleeSkill;      // 근접 스킬 처리 컴포넌트
        private PlayerJump jump;             // 점프 입력 처리 컴포넌트
        private WeaponHandler weaponHandler; // 무기 장착 및 해제 처리 컴포넌트
        private PlayerState state;            // 현재 무기 및 행동 상태 데이터   
        #endregion

        #region Property

        public Vector2 MoveInput { get; private set; }          // 이동 입력 벡터
        public bool ClickMovePressed { get; private set; }      // 클릭 이동 입력 여부
        public Vector2 LookInput { get; private set; }          // 시점 회전 입력
        public Vector2 MousePosition { get; private set; }      // 현재 마우스 좌표
        public bool RotatePressed { get; private set; }         // 회전 버튼 입력 여부
        public float ZoomInput { get; private set; }            // 카메라 줌 입력 값
        public bool IsKeyboardMove { get; private set; }        // 키보드 이동 중인지 여부
        public bool SlidePressed { get; private set; }          // 슬라이드 입력 여부

        public bool EscDown { get; private set; }               // Esc 단발 입력
        public bool TabDown { get; private set; }               // Tab 단발 입력

        #endregion

        #region Unity Event Methods

        private void Awake()
        {
            // Input System PlayerInput 컴포넌트 참조 캐싱
            if (playerInput == null) playerInput = GetComponent<PlayerInput>();

            playerInput.actions.Disable(); // 초기에는 모든 액션 맵 비활성화

            combat = GetComponentInChildren<PlayerCombat>();
            rangeCombat = GetComponentInChildren<RangeCombat>();
            meleeSkill = GetComponentInChildren<MeleeSkill>();
            jump = GetComponentInChildren<PlayerJump>();
            weaponHandler = GetComponentInChildren<WeaponHandler>();
            state = GetComponentInChildren<PlayerState>();
            rangeSkill = GetComponentInChildren<RangeSkill>();
        }

        private void OnEnable()
        {
            if (playerInput == null) return;

            playerInput.actions.FindActionMap("Player", true).Enable();
            playerInput.actions.FindActionMap("Attack", true).Enable();

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
            playerInput.actions["WeaponSlot1"].performed += OnWeaponSlot1;

            playerInput.actions["Skill_1"].started += OnSkill1;
            playerInput.actions["Skill_1"].canceled += OnSkill1;

            playerInput.actions["Skill_2"].started += OnSkill2;

            playerInput.actions["Skill_3"].started += OnSkill3;
            playerInput.actions["Skill_3"].canceled += OnSkill3;

            playerInput.actions["Esc"].performed += OnEsc;
            playerInput.actions["Tab"].performed += OnTab;

        }

        private void OnDisable()
        {
            if (playerInput == null) return;
            playerInput.actions.Disable();
        }

        private void Update()
        {
            if (Mouse.current != null)
                MousePosition = Mouse.current.position.ReadValue();

            HandleInputBlockingByTypingState();
        }

        // 단발 입력 리셋 처리
        private void LateUpdate()
        {
            EscDown = false;
            TabDown = false;
        }

        #endregion

        #region Custom Methods
        // 추가: 문자 입력 중일 때 게임 입력 차단 처리
        private void HandleInputBlockingByTypingState()
        {
            if (state == null || playerInput == null) return;

            // 타이핑 중이면 게임 입력 비활성화
            if (state.IsTypingInUI)
            {
                if (playerInput.currentActionMap != null && playerInput.currentActionMap.enabled)
                {
                    playerInput.DeactivateInput();
                }
            }
            // 타이핑 중이 아니면 게임 입력 활성화
            else
            {
                if (playerInput.currentActionMap != null && !playerInput.currentActionMap.enabled)
                {
                    playerInput.ActivateInput();
                }
            }
        }

        // 이동 입력 수신 및 키보드 이동 여부 판별
        private void OnMove(InputAction.CallbackContext context)
        {
            MoveInput = context.ReadValue<Vector2>();
            IsKeyboardMove = MoveInput.sqrMagnitude > 0.01f;
        }

        // 클릭 이동 입력 처리
        private void OnClickMove(InputAction.CallbackContext context)
        {
            ClickMovePressed = context.ReadValueAsButton();
        }

        // 시점 회전 입력 처리
        private void OnLook(InputAction.CallbackContext context)
        {
            LookInput = context.ReadValue<Vector2>();
        }

        // 회전 버튼 입력 처리
        private void OnRotate(InputAction.CallbackContext context)
        {
            RotatePressed = context.ReadValueAsButton();
        }

        // 카메라 줌 입력 처리
        private void OnZoom(InputAction.CallbackContext context)
        {
            ZoomInput = context.ReadValue<float>();
        }

        // 슬라이드 입력 처리
        private void OnSlide(InputAction.CallbackContext context)
        {
            SlidePressed = context.ReadValueAsButton();
        }

        // 점프 입력을 점프 시스템으로 전달
        private void OnJump(InputAction.CallbackContext context)
        {
            jump?.TryJump();
        }

        // 기본 공격 입력 처리
        // 무기 타입에 따라 근접 또는 원거리 기본 공격으로 라우팅한다
        private void OnBasicAttackStarted(InputAction.CallbackContext context)
        {
            /* 근접공격만 있었을때
            if (state != null && state.IsGunEquipped)
            {
                rangeCombat?.Fire();
            }
            else
            {
                combat?.OnBasicAttackStarted();
            }*/

            if (state != null && state.IsGunEquipped)
            {
                rangeCombat?.TryFire();
                return;
            }

            // 근접 기본 공격 실행
            combat?.OnBasicAttackStarted();
        }

        // 무기 슬롯 1 토글 입력 처리
        private void OnWeaponSlot1(InputAction.CallbackContext context)
        {
            if (!context.performed) return;
            weaponHandler?.ToggleWeaponByIndex(0);
        }

        // 스킬 1 입력 처리        
        private void OnSkill1(InputAction.CallbackContext context)
        {
            if (state != null && state.IsGunEquipped)
            {
                if (context.started)
                    rangeSkill?.ExecuteSkill01();

                if (context.canceled)
                    rangeSkill?.StopFire();

                return;
            }

            if (context.started)
                meleeSkill?.ExecuteSkill01();
        }

        // 스킬 2 입력 처리
        private void OnSkill2(InputAction.CallbackContext context)
        {
            if (!context.started) return;

            if (state != null && state.IsGunEquipped)
            {
                rangeSkill?.ExecuteSkill02();
                return;
            }

            meleeSkill?.ExecuteSkill02();
        }

        // 스킬 3 입력 처리
        private void OnSkill3(InputAction.CallbackContext context)
        {
            if (state != null && state.IsGunEquipped)
            {
                if (context.started)
                    rangeSkill?.ExecuteSkill03();

                if (context.canceled)
                    rangeSkill?.StopFire();

                return;
            }

            if (context.started)
                meleeSkill?.ExecuteSkill03();
        }

        // Esc 단발 입력 처리
        private void OnEsc(InputAction.CallbackContext context)
        {
            if (context.performed)
                EscDown = true;
        }

        // Tab 단발 입력 처리
        private void OnTab(InputAction.CallbackContext context)
        {
            if (context.performed)
                TabDown = true;
        }

        // 외부에서 줌 입력을 강제로 초기화
        public void ClearZoom()
        {
            ZoomInput = 0f;
        }

        #endregion
    }
}

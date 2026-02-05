using UnityEngine;
using UnityEngine.InputSystem;

namespace BRASS
{
    /// 플레이어 입력을 수집하여 이동, 카메라, 행동 로직에 전달하는 클래스
    public class PlayerInputHandler : MonoBehaviour
    {
        #region Variables
        [SerializeField] private PlayerInput playerInput;

        private PlayerCombat combat;
        private PlayerJump jump;
        private WeaponHandler weaponHandler;
        private MeleeSkill meleeSkill;       
        #endregion

        #region Property
        public Vector2 MoveInput { get; private set; }
        public bool ClickMovePressed { get; private set; }
        public Vector2 LookInput { get; private set; }
        public Vector2 MousePosition { get; private set; }
        public bool RotatePressed { get; private set; }
        public float ZoomInput { get; private set; }
        public bool IsKeyboardMove { get; private set; }
        public bool SlidePressed { get; private set; }
       
        public bool EscDown { get; private set; }
        public bool TabDown { get; private set; }
        #endregion

        #region Unity Event Methods
        private void Awake()
        {
            if (playerInput == null)
                playerInput = GetComponent<PlayerInput>();

            playerInput.actions.Disable();

            combat = GetComponentInChildren<PlayerCombat>();
            jump = GetComponentInChildren<PlayerJump>();
            weaponHandler = GetComponentInChildren<WeaponHandler>();
            meleeSkill = GetComponentInChildren<MeleeSkill>();            
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
            playerInput.actions["Skill_1"].performed += OnSkill1;
            playerInput.actions["Skill_2"].performed += OnSkill2;
            playerInput.actions["Skill_3"].performed += OnSkill3;            
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
        }

        // 단발 입력 리셋 (입력 샘플링 책임)
        private void LateUpdate()
        {
            EscDown = false;
            TabDown = false;
        }
        #endregion

        #region Custom Methods
        private void OnMove(InputAction.CallbackContext context)
        {
            MoveInput = context.ReadValue<Vector2>();
            IsKeyboardMove = MoveInput.sqrMagnitude > 0.01f;
        }

        private void OnClickMove(InputAction.CallbackContext context)
        {
            ClickMovePressed = context.ReadValueAsButton();
        }

        private void OnLook(InputAction.CallbackContext context)
        {
            LookInput = context.ReadValue<Vector2>();
        }

        private void OnRotate(InputAction.CallbackContext context)
        {
            RotatePressed = context.ReadValueAsButton();
        }

        private void OnZoom(InputAction.CallbackContext context)
        {
            ZoomInput = context.ReadValue<float>();
        }

        private void OnSlide(InputAction.CallbackContext context)
        {
            SlidePressed = context.ReadValueAsButton();
        }

        private void OnJump(InputAction.CallbackContext context)
        {
            jump?.TryJump();
        }

        private void OnBasicAttackStarted(InputAction.CallbackContext context)
        {
            combat?.OnBasicAttackStarted();
        }

        private void OnWeaponSlot1(InputAction.CallbackContext context)
        {
            if (!context.performed) return;
            weaponHandler?.ToggleWeaponByIndex(0);
        }

        private void OnSkill1(InputAction.CallbackContext context)
        {
            meleeSkill?.ExecuteSkill01();
        }

        private void OnSkill2(InputAction.CallbackContext context)
        {
            meleeSkill?.ExecuteSkill02();
        }

        private void OnSkill3(InputAction.CallbackContext context)
        {
            meleeSkill?.ExecuteSkill03();
        }
        
        private void OnEsc(InputAction.CallbackContext context)
        {
            if (context.performed)
                EscDown = true;
        }

        private void OnTab(InputAction.CallbackContext context)
        {
            if (context.performed)
                TabDown = true;
        }

        public void ClearZoom()
        {
            ZoomInput = 0f;
        }
        #endregion
    }
}

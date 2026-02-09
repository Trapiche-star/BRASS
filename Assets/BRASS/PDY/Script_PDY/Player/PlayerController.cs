using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace BRASS
{
    /// CharacterController를 활용하여 플레이어의 이동, 중력, 클릭 및 키보드 입력을 제어하는 클래스
    public class PlayerController : MonoBehaviour
    {
        #region Variables
        [SerializeField] private float moveSpeed = 5f;            // 일반 걷기 이동 속도
        [SerializeField] private float fastRunSpeed = 8f;         // Shift 키를 누른 빠른 달기 속도
        [SerializeField] private float gravity = -9.81f;          // 캐릭터에게 적용될 중력 값
        [SerializeField] private float rotationSpeed = 15f;       // 캐릭터가 회전할 때의 부드러움 정도
        [SerializeField] private Transform cameraPivot;           // 카메라의 방향을 기준으로 이동하기 위한 참조값
        [SerializeField] private float clickStopDistance = 0.1f;  // 클릭 이동 시 목적지에 도달했다고 판단할 거리

        [Header("ClickMoveBlock")]
        [SerializeField] private float clickBlockedStopTime = 1f; // 벽 등에 막혔을 때 클릭 이동을 취소할 시간

        private PlayerInputHandler input;         // 입력 처리를 담당하는 스크립트 참조
        private PlayerState state;                // 현재 캐릭터의 상태(이동 중 등) 참조
        private CharacterController controller;   // 실제 물리 이동을 처리하는 컴포넌트
        private Animator animator;                // 애니메이션 재생 제어

        private Vector3 velocity;                 // 수직 속도(중력) 계산용 변수
        private Vector3 moveDirection;            // 평면상에서의 이동 방향

        private Vector3 clickDestination;         // 마우스 클릭으로 설정된 목적지 좌표
        private bool isClickMoving;               // 현재 클릭으로 인한 자동 이동 상태인지 여부

        private Vector3 lastMovePosition;         // 이전 프레임의 위치 (막힘 감지용)
        private float clickBlockedTime;           // 장애물에 막혀 이동하지 못한 누적 시간

        private bool wasAttackingLastFrame;       // 이전 프레임에 공격 상태였는지 추적하여 공격 종료 직후 프레임을 감지하기 위한 플래그  

        //디버그용 임시 변수
        private bool prevIsGrounded;
        private bool prevIsJumping;
        #endregion

        #region Unity Event Method
        private void Awake()
        {
            input = GetComponent<PlayerInputHandler>();
            state = GetComponent<PlayerState>();
            controller = GetComponent<CharacterController>();
            animator = GetComponentInChildren<Animator>();
        }

        private void Update()
        {
            HandleMovement();
        }
        #endregion

        #region Custom Method
        // 전체적인 이동 흐름(클릭 이동, 키보드 이동)을 제어한다
        private void HandleMovement()
        {
            if (input == null || state == null || cameraPivot == null) return;

            // 공격 종료 직후 프레임 처리
            if (wasAttackingLastFrame && !state.IsAttacking)
            {
                moveDirection = Vector3.zero;
                isClickMoving = false;
            }

            // 공격 중 이동 차단 (루트모션 전용 구간)
            if (state.IsAttacking)
            {
                ApplyGravity();
                wasAttackingLastFrame = true;
                return;
            }

            // 이동 입력 처리
            HandleClickMoveInput();            // 클릭 이동 입력을 처리하여 목적지 설정/취소를 수행한다
            CalculateClickMoveDirection();            // 클릭 목적지 기준 이동 방향을 계산한다
            CalculateKeyboardMoveDirection();            // 키보드 입력 기준 이동 방향을 계산한다
            ApplyGravity();            // 중력 누적 및 접지/점프 상태 동기화를 수행한다
            UpdateState();            // 이동/패스트런 등 상태값을 갱신한다
            ApplyNormalMovement();            // 최종 이동 벡터를 CharacterController에 적용한다

            // 이전 프레임의 공격 상태를 갱신한다
            wasAttackingLastFrame = state.IsAttacking;
        }

        // 마우스 좌클릭 시 목적지를 설정한다
        private void HandleClickMoveInput()
        {
            if (input.ClickMovePressed)
            {
                // 1. EventSystem이 있는지 먼저 확인
                if (EventSystem.current != null)
                {
                    // 2. 현재 마우스가 UI 위에 있는지 검사
                    if (EventSystem.current.IsPointerOverGameObject())
                    {
                        return; // UI 위라면 이동 로직 전체를 실행하지 않음
                    }
                }

                Ray ray = Camera.main.ScreenPointToRay(input.MousePosition);

                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    clickDestination = hit.point;
                    isClickMoving = true;
                    clickBlockedTime = 0f;
                    lastMovePosition = transform.position;
                }
            }

            if (input.IsKeyboardMove)
                isClickMoving = false;
        }

        // 클릭 목적지까지의 방향 벡터를 계산한다
        private void CalculateClickMoveDirection()
        {
            if (!isClickMoving) return;

            Vector3 dir = clickDestination - transform.position;
            dir.y = 0f;

            if (dir.magnitude <= clickStopDistance)
            {
                isClickMoving = false;
                moveDirection = Vector3.zero;
                return;
            }

            moveDirection = dir.normalized;
        }

        // 카메라가 보는 방향을 기준으로 키보드 이동 방향을 계산한다
        private void CalculateKeyboardMoveDirection()
        {
            if (isClickMoving) return;

            moveDirection = Vector3.zero;

            if (!input.IsKeyboardMove) return;

            Transform cam = Camera.main.transform;
            Vector3 forward = cam.forward;
            Vector3 right = cam.right;

            forward.y = 0f;
            right.y = 0f;

            moveDirection = (forward.normalized * input.MoveInput.y +
                             right.normalized * input.MoveInput.x).normalized;
        }

        // PlayerState에 현재 이동/패스트런 상태를 전달한다
        private void UpdateState()
        {
            state.IsMoving = moveDirection != Vector3.zero;

            state.IsFastRun = input.IsKeyboardMove &&
                              Keyboard.current != null &&
                              Keyboard.current.leftShiftKey.isPressed;
        }

        // 계산된 방향과 속도를 바탕으로 실제 이동을 적용한다
        private void ApplyNormalMovement()
        {
            float speed = state.IsFastRun ? fastRunSpeed : moveSpeed;

            if (state.IsMoving)
            {
                Vector3 before = transform.position;

                controller.Move((moveDirection * speed + velocity) * Time.deltaTime);

                if (isClickMoving)
                {
                    float moved = Vector3.Distance(transform.position, before);

                    if (moved < 0.001f)
                        clickBlockedTime += Time.deltaTime;
                    else
                        clickBlockedTime = 0f;

                    if (clickBlockedTime >= clickBlockedStopTime)
                    {
                        isClickMoving = false;
                        moveDirection = Vector3.zero;
                    }

                    lastMovePosition = transform.position;
                }

                Quaternion targetRot = Quaternion.LookRotation(moveDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
            }
            else
            {
                controller.Move(velocity * Time.deltaTime);
            }
        }

        // 중력을 적용하고 접지 및 점프 상태를 갱신한다
        private void ApplyGravity()
        {
            bool groundedNow = controller.isGrounded;

            if (groundedNow && velocity.y <= 0f)
            {
                state.IsGrounded = true;

                if (!prevIsGrounded)
                {
                    state.IsJumping = false;
                    state.JumpIndex = 0;                  
                }

                velocity.y = -2f;
            }
            else
            {
                state.IsGrounded = false;

                if (state.JumpIndex > 0)
                {
                    state.IsJumping = true;
                }
                else
                {
                    state.IsJumping = false;
                }
            }

            prevIsGrounded = state.IsGrounded;
            prevIsJumping = state.IsJumping;

            velocity.y += gravity * Time.deltaTime;
        }
        #endregion

        #region Public Method
        // 외부에서 수직 속도를 강제 설정 (주로 점프 기능에서 호출)
        public void SetVerticalVelocity(float y)
        {
            velocity.y = y; // 전달받은 수치를 수직 속도 값에 대입한다
        }

        // 외부 시스템에 의해 캐릭터를 강제로 물리 이동 (밀려남 등)
        public void MoveExternal(Vector3 delta)
        {
            controller.Move(delta); // 인자로 받은 벡터만큼 컨트롤러를 즉시 움직인다
        }
        #endregion
    }
}
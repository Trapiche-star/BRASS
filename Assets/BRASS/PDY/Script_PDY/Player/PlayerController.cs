using UnityEngine;
using UnityEngine.InputSystem;

namespace BRASS
{
    /// 플레이어 이동·중력·좌클릭 자동 이동·슬라이드 및 상태 결정을 담당한다
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : MonoBehaviour
    {
        #region Variables
        [SerializeField] private float moveSpeed = 5f;          // 기본 이동 속도
        [SerializeField] private float fastRunSpeed = 8f;       // 패스트런 이동 속도
        [SerializeField] private float slideSpeed = 10f;        // 슬라이딩 초기 속도
        [SerializeField] private float slideDeceleration = 15f; // 슬라이딩 감속 속도
        [SerializeField] private float gravity = -9.81f;        // 중력 가속도
        [SerializeField] private float rotationSpeed = 15f;     // 회전 보간 속도
        [SerializeField] private Transform cameraPivot;         // 이동 기준 카메라 피벗
        [SerializeField] private float stuckLimitTime = 3f;     // 자동 이동 막힘 허용 시간(초)

        private PlayerInputHandler input;                        // 입력 수집 담당
        private PlayerState state;                               // 플레이어 상태 저장소
        private CharacterController controller;                  // 캐릭터 이동 컴포넌트

        private Vector3 velocity;                                // 중력 누적 속도
        private Vector3 moveDirection;                           // 현재 이동 방향
        private Vector3 slideVelocity;                           // 슬라이딩 이동 벡터

        private Vector3 clickDestination;                        // 좌클릭 이동 목적지
        private bool isClickMoving;                              // 좌클릭 자동 이동 여부

        private float stuckTimer;                                // 자동 이동 막힘 누적 시간
        private Vector3 lastPosition;                            // 이전 프레임 위치
        #endregion

        #region Unity Event Method
        private void Awake()
        {
            input = GetComponent<PlayerInputHandler>();
            // PlayerInputHandler를 참조한다

            state = GetComponent<PlayerState>();
            // PlayerState를 참조한다

            controller = GetComponent<CharacterController>();
            // CharacterController를 참조한다
        }

        private void Update()
        {
            HandleMovement();
            // 매 프레임 이동과 행동을 처리한다
        }
        #endregion

        #region Custom Method
        // 이동·좌클릭 자동 이동·슬라이드를 종합 처리한다
        private void HandleMovement()
        {
            if (input == null || state == null || cameraPivot == null) return;
            // 만약 [필수 참조가 없으면] [이 메서드에서는 더 이상 처리하지 않는다]

            HandleClickMoveInput();
            // 좌클릭 자동 이동 입력을 처리한다

            CalculateMoveDirection();
            // 이동 방향을 계산한다

            ApplyGravity();
            // 중력을 누적한다

            UpdateState();
            // 이동 상태를 갱신한다

            HandleClickMoveStuck();
            // 좌클릭 자동 이동 중 막힘 상태를 감지한다

            if (input.SlidePressed && CanSlide())
            {
                state.IsSliding = true;
                // 만약 [슬라이드 입력이 들어왔고 조건을 만족하면] [슬라이딩 상태로 전환한다]
            }

            if (state.IsSliding)
            {
                ApplySlideMovement();
                // 만약 [슬라이딩 상태이면] [슬라이딩 이동을 처리한다]

                lastPosition = transform.position;
                // 프레임 종료 시 위치를 기록한다

                return;
                // 슬라이딩 중에는 일반 이동을 처리하지 않는다
            }

            ApplyNormalMovement();
            // 일반 이동을 처리한다

            lastPosition = transform.position;
            // 프레임 종료 시 위치를 기록한다
        }

        // 좌클릭 자동 이동 입력을 처리한다
        private void HandleClickMoveInput()
        {
            if (input.ClickMovePressed)
            {
                Ray ray = Camera.main.ScreenPointToRay(input.MousePosition);
                // 마우스 위치를 기준으로 레이를 생성한다

                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    clickDestination = hit.point;
                    isClickMoving = true;
                    stuckTimer = 0f;
                    lastPosition = transform.position;
                    // 만약 [지면에 레이가 닿으면] [좌클릭 자동 이동을 시작하고 상태를 초기화한다]
                }
            }

            if (input.IsKeyboardMove)
            {
                isClickMoving = false;
                stuckTimer = 0f;
                // 만약 [WASD 입력이 들어오면] [좌클릭 자동 이동을 즉시 중단한다]
            }
        }

        // 이동 방향을 계산한다
        private void CalculateMoveDirection()
        {
            if (isClickMoving)
            {
                Vector3 dir = clickDestination - transform.position;
                dir.y = 0f;

                if (dir.magnitude < 0.1f)
                {
                    isClickMoving = false;
                    moveDirection = Vector3.zero;
                    // 만약 [목적지에 도달했으면] [자동 이동을 종료한다]
                    return;
                }

                moveDirection = dir.normalized;
                // 좌클릭 목적지를 향해 이동 방향을 설정한다
                return;
            }

            moveDirection = Vector3.zero;
            // 이동 방향을 초기화한다

            if (!input.IsKeyboardMove) return;
            // 만약 [WASD 입력이 없으면] [이동 방향을 계산하지 않는다]

            Transform cam = Camera.main.transform;

            Vector3 forward = cam.forward;
            Vector3 right = cam.right;

            forward.y = 0f;
            right.y = 0f;

            moveDirection =
                (forward.normalized * input.MoveInput.y +
                 right.normalized * input.MoveInput.x).normalized;
            // 카메라 기준 이동 방향을 계산한다
        }

        // 좌클릭 자동 이동 중 막힘 상태를 감지한다
        private void HandleClickMoveStuck()
        {
            if (!isClickMoving) return;
            // 만약 [자동 이동 중이 아니면] [막힘 판정을 하지 않는다]

            float movedDistance = Vector3.Distance(transform.position, lastPosition);
            // 이전 프레임 대비 실제 이동 거리를 계산한다

            if (movedDistance < 0.01f)
            {
                stuckTimer += Time.deltaTime;
                // 만약 [이동이 거의 없으면] [막힘 시간을 누적한다]
            }
            else
            {
                stuckTimer = 0f;
                // 만약 [조금이라도 이동했다면] [막힘 상태를 해제한다]
            }

            if (stuckTimer >= stuckLimitTime)
            {
                isClickMoving = false;
                stuckTimer = 0f;
                moveDirection = Vector3.zero;
                // 만약 [막힘 상태가 일정 시간 지속되면] [자동 이동을 강제로 종료한다]
            }
        }

        // 이동 상태를 갱신한다
        private void UpdateState()
        {
            state.IsMoving = moveDirection != Vector3.zero;
            // 만약 [이동 방향이 존재하면] [이동 중 상태로 판단한다]

            state.IsFastRun =
                input.IsKeyboardMove &&
                Keyboard.current != null &&
                Keyboard.current.leftShiftKey.isPressed;
            // 만약 [이동 중이고 Shift가 눌려 있으면] [패스트런 상태로 판단한다]
        }

        // 일반 이동을 처리한다
        private void ApplyNormalMovement()
        {
            float speed = state.IsFastRun ? fastRunSpeed : moveSpeed;
            // 현재 상태에 따라 이동 속도를 결정한다

            if (state.IsMoving)
            {
                controller.Move((moveDirection * speed + velocity) * Time.deltaTime);
                // 이동 방향과 중력을 함께 적용한다

                if (moveDirection != Vector3.zero)
                {
                    Quaternion targetRot = Quaternion.LookRotation(moveDirection);
                    transform.rotation = Quaternion.Slerp(
                        transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
                    // 이동 방향이 존재할 때만 해당 방향을 바라보도록 회전시킨다
                }
            }
            else
            {
                controller.Move(velocity * Time.deltaTime);
                // 이동 입력이 없으면 중력만 적용한다
            }
        }

        // 슬라이딩 이동을 처리한다
        private void ApplySlideMovement()
        {
            if (slideVelocity == Vector3.zero)
            {
                slideVelocity = moveDirection.normalized * slideSpeed;
                // 슬라이딩 초기 속도를 설정한다
            }

            controller.Move((slideVelocity + velocity) * Time.deltaTime);
            // 슬라이딩 이동과 중력을 함께 적용한다

            slideVelocity = Vector3.Lerp(
                slideVelocity, Vector3.zero, slideDeceleration * Time.deltaTime);
            // 슬라이딩 속도를 점진적으로 감속한다
        }

        // 슬라이드 가능 여부를 판단한다
        private bool CanSlide()
        {
            if (!controller.isGrounded) return false;
            // 만약 [지면에 있지 않으면] [슬라이드를 허용하지 않는다]

            if (!state.IsMoving) return false;
            // 만약 [이동 중이 아니면] [슬라이드를 허용하지 않는다]

            if (state.IsSliding) return false;
            // 만약 [이미 슬라이딩 중이면] [중복 실행하지 않는다]

            return true;
        }

        // 중력을 누적한다
        private void ApplyGravity()
        {
            if (controller.isGrounded && velocity.y < 0f)
                velocity.y = -2f;
            // 만약 [지면에 붙어 있으면] [낙하 속도를 초기화한다]

            velocity.y += gravity * Time.deltaTime;
            // 매 프레임 중력을 누적한다
        }

        // 애니메이션 타이밍에 맞춰 슬라이딩 이동을 시작한다
        public void StartSlide(Vector3 direction)
        {
            if (direction == Vector3.zero) return;
            // 만약 [슬라이드 방향이 없으면] [슬라이딩을 시작하지 않는다]

            state.IsSliding = true;
            slideVelocity = direction.normalized * slideSpeed;
            // 슬라이딩 상태로 전환하고 초기 속도를 설정한다
        }

        // 애니메이션 타이밍에서 슬라이딩을 종료한다
        public void EndSlide()
        {
            state.IsSliding = false;
            slideVelocity = Vector3.zero;
            // 슬라이딩 상태를 해제하고 이동을 종료한다
        }
        #endregion
    }
}

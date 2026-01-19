using UnityEngine;
using UnityEngine.InputSystem;

namespace BRASS
{
    /// 플레이어 이동, 중력, 좌클릭 자동 이동과 슬라이딩 동작을 제어한다
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
        [SerializeField] private float stuckLimitTime = 1.5f;   // 자동 이동 막힘 허용 시간

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
            input = GetComponent<PlayerInputHandler>();   // 입력 처리 컴포넌트를 참조한다
            state = GetComponent<PlayerState>();          // 상태 저장 컴포넌트를 참조한다
            controller = GetComponent<CharacterController>(); // 캐릭터 컨트롤러를 참조한다
        }

        private void Update()
        {
            HandleMovement(); // 매 프레임 이동 로직을 처리한다
        }
        #endregion

        #region Custom Method
        // 이동, 자동 이동, 슬라이딩을 종합 처리한다
        private void HandleMovement()
        {
            if (input == null || state == null || cameraPivot == null) return;
            // 필수 참조가 없으면 이 프레임에서는 아무 처리도 하지 않는다

            HandleClickMoveInput();   // 좌클릭 자동 이동 입력을 처리한다
            CalculateMoveDirection(); // 이동 방향을 계산한다
            ApplyGravity();           // 중력을 누적한다
            UpdateState();            // 이동 상태를 갱신한다
            HandleClickMoveStuck();   // 자동 이동 막힘 여부를 판정한다

            if (input.SlidePressed)
            {
                if (CanSlide())
                {
                    state.IsSliding = true;
                    // 이동 중이면 기존 방식으로 슬라이딩을 시작한다
                }
                else if (!state.IsMoving && controller.isGrounded)
                {
                    Vector3 mouseDir = GetMouseSlideDirection();

                    if (mouseDir != Vector3.zero)
                    {
                        StartSlide(mouseDir);
                        // 제자리에서는 마우스 방향으로 슬라이딩을 시작한다
                    }
                }
            }

            if (state.IsSliding)
            {
                ApplySlideMovement();                 // 슬라이딩 이동을 처리한다
                lastPosition = transform.position;   // 현재 위치를 기록한다
                return;                               // 슬라이딩 중에는 일반 이동을 처리하지 않는다
            }

            ApplyNormalMovement();                   // 일반 이동을 처리한다
            lastPosition = transform.position;       // 현재 위치를 기록한다
        }

        // 좌클릭 자동 이동 입력을 처리한다
        private void HandleClickMoveInput()
        {
            if (state.IsSliding) return;
            // 슬라이딩 중에는 자동 이동 입력을 무시한다

            if (input.ClickMovePressed)
            {
                Ray ray = Camera.main.ScreenPointToRay(input.MousePosition);

                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    clickDestination = hit.point;
                    isClickMoving = true;
                    stuckTimer = 0f;
                    lastPosition = transform.position;
                    // 클릭 지점을 목적지로 설정하고 자동 이동을 시작한다
                }
            }

            if (input.IsKeyboardMove)
            {
                isClickMoving = false;
                stuckTimer = 0f;
                // 키보드 이동이 시작되면 자동 이동을 중단한다
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
                    // 목적지에 도달했으면 자동 이동을 종료한다
                    return;
                }

                moveDirection = dir.normalized;
                // 목적지를 향해 이동 방향을 설정한다
                return;
            }

            moveDirection = Vector3.zero;
            // 기본 이동 방향을 초기화한다

            if (!input.IsKeyboardMove) return;
            // 키보드 입력이 없으면 더 이상 계산하지 않는다

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

        // 자동 이동 중 막힘 상태를 감지한다
        private void HandleClickMoveStuck()
        {
            if (!isClickMoving) return;
            // 자동 이동 중이 아니면 막힘 판정을 하지 않는다

            float movedDistance = Vector3.Distance(transform.position, lastPosition);

            if (movedDistance < 0.02f)
                stuckTimer += Time.deltaTime;
            else
                stuckTimer = 0f;
            // 이동이 거의 없을 때만 막힘 시간을 누적한다

            if (stuckTimer >= stuckLimitTime)
            {
                isClickMoving = false;
                stuckTimer = 0f;
                moveDirection = Vector3.zero;
                // 막힘이 지속되면 자동 이동을 강제로 종료한다
            }
        }

        // 이동 상태를 갱신한다
        private void UpdateState()
        {
            state.IsMoving = moveDirection != Vector3.zero;
            // 이동 방향이 있으면 이동 중 상태로 판단한다

            state.IsFastRun =
                input.IsKeyboardMove &&
                Keyboard.current != null &&
                Keyboard.current.leftShiftKey.isPressed;
            // 이동 중이고 Shift가 눌려 있으면 패스트런 상태로 판단한다
        }

        // 일반 이동을 처리한다
        private void ApplyNormalMovement()
        {
            float speed = state.IsFastRun ? fastRunSpeed : moveSpeed;

            if (state.IsMoving)
            {
                controller.Move((moveDirection * speed + velocity) * Time.deltaTime);
                // 이동 방향과 중력을 함께 적용한다

                if (moveDirection != Vector3.zero)
                {
                    Quaternion targetRot = Quaternion.LookRotation(moveDirection);
                    transform.rotation = Quaternion.Slerp(
                        transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
                    // 이동 방향을 바라보도록 회전시킨다
                }
            }
            else
            {
                controller.Move(velocity * Time.deltaTime);
                // 이동이 없으면 중력만 적용한다
            }
        }

        // 슬라이딩 이동을 처리한다
        private void ApplySlideMovement()
        {
            if (slideVelocity == Vector3.zero)
            {
                slideVelocity = transform.forward * slideSpeed;
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
            // 지면에 있지 않으면 슬라이드를 허용하지 않는다

            if (!state.IsMoving) return false;
            // 이동 중이 아니면 슬라이드를 허용하지 않는다

            if (state.IsSliding) return false;
            // 이미 슬라이딩 중이면 중복 실행하지 않는다

            return true;
        }

        // 중력을 누적한다
        private void ApplyGravity()
        {
            if (controller.isGrounded && velocity.y < 0f)
                velocity.y = -2f;
            // 지면에 붙어 있으면 낙하 속도를 초기화한다

            velocity.y += gravity * Time.deltaTime;
            // 매 프레임 중력을 누적한다
        }

        // 제자리 슬라이딩용 마우스 방향을 계산한다
        private Vector3 GetMouseSlideDirection()
        {
            Ray ray = Camera.main.ScreenPointToRay(input.MousePosition);

            if (!Physics.Raycast(ray, out RaycastHit hit)) return Vector3.zero;
            // 레이가 닿지 않으면 방향을 만들지 않는다

            Vector3 dir = hit.point - transform.position;
            dir.y = 0f;

            if (dir.sqrMagnitude < 0.01f) return Vector3.zero;
            // 방향 벡터가 너무 작으면 슬라이딩을 시작하지 않는다

            return dir.normalized;
        }

        // 애니메이션 타이밍에 맞춰 슬라이딩을 시작한다
        public void StartSlide(Vector3 direction)
        {
            if (direction == Vector3.zero) return;
            // 슬라이드 방향이 없으면 이 메서드에서는 더 이상 처리하지 않는다

            isClickMoving = false;
            // 슬라이딩 시작 시 자동 이동을 즉시 중단한다

            moveDirection = Vector3.zero;
            // 기존 이동 방향을 제거해 상태 해석 충돌을 방지한다

            direction.y = 0f;
            transform.rotation = Quaternion.LookRotation(direction);
            // 슬라이드 방향으로 캐릭터를 즉시 회전시킨다

            state.IsSliding = true;
            slideVelocity = direction.normalized * slideSpeed;
            // 슬라이딩 상태로 전환하고 초기 속도를 설정한다
        }

        // 애니메이션 타이밍에서 슬라이딩을 종료한다
        public void EndSlide()
        {
            state.IsSliding = false;
            slideVelocity = Vector3.zero;
            // 슬라이딩 상태와 속도를 초기화한다
        }
        #endregion
    }
}

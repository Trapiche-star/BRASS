using UnityEngine;
using UnityEngine.InputSystem;

namespace BRASS
{
    // 캐릭터 컨트롤러 컴포넌트를 필수적으로 포함하도록 강제함
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : MonoBehaviour
    {
        #region Variables
        [SerializeField] private float moveSpeed = 5f;            // 일반 걷기 이동 속도
        [SerializeField] private float fastRunSpeed = 8f;         // Shift 키를 누른 빠른 달기 속도
        [SerializeField] private float gravity = -9.81f;          // 캐릭터에게 적용될 중력 값
        [SerializeField] private float rotationSpeed = 15f;       // 캐릭터가 회전할 때의 부드러움 정도
        [SerializeField] private Transform cameraPivot;           // 카메라의 방향을 기준으로 이동하기 위한 참조값
        [SerializeField] private float clickStopDistance = 0.1f;  // 클릭 이동 시 목적지에 도달했다고 판단할 거리

        [Header("Slide")]
        [SerializeField] private float slideDistance = 2.5f;      // 슬라이딩으로 이동할 총 거리
        [SerializeField] private float slideSpeed = 6f;           // 슬라이딩 도중의 이동 속도

        [Header("ClickMoveBlock")]
        [SerializeField] private float clickBlockedStopTime = 1f; // 벽 등에 막혔을 때 클릭 이동을 취소할 시간

        private PlayerInputHandler input;        // 입력 처리를 담당하는 스크립트 참조
        private PlayerState state;               // 현재 캐릭터의 상태(이동 중, 슬라이딩 중 등) 참조
        private CharacterController controller;  // 실제 물리 이동을 처리하는 컴포넌트
        private Animator animator;               // 애니메이션 재생 제어

        private Vector3 velocity;                 // 수직 속도(중력) 계산용 변수
        private Vector3 moveDirection;            // 평면상에서의 이동 방향
        private Vector3 pendingSlideDirection;    // 슬라이딩 애니메이션 시작 전 결정된 방향 저장
        private Vector3 slideDirection;           // 슬라이딩 중 실제 이동에 사용할 방향

        private Vector3 clickDestination;         // 마우스 클릭으로 설정된 목적지 좌표
        private bool isClickMoving;               // 현재 클릭으로 인한 자동 이동 상태인지 여부

        private Vector3 lastMovePosition;         // 이전 프레임의 위치 (막힘 감지용)
        private float clickBlockedTime;           // 장애물에 막혀 이동하지 못한 누적 시간

        private float slideMovedDistance;         // 슬라이딩 시작 후 현재까지 이동한 거리
        private bool slideInputConsumed;          // 슬라이딩 키 입력이 중복 처리되지 않도록 체크
        #endregion

        #region Unity Event Method
        private void Awake()
        {
            // 필요한 컴포넌트들을 캐싱하여 초기화
            input = GetComponent<PlayerInputHandler>();
            state = GetComponent<PlayerState>();
            controller = GetComponent<CharacterController>();
            animator = GetComponentInChildren<Animator>();
        }

        private void Update()
        {
            // 매 프레임마다 이동 로직 처리
            HandleMovement();
        }
        #endregion

        #region Custom Method
        // 전체적인 이동 흐름(슬라이딩, 클릭 이동, 키보드 이동)을 제어
        private void HandleMovement()
        {
            // 필수 컴포넌트가 없으면 실행하지 않음
            if (input == null || state == null || cameraPivot == null) return;

            // 1. 슬라이딩 입력 확인 (연속 입력 방지 로직 포함)
            if (input.SlidePressed && !slideInputConsumed)
            {
                slideInputConsumed = true;        // 입력 소비 처리
                isClickMoving = false;           // 자동 이동 취소
                moveDirection = Vector3.zero;     // 일반 이동 초기화

                pendingSlideDirection = GetSlideDirection(); // 슬라이딩 방향 결정
                animator.SetTrigger("Slide");               // 애니메이션 트리거 작동

                return; // 슬라이딩 시작 프레임이므로 아래 로직 건너뜀
            }

            // 슬라이딩 키를 떼면 다시 입력 가능 상태로 변경
            if (!input.SlidePressed)
                slideInputConsumed = false;

            // 2. 현재 슬라이딩 상태인 경우의 처리
            if (state.IsSliding)
            {
                ApplyGravity(); // 슬라이딩 중에도 중력 적용

                float remaining = slideDistance - slideMovedDistance; // 남은 슬라이딩 거리
                if (remaining > 0f)
                {
                    // 이번 프레임에 이동할 거리 계산 (남은 거리와 속도 중 작은 값 선택)
                    float moveThisFrame = Mathf.Min(remaining, slideSpeed * Time.deltaTime);
                    
                    Vector3 slideMove = slideDirection * moveThisFrame;     // 슬라이드 시작 시 확정된 방향을 기준으로 이동시킨다
                    controller.Move(slideMove + velocity * Time.deltaTime);
                    slideMovedDistance += moveThisFrame; // 이동 거리 누적
                }
                else
                {
                    // 슬라이딩 이동이 끝나면 중력만 적용
                    controller.Move(velocity * Time.deltaTime);
                }

                return; // 슬라이딩 중에는 일반 이동 로직을 타지 않음
            }

            // 3. 일반 이동 처리 흐름
            HandleClickMoveInput();           // 클릭 입력 확인
            CalculateClickMoveDirection();    // 클릭 목적지 방향 계산
            CalculateKeyboardMoveDirection(); // 키보드 입력 방향 계산
            ApplyGravity();                   // 중력 계산
            UpdateState();                    // 상태값(IsMoving 등) 갱신
            ApplyNormalMovement();            // 최종 물리 이동 적용
        }

        // 마우스 좌클릭 시 목적지를 설정
        private void HandleClickMoveInput()
        {
            if (input.ClickMovePressed)
            {
                // 화면상의 마우스 위치에서 레이(Ray)를 쏨
                Ray ray = Camera.main.ScreenPointToRay(input.MousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    clickDestination = hit.point; // 레이가 닿은 곳을 목적지로 설정
                    isClickMoving = true;

                    clickBlockedTime = 0f;        // 막힘 타이머 초기화
                    lastMovePosition = transform.position;
                }
            }

            // 키보드 이동 입력이 들어오면 클릭 이동은 즉시 취소
            if (input.IsKeyboardMove)
                isClickMoving = false;
        }

        // 클릭 목적지까지의 방향 벡터 계산
        private void CalculateClickMoveDirection()
        {
            if (!isClickMoving) return;

            Vector3 dir = clickDestination - transform.position;
            dir.y = 0f; // 높이 차이는 무시

            // 목적지에 충분히 가까워졌는지 확인
            if (dir.magnitude <= clickStopDistance)
            {
                isClickMoving = false;
                moveDirection = Vector3.zero;
                return;
            }

            moveDirection = dir.normalized; // 방향 벡터 정규화
        }

        // 카메라가 보는 방향을 기준으로 키보드 이동 방향 계산
        private void CalculateKeyboardMoveDirection()
        {
            if (isClickMoving) return; // 클릭 이동 중이면 키보드 계산 안 함

            moveDirection = Vector3.zero;
            if (!input.IsKeyboardMove) return;

            Transform cam = Camera.main.transform;

            // 카메라의 앞방향과 오른쪽 방향 추출 (y축 평면화)
            Vector3 forward = cam.forward;
            Vector3 right = cam.right;
            forward.y = 0f;
            right.y = 0f;

            // 입력값(WASD)과 카메라 방향을 조합하여 최종 이동 방향 결정
            moveDirection = (forward.normalized * input.MoveInput.y +
                             right.normalized * input.MoveInput.x).normalized;
        }

        // PlayerState 스크립트에 현재 상태를 전달
        private void UpdateState()
        {
            state.IsMoving = moveDirection != Vector3.zero; // 이동 중인지 판단

            // 키보드 이동 중이고 Left Shift를 누르고 있다면 패스트런 상태
            state.IsFastRun = input.IsKeyboardMove &&
                             Keyboard.current != null &&
                             Keyboard.current.leftShiftKey.isPressed;
        }

        // 계산된 방향과 속도를 바탕으로 실제 이동 컴포넌트 작동
        private void ApplyNormalMovement()
        {
            // 현재 상태에 따른 이동 속도 선택
            float speed = state.IsFastRun ? fastRunSpeed : moveSpeed;

            if (state.IsMoving)
            {
                Vector3 before = transform.position; // 이동 전 위치 저장

                // 실제 이동 실행
                controller.Move((moveDirection * speed + velocity) * Time.deltaTime);

                // 클릭 이동 중 장애물에 걸렸는지 체크
                if (isClickMoving)
                {
                    float moved = Vector3.Distance(transform.position, before);
                    if (moved < 0.001f) // 거의 움직이지 못했다면
                        clickBlockedTime += Time.deltaTime;
                    else
                        clickBlockedTime = 0f;

                    // 특정 시간 동안 막혀있으면 이동 포기
                    if (clickBlockedTime >= clickBlockedStopTime)
                    {
                        isClickMoving = false;
                        moveDirection = Vector3.zero;
                    }

                    lastMovePosition = transform.position;
                }

                // 이동 방향으로 부드럽게 캐릭터 회전
                Quaternion targetRot = Quaternion.LookRotation(moveDirection);
                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    targetRot,
                    rotationSpeed * Time.deltaTime);
            }
            else
            {
                // 이동하지 않을 때도 중력은 계속 적용
                controller.Move(velocity * Time.deltaTime);
            }
        }

        // 바닥에 닿아있지 않을 때 아래로 가속도 적용
        private void ApplyGravity()
        {
            // 바닥에 닿아있으면 수직 속도 초기화 (살짝 -2를 주어 바닥 밀착 유지)
            if (controller.isGrounded && velocity.y < 0f)
                velocity.y = -2f;

            velocity.y += gravity * Time.deltaTime;
        }

        // 슬라이딩을 어느 방향으로 할지 결정 (이동 중이면 이동 방향, 아니면 마우스 방향)
        private Vector3 GetSlideDirection()
        {
            if (input.IsKeyboardMove && moveDirection != Vector3.zero)
                return moveDirection.normalized;
            // 이동 중이면 현재 이동 방향을 슬라이드 방향으로 사용한다

            Ray ray = Camera.main.ScreenPointToRay(input.MousePosition);
            // 마우스 포인터 위치 기준으로 카메라에서 레이를 쏜다

            Plane plane = new Plane(Vector3.up, transform.position);
            // 캐릭터가 서 있는 높이의 수평 평면을 만든다

            if (plane.Raycast(ray, out float distance))
            {
                Vector3 hitPoint = ray.GetPoint(distance);
                // 레이가 평면과 만나는 실제 월드 좌표를 계산한다

                Vector3 dir = hitPoint - transform.position;
                dir.y = 0f;

                if (dir.sqrMagnitude > 0.01f)
                    return dir.normalized;
                // 마우스 포인터가 가리키는 방향으로 슬라이드한다
            }
            
            return transform.forward;   // 예외 상황에서는 캐릭터 정면을 사용한다
        }

        // 애니메이션의 특정 시점(Animation Event)에서 호출되어 실제 슬라이딩 이동 시작
        public void StartSlideFromPending()
        {
            StartSlide(pendingSlideDirection);
            pendingSlideDirection = Vector3.zero;
        }

        // 슬라이딩 상태 변수 및 데이터 초기화
        public void StartSlide(Vector3 direction)
        {
            if (direction == Vector3.zero) return;
            // 방향이 없으면 슬라이드를 시작하지 않는다

            moveDirection = Vector3.zero;
            // 일반 이동과 충돌하지 않도록 이동 방향을 초기화한다

            direction.y = 0f;

            slideDirection = direction.normalized;
            // 슬라이딩 중 실제 이동에 사용할 방향을 확정한다

            transform.rotation = Quaternion.LookRotation(slideDirection);
            // 캐릭터 비주얼을 슬라이딩 방향으로 즉시 회전시킨다

            slideMovedDistance = 0f;
            // 새 슬라이드 시작이므로 누적 이동 거리를 초기화한다

            state.IsSliding = true;
            // 슬라이딩 상태로 전환한다
        }

        // 애니메이션이 끝날 때 호출되어 슬라이딩 상태 종료
        public void EndSlide()
        {
            state.IsSliding = false;
        }
        #endregion
    }
}
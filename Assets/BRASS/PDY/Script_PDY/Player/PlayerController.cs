using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace BRASS
{
    /// <summary>
    /// CharacterController 기반 이동·중력·슬라이딩·클릭이동을 처리하고
    /// 물리 접지 결과를 PlayerState(IsGrounded/IsJumping)에 동기화한다
    /// </summary>
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
        [SerializeField] private AnimationCurve slideMoveCurve;   // 슬라이드 애니메이션 진행도(0~1)에 따른 이동 비율을 정의한다
        [SerializeField] private float slideTotalDistance = 2.5f; // 슬라이드 애니메이션 전체 동안 이동할 총 거리를 담당한다

        private float lastSlideCurveValue;        // 이전 프레임의 이동 비율 값을 저장한다

        [Header("ClickMoveBlock")]
        [SerializeField] private float clickBlockedStopTime = 1f; // 벽 등에 막혔을 때 클릭 이동을 취소할 시간

        private PlayerInputHandler input;         // 입력 처리를 담당하는 스크립트 참조
        private PlayerState state;                // 현재 캐릭터의 상태(이동 중, 슬라이딩 중 등) 참조
        private CharacterController controller;   // 실제 물리 이동을 처리하는 컴포넌트
        private Animator animator;                // 애니메이션 재생 제어

        private Vector3 velocity;                 // 수직 속도(중력) 계산용 변수
        private Vector3 moveDirection;            // 평면상에서의 이동 방향
        private Vector3 pendingSlideDirection;    // 슬라이딩 애니메이션 시작 전 결정된 방향 저장
        private Vector3 slideDirection;           // 슬라이딩 중 실제 이동에 사용할 방향

        private Vector3 clickDestination;         // 마우스 클릭으로 설정된 목적지 좌표
        private bool isClickMoving;               // 현재 클릭으로 인한 자동 이동 상태인지 여부

        private Vector3 lastMovePosition;         // 이전 프레임의 위치 (막힘 감지용)
        private float clickBlockedTime;           // 장애물에 막혀 이동하지 못한 누적 시간

        private bool slideInputConsumed;          // 슬라이딩 키 입력이 중복 처리되지 않도록 체크

        //디버그용 임시 변수
        private bool prevIsGrounded;
        private bool prevIsJumping;
        #endregion

        #region Unity Event Method
        private void Awake()
        {
            input = GetComponent<PlayerInputHandler>();
            // 같은 오브젝트에서 입력 핸들러를 캐싱한다

            state = GetComponent<PlayerState>();
            // 같은 오브젝트에서 상태 컨테이너를 캐싱한다

            controller = GetComponent<CharacterController>();
            // 같은 오브젝트에서 CharacterController를 캐싱한다

            animator = GetComponentInChildren<Animator>();
            // 하위 계층에서 Animator를 탐색하여 캐싱한다
        }

        private void Update()
        {
            HandleMovement();
            // 매 프레임 이동(슬라이딩/클릭/키보드) 흐름을 처리한다
        }
        #endregion

        #region Custom Method
        // 전체적인 이동 흐름(슬라이딩, 클릭 이동, 키보드 이동)을 제어한다
        private void HandleMovement()
        {
            if (input == null || state == null || cameraPivot == null) return;
            // 만약 필수 참조가 없으면 이 프레임에서는 이동 로직을 수행하지 않는다

            if (input.SlidePressed && !slideInputConsumed)
            {
                slideInputConsumed = true;
                // 이번 프레임의 슬라이드 입력을 소비 처리하여 중복 처리를 방지한다

                isClickMoving = false;
                // 슬라이딩을 시작하면 클릭 이동은 즉시 취소한다

                moveDirection = Vector3.zero;
                // 슬라이딩 시작 프레임에는 일반 이동 벡터를 제거한다

                pendingSlideDirection = GetSlideDirection();
                // 슬라이딩에 사용할 방향을 먼저 확정해 둔다

                if (animator != null)
                    animator.SetTrigger("Slide");
                // Animator가 존재하면 슬라이딩 트리거를 발생시킨다

                return;
                // 슬라이딩 시작 프레임이므로 아래 일반 이동 흐름을 실행하지 않는다
            }

            if (!input.SlidePressed)
                slideInputConsumed = false;
            // 만약 슬라이드 키를 떼면 다음 입력부터 다시 슬라이딩이 가능하도록 플래그를 해제한다

            if (state.IsSliding)
            {
                ApplyGravity();
                // 슬라이딩 중에도 중력과 접지 상태 동기화를 유지한다

                AnimatorStateInfo info = animator.GetCurrentAnimatorStateInfo(0);
                // 현재 재생 중인 애니메이션 상태 정보를 가져온다

                float normalizedTime = Mathf.Clamp01(info.normalizedTime);
                // 애니메이션 진행도를 0~1로 제한하여 커브 입력 범위를 보장한다

                float curveValue = slideMoveCurve.Evaluate(normalizedTime);
                // 현재 진행도에서의 이동 비율을 커브로부터 평가한다

                float deltaCurve = curveValue - lastSlideCurveValue;
                // 이전 프레임 대비 증가한 비율만 추출하여 이번 프레임 이동량을 만든다

                float moveDistance = deltaCurve * slideTotalDistance;
                // 커브 증가량을 총 이동 거리로 환산하여 실제 이동 거리를 산출한다

                if (moveDistance > 0f)
                {
                    Vector3 slideMove = slideDirection * moveDistance;
                    // 슬라이딩 방향 기준으로 이동 벡터를 계산한다

                    controller.Move(slideMove + velocity * Time.deltaTime);
                    // 슬라이딩 이동과 중력 이동을 합산하여 CharacterController에 적용한다
                }

                lastSlideCurveValue = curveValue;
                // 다음 프레임 이동량 계산을 위해 현재 커브 값을 저장한다

                return;
                // 슬라이딩 중에는 일반 이동 흐름을 수행하지 않는다
            }

            HandleClickMoveInput();
            // 클릭 이동 입력을 처리하여 목적지 설정/취소를 수행한다

            CalculateClickMoveDirection();
            // 클릭 목적지 기준 이동 방향을 계산한다

            CalculateKeyboardMoveDirection();
            // 키보드 입력 기준 이동 방향을 계산한다

            ApplyGravity();
            // 중력 누적 및 접지/점프 상태 동기화를 수행한다

            UpdateState();
            // 이동/패스트런 등 상태값을 갱신한다

            ApplyNormalMovement();
            // 최종 이동 벡터를 CharacterController에 적용한다
        }

        // 마우스 좌클릭 시 목적지를 설정한다
        private void HandleClickMoveInput()
        {
            if (input.ClickMovePressed)
            {
                Ray ray = Camera.main.ScreenPointToRay(input.MousePosition);
                // 마우스 스크린 좌표에서 레이를 생성한다

                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    clickDestination = hit.point;
                    // 레이가 맞은 지점을 이동 목적지로 설정한다

                    isClickMoving = true;
                    // 클릭 이동 상태를 활성화한다

                    clickBlockedTime = 0f;
                    // 막힘 누적 시간을 초기화한다

                    lastMovePosition = transform.position;
                    // 막힘 감지를 위해 현재 위치를 저장한다
                }
            }

            if (input.IsKeyboardMove)
                isClickMoving = false;
            // 만약 키보드 이동 입력이 들어오면 클릭 이동을 즉시 취소한다
        }

        // 클릭 목적지까지의 방향 벡터를 계산한다
        private void CalculateClickMoveDirection()
        {
            if (!isClickMoving) return;
            // 만약 클릭 이동 상태가 아니면 방향 계산을 하지 않는다

            Vector3 dir = clickDestination - transform.position;
            // 목적지까지의 방향 벡터를 계산한다

            dir.y = 0f;
            // 평면 이동만 수행하므로 높이 차이는 제거한다

            if (dir.magnitude <= clickStopDistance)
            {
                isClickMoving = false;
                // 만약 목적지에 충분히 가까우면 클릭 이동을 종료한다

                moveDirection = Vector3.zero;
                // 이동 방향을 제거하여 정지 상태로 전환한다

                return;
                // 목적지 도달 프레임에서는 추가 계산을 수행하지 않는다
            }

            moveDirection = dir.normalized;
            // 목적지 방향을 단위 벡터로 정규화한다
        }

        // 카메라가 보는 방향을 기준으로 키보드 이동 방향을 계산한다
        private void CalculateKeyboardMoveDirection()
        {
            if (isClickMoving) return;
            // 만약 클릭 이동 중이면 키보드 이동 계산으로 덮어쓰지 않는다

            moveDirection = Vector3.zero;
            // 키보드 계산 프레임에서는 기본값을 먼저 초기화한다

            if (!input.IsKeyboardMove) return;
            // 만약 키보드 이동 입력이 없으면 방향 계산을 하지 않는다

            Transform cam = Camera.main.transform;
            // 실제 메인 카메라 Transform을 가져온다

            Vector3 forward = cam.forward;
            Vector3 right = cam.right;
            // 카메라의 전/우 방향 벡터를 가져온다

            forward.y = 0f;
            right.y = 0f;
            // 지면 기준 이동을 위해 y 성분을 제거한다

            moveDirection = (forward.normalized * input.MoveInput.y +
                             right.normalized * input.MoveInput.x).normalized;
            // 입력 벡터(WASD)와 카메라 방향을 조합하여 최종 이동 방향을 만든다
        }

        // PlayerState에 현재 이동/패스트런 상태를 전달한다
        private void UpdateState()
        {
            state.IsMoving = moveDirection != Vector3.zero;
            // 이동 방향이 존재하면 이동 중 상태로 갱신한다

            state.IsFastRun = input.IsKeyboardMove &&
                              Keyboard.current != null &&
                              Keyboard.current.leftShiftKey.isPressed;
            // 키보드 이동 중이고 Shift가 눌려있으면 패스트런 상태로 갱신한다
        }

        // 계산된 방향과 속도를 바탕으로 실제 이동을 적용한다
        private void ApplyNormalMovement()
        {
            float speed = state.IsFastRun ? fastRunSpeed : moveSpeed;
            // 현재 상태에 따라 이동 속도를 선택한다

            if (state.IsMoving)
            {
                Vector3 before = transform.position;
                // 클릭 막힘 감지를 위해 이동 전 위치를 저장한다

                controller.Move((moveDirection * speed + velocity) * Time.deltaTime);
                // 이동 벡터와 중력 벡터를 합산하여 CharacterController에 적용한다

                if (isClickMoving)
                {
                    float moved = Vector3.Distance(transform.position, before);
                    // 이번 프레임 실제 이동 거리를 계산한다

                    if (moved < 0.001f)
                        clickBlockedTime += Time.deltaTime;
                    else
                        clickBlockedTime = 0f;
                    // 만약 거의 움직이지 못하면 막힘 시간을 누적하고, 움직였으면 막힘 시간을 초기화한다

                    if (clickBlockedTime >= clickBlockedStopTime)
                    {
                        isClickMoving = false;
                        // 만약 일정 시간 이상 막히면 클릭 이동을 포기한다

                        moveDirection = Vector3.zero;
                        // 클릭 이동을 종료하므로 이동 방향을 제거한다
                    }

                    lastMovePosition = transform.position;
                    // 다음 프레임 비교를 위해 현재 위치를 저장한다
                }

                Quaternion targetRot = Quaternion.LookRotation(moveDirection);
                // 이동 방향을 바라보는 목표 회전을 계산한다

                transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
                // 현재 회전에서 목표 회전으로 부드럽게 보간한다
            }
            else
            {
                controller.Move(velocity * Time.deltaTime);
                // 이동하지 않을 때도 중력 이동은 계속 적용한다
            }
        }

        // 중력을 적용하고 접지 및 점프 상태를 갱신한다
        private void ApplyGravity()
        {
            bool groundedNow = controller.isGrounded;
            // 이번 프레임의 실제 접지 결과

            // 착지 확정: 공중 → 지면으로 전환되는 순간
            if (groundedNow && velocity.y <= 0f)
            {
                state.IsGrounded = true;

                if (!prevIsGrounded)
                {
                    // 실제 착지 프레임 단 1회

                    state.IsJumping = false;
                    // 점프 펄스 종료

                    state.JumpIndex = 0;
                    // 점프 단계 리셋 (착지 확정 시점에서만)

                    Debug.Log("[Gravity] 착지 확정");
                }

                velocity.y = -2f;
                // 바닥 밀착용 하강 속도 유지
            }
            else
            {
                state.IsGrounded = false;

                if (state.JumpIndex > 0)
                {
                    state.IsJumping = true;
                    // 점프 단계가 존재하는 동안 점프 상태 유지

                    if (!prevIsJumping)
                    {
                        // 점프 진입 순간 단 1회
                        Debug.Log($"[Gravity] 점프 진입 | JumpIndex={state.JumpIndex}");
                    }
                }
                else
                {
                    state.IsJumping = false;
                }
            }

            // 이전 상태 갱신 (프레임 마지막)
            prevIsGrounded = state.IsGrounded;
            prevIsJumping = state.IsJumping;

            velocity.y += gravity * Time.deltaTime;
            // 중력 누적
        }


        // 슬라이딩 방향을 카메라 전방 기준으로 결정한다
        private Vector3 GetSlideDirection()
        {
            Transform camTransform = Camera.main.transform;
            // 현재 메인 카메라 Transform을 가져온다

            Vector3 forward = camTransform.forward;
            // 카메라 전방 방향을 가져온다

            forward.y = 0f;
            // 지면 기준 이동을 위해 수직 성분을 제거한다

            if (forward.sqrMagnitude < 0.01f)
                return Vector3.zero;
            // 만약 유효한 방향이 아니면 슬라이딩을 시작하지 않도록 제로 벡터를 반환한다

            return forward.normalized;
            // 카메라 방향 기준 슬라이딩 방향을 반환한다
        }

        // Animation Event 시점에 호출되어 슬라이딩 이동을 시작한다
        public void StartSlideFromPending()
        {
            StartSlide(pendingSlideDirection);
            // 미리 저장해 둔 방향으로 슬라이딩을 시작한다

            pendingSlideDirection = Vector3.zero;
            // 시작에 사용한 방향을 초기화하여 다음 슬라이드 준비 상태로 만든다
        }

        // 슬라이딩 상태 변수 및 데이터 초기화
        public void StartSlide(Vector3 direction)
        {
            if (direction == Vector3.zero) return;
            // 만약 방향이 없으면 슬라이딩을 시작하지 않는다

            moveDirection = Vector3.zero;
            // 슬라이딩과 일반 이동이 충돌하지 않도록 이동 방향을 제거한다

            direction.y = 0f;
            // 슬라이딩은 지면 기준이므로 y 성분을 제거한다

            slideDirection = direction.normalized;
            // 슬라이딩 중 이동에 사용할 방향을 확정한다

            transform.rotation = Quaternion.LookRotation(slideDirection);
            // 비주얼을 슬라이딩 방향으로 즉시 회전시킨다

            lastSlideCurveValue = 0f;
            // 커브 기반 이동량 계산을 위해 누적 값을 초기화한다

            state.IsSliding = true;
            // 슬라이딩 상태로 전환한다
        }

        // 슬라이딩 종료 Animation Event 시점에 호출된다
        public void EndSlide()
        {
            state.IsSliding = false;
            // 슬라이딩 상태를 종료한다
        }
        #endregion

        #region External Action Bridge
        // 외부 액션(PlayerJump 등)에서 점프 애니메이션 트리거를 요청한다
        public void TriggerJumpAnimation(int index)
        {
            if (animator == null) return;
            // 만약 Animator가 없으면 점프 트리거 요청을 수행하지 않는다

            animator.ResetTrigger("Jump");
            // 만약 남아 있을 수 있는 Jump 트리거가 있다면 먼저 제거한다

            animator.SetInteger("JumpIndex", index);
            // 점프 단계 값을 애니메이터 파라미터로 전달한다

            animator.SetTrigger("Jump");
            // 점프 트리거를 1회 발생시킨다
        }

        // 지면 착지 시 애니메이터의 점프 인덱스를 0으로 초기화한다
        public void ResetJumpIndex()
        {
            if (animator == null) return;
            // 만약 Animator가 없으면 초기화 요청을 수행하지 않는다

            animator.SetInteger("JumpIndex", 0);
            // 다음 점프에서 1단부터 재생되도록 JumpIndex를 리셋한다
        }

        // 외부 액션(PlayerJump 등)에서 수직 속도를 설정하기 위한 통로
        public void SetVerticalVelocity(float value)
        {
            velocity.y = value;
            // 외부에서 전달된 값으로 수직 속도를 즉시 교체한다
        }
        #endregion
    }
}

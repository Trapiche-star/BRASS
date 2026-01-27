using UnityEngine;
using UnityEngine.InputSystem;

namespace BRASS
{
    /// <summary>
    /// CharacterController 기반의 이동 및 물리 처리를 담당하는 클래스
    /// </summary>
    public class PlayerController : MonoBehaviour
    {
        #region Variables
        [SerializeField] private float moveSpeed = 5f; // 기본적인 이동 속도
        [SerializeField] private float fastRunSpeed = 8f; // 고속 이동(달리기) 시의 속도
        [SerializeField] private float gravity = -9.81f; // 캐릭터에 적용되는 중력 수치
        [SerializeField] private float rotationSpeed = 15f; // 이동 방향으로 캐릭터가 회전하는 속도
        [SerializeField] private Transform cameraPivot; // 카메라 시점의 기준이 되는 피벗 트랜스폼
        [SerializeField] private float clickStopDistance = 0.1f; // 클릭 이동 시 목적지에 도달했다고 판단할 거리

        [Header("Slide")]
        [SerializeField] private AnimationCurve slideMoveCurve; // 슬라이딩 시 가속도를 제어하는 애니메이션 커브
        [SerializeField] private float slideTotalDistance = 2.5f; // 슬라이딩으로 이동할 최대 거리

        [Header("ClickMoveBlock")]
        [SerializeField] private float clickBlockedStopTime = 1f; // 장애물에 막혔을 때 클릭 이동을 강제 종료할 시간

        private PlayerInputHandler input; // 플레이어의 입력을 처리하는 컴포넌트 참조
        private PlayerState state; // 캐릭터의 현재 상태(공격, 이동 등)를 관리하는 컴포넌트 참조
        private CharacterController controller; // 물리적 이동을 담당하는 캐릭터 컨트롤러 컴포넌트
        private Animator animator; // 애니메이션 제어를 위한 애니메이터 컴포넌트

        private Vector3 velocity; // 수직 이동(중력 등)을 저장하는 벡터
        private Vector3 moveDirection; // 현재 프레임의 수평 이동 방향 벡터
        private Vector3 pendingSlideDirection; // 슬라이딩 애니메이션 대기 중 저장된 방향
        private Vector3 slideDirection; // 실제 슬라이딩이 진행되는 방향

        private Vector3 clickDestination; // 마우스 클릭으로 지정된 이동 목적지
        private bool isClickMoving; // 클릭 이동 모드가 활성화되어 있는지 여부

        private float lastSlideCurveValue; // 이전 프레임의 슬라이딩 커브 계산값
        private float clickBlockedTime; // 클릭 이동 중 막힌 상태가 지속된 시간
        private bool slideInputConsumed; // 슬라이딩 입력이 연속으로 처리되는 것을 방지하기 위한 플래그        

        private bool prevIsGrounded; // 이전 프레임의 접지 상태 기록
        private bool prevIsJumping; // 이전 프레임의 점프 상태 기록
        #endregion

        #region Unity Event Method
        private void Awake()
        {
            input = GetComponent<PlayerInputHandler>(); // 입력 처리기 연결
            state = GetComponent<PlayerState>(); // 상태 관리기 연결
            controller = GetComponent<CharacterController>(); // 캐릭터 컨트롤러 연결
            animator = GetComponentInChildren<Animator>(); // 애니메이터 연결
        }

        private void Update()
        {
            HandleMovement(); // 매 프레임 캐릭터의 이동 로직을 갱신함
        }
        #endregion

        #region Custom Method
        // 이동 입력 처리 및 물리 연산의 전반적인 흐름을 제어함
        private void HandleMovement()
        {
            // 필수적인 컴포넌트 참조가 비어있다면 로직을 수행하지 않음
            if (input == null || state == null || cameraPivot == null) return;           

            // 슬라이딩 입력이 감지되었을 때 상태를 전환하고 애니메이션을 트리거함
            if (input.SlidePressed && !slideInputConsumed)
            {
                slideInputConsumed = true; // 입력 중복 처리 방지
                isClickMoving = false; // 클릭 이동 중단
                moveDirection = Vector3.zero; // 일반 이동 벡터 초기화

                pendingSlideDirection = GetSlideDirection(); // 시점 기준 슬라이딩 방향 예약

                // 애니메이터가 존재하면 슬라이딩 트리거 실행
                if (animator != null)
                    animator.SetTrigger("Slide");

                return;
            }

            // 슬라이딩 입력 버튼이 해제되면 소모 플래그를 리셋함
            if (!input.SlidePressed)
                slideInputConsumed = false;

            // 현재 슬라이딩 중이라면 애니메이션 커브 데이터에 기반해 물리적 위치를 이동시킴
            if (state.IsSliding)
            {
                ApplyGravity(); // 슬라이딩 중 중력 반영

                AnimatorStateInfo info = animator.GetCurrentAnimatorStateInfo(0); // 현재 슬라이딩 애니메이션 정보 획득
                float normalizedTime = Mathf.Clamp01(info.normalizedTime); // 애니메이션 진행도 보정

                float curveValue = slideMoveCurve.Evaluate(normalizedTime); // 현재 시간대의 커브 값 추출
                float deltaCurve = curveValue - lastSlideCurveValue; // 이전 프레임과의 변위차 계산
                float moveDistance = deltaCurve * slideTotalDistance; // 실제 이동 거리 산출

                // 커브 상 이동 거리가 발생한 경우 실제 물리 이동을 수행함
                if (moveDistance > 0f)
                {
                    Vector3 slideMove = slideDirection * moveDistance; // 슬라이딩 방향 벡터 조합
                    controller.Move(slideMove + velocity * Time.deltaTime); // 최종 물리 이동
                }

                lastSlideCurveValue = curveValue; // 현재 커브 기록값 갱신
                return;
            }

            HandleClickMoveInput(); // 마우스 클릭에 의한 목적지 설정 처리
            CalculateClickMoveDirection(); // 클릭 이동 방향 산출
            CalculateKeyboardMoveDirection(); // 키보드 입력에 따른 시점 기준 방향 산출
            ApplyGravity(); // 중력 가속도 연산
            UpdateState(); // 이동 및 달리기 상태 동기화
            ApplyNormalMovement(); // 일반 이동 물리 반영            
        }

        // 최종 결정된 방향과 속도를 이용하여 실제 물리 이동 및 회전을 수행함
        private void ApplyNormalMovement()
        {
            float speed = state.IsFastRun ? fastRunSpeed : moveSpeed; // 상태에 따른 속도 선택

            // 이동 방향이 존재할 때만 로직을 실행함
            if (state.IsMoving)
            {
                Vector3 before = transform.position; // 이동 전 위치 기록
                controller.Move((moveDirection * speed + velocity) * Time.deltaTime); // 물리 이동 실행

                // 클릭 이동 모드일 경우 장애물에 의한 정체 현상을 감지함
                if (isClickMoving)
                {
                    float moved = Vector3.Distance(transform.position, before); // 실질적 이동 거리 계산

                    // 이동량이 극히 적다면 막힘 시간을 누적함
                    if (moved < 0.001f)
                        clickBlockedTime += Time.deltaTime;
                    else
                        clickBlockedTime = 0f;

                    // 특정 시간 이상 막혀있다면 목적지 도달 불가능으로 판단하여 정지함
                    if (clickBlockedTime >= clickBlockedStopTime)
                    {
                        isClickMoving = false; // 클릭 이동 모드 종료
                        moveDirection = Vector3.zero; // 이동 벡터 리셋
                    }
                }

                // 회전 잠금 상태가 아니며 이동 의지가 확실할 때 이동 방향으로 캐릭터를 회전시킴
                if (moveDirection.sqrMagnitude > 0.0001f)
                {
                    Quaternion targetRot = Quaternion.LookRotation(moveDirection); // 목표 쿼터니언 생성
                    transform.rotation = Quaternion.Slerp(
                        transform.rotation,
                        targetRot,
                        rotationSpeed * Time.deltaTime
                    ); // 부드러운 회전 보간 실행
                }
            }
            else
            {
                controller.Move(velocity * Time.deltaTime); // 정지 중에도 중력에 의한 수직 속도는 반영함
            }
            
        }

        // 지면 접지 상태를 확인하고 중력 및 수직 속도를 물리 엔진에 반영함
        private void ApplyGravity()
        {
            bool groundedNow = controller.isGrounded; // 현재 컨트롤러의 접지 여부 확인

            // 캐릭터가 땅에 닿아있고 낙하 중이라면 수직 속도를 보정함
            if (groundedNow && velocity.y <= 0f)
            {
                state.IsGrounded = true; // 접지 상태 활성화

                // 이전 프레임에 공중이었다면 점프 상태를 해제함
                if (!prevIsGrounded)
                {
                    state.IsJumping = false; // 점프 상태 종료
                    state.JumpIndex = 0; // 점프 인덱스 초기화
                }

                velocity.y = -2f; // 바닥에 안정적으로 밀착시키기 위한 최소 하중
            }
            else
            {
                state.IsGrounded = false; // 공중 상태 활성화
                state.IsJumping = state.JumpIndex > 0; // 점프 횟수가 있다면 점프 중으로 간주
            }

            prevIsGrounded = state.IsGrounded; // 상태값 기록 갱신
            prevIsJumping = state.IsJumping;

            velocity.y += gravity * Time.deltaTime; // 중력 가속도 적용
        }

        // 마우스 클릭 시 월드 좌표로 변환하여 이동 목적지를 설정함
        private void HandleClickMoveInput()
        {
            // 클릭 이동 입력이 활성화되었을 때 마우스 위치로 레이를 발사함
            if (input.ClickMovePressed)
            {
                Ray ray = Camera.main.ScreenPointToRay(input.MousePosition); // 화면 좌표를 월드 광선으로 변환

                // 광선이 어딘가에 충돌했다면 그 좌표를 목적지로 결정함
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    clickDestination = hit.point; // 목적지 좌표 저장
                    isClickMoving = true; // 클릭 이동 모드 시작
                    clickBlockedTime = 0f; // 막힘 카운트 리셋
                }
            }

            // 키보드 입력을 시도하면 마우스 이동 모드를 즉시 해제함
            if (input.IsKeyboardMove)
                isClickMoving = false;
        }

        // 현재 위치와 클릭 목적지 사이의 방향 벡터를 계산함
        private void CalculateClickMoveDirection()
        {
            if (!isClickMoving) return; // 클릭 이동 상태가 아니면 건너뜀

            Vector3 dir = clickDestination - transform.position; // 거리 벡터 계산
            dir.y = 0f; // 수평 이동만 고려함

            // 설정된 정지 거리 이내로 좁혀지면 도착으로 간주함
            if (dir.magnitude <= clickStopDistance)
            {
                isClickMoving = false; // 상태 해제
                moveDirection = Vector3.zero; // 벡터 초기화
                return;
            }

            moveDirection = dir.normalized; // 정규화된 방향 벡터 설정
        }

        // 키보드 입력값을 카메라 시점 기준으로 변환하여 이동 방향을 결정함
        private void CalculateKeyboardMoveDirection()
        {
            if (isClickMoving) return; // 클릭 이동이 우선이면 키보드 계산 안 함

            moveDirection = Vector3.zero; // 초기값 설정
            if (!input.IsKeyboardMove) return; // 키보드 입력이 없으면 리턴

            Transform cam = Camera.main.transform; // 카메라 참조
            Vector3 forward = cam.forward; // 카메라 정면 방향
            Vector3 right = cam.right; // 카메라 우측 방향

            forward.y = 0f; // 수평 평면 벡터화
            right.y = 0f;

            // 카메라 기준 수평 벡터들에 입력값을 조합하여 방향 결정
            moveDirection = (forward.normalized * input.MoveInput.y +
                             right.normalized * input.MoveInput.x).normalized;
        }

        // 캐릭터의 현재 이동 및 고속 주행 상태를 PlayerState 컴포넌트에 동기화함
        private void UpdateState()
        {
            state.IsMoving = moveDirection != Vector3.zero; // 이동 벡터 유무에 따른 이동 판정

            // 키보드 이동 중이며 쉬프트 키가 눌려있을 때만 고속 이동 상태로 간주함
            state.IsFastRun = input.IsKeyboardMove &&
                              Keyboard.current != null &&
                              Keyboard.current.leftShiftKey.isPressed;
        }

        // 카메라 정면을 기준으로 슬라이딩을 수행할 방향을 산출함
        private Vector3 GetSlideDirection()
        {
            Transform camTransform = Camera.main.transform; // 카메라 위치 정보
            Vector3 forward = camTransform.forward; // 카메라 정면 방향
            forward.y = 0f; // y축 무시

            // 벡터 크기가 거의 없다면 방향 없음으로 반환함
            if (forward.sqrMagnitude < 0.01f)
                return Vector3.zero;

            return forward.normalized; // 정규화된 정면 방향 반환
        }

        // 애니메이션 이벤트 등에서 대기 중이던 방향으로 슬라이딩을 강제 개시함
        public void StartSlideFromPending()
        {
            StartSlide(pendingSlideDirection); // 대기 방향으로 슬라이딩 실행
            pendingSlideDirection = Vector3.zero; // 대기열 비우기
        }

        // 입력된 방향을 기준으로 슬라이딩 상태를 활성화하고 캐릭터를 회전시킴
        public void StartSlide(Vector3 direction)
        {
            if (direction == Vector3.zero) return; // 방향이 유효하지 않으면 취소

            moveDirection = Vector3.zero; // 일반 이동 방향 초기화
            direction.y = 0f; // 수평축 고정

            slideDirection = direction.normalized; // 슬라이딩 방향 확정
            transform.rotation = Quaternion.LookRotation(slideDirection); // 해당 방향을 즉시 바라보게 함

            lastSlideCurveValue = 0f; // 커브 연산값 초기화
            state.IsSliding = true; // 슬라이딩 상태 활성화
        }

        // 슬라이딩 상태를 비활성화함
        public void EndSlide()
        {
            state.IsSliding = false; // 상태 해제
        }        

        // 애니메이터에 점프 트리거와 인덱스를 설정하여 애니메이션을 재생함
        public void TriggerJumpAnimation(int index)
        {
            if (animator == null) return; // 애니메이터가 없으면 무시

            animator.ResetTrigger("Jump"); // 이전 점프 트리거 초기화
            animator.SetInteger("JumpIndex", index); // 점프 단계(인덱스) 설정
            animator.SetTrigger("Jump"); // 애니메이션 재생 트리거
        }

        // 애니메이터의 점프 관련 인덱스 데이터를 초기화함
        public void ResetJumpIndex()
        {
            if (animator == null) return;
            animator.SetInteger("JumpIndex", 0); // 인덱스 변수 리셋
        }

        // 캐릭터의 현재 수직 속도값을 직접 수정함 (점프력 부여 등)
        public void SetVerticalVelocity(float value)
        {
            velocity.y = value; // 속도값 덮어쓰기
        }

        // 외부 시스템(Combat 등)에서 캐릭터를 월드 기준으로 이동시키기 위한 최소 인터페이스
        public void MoveExternal(Vector3 worldDelta)
        {
            controller.Move(worldDelta);
        }
        #endregion
    }
}
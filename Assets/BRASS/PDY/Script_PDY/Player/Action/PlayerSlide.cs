using UnityEngine;

namespace BRASS
{
    /// 슬라이드 입력을 감지하고 애니메이션 곡선에 맞춰 물리적 이동과 상태를 제어하는 클래스
    public class PlayerSlide : MonoBehaviour
    {
        #region Variables
        [Header("Slide Data")]
        [SerializeField] private AnimationCurve slideMoveCurve; // 슬라이드 진행도별 속도 변화를 정의하는 커브
        [SerializeField] private float slideTotalDistance = 2.5f; // 슬라이드 1회당 총 이동 거리

        [Header("References")]
        [SerializeField] private PlayerInputHandler input; // 사용자의 키 입력을 수신하는 핸들러
        [SerializeField] private PlayerState state; // 현재 플레이어의 상태 플래그 데이터
        [SerializeField] private CharacterController controller; // 실제 물리 이동을 처리하는 컴포넌트
        [SerializeField] private Animator animator; // 슬라이드 애니메이션 제어를 위한 컴포넌트
        [SerializeField] private PlayerController playerController; // 기존 이동 강제 중단용

        private Vector3 slideDirection; // 슬라이드가 시작될 때 결정된 전진 방향
        private float lastCurveValue;   // 프레임 간 이동량 계산을 위한 직전 커브 값
        private bool slideInputConsumed; // 연속 입력에 의한 중복 실행 방지용 플래그

        #endregion

        #region Unity Event Method        
        private void Awake()
        {
            // 자동 참조 할당
            if (input == null) input = GetComponentInParent<PlayerInputHandler>(); // 부모 객체에서 입력 핸들러 캐싱
            if (state == null) state = GetComponentInParent<PlayerState>(); // 부모 객체에서 상태 데이터 참조
            if (controller == null) controller = GetComponentInParent<CharacterController>(); // 부모의 캐릭터 컨트롤러 할당
            if (animator == null) animator = GetComponentInChildren<Animator>(); // 하위 오브젝트에서 애니메이터 탐색
            if (playerController == null) playerController = GetComponentInParent<PlayerController>();  // 부모 객체에서 플레이어 컨트롤러 캐싱

        }

        // 매 프레임 입력 처리 및 슬라이드 이동 갱신
        private void Update()
        {            
            if (!state.IsSliding) HandleSlideInput();   // 슬라이드 상태가 아닐 때만 입력 처리 수행 
            UpdateSlideMovement(); // 슬라이드 상태일 경우 위치 업데이트 수행
        }
        #endregion

        #region Custom Method
        // 입력 장치로부터 슬라이드 명령을 수신하고 조건을 검사하여 실행
        private void HandleSlideInput()
        {
            if (input == null || state == null) return; // 필수 참조가 없으면 로직을 수행하지 않는다

            if (!input.SlidePressed) // 슬라이드 버튼이 눌리지 않은 상태라면
            {
                slideInputConsumed = false; // 입력 소비 플래그를 초기화하여 재입력 가능하게 한다
                return;
            }

            if (slideInputConsumed) return; // 이미 이번 입력이 처리되었다면 중복 실행을 차단한다

            if (!CanSlide()) return; // 현재 상태가 슬라이드를 할 수 없는 조건이라면 종료한다

            slideInputConsumed = true; // 입력을 소비한 것으로 표시하여 연타를 방지한다            

            state.IsMoving = false;     // WASD 이동 논리 종료
            state.IsFastRun = false;    // 달리기 상태 제거
            state.SlideRequested = true;    // 슬라이드 요청 플래그 설정                            

            CancelInvoke(nameof(ForceResetSlideRequest)); // 혹시 모르니 이전 인보크 취소
            Invoke(nameof(ForceResetSlideRequest), 1.2f);

            Vector3 dir = GetCameraForwardDirection(); // 카메라가 바라보는 수평 정면 방향을 계산
            if (dir == Vector3.zero) return; // 방향 계산이 불가능하면 스킬 실행을 취소한다

            slideDirection = dir; // 결정된 방향을 멤버 변수에 저장

            if (animator != null) animator.SetTrigger("Slide"); // 애니메이터의 슬라이드 트리거를 활성화한다
        }

        // 강제 초기화 메서드
        private void ForceResetSlideRequest()
        {
            if (state.SlideRequested)
            {
                Debug.LogWarning("[Slide] 애니메이션 이벤트 미발생으로 인해 SlideRequested 강제 해제");
                state.SlideRequested = false;
            }
        }

        // 현재 캐릭터의 상태가 슬라이드 가능한지 여부 판단
        private bool CanSlide()
        {
            if (state.IsSliding) return false; // 이미 슬라이드 중이라면 다시 실행할 수 없다
            if (!state.IsGrounded) return false; // 공중에 떠 있는 상태에서는 슬라이드가 불가능하다
            if (state.IsJumping) return false; // 점프 동작 중에는 슬라이드로 전환할 수 없다
            if (state.IsAttacking) return false; // 공격 동작 수행 중에는 슬라이드 입력을 무시한다

            return true; // 모든 조건을 통과하면 사용 가능으로 판단한다
        }

        // 애니메이션 이벤트: 슬라이드 물리 이동이 시작되는 시점에 호출
        public void BeginSlide(Vector3 direction)
        {
            // 정상적으로 이벤트가 들어오면 안전장치 취소
            CancelInvoke(nameof(ForceResetSlideRequest));

            state.SlideRequested = false; // 슬라이드 진입 확정 → 요청 해제

            if (direction == Vector3.zero) return; // 유효한 방향이 전달되지 않으면 이동을 시작하지 않는다

            slideDirection = direction.normalized; // 방향 벡터를 정규화하여 저장
            transform.rotation = Quaternion.LookRotation(slideDirection);   // 캐릭터를 슬라이드 방향으로 즉시 회전시킴

            lastCurveValue = 0f;    // 커브 누적값 초기화
            state.IsSliding = true; // 슬라이드 상태 플래그 설정
            state.IsInputMovementLocked = true; // 일반 이동 입력이 물리 이동을 방해하지 못하도록 잠금
        }

        // 애니메이션 이벤트: 슬라이드 동작이 완전히 끝나는 시점에 호출
        public void EndSlide()
        {
            Debug.Log("[Slide] EndSlide - 슬라이드 종료 처리");

            if (state == null) return;

            state.IsSliding = false;                // 슬라이딩 상태 반드시 종료
            state.IsInputMovementLocked = false;    // 입력 락 반드시 해제
            state.IsAttacking = false; 

            lastCurveValue = 0f;                    // (선택) 다음 슬라이드 대비 초기화
            slideDirection = Vector3.zero;          // (선택)

            Debug.Log($"[Slide] IsSliding={state.IsSliding}, InputLocked={state.IsInputMovementLocked}");
        }

        // 슬라이드 진행 시간에 따라 커브를 평가하여 캐릭터를 실제로 이동
        private void UpdateSlideMovement()
        {
            if (!state.IsSliding) return; // 슬라이드 상태가 아니라면 이동 로직을 수행하지 않는다
            if (animator == null) return; // 애니메이터 참조가 손실되었다면 중단한다

            AnimatorStateInfo info = animator.GetCurrentAnimatorStateInfo(0); // 현재 재생 중인 애니메이션 정보 획득

            // 현재 재생 중인 애니메이션이 "Slide"가 맞는지 확인 (중요!)
            if (!info.IsName("Slide") && !info.IsTag("Slide"))
            {
                // 만약 트랜지션 중이라 이름이 다르면 이동이 안 될 수 있습니다.
                // 애니메이터의 상태 이름을 확인하거나 Tag를 설정하세요.
            }

            float normalizedTime = Mathf.Clamp01(info.normalizedTime); // 0~1 사이의 진행도 값 추출
            float curveValue = slideMoveCurve.Evaluate(normalizedTime); // 진행도에 따른 커브 상의 거리 비율 계산
            float delta = curveValue - lastCurveValue; // 이전 프레임과 현재 프레임 사이의 비율 차이 계산

            if (delta > 0f) // 이동해야 할 차이값이 양수라면
            {
                Vector3 move = slideDirection * (delta * slideTotalDistance); // 방향과 거리 수치를 곱해 실제 이동 벡터 생성
                // Debug.Log($"[Slide] Moving: {move.magnitude}, Delta: {delta}");
                controller.Move(move); // 캐릭터 컨트롤러를 통해 물리 이동 적용
            }

            lastCurveValue = curveValue; // 다음 프레임 계산을 위해 현재 커브 값을 저장
        }

        // 메인 카메라의 시야를 기준으로 수평 전방 벡터 반환
        private Vector3 GetCameraForwardDirection()
        {
            Camera cam = Camera.main; // 씬의 메인 카메라 참조
            if (cam == null) return Vector3.zero; // 카메라가 없으면 영벡터 반환

            Vector3 forward = cam.transform.forward; // 카메라의 시선 방향 획득
            forward.y = 0f; // 수직 성분을 제거하여 평면상의 방향만 남김

            if (forward.sqrMagnitude < 0.01f) return Vector3.zero; // 벡터가 너무 작아 방향을 정할 수 없으면 종료

            return forward.normalized; // 정규화된 방향 벡터 반환
        }
        #endregion
    }
}
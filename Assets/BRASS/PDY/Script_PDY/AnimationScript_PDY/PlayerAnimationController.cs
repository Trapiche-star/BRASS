using UnityEngine;

namespace BRASS
{
    /// PlayerAnimationController
    /// 플레이어 애니메이션 중앙 제어 허브
    /// 입력/이동 상태를 종합하여 Animator 파라미터만 제어한다
    public class PlayerAnimationController : MonoBehaviour
    {
        #region Variables
        [SerializeField] private Animator animator;                 // 애니메이션 재생 담당
        [SerializeField] private IdleAnimation idleAnimation;       // Idle 연출 판단 담당
        [SerializeField] private MovementAnimation movementAnimation; // 이동 상태 판단 담당

        private int hashIsMoving;    // 이동 여부 파라미터
        private int hashIsFastRun;   // 패스트런 여부 파라미터
        private int hashIsSliding;   // 슬라이딩 여부 파라미터
        private int hashIdleDwarf;   // Idle_Alt 트리거
        #endregion

        #region Unity Event Method
        private void Awake()
        {
            if (animator == null)
                animator = GetComponentInChildren<Animator>();
            // 만약 [직접 지정하지 않았다면] [자식 Animator를 자동 탐색한다]

            // Animator 파라미터 해시 캐싱
            hashIsMoving = Animator.StringToHash("IsMoving");
            hashIsFastRun = Animator.StringToHash("IsFastRun");
            hashIsSliding = Animator.StringToHash("IsSliding");
            hashIdleDwarf = Animator.StringToHash("Idle_Dwarf");
        }

        private void Update()
        {
            UpdateAnimationState(); // 매 프레임 애니메이션 상태 갱신
        }
        #endregion

        #region Custom Method
        // 현재 상태를 판단하여 Animator 파라미터 적용
        private void UpdateAnimationState()
        {
            if (animator == null || movementAnimation == null) return;
            // 만약 [필수 참조가 없으면] [이 프레임 처리를 중단한다]

            // 1. 슬라이딩 우선
            if (movementAnimation.IsSliding)
            {
                ApplySliding(); // 만약 [슬라이딩 중이면] [슬라이딩 애니메이션을 적용한다]
                return;
            }

            // 2. 패스트런
            if (movementAnimation.IsFastRun)
            {
                ApplyFastRun(); // 만약 [패스트런 중이면] [패스트런 애니메이션을 적용한다]
                return;
            }

            // 3. 일반 이동
            if (movementAnimation.IsMoving)
            {
                ApplyMove(); // 만약 [이동 중이면] [이동 애니메이션을 적용한다]
                return;
            }

            // 4. Idle
            ApplyIdle(); // 만약 [아무 입력도 없으면] [Idle 상태를 적용한다]
        }

        // Idle 상태 적용
        private void ApplyIdle()
        {
            animator.SetBool(hashIsSliding, false); // 슬라이딩 해제
            animator.SetBool(hashIsFastRun, false); // 패스트런 해제
            animator.SetBool(hashIsMoving, false);  // 이동 해제

            if (idleAnimation == null) return; // 만약 [Idle 모듈이 없으면] [연출을 생략한다]

            idleAnimation.SetIdleState(true); // Idle 상태 전달

            if (!idleAnimation.ShouldPlayIdleAlt()) return;
            // 만약 [Idle_Alt 조건이 아니면] [트리거를 발동하지 않는다]

            TriggerIdleAlt(); // Idle_Dwarf 트리거 실행
        }

        // 이동 상태 적용
        private void ApplyMove()
        {
            animator.SetBool(hashIsSliding, false); // 슬라이딩 해제
            animator.SetBool(hashIsFastRun, false); // 패스트런 해제
            animator.SetBool(hashIsMoving, true);   // 이동 활성

            if (idleAnimation == null) return; // 만약 [Idle 모듈이 없으면] [상태 전달을 생략한다]
            idleAnimation.SetIdleState(false); // Idle 해제
        }

        // 패스트런 상태 적용
        private void ApplyFastRun()
        {
            animator.SetBool(hashIsSliding, false); // 슬라이딩 해제
            animator.SetBool(hashIsFastRun, true);  // 패스트런 활성
            animator.SetBool(hashIsMoving, true);   // 이동 활성

            if (idleAnimation == null) return; // 만약 [Idle 모듈이 없으면] [상태 전달을 생략한다]
            idleAnimation.SetIdleState(false); // Idle 해제
        }

        // 슬라이딩 상태 적용
        private void ApplySliding()
        {
            animator.SetBool(hashIsSliding, true);  // 슬라이딩 활성
            animator.SetBool(hashIsFastRun, false); // 패스트런 해제
            animator.SetBool(hashIsMoving, true);   // 이동 활성

            if (idleAnimation == null) return; // 만약 [Idle 모듈이 없으면] [상태 전달을 생략한다]
            idleAnimation.SetIdleState(false); // Idle 해제
        }

        // Idle_Alt 트리거 발동
        private void TriggerIdleAlt()
        {
            animator.SetTrigger(hashIdleDwarf); // Idle_Dwarf 트리거 실행
        }
        #endregion

        #region Property
        public Animator Animator => animator; // 외부 접근용 Animator
        #endregion
    }
}

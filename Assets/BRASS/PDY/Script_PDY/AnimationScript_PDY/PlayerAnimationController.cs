using UnityEngine;

namespace BRASS
{
    /// PlayerAnimationController
    /// PlayerState를 기반으로 Animator 파라미터만 제어하는 연출 전용 컨트롤러
    public class PlayerAnimationController : MonoBehaviour
    {
        #region Variables
        [SerializeField] private Animator animator;           // 애니메이션 재생 담당
        [SerializeField] private PlayerState state;           // 플레이어 상태 참조
        [SerializeField] private IdleAnimation idleAnimation; // Idle 연출 담당

        private int hashIsMoving;   // 이동 여부 파라미터
        private int hashIsFastRun;  // 패스트런 여부 파라미터
        private int hashIsSliding;  // 슬라이딩 여부 파라미터
        private int hashIdleAlt;    // Idle_Alt 트리거
        #endregion

        #region Unity Event Method
        private void Awake()
        {
            if (animator == null)
                animator = GetComponentInChildren<Animator>();
            // 만약 [Animator가 지정되지 않았으면] [자식에서 자동 탐색한다]

            if (state == null)
                state = GetComponentInParent<PlayerState>();
            // 만약 [State가 지정되지 않았으면] [부모(Player)에서 탐색한다]

            hashIsMoving = Animator.StringToHash("IsMoving");
            hashIsFastRun = Animator.StringToHash("IsFastRun");
            hashIsSliding = Animator.StringToHash("IsSliding");
            hashIdleAlt = Animator.StringToHash("Idle_Dwarf");
        }

        private void Update()
        {
            UpdateAnimator(); // 매 프레임 상태를 Animator에 반영한다
        }
        #endregion

        #region Custom Method
        // PlayerState를 기준으로 Animator 파라미터를 갱신한다
        private void UpdateAnimator()
        {
            if (animator == null || state == null) return;
            // 만약 [필수 참조가 없으면] [이 프레임 처리를 중단한다]

            animator.SetBool(hashIsSliding, state.IsSliding);
            // 만약 [슬라이딩 상태이면] [슬라이딩 애니메이션을 활성화한다]

            animator.SetBool(hashIsFastRun, state.IsFastRun);
            // 만약 [패스트런 상태이면] [패스트런 애니메이션을 활성화한다]

            animator.SetBool(hashIsMoving, state.IsMoving);
            // 만약 [이동 중이면] [이동 애니메이션을 활성화한다]

            UpdateIdle();
        }

        // Idle 연출을 처리한다
        private void UpdateIdle()
        {
            if (idleAnimation == null) return;
            // 만약 [Idle 모듈이 없으면] [연출을 처리하지 않는다]

            idleAnimation.SetIdleState(state.IsIdle);
            // 현재 Idle 상태를 IdleAnimation에 전달한다

            if (!idleAnimation.ShouldPlayIdleAlt()) return;
            // 만약 [Idle_Alt 실행 조건이 아니면] [트리거를 실행하지 않는다]

            animator.SetTrigger(hashIdleAlt);
            // Idle_Alt 트리거를 실행한다
        }
        #endregion

        #region Property
        public Animator Animator => animator; // 외부 접근용 Animator
        #endregion
    }
}

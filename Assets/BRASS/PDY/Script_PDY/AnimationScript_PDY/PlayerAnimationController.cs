using UnityEngine;

namespace BRASS
{
    /// <summary>
    /// PlayerState를 기반으로 Animator 파라미터와 점프·이동·Idle 연출을 제어한다
    /// </summary>
    public class PlayerAnimationController : MonoBehaviour
    {
        #region Variables
        [SerializeField] private Animator animator;
        // 애니메이션 재생을 담당

        [SerializeField] private PlayerState state;
        // 플레이어 상태 참조를 담당

        [SerializeField] private IdleAnimation idleAnimation;
        // Idle 변형 애니메이션 타이밍 제어를 담당

        private int hashIsMoving;
        // 이동 여부 파라미터 해시

        private int hashIsSliding;
        // 슬라이딩 상태 파라미터 해시

        private int hashFastRun;
        // 패스트런 애니메이션 배율 파라미터 해시

        private int hashIsGrounded;
        // 지면 접촉 여부 파라미터 해시

        private int hashIsJumping;
        // 점프 상태 유지 여부 파라미터 해시

        private int hashJumpIndex;
        // 점프 단계(1단/2단) 파라미터 해시

        private int hashIdleDwarf;
        // Idle_Dwarf 트리거 파라미터 해시
        #endregion

        #region Unity Event Method
        private void Awake()
        {
            if (animator == null)
                animator = GetComponentInChildren<Animator>();
            // Animator가 지정되지 않았으면 자식 오브젝트에서 자동 탐색한다

            if (state == null)
                state = GetComponentInParent<PlayerState>();
            // PlayerState가 지정되지 않았으면 부모 오브젝트에서 자동 탐색한다

            hashIsMoving = Animator.StringToHash("IsMoving");
            hashIsSliding = Animator.StringToHash("IsSliding");
            hashFastRun = Animator.StringToHash("FastRun");
            hashIsGrounded = Animator.StringToHash("IsGrounded");
            hashIsJumping = Animator.StringToHash("IsJumping");
            hashJumpIndex = Animator.StringToHash("JumpIndex");
            hashIdleDwarf = Animator.StringToHash("Idle_Dwarf");
            // Animator에서 사용할 모든 파라미터 해시를 초기화한다
        }

        private void Update()
        {
            UpdateAnimator();
            // 매 프레임 PlayerState 값을 Animator에 반영한다
        }
        #endregion

        #region Custom Method
        // PlayerState 값을 Animator 파라미터로 변환하여 전달한다
        private void UpdateAnimator()
        {
            if (animator == null || state == null) return;
            // 필수 참조가 없으면 이 프레임에서는 애니메이션을 갱신하지 않는다

            animator.SetBool(hashIsMoving, state.IsMoving);
            // 현재 이동 중인지 여부를 전달한다

            animator.SetBool(hashIsSliding, state.IsSliding);
            // 슬라이딩 상태 여부를 전달한다

            animator.SetBool(hashIsGrounded, state.IsGrounded);
            // 캐릭터가 지면에 닿아 있는지 여부를 전달한다

            animator.SetBool(hashIsJumping, state.IsJumping);
            // 현재 점프 상태인지 여부를 전달한다

            //animator.SetInteger(hashJumpIndex, state.JumpIndex);
            // 현재 점프 단계(0=없음, 1=1단, 2=2단)를 전달한다

            animator.SetFloat(
                hashFastRun,
                state.IsFastRun ? 1.2f : 1f
            );
            // 패스트런 상태일 때만 애니메이션 재생 속도를 증가시킨다

            if (idleAnimation != null)
            {
                idleAnimation.SetIdleState(state.IsIdle);
                // 현재 플레이어가 논리적으로 Idle 상태인지 IdleAnimation에 전달한다

                if (idleAnimation.ShouldPlayIdleAlt())
                {
                    animator.SetTrigger(hashIdleDwarf);
                    // Idle 유지 시간이 충족되면 Idle_Dwarf 트리거를 1회 발생시킨다
                }
            }
        }
        #endregion
    }
}

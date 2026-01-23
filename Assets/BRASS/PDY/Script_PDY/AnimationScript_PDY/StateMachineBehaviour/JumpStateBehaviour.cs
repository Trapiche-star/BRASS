/*using UnityEngine;

namespace BRASS
{
    /// <summary>
    /// 점프 애니메이션 상태의 진입과 이탈 시점을 기준으로
    /// PlayerState의 IsJumping 수명을 관리한다
    /// </summary>
    public class JumpStateBehaviour : StateMachineBehaviour
    {
        private PlayerState state;
        // 점프 상태(IsJumping)를 제어하기 위한 PlayerState 참조

        public override void OnStateEnter(
            Animator animator,
            AnimatorStateInfo stateInfo,
            int layerIndex)
        {
            if (state == null)
                state = animator.GetComponentInParent<PlayerState>();
            // Animator 기준 부모 계층에서 PlayerState를 탐색한다

            if (state == null) return;
            // PlayerState가 없으면 점프 상태를 제어하지 않는다

            state.IsJumping = true;
            // Animator가 실제 Jump 상태에 진입했음을 확정한다
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (state == null)
                state = animator.GetComponentInParent<PlayerState>();

            if (state == null) return;

            state.IsJumping = false;
            // Jump 애니메이션 상태 종료            
        }
    }
}
*/
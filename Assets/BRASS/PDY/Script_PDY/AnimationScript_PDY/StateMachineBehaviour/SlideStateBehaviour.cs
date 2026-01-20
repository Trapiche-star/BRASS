using UnityEngine;

namespace BRASS
{
    /// 슬라이딩 애니메이션 상태 진입과 이탈 타이밍을 PlayerController에 전달한다
    public class SlideStateBehaviour : StateMachineBehaviour
    {
        // 슬라이딩 애니메이션 상태 진입 시 이동 시작 신호를 전달한다
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            var controller = animator.GetComponentInParent<PlayerController>();
            if (controller == null) return;
            // 컨트롤러가 없으면 이 상태에서는 슬라이딩을 시작하지 않는다

            controller.StartSlideFromPending();
            // 애니메이션 진입 시 슬라이딩 이동을 시작한다
        }

        // 슬라이딩 애니메이션 상태 이탈 시 이동 종료 신호를 전달한다
        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            var controller = animator.GetComponentInParent<PlayerController>();
            if (controller == null) return;
            // 컨트롤러가 없으면 이 상태에서는 슬라이딩을 종료하지 않는다

            controller.EndSlide();
            // 애니메이션 종료 시 슬라이딩 이동을 종료한다
        }
    }
}

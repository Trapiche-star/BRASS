using UnityEngine;

namespace BRASS
{
    /// 슬라이딩 애니메이션 상태 진입/이탈 시 이동 타이밍을 전달한다
    public class SlideStateBehaviour : StateMachineBehaviour
    {
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            var controller = animator.GetComponentInParent<PlayerController>();
            if (controller == null) return;
            // 만약 [컨트롤러가 없으면] [슬라이딩 이동을 시작하지 않는다]

            controller.StartSlide(animator.transform.forward);
            // 애니메이션 기준 전방 방향으로 슬라이딩 이동을 시작한다
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            var controller = animator.GetComponentInParent<PlayerController>();
            if (controller == null) return;
            // 만약 [컨트롤러가 없으면] [슬라이딩 이동을 종료하지 않는다]

            controller.EndSlide();
            // 슬라이딩 이동을 종료한다
        }
    }
}

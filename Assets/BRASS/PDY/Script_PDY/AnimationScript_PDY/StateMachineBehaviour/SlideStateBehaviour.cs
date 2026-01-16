using UnityEngine;

namespace BRASS
{
    /// 슬라이딩 애니메이션 상태 진입/이탈 시 플레이어 이동을 제어한다
    public class SlideStateBehaviour : StateMachineBehaviour
    {
        #region Unity Event Method
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            var controller = animator.GetComponentInParent<PlayerController>();
            if (controller == null) return; // 만약 [플레이어 컨트롤러가 없으면] [이 상태에서는 더 이상 처리하지 않는다]

            controller.OnSlideStart(); // 슬라이딩 애니메이션 시작 시 이동을 개시한다
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            var controller = animator.GetComponentInParent<PlayerController>();
            if (controller == null) return; // 만약 [플레이어 컨트롤러가 없으면] [이 상태에서는 더 이상 처리하지 않는다]

            controller.OnSlideEnd(); // 슬라이딩 애니메이션 종료 시 이동을 중단한다
        }
        #endregion
    }
}

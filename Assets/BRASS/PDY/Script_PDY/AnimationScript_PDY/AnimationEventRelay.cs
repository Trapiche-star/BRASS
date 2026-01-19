using UnityEngine;

namespace BRASS
{
    /// 애니메이션 이벤트를 PlayerController로 전달한다
    public class AnimationEventRelay : MonoBehaviour
    {
        #region Variables
        [SerializeField] private PlayerController controller; // 이동 로직 담당
        #endregion

        #region Custom Method
        // 슬라이딩 이동 시작 이벤트
        public void OnSlideMoveStart()
        {
            if (controller == null) return;
            // 만약 [컨트롤러가 없으면] [이벤트를 전달하지 않는다]

            controller.StartSlide(transform.forward);
            // 현재 비주얼 전방 기준으로 슬라이딩 이동을 시작한다
        }

        // 슬라이딩 이동 종료 이벤트
        public void OnSlideMoveEnd()
        {
            if (controller == null) return;
            // 만약 [컨트롤러가 없으면] [이벤트를 전달하지 않는다]

            controller.EndSlide();
            // 슬라이딩 이동을 종료한다
        }
        #endregion
    }
}

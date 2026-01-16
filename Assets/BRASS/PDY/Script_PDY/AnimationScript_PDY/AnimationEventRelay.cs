using UnityEngine;

namespace BRASS
{
    /// 애니메이션 이벤트를 플레이어 컨트롤러로 전달한다
    public class AnimationEventRelay : MonoBehaviour
    {
        #region Variables
        [SerializeField] private PlayerController controller; // 실제 이동 로직 담당
        #endregion

        #region Custom Method
        // 슬라이딩 이동 시작 이벤트 전달
        public void OnSlideMoveStart()
        {
            if (controller == null) return; // 만약 [컨트롤러가 없으면] [이벤트를 전달하지 않는다]
            controller.OnSlideMoveStart(); // 슬라이딩 이동 시작 전달
        }

        // 슬라이딩 이동 종료 이벤트 전달
        public void OnSlideMoveEnd()
        {
            if (controller == null) return; // 만약 [컨트롤러가 없으면] [이벤트를 전달하지 않는다]
            controller.OnSlideMoveEnd(); // 슬라이딩 이동 종료 전달
        }
        #endregion
    }
}

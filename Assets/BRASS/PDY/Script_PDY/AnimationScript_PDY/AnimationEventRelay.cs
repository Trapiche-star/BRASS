using UnityEngine;

namespace BRASS
{
    /// <summary>
    /// 애니메이션 이벤트를 각 로직 컴포넌트로 전달하는 공용 이벤트 허브
    /// </summary>
    public class AnimationEventRelay : MonoBehaviour
    {
        #region Variables
        [SerializeField] private PlayerController controller;
        // 이동 로직을 담당하는 PlayerController 참조

        [SerializeField] private PlayerJump jump;  // 점프 힘 정보를 가져오기 위한 PlayerJump 참조
        #endregion

        #region Unity Event Method
        private void Awake()
        {
            if (controller == null)
                controller = GetComponentInParent<PlayerController>();
            // PlayerController가 지정되지 않았으면 부모 계층에서 자동 탐색한다

            if (jump == null)
                jump = GetComponentInParent<PlayerJump>();
            // PlayerJump가 지정되지 않았으면 부모 계층에서 자동 탐색한다
        }
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

        /*// 점프 애니메이션의 특정 프레임에서 호출된다
        public void OnJumpImpulse()
        {
            if (controller == null || jump == null) return;
            // 필수 참조가 없으면 점프를 실행하지 않는다

            controller.ExecuteJumpImpulse(jump.CurrentJumpForce);
            // PlayerJump 인스펙터에서 지정한 힘으로 점프를 실행한다
        }*/
        #endregion
    }
}
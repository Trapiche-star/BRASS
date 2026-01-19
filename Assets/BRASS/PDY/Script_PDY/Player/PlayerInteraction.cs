using UnityEngine;
using UnityEngine.InputSystem;

namespace BRASS
{
    /// 상호작용 입력(G 키)을 받아 현재 감지된 대상에게 상호작용을 실행한다
    public class PlayerInteraction : MonoBehaviour
    {
        #region Variables
        [SerializeField] private PlayerInput playerInput; // Input System 기반 액션 맵 수신용 컴포넌트

        private PlayerCasting casting; // 현재 마우스 Ray로 감지된 상호작용 대상 정보 제공 컴포넌트
        #endregion

        #region Unity Event Method
        private void Awake()
        {
            // PlayerInput이 직접 지정되지 않았으면 동일 오브젝트에서 자동 탐색
            if (playerInput == null)
                playerInput = GetComponent<PlayerInput>();

            // PlayerCasting 컴포넌트 참조 저장 (상호작용 대상 확인용)
            casting = GetComponent<PlayerCasting>();
        }

        private void OnEnable()
        {
            // 입력 컴포넌트가 없으면 이벤트 구독 불가
            if (playerInput == null) return;

            // Interact 액션(G 키)이 눌렸을 때 OnInteract 콜백 등록
            playerInput.actions["Interact"].performed += OnInteract;
        }

        private void OnDisable()
        {
            // 입력 컴포넌트가 없으면 이벤트 해제 불가
            if (playerInput == null) return;

            // 오브젝트 비활성화 시 입력 이벤트 구독 해제
            playerInput.actions["Interact"].performed -= OnInteract;
        }
        #endregion

        #region Custom Method
        private void OnInteract(InputAction.CallbackContext context)
        {
            // 캐스팅 컴포넌트가 없으면 상호작용 불가
            if (casting == null) return;

            // 현재 프레임에 유효한 상호작용 대상이 없으면 처리 중단
            if (!casting.HasTarget) return;

            // 감지된 대상에게 상호작용 실행 요청
            casting.CurrentTarget?.Interact();
        }
        #endregion
    }
}

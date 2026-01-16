using UnityEngine;
using UnityEngine.InputSystem;

namespace BRASS
{
    /// 상호작용 입력(G 키)을 받아 현재 감지된 대상에게 상호작용을 실행한다
    public class PlayerInteraction : MonoBehaviour
    {
        #region Variables
        [SerializeField] private PlayerInput playerInput; // Input Actions 수신 담당

        private PlayerCasting casting; // 현재 상호작용 대상 감지 담당
        #endregion

        #region Unity Event Method
        private void Awake()
        {
            if (playerInput == null) playerInput = GetComponent<PlayerInput>(); // 만약 [참조가 없으면] [컴포넌트를 자동 탐색한다]
            casting = GetComponent<PlayerCasting>(); // 만약 [캐스팅 컴포넌트가 있으면] [참조를 저장한다]
        }

        private void OnEnable()
        {
            if (playerInput == null) return; // 만약 [입력 참조가 없으면] [이벤트를 구독하지 않는다]
            playerInput.actions["Interact"].performed += OnInteract; // 만약 [G 키가 눌리면] [상호작용을 시도한다]
        }

        private void OnDisable()
        {
            if (playerInput == null) return; // 만약 [입력 참조가 없으면] [이벤트 구독을 해제하지 않는다]
            playerInput.actions["Interact"].performed -= OnInteract; // 만약 [비활성화되면] [이벤트 구독을 해제한다]
        }
        #endregion

        #region Custom Method
        // G 키 입력 시 현재 감지된 대상에게 상호작용을 실행한다
        private void OnInteract(InputAction.CallbackContext context)
        {
            if (casting == null) return; // 만약 [캐스팅 컴포넌트가 없으면] [이 메서드에서는 더 이상 처리하지 않는다]
            if (!casting.HasTarget) return; // 만약 [현재 타겟이 없으면] [이 메서드에서는 더 이상 상호작용하지 않는다]

            casting.CurrentTarget?.Interact(); // 만약 [타겟이 존재하면] [상호작용을 실행한다]
        }
        #endregion
    }
}

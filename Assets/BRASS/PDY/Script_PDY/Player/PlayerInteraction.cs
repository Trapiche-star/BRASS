using UnityEngine;
using UnityEngine.InputSystem;

namespace BRASS
{
    using Team1;

    public class PlayerInteraction : MonoBehaviour
    {
        [SerializeField] private PlayerInput playerInput;
        private PlayerCasting casting;

        private void Awake()
        {
            if (playerInput == null) playerInput = GetComponent<PlayerInput>();
            casting = GetComponent<PlayerCasting>();
        }

        private void OnEnable()
        {
            if (playerInput == null) return;
            // Input System의 "Interact" 액션(G키)에 연결
            playerInput.actions["Interact"].performed += OnInteract;
        }

        private void OnDisable()
        {
            if (playerInput == null) return;
            playerInput.actions["Interact"].performed -= OnInteract;
        }

        private void OnInteract(InputAction.CallbackContext context)
        {
            if (UIManager_SY.Instance == null) return;

            // 1. 만약 UI가 이미 하나라도 열려 있다면? -> 닫기만 실행
            if (UIManager_SY.Instance.IsAnyUIOpen)
            {
                UIManager_SY.Instance.CloseAll();
                Debug.Log("UI 닫음 (PlayerInteraction)");
                return; // ⭐️ 여기서 리턴해서 아래 Interact()가 실행 안 되게 막음!
            }

            // 2. UI가 닫혀 있을 때만 상호작용 실행
            if (casting != null && casting.HasTarget)
            {
                Debug.Log($"상호작용 실행: {casting.CurrentTarget}");
                casting.CurrentTarget?.Interact();
            }
        }
    }
}
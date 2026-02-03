using UnityEngine;
using UnityEngine.InputSystem;

namespace BRASS
{
    using Team1; // UIManager_SY를 찾기 위해 추가

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
            playerInput.actions["Interact"].performed += OnInteract;
        }

        private void OnDisable()
        {
            if (playerInput == null) return;
            playerInput.actions["Interact"].performed -= OnInteract;
        }

        private void OnInteract(InputAction.CallbackContext context)
        {
            // UI가 열려있으면 아무것도 하지 않음 (InteractionController에서 처리하도록 양보)
            if (UIManager_SY.Instance != null && UIManager_SY.Instance.IsAnyUIOpen) return;

            if (!casting.HasTarget) return;
            casting.CurrentTarget?.Interact();
        }
    }
}
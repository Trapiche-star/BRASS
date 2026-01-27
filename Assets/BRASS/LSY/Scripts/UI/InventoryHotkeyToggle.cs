using UnityEngine;
using UnityEngine.InputSystem;

namespace Team1
{
    public class InventoryHotkeyToggle : MonoBehaviour
    {
        [SerializeField] private GameObject inventoryRoot;
        [SerializeField] private PlayerInput playerInput;

        private bool isOpen;

        private void Awake()
        {
            if (inventoryRoot != null)
                inventoryRoot.SetActive(false);

            if (playerInput == null)
                playerInput = FindObjectOfType<PlayerInput>();
        }

        private void Update()
        {
            if (Keyboard.current == null)
                return;

            if (Keyboard.current.iKey.wasPressedThisFrame)
            {
                Toggle();
            }
        }

        private void Toggle()
        {
            isOpen = !isOpen;
            inventoryRoot.SetActive(isOpen);

            if (playerInput != null)
                playerInput.enabled = !isOpen;

            Cursor.lockState = isOpen ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = isOpen;

            Time.timeScale = isOpen ? 0f : 1f;

            Debug.Log($"🎒 인벤토리 {(isOpen ? "열림" : "닫힘")}");
        }
    }
}

using UnityEngine;
using UnityEngine.InputSystem;

namespace Team1
{
    public class InventoryToggleUI : MonoBehaviour
    {
        [Header("인벤토리 UI 루트")]
        [SerializeField] private GameObject inventoryRoot;

        private bool isOpen;

        private void Awake()
        {
            if (inventoryRoot != null)
                inventoryRoot.SetActive(false);
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

            if (inventoryRoot != null)
                inventoryRoot.SetActive(isOpen);

            Debug.Log($"🎒 인벤토리 {(isOpen ? "열림" : "닫힘")}");
        }
    }
}

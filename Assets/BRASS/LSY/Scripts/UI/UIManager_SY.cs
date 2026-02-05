using UnityEngine;

namespace Team1
{
    public class UIManager_SY : MonoBehaviour
    {
        public static UIManager_SY Instance;

        public GameObject inventoryPanel;
        public GameObject shopPanel;
        public GameObject minimapUI;

        // ⭐️ 현재 UI가 켜져 있는지 아주 정확하게 체크
        public bool IsAnyUIOpen =>
            (inventoryPanel != null && inventoryPanel.activeInHierarchy) ||
            (shopPanel != null && shopPanel.activeInHierarchy) ||
            (minimapUI != null && minimapUI.activeInHierarchy);

        void Awake()
        {
            if (Instance == null) { Instance = this; transform.SetParent(null); DontDestroyOnLoad(gameObject); }
            else Destroy(gameObject);
        }

        public void OpenShop() { if (shopPanel != null) { shopPanel.SetActive(true); SetCursorState(true); } }
        public void OpenMinimap() { if (minimapUI != null) minimapUI.SetActive(true); }

        public void CloseAll()
        {
            if (inventoryPanel != null) inventoryPanel.SetActive(false);
            if (shopPanel != null) shopPanel.SetActive(false);
            if (minimapUI != null) minimapUI.SetActive(false);
            SetCursorState(false);
        }

        public void ToggleInventory()
        {
            if (inventoryPanel == null) return;
            bool state = !inventoryPanel.activeSelf;
            inventoryPanel.SetActive(state);
            SetCursorState(state || (shopPanel != null && shopPanel.activeInHierarchy));
        }

        private void SetCursorState(bool visible)
        {
            Cursor.visible = visible;
            Cursor.lockState = visible ? CursorLockMode.None : CursorLockMode.Locked;
        }
    }
}
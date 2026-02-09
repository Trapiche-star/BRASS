using UnityEngine;

namespace Team1
{
    public class UIManager_SY : MonoBehaviour
    {
        public static UIManager_SY Instance;

        // 🚩 다시 인스펙터에서 직접 드래그해서 넣어주세요
        [Header("직접 드래그해서 연결하세요")]
        public GameObject inventoryPanel;
        public GameObject shopPanel;
        public GameObject minimapUI;

        void Awake()
        {
            // 1. 이미 인스턴스가 있다면 나를 죽이지 말고 '기존 놈'을 죽여서 갱신하거나 유지
            if (Instance != null && Instance != this)
            {
                // 만약 씬마다 매니저를 새로 배치하고 싶다면:
                Destroy(Instance.gameObject);
            }

            Instance = this;

            // 2. 부모가 있다면 해제 (DontDestroyOnLoad는 최상위 오브젝트에만 작동함)
            transform.SetParent(null);

            // 3. 씬이 바뀌어도 절대 죽지 마라!
            DontDestroyOnLoad(gameObject);

            Debug.Log("<color=green>✅ UIManager가 무적 상태(DontDestroyOnLoad)가 되었습니다.</color>");
        }

        // 상점 열기 (기존 방식)
        public void OpenShop()
        {
            if (shopPanel != null)
            {
                shopPanel.SetActive(true);
                SetCursorState(true);
            }
        }

        // 지도 열기 (기존 방식)
        public void OpenMinimap()
        {
            if (minimapUI != null)
            {
                minimapUI.SetActive(true);
                SetCursorState(true);
            }
        }

        // 인벤토리 토글 (기존 방식)
        public void ToggleInventory()
        {
            if (inventoryPanel != null)
            {
                bool state = !inventoryPanel.activeSelf;
                inventoryPanel.SetActive(state);
                SetCursorState(state);
            }
        }

        public void CloseAll()
        {
            if (inventoryPanel) inventoryPanel.SetActive(false);
            if (shopPanel) shopPanel.SetActive(false);
            if (minimapUI) minimapUI.SetActive(false);
            SetCursorState(false);
        }

        public bool IsAnyUIOpen =>
            (inventoryPanel && inventoryPanel.activeInHierarchy) ||
            (shopPanel && shopPanel.activeInHierarchy) ||
            (minimapUI && minimapUI.activeInHierarchy);

        private void SetCursorState(bool visible)
        {
            Cursor.visible = visible;
            Cursor.lockState = visible ? CursorLockMode.None : CursorLockMode.Locked;
        }
    }
}
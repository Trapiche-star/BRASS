using UnityEngine;
using TMPro;
using System.Collections;

namespace Team1
{
    public class UIManager_SY : MonoBehaviour
    {
        public static UIManager_SY Instance { get; private set; }

        [Header("Currency UI")]
        [SerializeField] private TextMeshProUGUI inventoryGoldText;

        [Header("UI Panels")]
        [SerializeField] private GameObject inventoryUI;
        [SerializeField] private GameObject minimapUI;
        [SerializeField] private GameObject marketUI;

        public bool IsAnyUIOpen { get; private set; }
        private bool _canClose = true;

        private void Awake()
        {
            Instance = this;
            CloseAllImmediate();
        }

        private void OnEnable()
        {
            // GoldManager가 신호를 보낼 때 실행할 함수 연결
            GoldManager.OnGoldChanged += RefreshInventoryGoldUI;
        }

        private void OnDisable()
        {
            // 메모리 누수 방지를 위해 해제
            GoldManager.OnGoldChanged -= RefreshInventoryGoldUI;
        }

        public void ToggleMarket() => ToggleUI(marketUI);
        public void ToggleMinimap() => ToggleUI(minimapUI);
        public void ToggleInventory() => ToggleUI(inventoryUI);

        private void ToggleUI(GameObject target)
        {
            if (target == null) return;

            // 1. 꺼져 있다면 켠다
            if (!target.activeSelf)
            {
                CloseAllImmediate(); // 다른 UI 일단 다 끄기

                target.SetActive(true);
                IsAnyUIOpen = true;
                SetUIMode(true);

                // 인벤토리가 켜지는 순간 골드 갱신
                if (target == inventoryUI)
                {
                    RefreshInventoryGoldUI(GoldManager.Instance != null ? GoldManager.Instance.GetCurrentGold() : 0);
                }

                // 열린 직후 광클 방어
                StartCoroutine(CloseCooldown());
            }
            // 2. 이미 켜져 있다면 끈다
            else
            {
                CloseAll();
            }
        }

        public void CloseAll()
        {
            if (!_canClose) return;
            CloseAllImmediate();
        }

        public void CloseAllImmediate()
        {
            if (inventoryUI) inventoryUI.SetActive(false);
            if (minimapUI) minimapUI.SetActive(false);
            if (marketUI) marketUI.SetActive(false);

            IsAnyUIOpen = false;
            SetUIMode(false);
        }

        private void SetUIMode(bool enable)
        {
            Cursor.lockState = enable ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = enable;
            Time.timeScale = enable ? 0f : 1f;
        }

        private void RefreshInventoryGoldUI(int amount)
        {
            if (inventoryGoldText != null)
            {
                inventoryGoldText.text = amount.ToString("N0");
            }
        }

        private IEnumerator CloseCooldown()
        {
            _canClose = false;
            yield return new WaitForSecondsRealtime(0.1f);
            _canClose = true;
        }

        // 외부에서 수동 업데이트가 필요한 경우 사용
        public void UpdateInventoryGold()
        {
            if (GoldManager.Instance != null)
            {
                RefreshInventoryGoldUI(GoldManager.Instance.GetCurrentGold());
            }
        }
    }
}
using UnityEngine;
using TMPro;
using System.Collections;

namespace Team1
{
    public class ShopBuyButton : MonoBehaviour
    {
        [Header("구매할 아이템 템플릿")]
        [SerializeField] private ConsumableItemData itemData;

        [Header("플레이어 인벤토리")]
        [SerializeField] private Inventory inventory;

        [Header("구매 수량 UI")]
        [SerializeField] private TMP_Text quantityText;
        [SerializeField] private int maxQuantity = 99;

        [Header("구매 완료 편지 UI")]
        [SerializeField] private GameObject letterUI;
        [SerializeField] private float letterShowTime = 2f;

        private int quantity = 1;
        private Coroutine letterRoutine;

        private void Start()
        {
            UpdateQuantityUI();

            if (letterUI != null)
                letterUI.SetActive(false);
        }

        // ➕ 버튼
        public void IncreaseQuantity()
        {
            quantity++;
            if (quantity > maxQuantity)
                quantity = maxQuantity;

            UpdateQuantityUI();
        }

        // ➖ 버튼
        public void DecreaseQuantity()
        {
            quantity--;
            if (quantity < 1)
                quantity = 1;

            UpdateQuantityUI();
        }

        // 🛒 Sell 버튼
        public void Buy()
        {
            if (itemData == null || inventory == null)
            {
                Debug.LogError("❌ itemData 또는 inventory 연결 안됨");
                return;
            }

            for (int i = 0; i < quantity; i++)
            {
                ConsumableItem newItem = itemData.CreateItem();
                inventory.AddItem(newItem);
            }

            Debug.Log($"🛒 {itemData.itemName} x{quantity} 구매 완료");

            ShowLetterUI();
        }

        // 📩 편지 UI 표시 + 자동 닫기
        private void ShowLetterUI()
        {
            if (letterUI == null)
                return;

            letterUI.SetActive(true);

            // 기존 코루틴 중복 방지
            if (letterRoutine != null)
                StopCoroutine(letterRoutine);

            letterRoutine = StartCoroutine(HideLetterAfterDelay());
        }

        private IEnumerator HideLetterAfterDelay()
        {
            yield return new WaitForSecondsRealtime(letterShowTime);
            letterUI.SetActive(false);
        }

        private void UpdateQuantityUI()
        {
            if (quantityText != null)
                quantityText.text = quantity.ToString();
        }
        private void OnDisable()
        {
            // 🧹 상점이 꺼질 때 편지 UI 강제 정리
            if (letterRoutine != null)
            {
                StopCoroutine(letterRoutine);
                letterRoutine = null;
            }

            if (letterUI != null)
                letterUI.SetActive(false);
        }
    }
}

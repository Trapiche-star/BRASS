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

        public void IncreaseQuantity()
        {
            quantity++;
            if (quantity > maxQuantity) quantity = maxQuantity;
            UpdateQuantityUI();
        }

        public void DecreaseQuantity()
        {
            quantity--;
            if (quantity < 1) quantity = 1;
            UpdateQuantityUI();
        }

        // 🛒 Sell 버튼 (이름은 Sell이지만 로직은 구매인 버튼)
        public void Buy()
        {
            if (itemData == null || inventory == null)
            {
                Debug.LogError("❌ itemData 또는 inventory 연결 안됨");
                return;
            }

            // 1. 총 가격 계산 (단가 * 수량)
            int totalPrice = itemData.price * quantity;

            // 2. 골드 매니저 확인 및 골드 차감 시도
            if (GoldManager.Instance != null)
            {
                // RemoveGold가 내부적으로 잔액 체크 후 부족하면 false를 뱉음
                if (GoldManager.Instance.RemoveGold(totalPrice))
                {
                    // 3. 차감 성공 시 아이템 인벤토리 추가
                    for (int i = 0; i < quantity; i++)
                    {
                        ConsumableItem newItem = itemData.CreateItem();
                        inventory.AddItem(newItem);
                    }

                    Debug.Log($"🛒 {itemData.itemName} x{quantity} 구매 완료! 총 {totalPrice}G 차감.");
                    ShowLetterUI();
                }
                else
                {
                    // 4. 골드 부족 시 처리
                    Debug.LogWarning($"❌ 골드가 부족합니다! (필요: {totalPrice}G / 현재: {GoldManager.Instance.GetCurrentGold()}G)");
                    // 여기에 "골드가 부족합니다"라는 별도의 팝업을 띄워주면 좋습니다.
                }
            }
            else
            {
                Debug.LogError("❌ GoldManager 인스턴스를 찾을 수 없습니다.");
            }
        }

        private void ShowLetterUI()
        {
            if (letterUI == null) return;

            letterUI.SetActive(true);

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
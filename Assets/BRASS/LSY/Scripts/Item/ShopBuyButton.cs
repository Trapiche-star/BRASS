using UnityEngine;

namespace Team1
{
    public class ShopBuyButton : MonoBehaviour
    {
        [Header("구매할 아이템 템플릿")]
        [SerializeField] private ConsumableItemData itemData;

        [Header("플레이어 인벤토리")]
        [SerializeField] private Inventory inventory;

        public void Buy()
        {
            if (itemData == null || inventory == null)
            {
                Debug.LogError("❌ itemData 또는 inventory 연결 안됨");
                return;
            }

            ConsumableItem newItem = itemData.CreateItem();
            inventory.AddItem(newItem);

            Debug.Log($"🛒 구매 완료: {newItem.ItemName}");
        }
    }
}

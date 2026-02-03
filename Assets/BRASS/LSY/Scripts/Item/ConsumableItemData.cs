using UnityEngine;

namespace Team1
{
    public abstract class ConsumableItemData : ScriptableObject
    {
        public string itemName;
        public Sprite icon;
        public int price;   // 💰 구매 가격

        public abstract ConsumableItem CreateItem();
    }
}

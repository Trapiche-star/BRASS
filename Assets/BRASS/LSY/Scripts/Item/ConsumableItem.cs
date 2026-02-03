using UnityEngine;

namespace Team1
{
    public abstract class ConsumableItem : IItem
    {
        public string ItemName { get; protected set; }
        public Sprite Icon { get; protected set; }
        public int Price { get; protected set; }   // 💰 가격

        public ItemCategory Category => ItemCategory.Consumable;

        public abstract void Use(GameObject user);
        public abstract ConsumableItem Clone();
    }
}

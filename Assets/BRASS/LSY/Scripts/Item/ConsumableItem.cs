using UnityEngine;

namespace Team1
{
    public abstract class ConsumableItem : IItem
    {
        public string ItemName { get; protected set; }
        public Sprite Icon { get; protected set; }

        // 소비 아이템은 기본적으로 Consumable
        public ItemCategory Category => ItemCategory.Consumable;

        public abstract void Use(GameObject user);
        public abstract ConsumableItem Clone();
    }
}

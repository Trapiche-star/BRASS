using Team1;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/HealingItemData")]
public class HealingItemData : ConsumableItemData
{
    public float healValue = 30f;

    public override ConsumableItem CreateItem()
    {
        return new HealingItem
        {
            ItemName = this.itemName,
            Icon = this.icon,
            healAmount = this.healValue,
            Category = ItemCategory.Consumable // Enum에 따라 설정
        };
    }
}
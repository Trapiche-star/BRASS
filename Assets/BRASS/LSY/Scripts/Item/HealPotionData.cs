using UnityEngine;

namespace Team1
{
    [CreateAssetMenu(menuName = "Items/Heal Potion")]
    public class HealPotionData : ConsumableItemData
    {
        public PotionSize size;

        public int healAmount;

        public override ConsumableItem CreateItem()
        {
            return new HealPotion(
                itemName,
                icon,
                price,
                healAmount,
                size
            );
        }
    }
}

using UnityEngine;

namespace Team1
{
    [CreateAssetMenu(menuName = "Items/Heal Potion")]
    public class HealPotionData : ConsumableItemData
    {
        public int healAmount = 100;

        public override ConsumableItem CreateItem()
        {
            return new HealPotion(itemName, icon, healAmount);
        }
    }
}

using UnityEngine;

namespace Team1
{
    public class HealPotion : ConsumableItem
    {
        private int healAmount;
        private PotionSize size;

        public HealPotion(string name, Sprite icon, int price, int healAmount, PotionSize size)
        {
            ItemName = name;
            Icon = icon;
            Price = price;
            this.healAmount = healAmount;
            this.size = size;
        }

        public override void Use(GameObject user)
        {
            Debug.Log(
                $" {ItemName} ({size}) 사용 → 체력 {healAmount} 회복"
            );
        }

        public override ConsumableItem Clone()
        {
            return new HealPotion(ItemName, Icon, Price, healAmount, size);
        }
    }
}

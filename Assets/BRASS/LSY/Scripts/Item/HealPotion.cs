using UnityEngine;

namespace Team1
{
    public class HealPotion : ConsumableItem
    {
        private int healAmount;

        // ⭐ Data 쪽 호출 순서와 맞춤
        public HealPotion(string name, Sprite icon, int healAmount)
        {
            ItemName = name;
            Icon = icon;
            this.healAmount = healAmount;
        }

        public override void Use(GameObject user)
        {
            Debug.Log($"❤️ {ItemName} 사용 → 체력 {healAmount} 회복");
        }

        public override ConsumableItem Clone()
        {
            return new HealPotion(ItemName, Icon, healAmount);
        }
    }
}

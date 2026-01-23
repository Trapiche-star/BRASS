using UnityEngine;

namespace Team1
{
    public class HealPotion : ConsumableItem
    {
        private int healAmount;

        public HealPotion(string name, Sprite icon, int healAmount)
        {
            ItemName = name;
            Icon = icon;
            this.healAmount = healAmount;
        }

        public override void Use(GameObject user)
        {
            Debug.Log($"🧪 {ItemName} 사용! HP {healAmount} 회복!");
        }
    }
}

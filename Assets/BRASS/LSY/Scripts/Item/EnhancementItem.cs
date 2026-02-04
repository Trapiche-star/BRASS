using UnityEngine;

namespace Team1
{
    public class EnhancementItem : ConsumableItem
    {
        private float successRate;
        private int upgradePower;

        // 생성자: 필요한 모든 정보를 받아 부모와 자신에게 할당합니다.
        public EnhancementItem(string name, Sprite icon, int price, float successRate, int upgradePower, ItemCategory category)
        {
            this.ItemName = name;
            this.Icon = icon;
            this.Price = price;
            this.successRate = successRate;
            this.upgradePower = upgradePower;

            // ⚠️ 여기서 에러가 난다면 ConsumableItem.cs 혹은 IItem.cs에서 
            // Category가 { get; set; }인지 (set이 있는지) 확인해야 합니다.
            this.Category = category;
        }

        public override void Use(GameObject user)
        {
            Debug.Log($"{ItemName}을 사용했습니다. 강화 시스템을 불러옵니다...");
            // 강화 로직은 나중에 여기에 추가
        }

        public override ConsumableItem Clone()
        {
            // 복제 시에도 모든 정보를 유지합니다.
            return new EnhancementItem(ItemName, Icon, Price, successRate, upgradePower, this.Category);
        }
    }
}
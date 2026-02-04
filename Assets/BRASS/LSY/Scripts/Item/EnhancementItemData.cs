using UnityEngine;

namespace Team1
{
    [CreateAssetMenu(fileName = "New Enhancement Item", menuName = "ScriptableObject/Items/EnhancementItem")]
    public class EnhancementItemData : ConsumableItemData
    {
        [Header("카테고리 설정")]
        public ItemCategory itemCategory = ItemCategory.Material;

        [Header("강화 설정")]
        [Range(0f, 1f)] public float successRate = 0.5f;
        public int upgradePower = 1;

        // ⭐️ 함수는 딱 하나만 있어야 합니다!
        public override ConsumableItem CreateItem()
        {
            // EnhancementItem 생성자에 6개의 인자를 정확히 전달합니다.
            return new EnhancementItem(itemName, icon, price, successRate, upgradePower, itemCategory);
        }
    }
}
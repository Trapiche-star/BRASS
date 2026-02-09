using UnityEngine;

namespace Team1
{
    public abstract class ConsumableItem : IItem
    {
        public string ItemName { get; set; }
        public Sprite Icon { get; set; }
        public int Price { get; set; }
        public ItemCategory Category { get; set; }

        public abstract void Use(GameObject user);
        public abstract ConsumableItem Clone();
    }

    // ⭐ 추상 클래스는 new를 할 수 없으므로, 실제로 가방에 들어갈 '진짜' 클래스가 하나 필요합니다.
    public class RealConsumable : ConsumableItem
    {
        public override void Use(GameObject user) { /* 사용 로직 */ }
        public override ConsumableItem Clone() => (ConsumableItem)this.MemberwiseClone();
    }
}
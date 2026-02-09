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
        //public override void Use(GameObject user) { /* 사용 로직 */ }

        // RealConsumable.cs (또는 사용하는 실제 아이템 클래스)
        public override void Use(GameObject user)
        {
            // GaugeController에 접근해서 HP 20 회복 예시
            if (GaugeController.Instance != null)
            {
                GaugeController.Instance.SetHp(GaugeController.Instance.currentHp + 20f);
                Debug.Log("HP 20 회복됨!");
            }
        }

        public override ConsumableItem Clone() => (ConsumableItem)this.MemberwiseClone();
    }

}
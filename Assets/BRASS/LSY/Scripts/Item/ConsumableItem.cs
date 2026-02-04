// ConsumableItem.cs (혹은 상위 클래스)
using UnityEngine;

namespace Team1
{
    public abstract class ConsumableItem : IItem
    {
        public string ItemName { get; set; }
        public Sprite Icon { get; set; }
        public int Price { get; set; }

        // ⭐️ 이 부분이 public set이 가능해야 합니다!
        public ItemCategory Category { get; set; }

        public abstract void Use(GameObject user);
        public abstract ConsumableItem Clone();
    }
}
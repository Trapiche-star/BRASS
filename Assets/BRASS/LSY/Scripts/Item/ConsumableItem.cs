using UnityEngine;

namespace Team1
{
    public abstract class ConsumableItem : IItem
    {
        public string ItemName { get; protected set; }
        public Sprite Icon { get; protected set; }   // ⭐ 추가

        public abstract void Use(GameObject user);
    }
}

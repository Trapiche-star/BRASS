using UnityEngine;

namespace Team1
{
    public abstract class ConsumableItemData : ScriptableObject
    {
        public string itemName;
        public Sprite icon;

        public abstract ConsumableItem CreateItem();
    }
}

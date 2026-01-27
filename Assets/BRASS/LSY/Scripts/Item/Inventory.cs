using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Team1
{
    public class Inventory : MonoBehaviour
    {
        public List<InventorySlot> Slots { get; private set; } = new();

        public void AddItem(IItem item)
        {
            foreach (var slot in Slots)
            {
                if (slot.Item.ItemName == item.ItemName)
                {
                    slot.AddOne();
                    Debug.Log($"🎒 {item.ItemName} 수량 증가 → {slot.Count}");
                    return;
                }
            }

            Slots.Add(new InventorySlot(item));
            Debug.Log($"🎒 아이템 획득: {item.ItemName}");
        }

        public void UseItem(int index)
        {
            if (index < 0 || index >= Slots.Count)
                return;

            var slot = Slots[index];

            slot.Item.Use(gameObject);
            slot.RemoveOne();

            if (slot.Count <= 0)
            {
                Slots.RemoveAt(index);
            }
        }
        // ⭐ 카테고리 필터링
        public List<InventorySlot> GetSlotsByCategory(ItemCategory category)
        {
            if (category == ItemCategory.All)
                return Slots;

            return Slots.Where(s => s.Item.Category == category).ToList();
        }
    }
}

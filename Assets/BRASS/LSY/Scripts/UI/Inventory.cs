using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System; // Action 사용

namespace Team1
{
    public class Inventory : MonoBehaviour
    {
        // UI가 인벤토리의 변화를 감지할 수 있도록 이벤트 추가
        public event Action OnInventoryChanged;

        public List<InventorySlot> Slots { get; private set; } = new();

        public void AddItem(IItem item)
        {
            if (item == null) return;

            // 1. 기존에 같은 아이템이 있는지 확인 (카테고리도 같아야 함)
            var existingSlot = Slots.FirstOrDefault(s => s.Item.ItemName == item.ItemName);

            if (existingSlot != null)
            {
                existingSlot.AddOne();
                Debug.Log($"🎒 {item.ItemName} 수량 증가 → {existingSlot.Count}");
            }
            else
            {
                // 2. 새 아이템이면 슬롯 추가
                Slots.Add(new InventorySlot(item));
                Debug.Log($"🎒 아이템 획득: {item.ItemName} (카테고리: {item.Category})");
            }

            // UI 갱신 신호
            OnInventoryChanged?.Invoke();
        }

        public void UseItem(int index, ItemCategory currentCategory = ItemCategory.All)
        {
            // 현재 보고 있는 카테고리의 리스트를 가져옴
            var displaySlots = GetSlotsByCategory(currentCategory);

            if (index < 0 || index >= displaySlots.Count) return;

            var slot = displaySlots[index];
            slot.Item.Use(gameObject);
            slot.RemoveOne();

            if (slot.Count <= 0)
            {
                // 실제 전체 리스트에서 제거
                Slots.Remove(slot);
            }

            OnInventoryChanged?.Invoke();
        }

        // ⭐ 카테고리 필터링 (기존 코드 유지 및 최적화)
        public List<InventorySlot> GetSlotsByCategory(ItemCategory category)
        {
            if (category == ItemCategory.All)
                return Slots;

            return Slots.Where(s => s.Item.Category == category).ToList();
        }
    }
}
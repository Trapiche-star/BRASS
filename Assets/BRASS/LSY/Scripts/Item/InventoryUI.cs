using System.Collections.Generic;
using UnityEngine;

namespace Team1
{
    public class InventoryUI : MonoBehaviour
    {
        [SerializeField] private Inventory inventory;
        [SerializeField] private List<InventorySlotUI> slotUIs = new();

        private ItemCategory currentCategory = ItemCategory.All;

        private void Awake()
        {
            if (inventory == null)
                inventory = FindObjectOfType<Inventory>();
        }

        private void Update()
        {
            Refresh();
        }

        // ======================
        // ⭐ 버튼에서 직접 연결할 함수
        // ======================

        public void ShowAll()
        {
            SetCategory(ItemCategory.All);
        }

        public void ShowConsumable()
        {
            SetCategory(ItemCategory.Consumable);
        }

        public void ShowMaterial()
        {
            SetCategory(ItemCategory.Material);
        }

        public void ShowEtc()
        {
            SetCategory(ItemCategory.Etc);
        }

        // ======================

        private void SetCategory(ItemCategory category)
        {
            currentCategory = category;
        }

        private void Refresh()
        {
            if (inventory == null)
                return;

            var slots = inventory.GetSlotsByCategory(currentCategory);

            for (int i = 0; i < slotUIs.Count; i++)
            {
                if (i < slots.Count)
                    slotUIs[i].Refresh(slots[i]);
                else
                    slotUIs[i].Clear();
            }
        }
    }
}

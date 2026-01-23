using System.Collections.Generic;
using UnityEngine;

namespace Team1
{
    public class InventoryUI : MonoBehaviour
    {
        [SerializeField] private Inventory inventory;
        [SerializeField] private List<InventorySlotUI> slotUIs = new();

        private void Awake()
        {
            if (inventory == null)
                inventory = FindObjectOfType<Inventory>();
        }

        private void Update()
        {
            Refresh();
        }

        private void Refresh()
        {
            if (inventory == null)
                return;

            var slots = inventory.Slots;

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

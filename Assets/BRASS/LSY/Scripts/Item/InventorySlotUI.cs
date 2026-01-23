using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Team1
{
    public class InventorySlotUI : MonoBehaviour
    {
        [SerializeField] private Image iconImage;   // ⭐ 추가
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private TMP_Text countText;

        public void Refresh(InventorySlot slot)
        {
            nameText.text = slot.Item.ItemName;
            countText.text = slot.Count.ToString();

            iconImage.sprite = slot.Item.Icon;   // ⭐ 핵심
            iconImage.enabled = slot.Item.Icon != null;
        }

        public void Clear()
        {
            nameText.text = "";
            countText.text = "";
            iconImage.sprite = null;
            iconImage.enabled = false;
        }
    }
}

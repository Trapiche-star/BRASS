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
            iconImage.sprite = slot.Item.Icon;
            iconImage.enabled = slot.Item.Icon != null;

            // ⭐ 수정: iconImage 오브젝트에서 직접 DragHandler를 찾습니다.
            var dragHandler = iconImage.GetComponent<InventoryItemDragHandler>();
            if (dragHandler != null)
            {
                dragHandler.item = slot.Item as ConsumableItem;
            }
            else
            {
                Debug.LogError($"{iconImage.gameObject.name}에 InventoryItemDragHandler가 없습니다!");
            }
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

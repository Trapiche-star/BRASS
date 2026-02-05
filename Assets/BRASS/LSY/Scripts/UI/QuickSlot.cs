using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

namespace Team1
{
    public class QuickSlot : MonoBehaviour, IDropHandler
    {
        [Header("현재 장착된 아이템")]
        public ConsumableItem currentItem;

        public ConsumableItemData currentItemData;
        public Image iconImage; // 슬롯 내부의 이미지 오브젝트 (Z이미지 등)
        public TextMeshProUGUI quantityText;

        public void OnDrop(PointerEventData eventData)
        {
            GameObject draggedObj = eventData.pointerDrag;
            if (draggedObj != null)
            {
                var dragHandler = draggedObj.GetComponent<InventoryItemDragHandler>();
                if (dragHandler != null && dragHandler.item != null)
                {
                    SetItem(dragHandler.item);
                }
            }
        }

        public void SetItem(ConsumableItem newItem)
        {
            this.currentItem = newItem; // QuickSlot의 변수 타입도 ConsumableItem이어야 합니다.

            if (iconImage != null)
            {
                iconImage.sprite = newItem.Icon;
                iconImage.enabled = true;
                iconImage.color = Color.white;

                // 연출 효과
                iconImage.transform.localScale = Vector3.one * 1.5f;
            }
        }
        void Update()
        {
            // 장착 애니메이션 효과
            if (iconImage != null && iconImage.transform.localScale.x > 1.0f)
            {
                iconImage.transform.localScale = Vector3.Lerp(iconImage.transform.localScale, Vector3.one, Time.deltaTime * 8f);
            }
        }
    }
}
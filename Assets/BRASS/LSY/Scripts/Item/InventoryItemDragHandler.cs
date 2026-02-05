using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Team1
{
    [RequireComponent(typeof(CanvasGroup))]
    public class InventoryItemDragHandler : MonoBehaviour,  IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        public ConsumableItem item;
        private RectTransform rectTransform;
        private CanvasGroup canvasGroup;
        private Vector3 originalPosition;
        private Transform originalParent; // ⭐ 원래 슬롯을 저장
        private Canvas mainCanvas;

        void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            canvasGroup = GetComponent<CanvasGroup>() ?? gameObject.AddComponent<CanvasGroup>();
            mainCanvas = GetComponentInParent<Canvas>()?.rootCanvas;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (item == null) return;

            // 1. 드래그 시작 시점의 부모(Slot)와 위치를 정확히 기억합니다.
            originalParent = transform.parent;
            originalPosition = rectTransform.position;

            if (mainCanvas != null) transform.SetParent(mainCanvas.transform);
            transform.SetAsLastSibling();

            canvasGroup.blocksRaycasts = false;
            canvasGroup.alpha = 0.6f;
        }

        public void OnDrag(PointerEventData eventData) => rectTransform.position = eventData.position;

        public void OnEndDrag(PointerEventData eventData)
        {
            canvasGroup.blocksRaycasts = true;
            canvasGroup.alpha = 1.0f;

            // 2. ⭐ 드래그가 끝나면 '어디에 있든' 무조건 원래 부모(Slot)로 복귀시킵니다.
            // 이 코드가 있어야 슬롯이 사라지지 않습니다.
            if (originalParent != null)
            {
                transform.SetParent(originalParent);
                // Grid Layout Group 등이 위치를 잡을 수 있게 localPosition을 0으로.
                rectTransform.localPosition = Vector3.zero;
            }
            else
            {
                // 혹시라도 부모를 잃어버렸을 때를 대비한 안전장치
                rectTransform.position = originalPosition;
            }
        }
    }
}
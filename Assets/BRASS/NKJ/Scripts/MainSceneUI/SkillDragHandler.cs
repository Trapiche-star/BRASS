using UnityEngine;
using UnityEngine.EventSystems;
using Team1;

public class SkillDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public SkillData skillData; // 이 핸들러가 가진 스킬 데이터
    private CanvasGroup canvasGroup;
    private Vector3 originalPosition;
    private Transform originalParent;

    void Awake() => canvasGroup = GetComponent<CanvasGroup>() ?? gameObject.AddComponent<CanvasGroup>();

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalParent = transform.parent;
        originalPosition = transform.position;
        canvasGroup.blocksRaycasts = false;
        canvasGroup.alpha = 0.6f;
    }

    public void OnDrag(PointerEventData eventData) => transform.position = eventData.position;

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;
        canvasGroup.alpha = 1.0f;
        transform.SetParent(originalParent);
        transform.position = originalPosition;
    }
}
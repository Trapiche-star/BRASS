using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

/// <summary>
/// 퀘스트 받기 안내 슬롯 (더블클릭 시 AcceptPopup 열림)
/// </summary>
public class QuestOfferSlot : MonoBehaviour, IPointerClickHandler
{
    [Header("UI Components")]
    public TextMeshProUGUI offerText; // "퀘스트를 받으세요" 텍스트
    public Image icon; // 아이콘 (선택사항)

    private QuestData questData; // 제공할 퀘스트 데이터

    // 더블클릭 감지용
    private float lastClickTime = 0f;
    private const float doubleClickThreshold = 0.3f;

    /// <summary>
    /// 퀘스트 제공 슬롯 설정
    /// </summary>
    public void Setup(QuestData data)
    {
        try
        {
            if (data == null)
            {
                Debug.LogError("[QuestOfferSlot] Setup: QuestData가 null입니다!");
                return;
            }

            questData = data;

            // UI 업데이트
            if (offerText != null)
            {
                offerText.text = $"[새 퀘스트] {data.questName}\n클릭하여 확인";
            }

            Debug.Log($"[QuestOfferSlot] Setup 완료: {data.questName}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[QuestOfferSlot] Setup 에러: {e.Message}");
        }
    }

    // 클릭 이벤트 처리
    public void OnPointerClick(PointerEventData eventData)
    {
        try
        {
            float timeSinceLastClick = Time.time - lastClickTime;

            if (timeSinceLastClick <= doubleClickThreshold)
            {
                // 더블클릭 감지
                OnDoubleClick();
            }

            lastClickTime = Time.time;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[QuestOfferSlot] OnPointerClick 에러: {e.Message}");
        }
    }

    // 더블클릭 시 수락 팝업 표시
    private void OnDoubleClick()
    {
        try
        {
            if (QuestManager.Instance != null && questData != null)
            {
                QuestManager.Instance.ShowAcceptPopup(questData.questID);
                Debug.Log($"[QuestOfferSlot] 더블클릭: {questData.questName} 수락 팝업 열림");
            }
            else
            {
                if (QuestManager.Instance == null)
                    Debug.LogWarning("[QuestOfferSlot] QuestManager.Instance가 null입니다!");
                if (questData == null)
                    Debug.LogWarning("[QuestOfferSlot] questData가 null입니다!");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[QuestOfferSlot] OnDoubleClick 에러: {e.Message}");
        }
    }
}
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class QuestSlot : MonoBehaviour, IPointerClickHandler
{
    [Header("UI Components")]
    public TextMeshProUGUI titleText;       // 퀘스트 제목
    public TextMeshProUGUI descText;        // 설명
    public TextMeshProUGUI progressText;    // 진행도 (0/10)
    public TextMeshProUGUI rewardText;      // 보상 (100 G)
    public Image stateIndicator;            // 상태 표시 (선택사항)

    [Header("State Colors (Optional)")]
    public Color inProgressColor = Color.yellow;
    public Color canCompleteColor = Color.green;

    private QuestData questData;
    private QuestProgress questProgress;

    // 더블클릭 감지용
    private float lastClickTime = 0f;
    private const float doubleClickThreshold = 0.3f;

    // 데이터 설정 (방어 코드 추가)
    public void Setup(QuestData data, QuestProgress progress)
    {
        try
        {
            // Null 체크
            if (data == null)
            {
                Debug.LogError("[QuestSlot] Setup: QuestData가 null입니다!");
                return;
            }

            if (progress == null)
            {
                Debug.LogError("[QuestSlot] Setup: QuestProgress가 null입니다!");
                return;
            }

            questData = data;
            questProgress = progress;

            // UI 업데이트
            UpdateUI();
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[QuestSlot] Setup 에러: {e.Message}");
        }
    }

    private void UpdateUI()
    {
        try
        {
            if (questData == null || questProgress == null)
            {
                Debug.LogWarning("[QuestSlot] UpdateUI: questData 또는 questProgress가 null입니다!");
                return;
            }

            // 제목
            if (titleText != null)
            {
                titleText.text = questData.questName;
            }
            else
            {
                Debug.LogWarning("[QuestSlot] titleText가 연결되지 않았습니다!");
            }

            // 설명 (Description)
            if (descText != null)
            {
                descText.text = questData.description;
            }

            // 진행도
            if (progressText != null)
            {
                progressText.text = $"진행: {questProgress.currentProgress} / {questData.targetCount}";
            }
            else
            {
                Debug.LogWarning("[QuestSlot] progressText가 연결되지 않았습니다!");
            }

            // 보상 (Gold)
            if (rewardText != null)
            {
                rewardText.text = $"보상: {questData.rewardGold} G";
                if (questData.rewardExp > 0)
                {
                    rewardText.text += $", {questData.rewardExp} EXP";
                }
            }

            // 상태 표시 (선택사항)
            if (stateIndicator != null)
            {
                switch (questProgress.state)
                {
                    case QuestState.InProgress:
                        stateIndicator.color = inProgressColor;
                        break;
                    case QuestState.CanComplete:
                        stateIndicator.color = canCompleteColor;
                        break;
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[QuestSlot] UpdateUI 에러: {e.Message}");
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
            Debug.LogError($"[QuestSlot] OnPointerClick 에러: {e.Message}");
        }
    }

    // 더블클릭 시 상세 팝업 표시
    private void OnDoubleClick()
    {
        try
        {
            if (QuestManager.Instance != null && questData != null && questProgress != null)
            {
                QuestManager.Instance.ShowDetailPopup(questData, questProgress);
                Debug.Log($"[QuestSlot] 더블클릭: {questData.questName}");
            }
            else
            {
                if (QuestManager.Instance == null)
                    Debug.LogWarning("[QuestSlot] QuestManager.Instance가 null입니다!");
                if (questData == null)
                    Debug.LogWarning("[QuestSlot] questData가 null입니다!");
                if (questProgress == null)
                    Debug.LogWarning("[QuestSlot] questProgress가 null입니다!");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[QuestSlot] OnDoubleClick 에러: {e.Message}");
        }
    }
}
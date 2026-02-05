using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 퀘스트 정보를 표시하는 패널 (완료/미진행/진행중 공유)
/// </summary>
public class QuestInfoPanel : MonoBehaviour
{
    [Header("UI Components")]
    public TextMeshProUGUI questNameText;
    public TextMeshProUGUI questLevelText;
    public TextMeshProUGUI questDescText;
    public TextMeshProUGUI progressText;
    public TextMeshProUGUI rewardText;
    public Image questIcon; // 선택사항

    private QuestData questData;
    private QuestProgress questProgress;

    /// <summary>
    /// 퀘스트 정보 설정
    /// </summary>
    public void Setup(QuestData data, QuestProgress progress)
    {
        try
        {
            if (data == null || progress == null)
            {
                Debug.LogError("[QuestInfoPanel] data 또는 progress가 null입니다!");
                return;
            }

            questData = data;
            questProgress = progress;

            // UI 업데이트
            if (questNameText != null)
                questNameText.text = data.questName;

            if (questLevelText != null)
                questLevelText.text = $"Lv.{data.questLevel}";

            if (questDescText != null)
                questDescText.text = data.description;

            if (progressText != null)
            {
                string stateText = GetStateText(progress.state);
                progressText.text = $"{stateText} ({progress.currentProgress}/{data.targetCount})";
            }

            if (rewardText != null)
            {
                rewardText.text = $"보상: {data.rewardGold}G";
                if (data.rewardExp > 0)
                    rewardText.text += $", {data.rewardExp} EXP";
            }

            Debug.Log($"[QuestInfoPanel] Setup 완료: {data.questName}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[QuestInfoPanel] Setup 에러: {e.Message}");
        }
    }

    private string GetStateText(QuestState state)
    {
        switch (state)
        {
            case QuestState.NotAccepted: return "미수락";
            case QuestState.InProgress: return "진행중";
            case QuestState.CanComplete: return "완료 가능";
            case QuestState.Completed: return "완료됨";
            default: return "알 수 없음";
        }
    }
}
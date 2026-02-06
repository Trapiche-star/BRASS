using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// QuestDetailPopup에 표시되는 퀘스트 슬롯
/// </summary>
public class QuestDetailSlot : MonoBehaviour
{
    [Header("UI Components - 있는 것만 연결")]
    public TextMeshProUGUI questNameText;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI descriptionText;
    public TextMeshProUGUI progressText;
    public Slider progressSlider;
    public TextMeshProUGUI targetInfoText;
    public TextMeshProUGUI rewardGoldText;
    public TextMeshProUGUI rewardExpText;
    public Image questIcon;
    public Image npcPortrait;

    private QuestData questData;
    private QuestProgress questProgress;

    /// <summary>
    /// 퀘스트 데이터로 슬롯 업데이트
    /// </summary>
    public void Setup(QuestData data, QuestProgress progress)
    {
        try
        {
            if (data == null || progress == null)
            {
                Debug.LogError("[QuestDetailSlot] Setup: data 또는 progress가 null입니다!");
                return;
            }

            questData = data;
            questProgress = progress;

            // Quest Name
            if (questNameText != null)
            {
                questNameText.text = data.questName;
            }

            // Level
            if (levelText != null)
            {
                levelText.text = $"Lv.{data.questLevel}";
            }

            // Description
            if (descriptionText != null)
            {
                descriptionText.text = data.description;
            }

            // Progress Text
            if (progressText != null)
            {
                progressText.text = $"{progress.currentProgress} / {data.targetCount}";
            }

            // Progress Slider
            if (progressSlider != null)
            {
                progressSlider.maxValue = data.targetCount;
                progressSlider.value = progress.currentProgress;
            }

            // Target Info
            if (targetInfoText != null)
            {
                string typeText = GetQuestTypeText(data.questType);
                targetInfoText.text = $"{typeText}: {data.targetName} {data.targetCount}개";
            }

            // Reward Gold
            if (rewardGoldText != null)
            {
                rewardGoldText.text = $"{data.rewardGold} G";
            }

            // Reward Exp
            if (rewardExpText != null && data.rewardExp > 0)
            {
                rewardExpText.text = $"{data.rewardExp} EXP";
            }

            // NPC Portrait
            if (npcPortrait != null && data.npcPortrait != null)
            {
                npcPortrait.sprite = data.npcPortrait;
                npcPortrait.gameObject.SetActive(true);
            }
            else if (npcPortrait != null)
            {
                npcPortrait.gameObject.SetActive(false);
            }

            Debug.Log($"[QuestDetailSlot] Setup 완료: {data.questName}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[QuestDetailSlot] Setup 에러: {e.Message}");
        }
    }

    private string GetQuestTypeText(QuestType type)
    {
        switch (type)
        {
            case QuestType.Kill: return "처치";
            case QuestType.Collect: return "수집";
            case QuestType.Investigate: return "조사";
            case QuestType.Talk: return "대화";
            case QuestType.Escort: return "호위";
            case QuestType.Craft: return "제작";
            default: return "목표";
        }
    }
}
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// QuestAcceptPopup에 표시되는 퀘스트 슬롯
/// 프리팹에 이 스크립트를 붙이고, 데이터를 받아서 자체적으로 표시
/// </summary>
public class QuestAcceptSlot : MonoBehaviour
{
    [Header("UI Components - 있는 것만 연결")]
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI descriptionText;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI rewardGoldText;
    public TextMeshProUGUI rewardExpText;
    public Image questIcon;
    public Image npcPortrait;

    private QuestData questData;

    /// <summary>
    /// 퀘스트 데이터로 슬롯 업데이트
    /// </summary>
    public void Setup(QuestData data)
    {
        try
        {
            if (data == null)
            {
                Debug.LogError("[QuestAcceptSlot] Setup: QuestData가 null입니다!");
                return;
            }

            questData = data;

            // Title (있으면)
            if (titleText != null)
            {
                titleText.text = data.questName;
            }

            // Description (있으면)
            if (descriptionText != null)
            {
                descriptionText.text = data.description;
            }

            // Level (있으면)
            if (levelText != null)
            {
                levelText.text = $"Lv.{data.questLevel}";
            }

            // Reward Gold (있으면)
            if (rewardGoldText != null)
            {
                rewardGoldText.text = $"{data.rewardGold} G";
            }

            // Reward Exp (있으면)
            if (rewardExpText != null && data.rewardExp > 0)
            {
                rewardExpText.text = $"{data.rewardExp} EXP";
            }

            // NPC Portrait (있으면)
            if (npcPortrait != null && data.npcPortrait != null)
            {
                npcPortrait.sprite = data.npcPortrait;
                npcPortrait.gameObject.SetActive(true);
            }
            else if (npcPortrait != null)
            {
                npcPortrait.gameObject.SetActive(false);
            }

            Debug.Log($"[QuestAcceptSlot] Setup 완료: {data.questName}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[QuestAcceptSlot] Setup 에러: {e.Message}");
        }
    }
}
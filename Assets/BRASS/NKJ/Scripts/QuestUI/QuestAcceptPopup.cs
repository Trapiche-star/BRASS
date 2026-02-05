using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// NPC와 대화 시 나타나는 퀘스트 수락/거절 팝업 (방어 코드 추가)
/// </summary>
public class QuestAcceptPopup : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI questTitleText;
    public TextMeshProUGUI questDescriptionText;
    public TextMeshProUGUI questLevelText;
    public TextMeshProUGUI targetText; // "고블린 10마리 처치"
    public TextMeshProUGUI rewardText; // "보상: 100 Gold, 50 EXP"
    public Image npcPortraitImage; // NPC 초상화

    [Header("Buttons")]
    public Button acceptButton;
    public Button rejectButton;
    public Button closeButton; // X 버튼

    private QuestData currentQuest;

    private bool _isInitialized = false;

    void OnEnable()
    {
        // 팝업이 활성화될 때마다 버튼 연결 (한 번만)
        if (!_isInitialized)
        {
            InitializeButtons();
            _isInitialized = true;
        }
    }

    void InitializeButtons()
    {
        try
        {
            // 버튼 이벤트 연결
            if (acceptButton != null)
            {
                acceptButton.onClick.RemoveAllListeners();
                acceptButton.onClick.AddListener(OnAcceptClicked);
            }
            else
                Debug.LogWarning("[QuestAcceptPopup] acceptButton이 null입니다!");

            if (rejectButton != null)
            {
                rejectButton.onClick.RemoveAllListeners();
                rejectButton.onClick.AddListener(OnRejectClicked);
            }
            else
                Debug.LogWarning("[QuestAcceptPopup] rejectButton이 null입니다!");

            if (closeButton != null)
            {
                closeButton.onClick.RemoveAllListeners();
                closeButton.onClick.AddListener(OnRejectClicked);
            }

            Debug.Log("[QuestAcceptPopup] 버튼 초기화 완료");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[QuestAcceptPopup] InitializeButtons 에러: {e.Message}");
        }
    }

    public void Setup(QuestData quest)
    {
        try
        {
            if (quest == null)
            {
                Debug.LogError("[QuestAcceptPopup] Setup: QuestData가 null입니다!");
                return;
            }

            currentQuest = quest;

            // UI 업데이트
            if (questTitleText != null)
                questTitleText.text = quest.questName;

            if (questDescriptionText != null)
                questDescriptionText.text = quest.description;

            if (questLevelText != null)
                questLevelText.text = $"Lv. {quest.questLevel}";

            // 목표 텍스트
            if (targetText != null)
            {
                string typeText = GetQuestTypeText(quest.questType);
                targetText.text = $"{typeText}: {quest.targetName} {quest.targetCount}개";
            }

            // 보상 텍스트
            if (rewardText != null)
            {
                rewardText.text = $"보상: {quest.rewardGold} Gold";
                if (quest.rewardExp > 0)
                    rewardText.text += $", {quest.rewardExp} EXP";
            }

            // NPC 초상화
            if (npcPortraitImage != null && quest.npcPortrait != null)
            {
                npcPortraitImage.sprite = quest.npcPortrait;
                npcPortraitImage.gameObject.SetActive(true);
            }
            else if (npcPortraitImage != null)
            {
                npcPortraitImage.gameObject.SetActive(false);
            }

            Debug.Log($"[QuestAcceptPopup] Setup 완료: {quest.questName}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[QuestAcceptPopup] Setup 에러: {e.Message}");
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

    private void OnAcceptClicked()
    {
        try
        {
            Debug.Log("[QuestAcceptPopup] 수락 버튼 클릭");

            if (QuestManager.Instance != null)
            {
                QuestManager.Instance.AcceptQuest();
            }
            else
            {
                Debug.LogError("[QuestAcceptPopup] QuestManager.Instance가 null입니다!");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[QuestAcceptPopup] OnAcceptClicked 에러: {e.Message}");
        }
    }

    private void OnRejectClicked()
    {
        try
        {
            Debug.Log("[QuestAcceptPopup] 거절 버튼 클릭");

            if (QuestManager.Instance != null)
            {
                QuestManager.Instance.RejectQuest();
            }
            else
            {
                Debug.LogError("[QuestAcceptPopup] QuestManager.Instance가 null입니다!");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[QuestAcceptPopup] OnRejectClicked 에러: {e.Message}");
        }
    }
}
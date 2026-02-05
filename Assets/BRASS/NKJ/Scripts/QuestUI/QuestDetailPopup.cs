using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 퀘스트 슬롯 더블클릭 시 나타나는 상세 정보 팝업 (방어 코드 추가)
/// 완료/미진행/진행중 탭으로 필터링 가능
/// </summary>
public class QuestDetailPopup : MonoBehaviour
{
    [Header("Basic Info")]
    public TextMeshProUGUI questTitleText;
    public TextMeshProUGUI questLevelText;
    public TextMeshProUGUI questDescriptionText;
    public Image npcPortraitImage;

    [Header("Progress")]
    public TextMeshProUGUI progressText; // "6/25"
    public Slider progressSlider; // 진행도 바 (선택사항)
    public TextMeshProUGUI targetInfoText; // "목표: 고블린 처치"

    [Header("Rewards")]
    public TextMeshProUGUI rewardGoldText;
    public TextMeshProUGUI rewardExpText;
    public Transform rewardItemsParent; // 아이템 보상 아이콘을 표시할 부모 (나중에 인벤토리 연동 시 사용)

    [Header("Tab Buttons")]
    public Button inProgressTab; // 진행중 탭
    public Button notStartedTab; // 미진행 탭
    public Button completedTab; // 완료 탭

    [Header("Quest Info Container")]
    public Transform questInfoContainer; // 퀘스트 정보 프리팹이 들어갈 부모
    public GameObject questInfoPrefab; // 퀘스트 정보 프리팹 (공유)

    [Header("Buttons")]
    public Button completeButton; // 완료 버튼
    public Button abandonButton; // 포기 버튼
    public Button closeButton; // 닫기 버튼

    private QuestData currentQuest;
    private QuestProgress currentProgress;
    private QuestState currentFilter = QuestState.InProgress; // 현재 필터
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
            if (completeButton != null)
            {
                completeButton.onClick.RemoveAllListeners();
                completeButton.onClick.AddListener(OnCompleteClicked);
            }
            else
                Debug.LogWarning("[QuestDetailPopup] completeButton이 null입니다!");

            if (abandonButton != null)
            {
                abandonButton.onClick.RemoveAllListeners();
                abandonButton.onClick.AddListener(OnAbandonClicked);
            }
            else
                Debug.LogWarning("[QuestDetailPopup] abandonButton이 null입니다!");

            if (closeButton != null)
            {
                closeButton.onClick.RemoveAllListeners();
                closeButton.onClick.AddListener(OnCloseClicked);
            }
            else
                Debug.LogWarning("[QuestDetailPopup] closeButton이 null입니다!");

            // 탭 버튼 이벤트 연결
            if (inProgressTab != null)
            {
                inProgressTab.onClick.RemoveAllListeners();
                inProgressTab.onClick.AddListener(() => OnTabClicked(QuestState.InProgress));
            }

            if (notStartedTab != null)
            {
                notStartedTab.onClick.RemoveAllListeners();
                notStartedTab.onClick.AddListener(() => OnTabClicked(QuestState.NotAccepted));
            }

            if (completedTab != null)
            {
                completedTab.onClick.RemoveAllListeners();
                completedTab.onClick.AddListener(() => OnTabClicked(QuestState.Completed));
            }

            Debug.Log("[QuestDetailPopup] 버튼 초기화 완료");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[QuestDetailPopup] InitializeButtons 에러: {e.Message}");
        }
    }

    public void Setup(QuestData quest, QuestProgress progress)
    {
        try
        {
            if (quest == null)
            {
                Debug.LogError("[QuestDetailPopup] Setup: QuestData가 null입니다!");
                return;
            }

            if (progress == null)
            {
                Debug.LogError("[QuestDetailPopup] Setup: QuestProgress가 null입니다!");
                return;
            }

            currentQuest = quest;
            currentProgress = progress;

            UpdateUI();

            Debug.Log($"[QuestDetailPopup] Setup 완료: {quest.questName}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[QuestDetailPopup] Setup 에러: {e.Message}");
        }
    }

    private void UpdateUI()
    {
        try
        {
            if (currentQuest == null || currentProgress == null)
            {
                Debug.LogWarning("[QuestDetailPopup] UpdateUI: currentQuest 또는 currentProgress가 null입니다!");
                return;
            }

            // 기본 정보
            if (questTitleText != null)
                questTitleText.text = currentQuest.questName;

            if (questLevelText != null)
                questLevelText.text = $"Lv. {currentQuest.questLevel}";

            if (questDescriptionText != null)
                questDescriptionText.text = currentQuest.description;

            // NPC 초상화
            if (npcPortraitImage != null && currentQuest.npcPortrait != null)
            {
                npcPortraitImage.sprite = currentQuest.npcPortrait;
                npcPortraitImage.gameObject.SetActive(true);
            }
            else if (npcPortraitImage != null)
            {
                npcPortraitImage.gameObject.SetActive(false);
            }

            // 진행도
            if (progressText != null)
            {
                progressText.text = $"{currentProgress.currentProgress} / {currentQuest.targetCount}";
            }

            if (progressSlider != null)
            {
                progressSlider.maxValue = currentQuest.targetCount;
                progressSlider.value = currentProgress.currentProgress;
            }

            // 목표 정보
            if (targetInfoText != null)
            {
                string typeText = GetQuestTypeText(currentQuest.questType);
                targetInfoText.text = $"{typeText}: {currentQuest.targetName} {currentQuest.targetCount}개";
            }

            // 보상
            if (rewardGoldText != null)
                rewardGoldText.text = $"{currentQuest.rewardGold} G";

            if (rewardExpText != null)
                rewardExpText.text = $"{currentQuest.rewardExp} EXP";

            // 완료 버튼 활성화 여부
            if (completeButton != null)
            {
                bool canComplete = currentProgress.state == QuestState.CanComplete;
                completeButton.interactable = canComplete;

                // 버튼 텍스트 변경 (선택사항)
                TextMeshProUGUI buttonText = completeButton.GetComponentInChildren<TextMeshProUGUI>();
                if (buttonText != null)
                {
                    buttonText.text = canComplete ? "완료" : $"진행중 ({currentProgress.currentProgress}/{currentQuest.targetCount})";
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[QuestDetailPopup] UpdateUI 에러: {e.Message}");
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

    private void OnCompleteClicked()
    {
        try
        {
            Debug.Log("[QuestDetailPopup] 완료 버튼 클릭");

            if (QuestManager.Instance != null && currentQuest != null)
            {
                QuestManager.Instance.CompleteQuest(currentQuest.questID);
                gameObject.SetActive(false);
            }
            else
            {
                if (QuestManager.Instance == null)
                    Debug.LogError("[QuestDetailPopup] QuestManager.Instance가 null입니다!");
                if (currentQuest == null)
                    Debug.LogError("[QuestDetailPopup] currentQuest가 null입니다!");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[QuestDetailPopup] OnCompleteClicked 에러: {e.Message}");
        }
    }

    private void OnAbandonClicked()
    {
        try
        {
            Debug.Log("[QuestDetailPopup] 포기 버튼 클릭");

            if (QuestManager.Instance != null && currentQuest != null)
            {
                // 확인 팝업을 띄우는 것이 좋음 (선택사항)
                bool confirm = true; // 실제로는 확인 팝업 결과

                if (confirm)
                {
                    QuestManager.Instance.AbandonQuest(currentQuest.questID);
                    gameObject.SetActive(false);
                }
            }
            else
            {
                if (QuestManager.Instance == null)
                    Debug.LogError("[QuestDetailPopup] QuestManager.Instance가 null입니다!");
                if (currentQuest == null)
                    Debug.LogError("[QuestDetailPopup] currentQuest가 null입니다!");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[QuestDetailPopup] OnAbandonClicked 에러: {e.Message}");
        }
    }

    private void OnCloseClicked()
    {
        try
        {
            Debug.Log("[QuestDetailPopup] 닫기 버튼 클릭");
            gameObject.SetActive(false);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[QuestDetailPopup] OnCloseClicked 에러: {e.Message}");
        }
    }

    // 탭 클릭 시 필터 변경
    private void OnTabClicked(QuestState filter)
    {
        try
        {
            Debug.Log($"[QuestDetailPopup] 탭 변경: {filter}");
            currentFilter = filter;
            RefreshQuestList();
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[QuestDetailPopup] OnTabClicked 에러: {e.Message}");
        }
    }

    // 필터에 맞는 퀘스트 목록 표시
    private void RefreshQuestList()
    {
        try
        {
            if (questInfoContainer == null)
            {
                Debug.LogWarning("[QuestDetailPopup] questInfoContainer가 null입니다!");
                return;
            }

            // 기존 목록 삭제
            foreach (Transform child in questInfoContainer)
            {
                Destroy(child.gameObject);
            }

            if (QuestManager.Instance == null)
            {
                Debug.LogError("[QuestDetailPopup] QuestManager.Instance가 null입니다!");
                return;
            }

            // 필터에 맞는 퀘스트 가져오기
            List<QuestData> filteredQuests = new List<QuestData>();

            foreach (var kvp in QuestManager.Instance.questProgressDict)
            {
                if (kvp.Value.state == currentFilter)
                {
                    QuestData data = QuestManager.Instance.GetQuestData(kvp.Key);
                    if (data != null)
                    {
                        filteredQuests.Add(data);
                    }
                }
            }

            // 프리팹 생성
            foreach (var quest in filteredQuests)
            {
                if (questInfoPrefab != null)
                {
                    GameObject infoObj = Instantiate(questInfoPrefab, questInfoContainer);

                    // 프리팹에 정보 표시 (QuestInfoPanel 스크립트 필요)
                    QuestInfoPanel infoPanel = infoObj.GetComponent<QuestInfoPanel>();
                    if (infoPanel != null)
                    {
                        QuestProgress progress = QuestManager.Instance.questProgressDict[quest.questID];
                        infoPanel.Setup(quest, progress);
                    }
                }
            }

            Debug.Log($"[QuestDetailPopup] {currentFilter} 퀘스트 {filteredQuests.Count}개 표시");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[QuestDetailPopup] RefreshQuestList 에러: {e.Message}");
        }
    }
}
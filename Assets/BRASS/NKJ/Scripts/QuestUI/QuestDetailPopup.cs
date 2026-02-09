using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 퀘스트 상세 정보 팝업 - 슬롯 기반
/// </summary>
public class QuestDetailPopup : MonoBehaviour
{
    [Header("Slot Prefab")]
    public GameObject questDetailSlotPrefab; // QuestDetailSlot 프리팹
    public Transform slotContainer; // 슬롯이 생성될 부모

    [Header("Tab Buttons")]
    public Button inProgressTab;
    public Button notStartedTab;
    public Button completedTab;

    [Header("Quest Info Container")]
    public Transform questInfoContainer; // 탭별 퀘스트 목록
    public GameObject questInfoPrefab; // QuestInfoPanel 프리팹 (탭용)

    [Header("Buttons")]
    public Button closeButton;

    private QuestData currentQuest;
    private QuestProgress currentProgress;
    private GameObject currentSlotInstance;
    private QuestState currentFilter = QuestState.InProgress;
    private bool _isInitialized = false;

    void OnEnable()
    {
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
            if (closeButton != null)
            {
                closeButton.onClick.RemoveAllListeners();
                closeButton.onClick.AddListener(OnCloseClicked);
            }

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
            if (quest == null || progress == null)
            {
                Debug.LogError("[QuestDetailPopup] Setup: quest 또는 progress가 null입니다!");
                return;
            }

            currentQuest = quest;
            currentProgress = progress;

            // 기존 슬롯 삭제
            if (currentSlotInstance != null)
            {
                Destroy(currentSlotInstance);
            }

            // 슬롯 컨테이너 찾기
            Transform container = slotContainer != null ? slotContainer : transform;

            // 새 슬롯 생성
            if (questDetailSlotPrefab != null)
            {
                currentSlotInstance = Instantiate(questDetailSlotPrefab, container);

                QuestDetailSlot slot = currentSlotInstance.GetComponent<QuestDetailSlot>();
                if (slot != null)
                {
                    slot.Setup(quest, progress);
                    Debug.Log($"[QuestDetailPopup] 슬롯 생성 완료: {quest.questName}");
                }
                else
                {
                    Debug.LogError("[QuestDetailPopup] 프리팹에 QuestDetailSlot 스크립트가 없습니다!");
                }
            }

            // 탭 초기화
            OnTabClicked(QuestState.InProgress);

            Debug.Log($"[QuestDetailPopup] Setup 완료: {quest.questName}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[QuestDetailPopup] Setup 에러: {e.Message}");
        }
    }

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

    private void RefreshQuestList()
    {
        try
        {
            if (questInfoContainer == null || questInfoPrefab == null)
            {
                Debug.LogWarning("[QuestDetailPopup] questInfoContainer 또는 questInfoPrefab이 null입니다!");
                return;
            }

            // 기존 목록 삭제
            foreach (Transform child in questInfoContainer)
            {
                Destroy(child.gameObject);
            }

            if (QuestManager.Instance == null) return;

            // 필터에 맞는 퀘스트 찾기
            foreach (var kvp in QuestManager.Instance.questProgressDict)
            {
                if (kvp.Value.state == currentFilter)
                {
                    QuestData data = QuestManager.Instance.GetQuestData(kvp.Key);
                    if (data != null)
                    {
                        GameObject infoObj = Instantiate(questInfoPrefab, questInfoContainer);
                        QuestInfoPanel infoPanel = infoObj.GetComponent<QuestInfoPanel>();

                        if (infoPanel != null)
                        {
                            infoPanel.Setup(data, kvp.Value);
                        }
                    }
                }
            }

            Debug.Log($"[QuestDetailPopup] {currentFilter} 퀘스트 목록 갱신");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[QuestDetailPopup] RefreshQuestList 에러: {e.Message}");
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
}
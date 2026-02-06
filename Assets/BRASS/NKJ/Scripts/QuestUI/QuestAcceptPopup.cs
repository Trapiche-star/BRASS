using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 퀘스트 수락/거절 팝업 - 슬롯 기반
/// </summary>
public class QuestAcceptPopup : MonoBehaviour
{
    [Header("Slot Prefab")]
    public GameObject questAcceptSlotPrefab; // QuestAcceptSlot 프리팹
    public Transform slotContainer; // 슬롯이 생성될 부모

    [Header("Buttons")]
    public Button acceptButton;
    public Button rejectButton;
    public Button closeButton;

    private QuestData currentQuest;
    private GameObject currentSlotInstance;
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
            if (acceptButton != null)
            {
                acceptButton.onClick.RemoveAllListeners();
                acceptButton.onClick.AddListener(OnAcceptClicked);
            }

            if (rejectButton != null)
            {
                rejectButton.onClick.RemoveAllListeners();
                rejectButton.onClick.AddListener(OnRejectClicked);
            }

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

            // 기존 슬롯 삭제
            if (currentSlotInstance != null)
            {
                Destroy(currentSlotInstance);
            }

            // 슬롯 컨테이너 찾기 (없으면 자기 자신)
            Transform container = slotContainer != null ? slotContainer : transform;

            // 새 슬롯 생성
            if (questAcceptSlotPrefab != null)
            {
                currentSlotInstance = Instantiate(questAcceptSlotPrefab, container);

                QuestAcceptSlot slot = currentSlotInstance.GetComponent<QuestAcceptSlot>();
                if (slot != null)
                {
                    slot.Setup(quest);
                    Debug.Log($"[QuestAcceptPopup] 슬롯 생성 완료: {quest.questName}");
                }
                else
                {
                    Debug.LogError("[QuestAcceptPopup] 프리팹에 QuestAcceptSlot 스크립트가 없습니다!");
                }
            }
            else
            {
                Debug.LogError("[QuestAcceptPopup] questAcceptSlotPrefab이 null입니다!");
            }

            Debug.Log($"[QuestAcceptPopup] Setup 완료: {quest.questName}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[QuestAcceptPopup] Setup 에러: {e.Message}");
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
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[QuestAcceptPopup] OnRejectClicked 에러: {e.Message}");
        }
    }
}
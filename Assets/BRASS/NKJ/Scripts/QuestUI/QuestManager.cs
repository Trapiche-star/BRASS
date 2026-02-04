using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[System.Serializable]
public class QuestProgress
{
    public int questID;
    public QuestState state;
    public int currentProgress; // 현재 진행도

    public QuestProgress(int id)
    {
        questID = id;
        state = QuestState.NotAccepted;
        currentProgress = 0;
    }
}

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance;

    [Header("Quest Database")]
    public List<QuestData> allQuestDB; // 모든 퀘스트 데이터

    // 퀘스트 진행 상황 저장
    public Dictionary<int, QuestProgress> questProgressDict = new Dictionary<int, QuestProgress>();

    [Header("UI References")]
    public GameObject questPanel; // 메인 퀘스트 창
    public Transform questListParent; // Scroll View의 Content
    public GameObject questSlotPrefab;

    [Header("Popup References")]
    public GameObject questDetailPopup; // 상세 정보 팝업
    public GameObject questAcceptPopup; // 수락/거절 팝업

    [Header("Input Settings")]
    public InputActionReference toggleQuestPanelAction; // Q키 액션

    // 이벤트
    public event Action OnQuestUpdated;
    public event Action<int> OnQuestCompleted; // 퀘스트 완료 시

    // 현재 선택된 퀘스트 (팝업용)
    private QuestData currentSelectedQuest;
    private int _nextQuestToShow = -1; // 다음 퀘스트 ID 임시 저장용

    // 방어 코드: 재진입 방지
    private bool _isRefreshing = false;
    private bool _isInitialized = false;

    void Awake()
    {
        // Singleton 중복 방지
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("[QuestManager] 중복 인스턴스 감지! 기존 인스턴스 유지");
            Destroy(gameObject);
            return;
        }

        Instance = this;
        Debug.Log("[QuestManager] Awake 완료");
    }

    void Start()
    {
        if (_isInitialized)
        {
            Debug.LogWarning("[QuestManager] 이미 초기화됨! Start 중복 호출 방지");
            return;
        }

        try
        {
            Debug.Log("[QuestManager] Start 시작");

            // 이벤트 중복 등록 방지
            OnQuestUpdated -= RefreshQuestList;
            OnQuestUpdated += RefreshQuestList;

            // Input System 연결
            if (toggleQuestPanelAction != null)
            {
                toggleQuestPanelAction.action.performed -= OnToggleQuestPanel;
                toggleQuestPanelAction.action.performed += OnToggleQuestPanel;
                toggleQuestPanelAction.action.Enable();
            }

            // 초기 상태: 팝업들만 숨김, 퀘스트창은 그대로
            // questPanel은 사용자가 Inspector에서 설정한 상태 유지

            if (questDetailPopup != null)
                questDetailPopup.SetActive(false);

            if (questAcceptPopup != null)
                questAcceptPopup.SetActive(false);

            // 저장된 데이터 로드
            LoadQuestData();

            _isInitialized = true;
            Debug.Log("[QuestManager] Start 완료");
        }
        catch (Exception e)
        {
            Debug.LogError($"[QuestManager] Start 에러: {e.Message}\n{e.StackTrace}");
        }
    }

    void OnDestroy()
    {
        Debug.Log("[QuestManager] OnDestroy 호출");

        // 이벤트 정리
        OnQuestUpdated -= RefreshQuestList;

        if (toggleQuestPanelAction != null)
        {
            toggleQuestPanelAction.action.performed -= OnToggleQuestPanel;
            toggleQuestPanelAction.action.Disable();
        }
    }

    // Q키로 퀘스트창 토글
    private void OnToggleQuestPanel(InputAction.CallbackContext context)
    {
        try
        {
            if (questPanel != null)
            {
                bool isActive = questPanel.activeSelf;
                questPanel.SetActive(!isActive);

                Debug.Log($"[QuestManager] 퀘스트창 {(isActive ? "닫힘" : "열림")}");

                if (!isActive) // 켜질 때마다 새로고침
                {
                    RefreshQuestList();
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"[QuestManager] OnToggleQuestPanel 에러: {e.Message}");
        }
    }

    #region 퀘스트 수락/거절 시스템

    // NPC와 대화 시 호출
    public void ShowAcceptPopup(int questID)
    {
        try
        {
            QuestData quest = GetQuestData(questID);
            if (quest == null)
            {
                Debug.LogError($"[QuestManager] 퀘스트 ID {questID}를 찾을 수 없습니다.");
                return;
            }

            // 이미 수락했거나 완료한 퀘스트는 팝업 안띄움
            if (questProgressDict.ContainsKey(questID))
            {
                QuestState state = questProgressDict[questID].state;
                if (state != QuestState.NotAccepted)
                {
                    Debug.Log($"[QuestManager] 퀘스트 '{quest.questName}'은(는) 이미 진행중이거나 완료되었습니다.");
                    return;
                }
            }

            currentSelectedQuest = quest;

            // 팝업 UI에 정보 표시
            if (questAcceptPopup != null)
            {
                questAcceptPopup.SetActive(true);
                var popup = questAcceptPopup.GetComponent<QuestAcceptPopup>();
                if (popup != null)
                {
                    popup.Setup(quest);
                }
                else
                {
                    Debug.LogWarning("[QuestManager] QuestAcceptPopup 컴포넌트를 찾을 수 없습니다.");
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"[QuestManager] ShowAcceptPopup 에러: {e.Message}");
        }
    }

    // 수락 버튼 클릭 시
    public void AcceptQuest()
    {
        try
        {
            if (currentSelectedQuest == null)
            {
                Debug.LogWarning("[QuestManager] currentSelectedQuest가 null입니다.");
                return;
            }

            int questID = currentSelectedQuest.questID;

            // 선행 퀘스트 확인
            if (currentSelectedQuest.prerequisiteQuestID != -1)
            {
                if (!IsQuestCompleted(currentSelectedQuest.prerequisiteQuestID))
                {
                    Debug.Log($"[QuestManager] 선행 퀘스트를 먼저 완료해야 합니다.");
                    return;
                }
            }

            // 퀘스트 진행 상태 추가
            if (!questProgressDict.ContainsKey(questID))
            {
                questProgressDict.Add(questID, new QuestProgress(questID));
            }

            questProgressDict[questID].state = QuestState.InProgress;
            questProgressDict[questID].currentProgress = 0;

            Debug.Log($"[QuestManager] 퀘스트 수락: {currentSelectedQuest.questName}");

            // 팝업 닫기
            if (questAcceptPopup != null)
                questAcceptPopup.SetActive(false);

            OnQuestUpdated?.Invoke();
            SaveQuestData();
        }
        catch (Exception e)
        {
            Debug.LogError($"[QuestManager] AcceptQuest 에러: {e.Message}");
        }
    }

    // 거절 버튼 클릭 시
    public void RejectQuest()
    {
        try
        {
            Debug.Log($"[QuestManager] 퀘스트 거절: {currentSelectedQuest?.questName}");

            // 팝업만 닫기
            if (questAcceptPopup != null)
                questAcceptPopup.SetActive(false);

            currentSelectedQuest = null;
        }
        catch (Exception e)
        {
            Debug.LogError($"[QuestManager] RejectQuest 에러: {e.Message}");
        }
    }

    #endregion

    #region 퀘스트 진행 및 완료

    // 퀘스트 진행도 업데이트
    public void UpdateQuestProgress(int questID, int amount = 1)
    {
        try
        {
            if (!questProgressDict.ContainsKey(questID))
            {
                Debug.LogWarning($"[QuestManager] 진행중인 퀘스트가 아닙니다: {questID}");
                return;
            }

            QuestProgress progress = questProgressDict[questID];

            if (progress.state != QuestState.InProgress)
            {
                return;
            }

            QuestData questData = GetQuestData(questID);
            if (questData == null) return;

            progress.currentProgress += amount;

            // 목표 달성 확인
            if (progress.currentProgress >= questData.targetCount)
            {
                progress.currentProgress = questData.targetCount; // 최대치 제한
                progress.state = QuestState.CanComplete;
                Debug.Log($"[QuestManager] 퀘스트 완료 가능: {questData.questName}");
            }

            OnQuestUpdated?.Invoke();
            SaveQuestData();
        }
        catch (Exception e)
        {
            Debug.LogError($"[QuestManager] UpdateQuestProgress 에러: {e.Message}");
        }
    }

    // 퀘스트 완료 처리
    public void CompleteQuest(int questID)
    {
        try
        {
            if (!questProgressDict.ContainsKey(questID))
            {
                Debug.LogWarning($"[QuestManager] 진행중인 퀘스트가 아닙니다: {questID}");
                return;
            }

            QuestProgress progress = questProgressDict[questID];
            QuestData questData = GetQuestData(questID);

            if (questData == null) return;

            // 목표 달성 확인
            if (progress.currentProgress < questData.targetCount)
            {
                Debug.LogWarning($"[QuestManager] 퀘스트 목표를 아직 달성하지 못했습니다.");
                return;
            }

            // 보상 지급
            GiveReward(questData);

            // 상태 변경
            progress.state = QuestState.Completed;

            Debug.Log($"[QuestManager] 퀘스트 완료: {questData.questName}");
            OnQuestCompleted?.Invoke(questID);
            OnQuestUpdated?.Invoke();

            // 다음 퀘스트가 있다면 자동으로 수락 팝업 표시 (선택사항)
            if (questData.nextQuestID != -1)
            {
                _nextQuestToShow = questData.nextQuestID;
                Invoke(nameof(ShowNextQuestPopup), 0.5f);
            }

            SaveQuestData();
        }
        catch (Exception e)
        {
            Debug.LogError($"[QuestManager] CompleteQuest 에러: {e.Message}");
        }
    }

    // 보상 지급
    private void GiveReward(QuestData quest)
    {
        try
        {
            Debug.Log($"[QuestManager] 보상 획득: {quest.rewardGold} Gold, {quest.rewardExp} EXP");

            // TODO: 실제 골드/경험치 지급 로직
            // PlayerManager.Instance.AddGold(quest.rewardGold);
            // PlayerManager.Instance.AddExp(quest.rewardExp);

            // TODO: 아이템 보상 지급
            // foreach (var item in quest.rewardItems)
            // {
            //     InventoryManager.Instance.AddItem(item);
            // }
        }
        catch (Exception e)
        {
            Debug.LogError($"[QuestManager] GiveReward 에러: {e.Message}");
        }
    }

    // 퀘스트 포기
    public void AbandonQuest(int questID)
    {
        try
        {
            if (!questProgressDict.ContainsKey(questID))
            {
                Debug.LogWarning($"[QuestManager] 진행중인 퀘스트가 아닙니다: {questID}");
                return;
            }

            questProgressDict.Remove(questID);
            Debug.Log($"[QuestManager] 퀘스트 포기: {questID}");

            OnQuestUpdated?.Invoke();
            SaveQuestData();
        }
        catch (Exception e)
        {
            Debug.LogError($"[QuestManager] AbandonQuest 에러: {e.Message}");
        }
    }

    #endregion

    #region UI 관리

    // 퀘스트 목록 새로고침 (방어 코드 강화)
    public void RefreshQuestList()
    {
        // 재진입 방지
        if (_isRefreshing)
        {
            Debug.LogWarning("[QuestManager] RefreshQuestList 재진입 감지! 무한 루프 방지");
            return;
        }

        if (questListParent == null)
        {
            Debug.LogWarning("[QuestManager] questListParent가 null입니다!");
            return;
        }

        _isRefreshing = true;

        try
        {
            Debug.Log("[QuestManager] 퀘스트 목록 새로고침 시작");

            // 기존 슬롯 전부 삭제 (안전하게)
            int childCount = questListParent.childCount;
            for (int i = childCount - 1; i >= 0; i--)
            {
                Transform child = questListParent.GetChild(i);
                if (child != null)
                {
                    Destroy(child.gameObject);
                }
            }

            // 딕셔너리 null 체크
            if (questProgressDict == null || questProgressDict.Count == 0)
            {
                Debug.Log("[QuestManager] 진행중인 퀘스트가 없습니다.");
                return;
            }

            // 진행중인 퀘스트만 표시
            int slotCount = 0;
            foreach (var kvp in questProgressDict)
            {
                QuestProgress progress = kvp.Value;

                if (progress == null) continue;

                // 완료된 퀘스트는 리스트에서 제거 (선택사항)
                if (progress.state == QuestState.Completed)
                    continue;

                QuestData data = GetQuestData(kvp.Key);
                if (data != null && questSlotPrefab != null)
                {
                    GameObject slotGo = Instantiate(questSlotPrefab, questListParent);
                    QuestSlot slot = slotGo.GetComponent<QuestSlot>();

                    if (slot != null)
                    {
                        slot.Setup(data, progress);
                        slotCount++;
                    }
                    else
                    {
                        Debug.LogWarning($"[QuestManager] QuestSlot 컴포넌트를 찾을 수 없습니다: {slotGo.name}");
                    }
                }
            }

            Debug.Log($"[QuestManager] 퀘스트 슬롯 {slotCount}개 생성 완료");
        }
        catch (Exception e)
        {
            Debug.LogError($"[QuestManager] RefreshQuestList 에러: {e.Message}\n{e.StackTrace}");
        }
        finally
        {
            _isRefreshing = false;
        }
    }

    // 상세 팝업 표시 (더블클릭 시)
    public void ShowDetailPopup(QuestData quest, QuestProgress progress)
    {
        try
        {
            if (questDetailPopup == null)
            {
                Debug.LogWarning("[QuestManager] questDetailPopup이 null입니다!");
                return;
            }

            if (quest == null || progress == null)
            {
                Debug.LogWarning("[QuestManager] quest 또는 progress가 null입니다!");
                return;
            }

            questDetailPopup.SetActive(true);

            var popup = questDetailPopup.GetComponent<QuestDetailPopup>();
            if (popup != null)
            {
                popup.Setup(quest, progress);
            }
            else
            {
                Debug.LogWarning("[QuestManager] QuestDetailPopup 컴포넌트를 찾을 수 없습니다.");
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"[QuestManager] ShowDetailPopup 에러: {e.Message}");
        }
    }

    #endregion

    #region 헬퍼 함수

    public QuestData GetQuestData(int questID)
    {
        try
        {
            if (allQuestDB == null)
            {
                Debug.LogError("[QuestManager] allQuestDB가 null입니다!");
                return null;
            }

            return allQuestDB.Find(q => q.questID == questID);
        }
        catch (Exception e)
        {
            Debug.LogError($"[QuestManager] GetQuestData 에러: {e.Message}");
            return null;
        }
    }

    public bool IsQuestCompleted(int questID)
    {
        if (questProgressDict.ContainsKey(questID))
        {
            return questProgressDict[questID].state == QuestState.Completed;
        }
        return false;
    }

    public List<QuestData> GetInProgressQuests()
    {
        List<QuestData> quests = new List<QuestData>();

        try
        {
            foreach (var kvp in questProgressDict)
            {
                if (kvp.Value.state == QuestState.InProgress || kvp.Value.state == QuestState.CanComplete)
                {
                    QuestData data = GetQuestData(kvp.Key);
                    if (data != null)
                        quests.Add(data);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"[QuestManager] GetInProgressQuests 에러: {e.Message}");
        }

        return quests;
    }

    #endregion

    #region 데이터 저장/로드

    // 로컬 저장
    public void SaveQuestData()
    {
        try
        {
            QuestSaveData saveData = new QuestSaveData();

            foreach (var kvp in questProgressDict)
            {
                saveData.questIDs.Add(kvp.Key);
                saveData.questStates.Add((int)kvp.Value.state);
                saveData.questProgress.Add(kvp.Value.currentProgress);
            }

            string json = JsonUtility.ToJson(saveData);
            PlayerPrefs.SetString("QuestSaveData", json);
            PlayerPrefs.Save();

            Debug.Log("[QuestManager] 퀘스트 데이터 저장 완료");
        }
        catch (Exception e)
        {
            Debug.LogError($"[QuestManager] SaveQuestData 에러: {e.Message}");
        }
    }

    // 로컬 로드
    public void LoadQuestData()
    {
        try
        {
            if (!PlayerPrefs.HasKey("QuestSaveData"))
            {
                Debug.Log("[QuestManager] 저장된 퀘스트 데이터가 없습니다.");
                return;
            }

            string json = PlayerPrefs.GetString("QuestSaveData");
            QuestSaveData saveData = JsonUtility.FromJson<QuestSaveData>(json);

            if (saveData == null)
            {
                Debug.LogWarning("[QuestManager] 저장 데이터 파싱 실패");
                return;
            }

            questProgressDict.Clear();

            for (int i = 0; i < saveData.questIDs.Count; i++)
            {
                int questID = saveData.questIDs[i];
                QuestProgress progress = new QuestProgress(questID);
                progress.state = (QuestState)saveData.questStates[i];
                progress.currentProgress = saveData.questProgress[i];

                questProgressDict.Add(questID, progress);
            }

            Debug.Log($"[QuestManager] 퀘스트 데이터 로드 완료: {questProgressDict.Count}개");

            // 퀘스트창이 열려있을 때만 새로고침 (무한 루프 방지)
            if (questPanel != null && questPanel.activeSelf)
            {
                OnQuestUpdated?.Invoke();
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"[QuestManager] LoadQuestData 에러: {e.Message}");
        }
    }

    // 저장 데이터 초기화 (테스트용)
    public void ClearSaveData()
    {
        try
        {
            PlayerPrefs.DeleteKey("QuestSaveData");
            questProgressDict.Clear();
            OnQuestUpdated?.Invoke();
            Debug.Log("[QuestManager] 퀘스트 데이터 초기화 완료");
        }
        catch (Exception e)
        {
            Debug.LogError($"[QuestManager] ClearSaveData 에러: {e.Message}");
        }
    }

    #endregion

    // 다음 퀘스트 팝업 표시 (Invoke용 헬퍼)
    void ShowNextQuestPopup()
    {
        if (_nextQuestToShow != -1)
        {
            ShowAcceptPopup(_nextQuestToShow);
            _nextQuestToShow = -1;
        }
    }

    // 테스트용 함수
    void TestShowAcceptPopup()
    {
        if (allQuestDB != null && allQuestDB.Count > 0)
        {
            ShowAcceptPopup(allQuestDB[0].questID);
        }
    }
}

// 저장 데이터 클래스
[System.Serializable]
public class QuestSaveData
{
    public List<int> questIDs = new List<int>();
    public List<int> questStates = new List<int>();
    public List<int> questProgress = new List<int>();
}
using System;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance;

    [Header("Data")]
    public List<QuestData> allQuestDB; // 모든 퀘스트 SO 리스트
    public Dictionary<int, int> activeQuests = new Dictionary<int, int>(); // 현재 진행도 (ID, 현재수량)

    [Header("UI Reference")]
    public Transform questListParent; // 이미지의 'QuestList' 오브젝트 (Content)
    public GameObject questSlotPrefab; // 위에서 만든 QuestSlot 프리팹

    public event Action OnQuestUpdated;

    void Awake() => Instance = this;

    void Start()
    {
        // UI가 스크롤을 벗어나지 않게 하려면 
        // QuestList(Content)에 'Vertical Layout Group'과 'Content Size Fitter'가 있어야 합니다.
        OnQuestUpdated += RefreshQuestList;

        // 테스트용: 게임 시작 시 DB의 첫 번째 퀘스트 수락
        if (allQuestDB.Count > 0) AcceptQuest(allQuestDB[0].questID);
    }

    public void AcceptQuest(int id)
    {
        if (!activeQuests.ContainsKey(id))
        {
            activeQuests.Add(id, 0);
            OnQuestUpdated?.Invoke();
        }
    }

    // UI 전체 새로고침
    public void RefreshQuestList()
    {
        // 1. 기존 리스트 삭제
        foreach (Transform child in questListParent)
            Destroy(child.gameObject);

        // 2. 현재 진행 중인 퀘스트만큼 슬롯 생성
        foreach (var quest in activeQuests)
        {
            QuestData data = allQuestDB.Find(q => q.questID == quest.Key);
            if (data == null) continue;

            GameObject slotGo = Instantiate(questSlotPrefab, questListParent);
            slotGo.GetComponent<QuestSlot>().UpdateUI(data.questName, quest.Value, data.targetCount);
        }
    }
}
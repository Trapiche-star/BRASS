using System;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance;

    [Header("Data")]
    public List<QuestData> allQuestDB;
    public Dictionary<int, int> activeQuests = new Dictionary<int, int>();

    [Header("UI Reference")]
    public Transform questListParent;
    public GameObject questSlotPrefab;

    public event Action OnQuestUpdated;

    void Awake() => Instance = this;

    void Start()
    {
        OnQuestUpdated += RefreshQuestList;

        // [수정됨] 테스트를 위해 DB에 있는 '모든' 퀘스트를 수락 상태로 만듭니다.
        // 나중에는 이 반복문을 지우고 NPC 대화 시 AcceptQuest를 호출하면 됩니다.
        foreach (var quest in allQuestDB)
        {
            AcceptQuest(quest.questID);
        }
    }

    public void AcceptQuest(int id)
    {
        if (!activeQuests.ContainsKey(id))
        {
            activeQuests.Add(id, 0);
            OnQuestUpdated?.Invoke();
        }
    }

    public void RefreshQuestList()
    {
        foreach (Transform child in questListParent)
            Destroy(child.gameObject);

        foreach (var quest in activeQuests)
        {
            QuestData data = allQuestDB.Find(q => q.questID == quest.Key);

            // 데이터가 있는 경우만 생성
            if (data != null)
            {
                GameObject slotGo = Instantiate(questSlotPrefab, questListParent);
                // [수정됨] 낱개가 아니라 데이터(data)와 진행도(value)를 넘김
                slotGo.GetComponent<QuestSlot>().Setup(data, quest.Value);
            }
        }
    }
}
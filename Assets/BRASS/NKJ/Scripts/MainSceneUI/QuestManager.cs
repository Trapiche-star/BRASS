using System;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance;
    public List<QuestData> allQuests; // DB 역할
    public Dictionary<int, int> activeQuests = new Dictionary<int, int>(); // ID, 현재 진행도

    public event Action OnQuestUpdated; // UI 갱신용 이벤트

    void Awake() => Instance = this;

    public void AcceptQuest(int id)
    {
        if (!activeQuests.ContainsKey(id))
        {
            activeQuests.Add(id, 0);
            OnQuestUpdated?.Invoke();
        }
    }

    public void UpdateProgress(int id, int amount)
    {
        if (activeQuests.ContainsKey(id))
        {
            activeQuests[id] += amount;
            OnQuestUpdated?.Invoke();
        }
    }
}
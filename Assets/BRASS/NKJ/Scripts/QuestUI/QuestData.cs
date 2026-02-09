using UnityEngine;

[CreateAssetMenu(fileName = "New Quest", menuName = "Quest/QuestData")]
public class QuestData : ScriptableObject
{
    [Header("Basic Information")]
    public int questID;
    public string questName;
    [TextArea(3, 6)] public string description;
    public int questLevel = 1; // 권장 레벨

    [Header("Quest Type")]
    public QuestType questType;

    [Header("Target Information")]
    public string targetName; // 예: "고블린", "철광석", "마을 주민"
    public int targetCount = 1; // 목표 수량

    [Header("Rewards")]
    public int rewardGold;
    public int rewardExp;
    // 나중에 인벤토리 연동 시 추가
    // public List<ItemData> rewardItems;

    [Header("Quest Chain (Optional)")]
    public int prerequisiteQuestID = -1; // 선행 퀘스트 ID (-1이면 없음)
    public int nextQuestID = -1; // 다음 퀘스트 ID (-1이면 없음)

    [Header("NPC Information (Optional)")]
    public string npcName; // 퀘스트 주는 NPC
    public Sprite npcPortrait; // NPC 초상화

    // 유효성 검사
    void OnValidate()
    {
        if (targetCount < 1)
            targetCount = 1;

        if (rewardGold < 0)
            rewardGold = 0;

        if (rewardExp < 0)
            rewardExp = 0;

        if (questLevel < 1)
            questLevel = 1;
    }
}

// 퀘스트 타입 열거형
public enum QuestType
{
    Kill,        // 몬스터 처치
    Collect,     // 아이템 수집
    Investigate, // 조사/탐험
    Talk,        // 대화
    Escort,      // 호위
    Craft        // 제작
}

// 퀘스트 상태 열거형
public enum QuestState
{
    NotAccepted,  // 미수락
    InProgress,   // 진행중
    CanComplete,  // 완료 가능 (목표 달성)
    Completed     // 완료됨
}
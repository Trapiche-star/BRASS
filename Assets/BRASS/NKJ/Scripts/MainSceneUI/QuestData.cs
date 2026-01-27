using UnityEngine;

[CreateAssetMenu(fileName = "New Quest", menuName = "Quest/QuestData")]
public class QuestData : ScriptableObject
{
    public int questID;
    public string questName;
    [TextArea] public string description;
    public int targetCount; // 목표 수량 (예: 몬스터 10마리)
    public int rewardGold;
}
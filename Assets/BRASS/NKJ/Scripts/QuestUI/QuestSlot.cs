using UnityEngine;
using TMPro;

public class QuestSlot : MonoBehaviour
{
    [Header("UI Components")]
    public TextMeshProUGUI titleText;       // 퀘스트 제목
    public TextMeshProUGUI descText;        // 설명 (Description)
    public TextMeshProUGUI progressText;    // 진행도 (0/10)
    public TextMeshProUGUI rewardText;      // 보상 (100 G)

    // 데이터를 통째로 받아서 UI에 뿌려주는 함수
    public void Setup(QuestData data, int currentProgress)
    {
        // 1. 제목
        titleText.text = data.questName;

        // 2. 설명 (한글 가능)
        descText.text = data.description;

        // 3. 진행도
        progressText.text = $"진행: {currentProgress} / {data.targetCount}";

        // 4. 보상 (골드)
        rewardText.text = $"보상: {data.rewardGold} G";
    }
}
using UnityEngine;
using TMPro;

public class QuestSlot : MonoBehaviour
{
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI progressText;

    // 데이터를 받아서 텍스트를 갈아끼우는 함수
    public void UpdateUI(string title, int current, int target)
    {
        titleText.text = title;
        progressText.text = $"{current} / {target}";
    }
}
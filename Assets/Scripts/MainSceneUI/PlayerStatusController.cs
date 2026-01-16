using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerStatusController : MonoBehaviour
{
    public Slider hpBar;
    public Slider mpBar;
    public TextMeshProUGUI hpText;
    public GameObject restIcon; // 휴식 보너스 아이콘

    // 이 함수를 플레이어 스크립트에서 호출
    public void UpdateHP(float current, float max)
    {
        hpBar.value = current / max;
        hpText.text = $"{current} / {max}";
    }

    // 특정 지역(여관 등) 진입 시 호출
    public void SetRestArea(bool isInside)
    {
        restIcon.SetActive(isInside);
    }
}
using UnityEngine;
using UnityEngine.UI;

public class TargetFrame : MonoBehaviour
{
    public Image hpBarFill;
    public Color enemyColor = Color.red;
    public Color npcColor = Color.green;

    public void SetTarget(string type, float hpPercent)
    {
        hpBarFill.color = (type == "Enemy") ? enemyColor : npcColor;
        hpBarFill.fillAmount = hpPercent;
    }
}
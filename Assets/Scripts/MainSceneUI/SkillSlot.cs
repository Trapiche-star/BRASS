using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class SkillSlot : MonoBehaviour
{
    public SkillData skillData;
    public Image iconImage;        // 스킬 아이콘 (하단 레이어)
    public Image cooldownImage;    // 쿨다운 그림자 (상단 레이어)
    public TextMeshProUGUI mpText;

    private bool isCooldown = false;

    void Start()
    {
        if (skillData != null) SetSkill(skillData);

        // 시작할 때 쿨다운 그림자는 안 보여야 함
        if (cooldownImage != null) cooldownImage.fillAmount = 0;
    }

    public void SetSkill(SkillData data)
    {
        skillData = data;
        iconImage.sprite = data.icon;
        iconImage.enabled = true; // 아이콘은 항상 활성화

        if (mpText != null) mpText.text = data.mpCost.ToString();
    }

    public void UseSkill()
    {
        if (skillData == null || isCooldown) return;

        Debug.Log($"{skillData.skillName} 사용!");
        StartCoroutine(CooldownRoutine());
    }

    private IEnumerator CooldownRoutine()
    {
        isCooldown = true;
        float timer = skillData.cooldown;

        while (timer > 0)
        {
            timer -= Time.deltaTime;

            // 그림자 이미지가 1(꽉 참)에서 0(사라짐)으로 변함
            if (cooldownImage != null)
            {
                cooldownImage.fillAmount = timer / skillData.cooldown;
            }
            yield return null;
        }

        isCooldown = false;
        if (cooldownImage != null) cooldownImage.fillAmount = 0;
    }
}
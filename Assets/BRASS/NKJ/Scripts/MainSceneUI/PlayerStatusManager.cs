using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class PlayerStatusManager : MonoBehaviour
{
    public static PlayerStatusManager Instance;

    [Header("Profile Info")]
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI nickNameText;

    [Header("Gauges (EXP -> HP -> MP 순서)")]
    public Image expFill;
    public Image hpFill;
    public Image mpFill;

    [Header("Weapon & Stats")]
    public Image weaponIcon;      // 게임기 모양 임시 이미지 위치
    public TextMeshProUGUI atkText;
    public TextMeshProUGUI defText;

    [Header("Tooltip System")]
    public GameObject tooltipPanel;
    public TextMeshProUGUI tooltipText;

    private void Awake()
    {
        Instance = this;
        if (tooltipPanel != null) tooltipPanel.SetActive(false);
    }

    // 1. 상태 업데이트 (게이지 및 스탯)
    public void UpdateAllStatus(float hp, float maxHp, float mp, float maxMp, float exp, float maxExp, int atk, int def)
    {
        if (hpFill) hpFill.fillAmount = hp / maxHp;
        if (mpFill) mpFill.fillAmount = mp / maxMp;
        if (expFill) expFill.fillAmount = exp / maxExp;

        if (atkText) atkText.text = $"ATK  {atk}";
        if (defText) defText.text = $"DEF  {def}";
    }

    // 2. 무기 아이콘 교체 (클릭 시 호출용)
    public void ChangeWeapon(Sprite newWeaponSprite)
    {
        if (weaponIcon != null)
        {
            weaponIcon.sprite = newWeaponSprite;
            Debug.Log("무기가 교체되었습니다!");
        }
    }

    // 3. 툴팁 로직
    public void ShowTooltip(string message)
    {
        if (tooltipPanel == null) return;
        tooltipPanel.SetActive(true);
        tooltipText.text = message;
    }

    public void HideTooltip()
    {
        if (tooltipPanel != null) tooltipPanel.SetActive(false);
    }

    private void Update()
    {
        // 툴팁이 마우스를 따라다니게 설정
        if (tooltipPanel != null && tooltipPanel.activeSelf)
        {
            tooltipPanel.transform.position = Input.mousePosition + new Vector3(15, 15, 0);
        }
    }
}
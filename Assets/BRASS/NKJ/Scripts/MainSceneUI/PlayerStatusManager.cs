using UnityEngine;
using UnityEngine.UI;
using TMPro;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class PlayerStatusManager : MonoBehaviour
{
    [System.Serializable]
    public class WeaponData
    {
        public string weaponName;
        public Sprite icon;
        public int atk;
        public int def;
    }

    [Header("Main UI Connection")]
    public Image currentWeaponIcon;     // Current_Weapon_Slot의 Image
    public TextMeshProUGUI atkValueText; // ATK_Value (자식 숫자 텍스트)
    public TextMeshProUGUI defValueText; // DEF_Value (자식 숫자 텍스트)

    [Header("Popup System")]
    public GameObject weaponPopup;       // Weapon_Select_Popup 오브젝트
    public WeaponData[] availableWeapons; // 여기서 무기 정보를 자유롭게 수정하세요

    [Header("Tooltip")]
    public GameObject tooltipPanel;
    public TextMeshProUGUI tooltipText;

    void Awake()
    {
        if (weaponPopup) weaponPopup.SetActive(false);
        if (tooltipPanel) tooltipPanel.SetActive(false);
    }

    void Update()
    {
        // 툴팁 마우스 추적 (새 인풋 시스템 대응)
        if (tooltipPanel != null && tooltipPanel.activeSelf)
        {
            Vector2 mousePos;
#if ENABLE_INPUT_SYSTEM
            mousePos = Mouse.current.position.ReadValue();
#else
            mousePos = Input.mousePosition;
#endif
            tooltipPanel.transform.position = mousePos + new Vector2(15, 15);
        }
    }

    public void OpenPopup() => weaponPopup.SetActive(true);
    public void ClosePopup() => weaponPopup.SetActive(false);

    public void SelectWeapon(int index)
    {
        if (index >= 0 && index < availableWeapons.Length)
        {
            WeaponData data = availableWeapons[index];
            currentWeaponIcon.sprite = data.icon;
            atkValueText.text = data.atk.ToString();
            defValueText.text = data.def.ToString();
            ClosePopup();
        }
    }

    public void ShowTooltip(int index)
    {
        if (index < availableWeapons.Length)
        {
            tooltipPanel.SetActive(true);
            tooltipText.text = availableWeapons[index].weaponName;
        }
    }
    public void HideTooltip() => tooltipPanel.SetActive(false);
}
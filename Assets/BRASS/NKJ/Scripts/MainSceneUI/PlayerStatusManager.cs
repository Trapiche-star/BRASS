using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

// 새 입력 시스템 에러 방지 (패키지가 설치되어 있을 경우)
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class PlayerStatusManager : MonoBehaviour
{
    public static PlayerStatusManager Instance;

    [System.Serializable]
    public class WeaponData
    {
        public string weaponName;
        public Sprite icon;
        public int atk;
        public int def;
    }



    [Header("Current Status (UI Objects)")]
    public Image currentWeaponIcon;
    public TextMeshProUGUI atkText;
    public TextMeshProUGUI defText;

    [Header("Weapon Selection Popup")]
    public GameObject weaponSelectPopup;
    public WeaponData[] availableWeapons; // 인스펙터에서 무기 정보를 입력하세요

    [Header("Tooltip")]
    public GameObject tooltipPanel;
    public TextMeshProUGUI tooltipText;

    private void Awake()
    {
        Instance = this;
        if (weaponSelectPopup) weaponSelectPopup.SetActive(false);
        if (tooltipPanel) tooltipPanel.SetActive(false);
    }

    private void Update()
    {




        // 2. 툴팁 마우스 추적 (새 입력 시스템 대응)
        if (tooltipPanel && tooltipPanel.activeSelf)
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

    // --- 팝업 제어 ---
    public void OpenWeaponMenu()
    {
        if (weaponSelectPopup) weaponSelectPopup.SetActive(true);
    }

    public void CloseWeaponMenu()
    {
        if (weaponSelectPopup) weaponSelectPopup.SetActive(false);
    }

    // --- 무기 교체 (이미지 + 스탯) ---
    public void SelectWeapon(int index)
    {
        if (index >= 0 && index < availableWeapons.Length)
        {
            WeaponData selected = availableWeapons[index];

            // 이미지 변경
            if (currentWeaponIcon) currentWeaponIcon.sprite = selected.icon;

            // 스탯 변경
            if (atkText) atkText.text = $"ATK {selected.atk}";
            if (defText) defText.text = $"DEF {selected.def}";

            Debug.Log($"{selected.weaponName} 장착 완료!");
            CloseWeaponMenu(); // 선택 후 팝업 닫기
        }
    }

    // --- 툴팁 제어 ---
    public void ShowTooltip(string message)
    {
        if (tooltipPanel && tooltipText)
        {
            tooltipPanel.SetActive(true);
            tooltipText.text = message;
        }
    }

    public void HideTooltip()
    {
        if (tooltipPanel) tooltipPanel.SetActive(false);
    }
}
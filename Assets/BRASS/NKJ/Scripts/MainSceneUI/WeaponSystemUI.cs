using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

[System.Serializable]
public class WeaponData
{
    public string weaponName;
    public Sprite weaponIcon;
    public int attackValue;
    public int defenseValue;
}

public class WeaponSystemUI : MonoBehaviour
{
    [Header("Main UI Display")]
    public Image currentWeaponIcon;
    public TMP_Text currentWeaponATK;
    public TMP_Text currentWeaponDEF;

    [Header("Popup Settings")]
    public GameObject weaponSelectPopup;
    public Transform slotContainer;
    public GameObject slotPrefab;
    public Sprite errorIcon;

    [Header("Weapon Database")]
    public List<WeaponData> availableWeapons = new List<WeaponData>();

    private void Start()
    {
        if (weaponSelectPopup != null) weaponSelectPopup.SetActive(false);

        // 슬롯 생성
        CreateWeaponSlots();

        // 현재 메인 UI에도 데이터가 있다면 초기값 세팅 (없으면 주석 처리 하세요)
        if (availableWeapons.Count > 0) UpdateMainDisplay(0);
    }

    // [중요] 메인 버튼의 OnClick에 이 함수를 연결하세요
    public void OpenWeaponPopup()
    {
        if (weaponSelectPopup != null) weaponSelectPopup.SetActive(true);
    }

    private void CreateWeaponSlots()
    {
        if (slotPrefab == null || slotContainer == null) return;

        foreach (Transform child in slotContainer) Destroy(child.gameObject);

        for (int i = 0; i < availableWeapons.Count; i++)
        {
            int index = i;
            GameObject slotObj = Instantiate(slotPrefab, slotContainer);

            // 1. 이미지 찾기 (보내주신 이미지의 'Icon' 오브젝트 자동 매칭)
            Image img = null;
            // Button 자식 안에 있는 Icon을 찾습니다.
            Transform buttonTr = slotObj.transform.Find("Button");
            if (buttonTr != null)
            {
                Transform iconTr = buttonTr.Find("Icon");
                if (iconTr != null) img = iconTr.GetComponent<Image>();
            }

            // 만약 위에서 못 찾으면 전체 자식 중 'Icon'이라는 이름의 이미지를 찾습니다.
            if (img == null)
            {
                foreach (Image childImg in slotObj.GetComponentsInChildren<Image>(true))
                {
                    if (childImg.name == "Icon") { img = childImg; break; }
                }
            }

            if (img != null) img.sprite = availableWeapons[i].weaponIcon ?? errorIcon;

            // 2. 능력치 텍스트 찾기 (Stat 자식 안에 있는 TMP_Text들을 찾습니다)
            TMP_Text[] allTexts = slotObj.GetComponentsInChildren<TMP_Text>(true);
            foreach (var t in allTexts)
            {
                // 'Stat' 자식 오브젝트 내부에 있거나 이름에 ATK, DEF가 포함된 것을 매칭
                if (t.name.ToUpper().Contains("ATK") || t.transform.parent.name == "Stat")
                {
                    // 순서나 이름으로 매칭 (사용자님이 설정한 데이터 값 입력)
                    if (t.name.Contains("ATK")) t.text = availableWeapons[i].attackValue.ToString();
                    if (t.name.Contains("DEF")) t.text = availableWeapons[i].defenseValue.ToString();
                }
            }

            // 3. 버튼 이벤트 연결 (구조상 존재하는 Button 오브젝트에 연결)
            Button btn = slotObj.GetComponentInChildren<Button>();
            if (btn != null)
            {
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(() => OnSelectWeapon(index));
            }
        }
    }

    private void OnSelectWeapon(int index)
    {
        // 팝업 안의 무기를 클릭했을 때만 실행되는 로직
        UpdateMainDisplay(index);

        // 클릭 완료 후 팝업 닫기
        if (weaponSelectPopup) weaponSelectPopup.SetActive(false);
    }

    private void UpdateMainDisplay(int index)
    {
        if (index < 0 || index >= availableWeapons.Count) return;

        WeaponData data = availableWeapons[index];
        if (currentWeaponIcon) currentWeaponIcon.sprite = data.weaponIcon ?? errorIcon;
        if (currentWeaponATK) currentWeaponATK.text = data.attackValue.ToString();
        if (currentWeaponDEF) currentWeaponDEF.text = data.defenseValue.ToString();
    }
}
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
        public string name;
        public Sprite icon;
        public int atk;
        public int def;
    }

    [Header("Data Source")]
    public WeaponData[] availableWeapons; // 여기서 무기를 추가하면 팝업에 자동 생성됨
    public GameObject iconPrefab;        // 팝업에 들어갈 버튼 프리팹 (아래 설명 참조)

    [Header("UI References")]
    public GameObject weaponSelectPopup;
    public Image currentWeaponIcon;
    public TextMeshProUGUI atkValueText;
    public TextMeshProUGUI defValueText;

    void Awake()
    {
        if (weaponSelectPopup) weaponSelectPopup.SetActive(false);
        CreatePopupIcons(); // 시작할 때 리스트 기반으로 아이콘 생성
    }

    // 1. 팝업창에 아이콘 버튼들을 리스트 개수만큼 생성
    void CreatePopupIcons()
    {
        // 기존에 혹시 있을지 모를 아이콘 삭제
        foreach (Transform child in weaponSelectPopup.transform)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < availableWeapons.Length; i++)
        {
            int index = i; // 클로저 문제 방지
            GameObject newIcon = Instantiate(iconPrefab, weaponSelectPopup.transform);

            // 프리팹의 Image 컴포넌트에 아이콘 적용
            newIcon.GetComponent<Image>().sprite = availableWeapons[i].icon;

            // 프리팹의 Button 컴포넌트에 클릭 이벤트 연결
            Button btn = newIcon.GetComponent<Button>();
            btn.onClick.AddListener(() => SelectWeapon(index));
        }
    }

    // 2. 메인 무기 클릭 시 호출
    public void OpenPopup() => weaponSelectPopup.SetActive(true);

    // 3. 팝업 밖으로 마우스 나갈 때 호출 (Event Trigger용)
    public void ClosePopup() => weaponSelectPopup.SetActive(false);

    // 4. 무기 선택 시 데이터 교체
    public void SelectWeapon(int index)
    {
        WeaponData data = availableWeapons[index];
        currentWeaponIcon.sprite = data.icon;
        atkValueText.text = data.atk.ToString();
        defValueText.text = data.def.ToString();

        ClosePopup();
        Debug.Log($"{data.name} 장착 완료!");
    }
}
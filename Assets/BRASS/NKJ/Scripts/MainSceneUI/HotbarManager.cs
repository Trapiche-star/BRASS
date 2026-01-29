using UnityEngine;
using UnityEngine.InputSystem;

public class HotbarManager : MonoBehaviour
{
    public SkillSlot[] slots;

    [Header("UI Settings")]
    public GameObject hotbarUI;

    // 내부적으로 사용할 CanvasGroup
    private CanvasGroup hotbarCanvasGroup;

    void Start()
    {
        // 시작할 때 CanvasGroup 컴포넌트를 가져옵니다.
        if (hotbarUI != null)
        {
            hotbarCanvasGroup = hotbarUI.GetComponent<CanvasGroup>();

            // 만약 CanvasGroup이 없다면 자동으로 추가해줍니다.
            if (hotbarCanvasGroup == null)
            {
                hotbarCanvasGroup = hotbarUI.AddComponent<CanvasGroup>();
            }
        }
    }

    void Update()
    {
        Keyboard kb = Keyboard.current;
        if (kb == null) return;

        // 1. H 키를 누르면 시각적 노출 여부만 토글
        if (kb.hKey.wasPressedThisFrame)
        {
            ToggleHotbar();
        }

        // 2. 스킬 사용 로직 (UI가 투명해도 오브젝트가 켜져있으므로 정상 작동)
        if (kb.wKey.wasPressedThisFrame) { LogKeyAndUse(kb.wKey.displayName, 0); }
        if (kb.aKey.wasPressedThisFrame) { LogKeyAndUse(kb.aKey.displayName, 1); }
        if (kb.sKey.wasPressedThisFrame) { LogKeyAndUse(kb.sKey.displayName, 2); }
        if (kb.dKey.wasPressedThisFrame) { LogKeyAndUse(kb.dKey.displayName, 3); }
        if (kb.rKey.wasPressedThisFrame) { LogKeyAndUse(kb.rKey.displayName, 4); }
        if (kb.eKey.wasPressedThisFrame) { LogKeyAndUse(kb.eKey.displayName, 5); }
        if (kb.gKey.wasPressedThisFrame) { LogKeyAndUse(kb.gKey.displayName, 6); }
        if (kb.iKey.wasPressedThisFrame) { LogKeyAndUse(kb.iKey.displayName, 7); }
        if (kb.spaceKey.wasPressedThisFrame) { LogKeyAndUse(kb.spaceKey.displayName, 8); }
    }

    void ToggleHotbar()
    {
        if (hotbarCanvasGroup != null)
        {
            // alpha가 0이면(숨김) 1로(보임), 1이면 0으로 변경
            bool isVisible = hotbarCanvasGroup.alpha > 0;

            hotbarCanvasGroup.alpha = isVisible ? 0 : 1;
            hotbarCanvasGroup.interactable = !isVisible; // 마우스 클릭 방지/허용
            hotbarCanvasGroup.blocksRaycasts = !isVisible; // 마우스 클릭 방지/허용

            Debug.Log($"핫바 가시성: {(!isVisible ? "보임" : "숨김")} (쿨타임은 계속 돌아가는 중)");
        }
    }

    void LogKeyAndUse(string keyName, int index)
    {
        Debug.Log($"{keyName} 키로 스킬 사용!");
        CheckAndUse(index);
    }

    void CheckAndUse(int index)
    {
        if (slots == null || slots.Length <= index) return;
        if (slots[index] == null) return;

        slots[index].UseSkill();
    }
}
/*
 * 단축키 수정
using UnityEngine;
using UnityEngine.InputSystem;

public class HotbarManager : MonoBehaviour
{
    // 하이라키의 Slot들을 드래그해서 넣는 곳
    public SkillSlot[] slots;

    void Update()
    {
        // Keyboard 클래스를 통해 현재 키보드 상태 확인
        Keyboard kb = Keyboard.current;
        if (kb == null) return;


        if (kb.wKey.wasPressedThisFrame) { LogKeyAndUse(kb.wKey.displayName, 0); }
        if (kb.aKey.wasPressedThisFrame) { LogKeyAndUse(kb.aKey.displayName, 1); }
        if (kb.sKey.wasPressedThisFrame) { LogKeyAndUse(kb.sKey.displayName, 2); }
        if (kb.dKey.wasPressedThisFrame) { LogKeyAndUse(kb.dKey.displayName, 3); }
        if (kb.rKey.wasPressedThisFrame) { LogKeyAndUse(kb.rKey.displayName, 4); }
        if (kb.eKey.wasPressedThisFrame) { LogKeyAndUse(kb.eKey.displayName, 5); }
        if (kb.gKey.wasPressedThisFrame) { LogKeyAndUse(kb.gKey.displayName, 6); }
        if (kb.iKey.wasPressedThisFrame) { LogKeyAndUse(kb.iKey.displayName, 7); }
        if (kb.spaceKey.wasPressedThisFrame) { LogKeyAndUse(kb.spaceKey.displayName, 8); }
    }

    // 로그 출력을 범용적으로 처리하기 위한 헬퍼 로직
    void LogKeyAndUse(string keyName, int index)
    {
        Debug.Log($"{keyName} 키가 눌렸습니다!");
        CheckAndUse(index);
    }

    void CheckAndUse(int index)
    {
        if (slots == null || slots.Length <= index)
        {
            Debug.LogError($"{index}번 슬롯 배열이 설정되지 않았습니다!");
            return;
        }

        if (slots[index] == null)
        {
            Debug.LogError($"{index}번 칸에 SkillSlot 오브젝트가 비어있습니다(None)!");
            return;
        }

        slots[index].UseSkill();
    }
}

*/

/*
 * 기존코드
 
using UnityEngine;
using UnityEngine.InputSystem;

public class HotbarManager : MonoBehaviour
{
    // 하이라키의 Slot들을 드래그해서 넣는 곳
    public SkillSlot[] slots;

    void Update()
    {
        // Keyboard 클래스를 통해 현재 키보드 상태 확인
        Keyboard kb = Keyboard.current;
        if (kb == null) return;

        // 1~0번 키 감지
        if (kb.digit1Key.wasPressedThisFrame)
        {
            Debug.Log("1번 키가 눌렸습니다!"); 
            CheckAndUse(0);
        }
        if (kb.digit2Key.wasPressedThisFrame) CheckAndUse(1);
        if (kb.digit3Key.wasPressedThisFrame) CheckAndUse(2);
        if (kb.digit4Key.wasPressedThisFrame) CheckAndUse(3);
        if (kb.digit5Key.wasPressedThisFrame) CheckAndUse(4);
        if (kb.digit6Key.wasPressedThisFrame) CheckAndUse(5);
        if (kb.digit7Key.wasPressedThisFrame) CheckAndUse(6);
        if (kb.digit8Key.wasPressedThisFrame) CheckAndUse(7);
        if (kb.digit9Key.wasPressedThisFrame) CheckAndUse(8);
        if (kb.digit0Key.wasPressedThisFrame) CheckAndUse(9);
    }

    void CheckAndUse(int index)
    {
        if (slots == null || slots.Length <= index)
        {
            Debug.LogError($"{index}번 슬롯 배열이 설정되지 않았습니다!");
            return;
        }
        if (slots[index] == null)
        {
            Debug.LogError($"{index}번 칸에 SkillSlot 오브젝트가 비어있습니다(None)!");
            return;
        }
        slots[index].UseSkill();
    }
}

*/

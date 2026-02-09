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

using UnityEngine;
using UnityEngine.InputSystem;
using Team1;

public class HotbarManager : MonoBehaviour
{
    public HotbarSlot[] slots; // ⭐ SkillSlot → HotbarSlot 변경

    [Header("UI Settings")]
    public GameObject hotbarUI;

    private CanvasGroup hotbarCanvasGroup;

    void Start()
    {
        if (hotbarUI != null)
        {
            hotbarCanvasGroup = hotbarUI.GetComponent<CanvasGroup>();
            if (hotbarCanvasGroup == null)
            {
                hotbarCanvasGroup = hotbarUI.AddComponent<CanvasGroup>();
            }
        }
    }

    void Update()
    {
        if (!InputFieldManager.CanReceiveHotkey())
            return;

        if (Team1.UIManager_SY.Instance != null &&
            Team1.UIManager_SY.Instance.IsAnyUIOpen)
        {
            return;
        }

        Keyboard kb = Keyboard.current;
        if (kb == null) return;

        if (kb.hKey.wasPressedThisFrame)
        {
            ToggleHotbar();
        }

        if (kb.altKey.isPressed)
        {
            if (kb.digit1Key.wasPressedThisFrame) { UseSlot(0); }
            if (kb.digit2Key.wasPressedThisFrame) { UseSlot(1); }
            if (kb.digit3Key.wasPressedThisFrame) { UseSlot(2); }
            if (kb.digit4Key.wasPressedThisFrame) { UseSlot(3); }
            if (kb.digit5Key.wasPressedThisFrame) { UseSlot(4); }
            if (kb.digit6Key.wasPressedThisFrame) { UseSlot(5); }
            if (kb.digit7Key.wasPressedThisFrame) { UseSlot(6); }
            if (kb.digit8Key.wasPressedThisFrame) { UseSlot(7); }
            if (kb.digit9Key.wasPressedThisFrame) { UseSlot(8); }
        }
    }

    void CheckAndUse(int index)
    {
        if (slots == null || slots.Length <= index) return;

        // ⭐ UseSkill()이 아니라 UseSlot()을 호출해야 아이템/스킬 둘 다 나갑니다.
        if (slots[index] != null)
        {
            slots[index].UseSlot();
        }
    }

    void ToggleHotbar()
    {
        if (hotbarCanvasGroup == null) return;

        bool isVisible = hotbarCanvasGroup.alpha > 0;

        hotbarCanvasGroup.alpha = isVisible ? 0 : 1;
        hotbarCanvasGroup.interactable = !isVisible;
        hotbarCanvasGroup.blocksRaycasts = !isVisible;
    }

    void UseSlot(int index)
    {
        if (slots == null || slots.Length <= index) return;
        if (slots[index] == null) return;

        slots[index].UseSlot(); // ⭐ UseSkill() → UseSlot() 변경
    }
}




*/
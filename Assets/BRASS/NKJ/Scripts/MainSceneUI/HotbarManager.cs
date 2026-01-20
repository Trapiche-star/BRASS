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
            Debug.Log("1번 키가 눌렸습니다!"); //Debug.Log("1번 키가 눌렸습니다!"); 이 부분 1키만이 아니라 다른 키 눌렀을때도 나오게 범용적으로 되게하기
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
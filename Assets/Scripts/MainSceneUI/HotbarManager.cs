using UnityEngine;

public class HotbarManager : MonoBehaviour
{
    // 하이라키의 Slot들을 드래그해서 넣는 곳
    public SkillSlot[] slots;

    void Update()
    {
        // 1번키 감지 테스트
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Debug.Log("1번 키가 눌렸습니다!");
            CheckAndUse(0);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2)) CheckAndUse(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) CheckAndUse(2);
        // ... (나머지 숫자들도 동일하게)
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



// void Update()
//     {
//         // 키보드 숫자 1~9, 0 감지
//         if (Input.GetKeyDown(KeyCode.Alpha1)) slots[0]?.UseSkill();
//         if (Input.GetKeyDown(KeyCode.Alpha2)) slots[1]?.UseSkill();
//         if (Input.GetKeyDown(KeyCode.Alpha3)) slots[2]?.UseSkill();
//         if (Input.GetKeyDown(KeyCode.Alpha4)) slots[3]?.UseSkill();
//         if (Input.GetKeyDown(KeyCode.Alpha5)) slots[4]?.UseSkill();
//         if (Input.GetKeyDown(KeyCode.Alpha6)) slots[5]?.UseSkill();
//         if (Input.GetKeyDown(KeyCode.Alpha7)) slots[6]?.UseSkill();
//         if (Input.GetKeyDown(KeyCode.Alpha8)) slots[7]?.UseSkill();
//         if (Input.GetKeyDown(KeyCode.Alpha9)) slots[8]?.UseSkill();
//         if (Input.GetKeyDown(KeyCode.Alpha0)) slots[9]?.UseSkill();
//     }

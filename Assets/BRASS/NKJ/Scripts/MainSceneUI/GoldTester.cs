using UnityEngine;
using UnityEngine.InputSystem;

public class GoldTester : MonoBehaviour
{
    private void Update()
    {
        // 숫자 키패드로 테스트 (기존 키와 겹치지 않음)
        if (Keyboard.current.numpad1Key.wasPressedThisFrame)
        {
            GoldManager.Instance.AddGold(100);
            Debug.Log("골드 +100");
        }

        if (Keyboard.current.numpad2Key.wasPressedThisFrame)
        {
            GoldManager.Instance.RemoveGold(50);
            Debug.Log("골드 -50");
        }

        if (Keyboard.current.numpad3Key.wasPressedThisFrame)
        {
            GoldManager.Instance.SetGold(999999);
            Debug.Log("골드 최대치 설정");
        }
    }
}
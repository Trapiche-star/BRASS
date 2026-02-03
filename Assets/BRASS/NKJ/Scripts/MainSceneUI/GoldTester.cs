using UnityEngine;
using UnityEngine.InputSystem;

public class GoldTester : MonoBehaviour
{
    private void Update()
    {
        // Keyboard.current가 null인 경우(입력 장치 미연결 등) 예외 방지
        if (Keyboard.current == null) return;

        // 숫자 키패드로 테스트 (기존 키와 겹치지 않음)

        // 1번: 골드 추가
        if (Keyboard.current.numpad1Key.wasPressedThisFrame)
        {
            if (GoldManager.Instance != null)
            {
                GoldManager.Instance.AddGold(100);
                Debug.Log($"<color=yellow>골드 +100 | 현재: {GoldManager.Instance.GetCurrentGold()}</color>");
            }
        }

        // 2번: 골드 차감
        if (Keyboard.current.numpad2Key.wasPressedThisFrame)
        {
            if (GoldManager.Instance != null)
            {
                bool success = GoldManager.Instance.RemoveGold(50);
                if (success)
                    Debug.Log($"<color=red>골드 -50 | 현재: {GoldManager.Instance.GetCurrentGold()}</color>");
                else
                    Debug.Log("<color=orange>골드가 부족하여 차감할 수 없습니다.</color>");
            }
        }

        // 3번: 골드 최대치 또는 특정 값 설정
        if (Keyboard.current.numpad3Key.wasPressedThisFrame)
        {
            if (GoldManager.Instance != null)
            {
                GoldManager.Instance.SetGold(999999);
                Debug.Log("<color=cyan>골드를 최대치(999,999)로 설정했습니다.</color>");
            }
        }

        // (추가 팁) 0번: 골드 초기화
        if (Keyboard.current.numpad0Key.wasPressedThisFrame)
        {
            if (GoldManager.Instance != null)
            {
                GoldManager.Instance.ResetGold();
                Debug.Log("<color=white>골드를 0으로 초기화했습니다.</color>");
            }
        }
    }
}
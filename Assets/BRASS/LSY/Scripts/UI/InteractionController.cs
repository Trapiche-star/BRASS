using UnityEngine;
using UnityEngine.InputSystem;

namespace Team1
{
    public class InteractionController : MonoBehaviour
    {
        // G키 관련 로직은 이제 BRASS.PlayerInteraction에서 담당하므로 
        // 여기서는 인벤토리(I키)와 UI 상태 제어만 남깁니다.

        private void Update()
        {
            if (Keyboard.current == null) return;

            // 1. 인벤토리 토글 (I 키)
            if (Keyboard.current.iKey.wasPressedThisFrame)
            {
                if (UIManager_SY.Instance != null)
                {
                    UIManager_SY.Instance.ToggleInventory();
                    Debug.Log("인벤토리 토글 실행");
                }
            }

            // 2. (선택 사항) ESC 키를 눌러도 모든 UI를 닫고 싶다면 추가하세요
            if (Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                if (UIManager_SY.Instance != null && UIManager_SY.Instance.IsAnyUIOpen)
                {
                    UIManager_SY.Instance.CloseAll();
                }
            }
        }
    }
}
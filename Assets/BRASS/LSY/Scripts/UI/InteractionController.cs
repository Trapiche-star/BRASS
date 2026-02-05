using UnityEngine;
using UnityEngine.InputSystem;
using BRASS;

namespace Team1
{
    public class InteractionController : MonoBehaviour
    {
        [SerializeField] private PlayerCasting casting;

        private void Update()
        {
            if (Keyboard.current == null) return;

            if (Keyboard.current.gKey.wasPressedThisFrame)
            {
                // 1. 이미 UI가 열려있다면? -> 묻지도 따지지도 않고 닫기!
                if (UIManager_SY.Instance != null && UIManager_SY.Instance.IsAnyUIOpen)
                {
                    UIManager_SY.Instance.CloseAll();
                    return; // ⭐️ 중요: 여기서 리턴해야 아래 '열기'가 안 씹힘
                }

                // 2. UI가 닫혀있을 때만 타겟 상호작용 시도
                if (casting != null && casting.HasTarget)
                {
                    Debug.Log($"상호작용 시도 타겟: {casting.CurrentTarget}");
                    casting.CurrentTarget?.Interact();
                }
                else
                {
                    Debug.Log("상호작용 타겟을 찾을 수 없습니다. (Casting 체크 필요)");
                }
            }

            if (Keyboard.current.iKey.wasPressedThisFrame)
            {
                UIManager_SY.Instance?.ToggleInventory();
            }
        }
    }
}
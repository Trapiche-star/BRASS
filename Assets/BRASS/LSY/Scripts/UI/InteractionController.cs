using UnityEngine;
using UnityEngine.InputSystem;
using BRASS;

namespace Team1
{
    public class InteractionController : MonoBehaviour
    {
        [SerializeField] private PlayerCasting casting;

        private void Awake()
        {
            if (casting == null) casting = GetComponent<PlayerCasting>();
        }

        private void Update()
        {
            if (Keyboard.current == null) return;

            // 1. G 키 처리
            if (Keyboard.current.gKey.wasPressedThisFrame)
            {
                // ⭐️ 핵심: 이미 UI가 열려있을 때만 '닫기'를 수행
                if (UIManager_SY.Instance != null && UIManager_SY.Instance.IsAnyUIOpen)
                {
                    UIManager_SY.Instance.CloseAll();
                    // 닫았으므로 여기서 리턴하여 아래의 '상호작용(열기)' 코드가 실행되지 않게 함
                    return;
                }

                // ⭐️ UI가 닫혀있을 때만 '상호작용'을 시도
                if (casting != null && casting.HasTarget)
                {
                    casting.CurrentTarget?.Interact();
                    // 여기서 return을 안 하더라도, 위에서 IsAnyUIOpen 체크를 이미 했으므로 안전함
                }
            }

            // 2. I 키 처리 (인벤토리는 별도의 키라 충돌이 적음)
            if (Keyboard.current.iKey.wasPressedThisFrame)
            {
                UIManager_SY.Instance?.ToggleInventory();
            }
        }
    }
}
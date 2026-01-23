using UnityEngine;
using UnityEngine.InputSystem;
using BRASS;

namespace Team1
{
    public class PlayerInteractionUI : MonoBehaviour
    {
        [SerializeField] private PlayerCasting casting;
        [SerializeField] private InteractionPromptUI promptUI;

        // 🔒 같은 프레임 중복 실행 차단
        private static int lastInteractFrame = -1;

        private void Update()
        {
            if (casting == null || promptUI == null)
                return;

            if (!casting.HasTarget)
            {
                promptUI.Hide();
                return;
            }

            var interactable = casting.CurrentTarget as OpenMyUIInteractable;
            bool isUIOpen = interactable != null && interactable.IsOpen;

            // UI 열려있으면 안내 숨김, 닫혀있으면 표시
            if (isUIOpen)
                promptUI.Hide();
            else
                promptUI.Show();

            // ✅ G 키 단발 입력
            if (Keyboard.current != null && Keyboard.current.gKey.wasPressedThisFrame)
            {
                // 🚫 같은 프레임이면 무시
                if (Time.frameCount == lastInteractFrame)
                {
                    return;
                }

                lastInteractFrame = Time.frameCount;

                casting.CurrentTarget?.Interact();
            }
        }
    }
}

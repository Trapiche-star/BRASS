using UnityEngine;
using BRASS;

namespace Team1
{
    public class PlayerInteractionUI : MonoBehaviour
    {
        [SerializeField] private PlayerCasting casting;
        [SerializeField] private InteractionPromptUI promptUI;

        private void Update()
        {
            if (casting == null || promptUI == null) return;

            // UI가 하나라도 열려 있으면 무조건 숨김
            if (UIManager_SY.Instance != null && UIManager_SY.Instance.IsAnyUIOpen)
            {
                promptUI.Hide();
                return;
            }

            // UI가 닫힌 상태에서만 감지 표시
            if (casting.HasTarget) promptUI.Show();
            else promptUI.Hide();
        }
    }
}
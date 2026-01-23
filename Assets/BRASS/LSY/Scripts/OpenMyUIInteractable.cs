using UnityEngine;
using BRASS;

namespace Team1
{
    public class OpenMyUIInteractable : MonoBehaviour, IInteractable
    {
        [SerializeField] private GameObject myUI;

        // 🔒 중복 호출 방지용
        private float lastInteractTime = -1f;
        private const float interactCooldown = 0.2f; // 0.2초 이내 중복 무시

        public void Interact()
        {
            // 🚫 너무 짧은 시간에 다시 호출되면 무시
            if (Time.time - lastInteractTime < interactCooldown)
            {
                return;
            }

            lastInteractTime = Time.time;

            if (myUI == null)
            {
                return;
            }

            // 실제 UI 활성 상태 기준으로 토글
            bool nextState = !myUI.activeSelf;
            myUI.SetActive(nextState);
        }

        // Player 쪽에서 상태 볼 때도 실제 UI 기준
        public bool IsOpen => myUI != null && myUI.activeSelf;
    }
}

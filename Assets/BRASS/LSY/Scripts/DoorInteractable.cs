using UnityEngine;
using BRASS;

namespace Team1
{
    public class DoorInteractable : MonoBehaviour, IInteractable
    {
        [SerializeField] private Animator animator;
        [SerializeField] private string triggerName = "Open";

        private bool isOpen;

        private void Awake()
        {
            if (animator == null)
                animator = GetComponent<Animator>();
        }

        public void Interact()
        {
            if (animator == null)
            {
                Debug.LogError("❌ DoorInteractable : Animator 없음");
                return;
            }

            isOpen = !isOpen;

            animator.SetTrigger(triggerName);

            Debug.Log($"🚪 문 인터랙션 실행 (Open = {isOpen})");
        }
    }
}

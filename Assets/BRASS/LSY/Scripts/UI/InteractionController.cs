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
            if (casting == null)
                casting = GetComponent<PlayerCasting>();
        }

        private void Update()
        {
            if (Keyboard.current == null)
                return;

            if (Keyboard.current.gKey.wasPressedThisFrame)
            {
                if (casting != null && casting.HasTarget)
                {
                    casting.CurrentTarget?.Interact();
                }
            }
        }
    }
}

using UnityEngine;
using UnityEngine.InputSystem;
using BRASS;

namespace Team1
{
    public class PlayerDoorInteraction : MonoBehaviour
    {
        [SerializeField] private PlayerCasting casting;

        private void Update()
        {
            if (casting == null)
                return;

            if (!casting.HasTarget)
                return;

            if (Keyboard.current != null && Keyboard.current.gKey.wasPressedThisFrame)
            {
                casting.CurrentTarget?.Interact();
            }
        }
    }
}

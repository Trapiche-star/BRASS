using UnityEngine;
using BRASS;

namespace Team1
{
    public class ShopInteractable : MonoBehaviour, IInteractable
    {
        [SerializeField] private InventoryToggleUI shopUI;

        public void Interact()
        {
            if (shopUI == null)
            {
                Debug.LogError("❌ ShopUI 연결 안됨");
                return;
            }

            shopUI.Toggle();
        }
    }
}

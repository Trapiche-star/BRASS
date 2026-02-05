using UnityEngine;
using BRASS;
using Team1;

public class MarketObject : MonoBehaviour, IInteractable
{
    public void Interact()
    {
        if (UIManager_SY.Instance != null)
            UIManager_SY.Instance.OpenShop();
    }
}
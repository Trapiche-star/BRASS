using UnityEngine;
using BRASS; // 인터페이스 네임스페이스
using Team1; // UIManager_SY 네임스페이스

public class MarketObject : MonoBehaviour, IInteractable
{
    public void Interact()
    {
        UIManager_SY.Instance.ToggleMarket();
    }
}
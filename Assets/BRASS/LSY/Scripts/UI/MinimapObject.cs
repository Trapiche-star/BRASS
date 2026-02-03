using UnityEngine;
using BRASS;
using Team1;

public class MinimapObject : MonoBehaviour, IInteractable
{
    public void Interact()
    {
        UIManager_SY.Instance.ToggleMinimap();
    }
}
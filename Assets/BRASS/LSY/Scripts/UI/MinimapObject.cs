using UnityEngine;
using BRASS;
using Team1;

public class MinimapObject : MonoBehaviour, IInteractable
{
    public void Interact()
    {
        if (UIManager_SY.Instance != null)
            UIManager_SY.Instance.OpenMinimap();
    }
}
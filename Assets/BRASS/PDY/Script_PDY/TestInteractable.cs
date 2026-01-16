using UnityEngine;

namespace BRASS
{
    /// 상호작용 테스트용 오브젝트 스크립트
    public class TestInteractable : MonoBehaviour, IInteractable
    {
        public void Interact()
        {
            Debug.Log("[INTERACT] 상호작용 성공");
        }
    }
}

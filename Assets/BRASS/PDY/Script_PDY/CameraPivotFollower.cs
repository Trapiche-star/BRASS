using UnityEngine;

namespace BRASS
{
    /// CameraPivot을 플레이어 위치에 고정시키는 클래스
    public class CameraPivotFollower : MonoBehaviour
    {
        [SerializeField] private Transform target; // 따라갈 대상(플레이어)
        [SerializeField] private Vector3 offset = new Vector3(0f, 1.5f, 0f); // 피벗 오프셋

        private void LateUpdate()
        {
            if (target == null) return; // 만약 [대상이 없으면] [처리하지 않는다]
            transform.position = target.position + offset; // 대상 위치를 따라간다
        }
    }
}

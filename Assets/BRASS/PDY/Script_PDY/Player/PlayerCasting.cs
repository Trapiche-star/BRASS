using UnityEngine;

namespace BRASS
{
    /// 마우스 포인터 기준 카메라 Raycast로 상호작용 대상을 감지하고 포커스 상태를 관리한다
    public class PlayerCasting : MonoBehaviour
    {
        #region Variables
        [Header("Reference")]
        [SerializeField] private Camera playerCamera;   // Ray를 쏘는 기준 카메라
        [SerializeField] private Transform player;       // 상호작용 거리 계산 기준(플레이어 위치)

        [Header("Cast Option")]
        [SerializeField] private float castDistance = 5f;       // 카메라 Ray 최대 길이
        [SerializeField] private float interactDistance = 1.2f; // 플레이어 기준 상호작용 허용 거리
        [SerializeField] private LayerMask targetLayer;          // 상호작용 가능한 오브젝트 레이어

        private PlayerInputHandler input;

        private IInteractable currentTarget;
        private IInteractable lastTarget;

        public bool HasTarget { get; private set; }
        public IInteractable CurrentTarget => currentTarget;
        #endregion

        #region Unity Event Method
        private void Awake()
        {
            input = GetComponent<PlayerInputHandler>();

            if (playerCamera == null)
                playerCamera = Camera.main;

            if (player == null)
                player = transform;
        }

        private void Update()
        {
            Cast();
        }
        #endregion

        #region Custom Method
        private void Cast()
        {
            HasTarget = false;
            currentTarget = null;

            if (playerCamera == null)
                return;

            Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

            if (Physics.Raycast(ray, out RaycastHit hit, castDistance, targetLayer))
            {
                Collider col = hit.collider;
                Vector3 closestPoint = col.ClosestPoint(player.position);
                float distanceFromPlayer = Vector3.Distance(player.position, closestPoint);

                if (distanceFromPlayer > interactDistance)
                    return;

                // ⭐ 핵심: 부모까지 올라가서 Interactable 찾기
                IInteractable target = hit.collider.GetComponentInParent<IInteractable>();

                if (target != null)
                {
                    HasTarget = true;
                    currentTarget = target;
                }
            }
        }
        #endregion
    }
}

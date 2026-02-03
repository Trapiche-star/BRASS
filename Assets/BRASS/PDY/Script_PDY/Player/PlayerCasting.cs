using UnityEngine;
using Team1;

namespace BRASS
{
    public class PlayerCasting : MonoBehaviour
    {
        [SerializeField] private Camera playerCamera;
        [SerializeField] private Transform player;
        [SerializeField] private float castDistance = 5f;
        [SerializeField] private float interactDistance = 5f;
        [SerializeField] private LayerMask targetLayer;

        // ✅ 다른 스크립트에서 읽을 수 있도록 public 선언
        public bool HasTarget { get; private set; }
        public IInteractable CurrentTarget { get; private set; }

        private void Awake()
        {
            if (playerCamera == null) playerCamera = Camera.main;
        }

        public void Cast() // Update에서 호출하거나 외부에서 호출 가능
        {
            HasTarget = false;
            CurrentTarget = null;

            Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
            if (Physics.Raycast(ray, out RaycastHit hit, castDistance, targetLayer))
            {
                float dist = Vector3.Distance(player.position, hit.collider.ClosestPoint(player.position));
                if (dist <= interactDistance)
                {
                    IInteractable target = hit.collider.GetComponentInParent<IInteractable>();
                    if (target != null)
                    {
                        HasTarget = true;
                        CurrentTarget = target;
                    }
                }
            }
        }

        private void Update()
        {
            // UI가 열려있으면 감지 로직 자체를 중단
            if (UIManager_SY.Instance != null && UIManager_SY.Instance.IsAnyUIOpen)
            {
                HasTarget = false;
                CurrentTarget = null;
                return;
            }

            Cast();
        }
    }
}
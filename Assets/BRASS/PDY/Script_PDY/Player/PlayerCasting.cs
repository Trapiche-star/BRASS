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

        private PlayerInputHandler input; // 마우스 위치를 받기 위한 입력 처리 컴포넌트

        private IInteractable currentTarget; // 이번 프레임에 감지된 상호작용 대상
        private IInteractable lastTarget;    // 이전 프레임에 포커스 중이던 대상

        public bool HasTarget { get; private set; }          // 현재 유효한 상호작용 대상 존재 여부
        public IInteractable CurrentTarget => currentTarget; // 외부(PlayerInteraction)에서 참조할 현재 대상
        #endregion

        #region Unity Event Method
        private void Awake()
        {
            // 플레이어 입력 핸들러 컴포넌트 참조
            input = GetComponent<PlayerInputHandler>();

            // 카메라가 지정되지 않았으면 메인 카메라 사용
            if (playerCamera == null)
                playerCamera = Camera.main;

            // 플레이어 트랜스폼이 없으면 자기 자신을 기준으로 사용
            if (player == null)
                player = transform;
        }

        private void Update()
        {
            // 매 프레임 마우스 기준 캐스팅 수행
            Cast();
        }
        #endregion

        #region Custom Method
        private void Cast()
        {        
            // 매 프레임 상태 초기화
            HasTarget = false;
            currentTarget = null;

            if (input == null || playerCamera == null) return;

            // 마우스 포인터 기준 Ray 생성
            Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

            if (Physics.Raycast(ray, out RaycastHit hit, castDistance, targetLayer))
            {
                // Ray에 맞은 오브젝트의 콜라이더 참조를 가져온다
                Collider col = hit.collider;
                Vector3 closestPoint = col.ClosestPoint(player.position);   // 플레이어 위치에서 해당 콜라이더까지의 가장 가까운 지점을 계산한다

                // 플레이어 기준 거리 계산
                float distanceFromPlayer = Vector3.Distance(player.position, closestPoint);

                // 상호작용 가능 거리 내인지 확인
                if (distanceFromPlayer > interactDistance)
                    return;

                // 상호작용 대상인지 확인
                if (hit.collider.TryGetComponent<IInteractable>(out var target))
                {
                    HasTarget = true;        // “반응 가능 상태”
                    currentTarget = target; // G 키 입력 시 실행 대상
                }
            }
        }
        #endregion
    }
}

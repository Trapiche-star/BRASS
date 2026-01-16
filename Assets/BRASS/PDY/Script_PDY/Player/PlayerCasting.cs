using UnityEngine;

namespace BRASS
{
    /// 마우스 커서 기준 Raycast와 LayerMask 필터링으로 현재 상호작용 대상을 감지한다
    public class PlayerCasting : MonoBehaviour
    {
        #region Variables
        [SerializeField] private Camera playerCamera;     // 레이 발사용 기준 카메라
        [SerializeField] private float castDistance = 3f; // 감지 최대 거리
        [SerializeField] private LayerMask targetLayer;   // 감지 대상 레이어

        private PlayerInputHandler input; // 플레이어 입력 수신 담당
        #endregion

        #region Property
        public bool HasTarget { get; private set; }               // 현재 타겟 존재 여부
        public RaycastHit CurrentHit { get; private set; }       // 마지막 히트 정보
        public IInteractable CurrentTarget { get; private set; } // 현재 상호작용 대상
        #endregion

        #region Unity Event Method
        private void Awake()
        {
            input = GetComponent<PlayerInputHandler>(); // 만약 [입력 컴포넌트가 있으면] [참조를 저장한다]
            if (playerCamera == null) playerCamera = Camera.main; // 만약 [카메라 참조가 없으면] [메인 카메라를 사용한다]
        }

        private void Update()
        {
            Cast(); // 매 프레임 마우스 기준으로 대상 감지를 갱신한다
        }
        #endregion

        #region Custom Method
        // 마우스 커서 위치 기준 Raycast로 레이어 필터링하여 대상 정보를 갱신한다
        private void Cast()
        {
            HasTarget = false;      // 만약 [이전 프레임 타겟이 있더라도] [기본값으로 초기화한다]
            CurrentTarget = null;  // 만약 [현재 타겟이 없다면] [참조를 비운다]

            if (input == null || playerCamera == null) return; // 만약 [필수 참조가 없으면] [이 메서드에서는 더 이상 처리하지 않는다]

            Ray ray = playerCamera.ScreenPointToRay(input.MousePosition); // 마우스 화면 좌표를 월드 레이로 변환한다

            if (Physics.Raycast(ray, out RaycastHit hit, castDistance, targetLayer)) // 만약 [지정 레이어에 레이가 닿으면] [타겟을 판별한다]
            {
                CurrentHit = hit; // 만약 [히트가 발생했으면] [히트 정보를 저장한다]

                if (hit.collider.TryGetComponent<IInteractable>(out var interactable)) // 만약 [상호작용 인터페이스를 구현했다면] [타겟으로 등록한다]
                {
                    HasTarget = true;     // 만약 [유효한 타겟이면] [타겟 존재 상태를 활성화한다]
                    CurrentTarget = interactable; // 만약 [상호작용 가능하면] [현재 타겟으로 저장한다]
                }
            }
        }
        #endregion
    }
}

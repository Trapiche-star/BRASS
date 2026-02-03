using System.Linq;
using UnityEngine;

namespace BRASS
{
    /// 주변의 적을 탐색하여 타겟으로 지정하거나 마우스 클릭으로 대상을 선택하는 클래스
    public class PlayerTargeting : MonoBehaviour
    {
        #region Variables
        [SerializeField] private PlayerInputHandler input; // 입력 이벤트 수신
        [SerializeField] private PlayerState state;         // 타겟 상태 데이터
        [SerializeField] private LayerMask enemyLayer;      // 적 레이어
        [SerializeField] private float targetRange = 15f;   // TAB 타겟 탐색 반경

        private bool clickInputConsumed; // 클릭 타겟 중복 방지
        #endregion

        #region Unity Event Method
        private void Awake()
        {
            if (input == null)
                input = GetComponent<PlayerInputHandler>();

            if (state == null)
                state = GetComponent<PlayerState>();
        }

        private void Update()
        {
            HandleTargetInput();
        }
        #endregion

        #region Custom Method
        // 타겟팅 입력 처리 (ESC / TAB / 클릭)
        private void HandleTargetInput()
        {
            if (input == null || state == null)
                return;

            // ===== ESC : 타겟 해제 (최우선) =====
            if (input.EscDown)
            {
                ClearTarget();
                return; // ESC 프레임에서는 다른 타겟 입력 무시
            }

            // ===== TAB : 다음 타겟 순환 =====
            if (input.TabDown)
            {
                SelectNextTarget();
            }

            // ===== 클릭 : 직접 타겟 지정 =====
            if (input.ClickMovePressed)
            {
                if (!clickInputConsumed)
                {
                    clickInputConsumed = true;
                    TrySelectTargetByClick();
                }
            }
            else
            {
                clickInputConsumed = false;
            }
        }

        // 현재 타겟 해제
        private void ClearTarget()
        {
            state.CurrentTarget = null;
            state.IsEngagedWithTarget = false;

            Debug.Log("[Targeting] 타겟 해제 완료");
        }

        // TAB으로 주변 적 순환 선택
        private void SelectNextTarget()
        {
            var enemies =
                Physics.OverlapSphere(transform.position, targetRange, enemyLayer)
                       .OrderBy(e => Vector3.Distance(transform.position, e.transform.position))
                       .ToList();

            if (enemies.Count == 0)
            {
                ClearTarget();
                return;
            }

            int targetIndex = 0;

            if (state.CurrentTarget != null)
            {
                int currentIndex =
                    enemies.FindIndex(e => e.transform.root == state.CurrentTarget);

                if (currentIndex != -1)
                    targetIndex = (currentIndex + 1) % enemies.Count;
            }

            state.CurrentTarget = enemies[targetIndex].transform.root;
            state.IsEngagedWithTarget = true;

            Debug.Log($"[Targeting] TAB 타겟팅: {state.CurrentTarget.name}");
        }

        // 클릭으로 직접 타겟 선택
        private void TrySelectTargetByClick()
        {
            Camera cam = Camera.main;
            if (cam == null)
                return;

            Ray ray = cam.ScreenPointToRay(input.MousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit, 100f, enemyLayer))
            {
                state.CurrentTarget = hit.collider.transform.root;
                state.IsEngagedWithTarget = true;

                Debug.Log($"[Targeting] 클릭 타겟팅: {state.CurrentTarget.name}");
            }
        }
        #endregion
    }
}

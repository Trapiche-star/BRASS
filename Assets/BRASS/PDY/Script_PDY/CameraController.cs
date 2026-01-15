using UnityEngine;
using Unity.Cinemachine;

namespace BRASS
{
    /// 마우스 입력으로 카메라 회전과 줌을 직접 제어
    public class CameraController : MonoBehaviour
    {
        #region Variables

        [SerializeField] private CinemachineCamera cmCamera; // 제어할 Cinemachine 카메라
        [SerializeField] private float yawSpeed = 120f; // 좌우 회전 속도
        [SerializeField] private float pitchSpeed = 90f; // 상하 회전 속도

        [SerializeField] private float minZoom = 0f; // 줌 최소값
        [SerializeField] private float maxZoom = 1f; // 줌 최대값
        [SerializeField] private float zoomSpeed = 3f; // 휠 줌 감도
        [SerializeField] private float zoomDamping = 10f; // 줌 보간 속도

        private PlayerInputHandler input; // 입력 처리 담당
        private CinemachineOrbitalFollow orbital; // 궤도/반경 제어 담당

        #endregion

        #region Unity Event Method

        private void Awake()
        {
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null) input = player.GetComponent<PlayerInputHandler>();
            // 만약 [플레이어가 존재하면] [입력 컴포넌트를 참조한다]

            if (cmCamera == null) cmCamera = GetComponent<CinemachineCamera>();
            // 만약 [카메라 참조가 없으면] [자기 자신에서 찾는다]

            if (cmCamera == null) return;
            // 만약 [카메라가 없으면] [이후 로직을 중단한다]

            orbital = cmCamera.GetComponent<CinemachineOrbitalFollow>();
            if (orbital == null) return;
            // 만약 [OrbitalFollow가 없으면] [이후 로직을 중단한다]
        }

        private void Update()
        {
            if (input == null || orbital == null) return;
            // 만약 [필수 참조가 없으면] [이 프레임 처리를 중단한다]

            HandleRotation();
            HandleZoom();
        }

        #endregion

        #region Custom Method

        // 우클릭 중일 때만 카메라 회전 처리
        private void HandleRotation()
        {
            // 1. 우클릭 상태가 아니면 커서 풀고 즉시 종료
            if (!input.RotatePressed)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                return;
            }

            // 2. 우클릭 중이면 커서 고정 및 숨김
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            Vector2 look = input.LookInput;
            if (look == Vector2.zero) return;

            // 3. 좌우 회전 (Horizontal)
            orbital.HorizontalAxis.Value += look.x * yawSpeed * Time.deltaTime;

            // 4. 상하 회전 (Vertical) - 0과 1 사이의 미세한 값으로 계산
            // pitchSpeed(90) 기준, 0.01을 곱해 0~1 범위에 맞게 감도를 대폭 낮춤
            float pitchDelta = -look.y * (pitchSpeed * 0.005f) * Time.deltaTime;
            float v = orbital.VerticalAxis.Value + pitchDelta;

            // 인스펙터 Range가 0~1이므로 반드시 0과 1 사이로 가둡니다.
            orbital.VerticalAxis.Value = Mathf.Clamp01(v);

            // 로그로 0.0 ~ 1.0 사이에서 소수점이 변하는지 확인하세요.
            // Debug.Log($"[ROT] Vertical: {orbital.VerticalAxis.Value}");
        }

        // 마우스 휠로만 줌을 처리
        private void HandleZoom()
        {
            // 우클릭 중이면 줌 관련 로직은 아예 읽지도 않고 통과
            if (input.RotatePressed)
            {
                input.ClearZoom(); // 우클릭 중 들어온 줌 입력은 무시하고 비움
                return;
            }

            float scroll = input.ZoomInput;
            if (Mathf.Abs(scroll) < 0.01f) return;

            float target = orbital.RadialAxis.Value + scroll * zoomSpeed;
            target = Mathf.Clamp(target, minZoom, maxZoom);

            orbital.RadialAxis.Value = Mathf.Lerp(
                orbital.RadialAxis.Value,
                target,
                zoomDamping * Time.deltaTime
            );

            input.ClearZoom();
        }

        #endregion
    }
}

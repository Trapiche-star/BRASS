using UnityEngine;
using Unity.Cinemachine;

namespace BRASS
{
    /// 우클릭 시 시점 회전 및 커서 잠금을 제어하는 카메라 컨트롤 클래스
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private CinemachineInputAxisController axisController; // 시점 회전 입력 제어 컴포넌트
        private PlayerInputHandler input; // 입력 핸들러 참조 변수

        private void Awake()
        {
            GameObject player = GameObject.FindWithTag("Player"); // 플레이어 태그로 오브젝트 탐색
            if (player != null) input = player.GetComponent<PlayerInputHandler>();
        }

        private void Update()
        {
            if (input == null || axisController == null) return; // 만약 [참조가 없으면] [로직 중단]

            HandleRotation(); // 시점 회전 실행
        }

        // 우클릭 상태에 따라 입력 활성화 및 커서 상태 제어
        private void HandleRotation()
        {
            // [중요] 시네머신 입력 컴포넌트를 우클릭 시에만 활성화
            axisController.enabled = input.RotatePressed;

            if (input.RotatePressed) // 만약 [우클릭이 눌린 상태면]
            {
                Cursor.lockState = CursorLockMode.Locked; // 커서 중앙 고정
                Cursor.visible = false; // 커서 숨김
            }
            else // 만약 [우클릭을 떼면]
            {
                Cursor.lockState = CursorLockMode.None; // 커서 고정 해제
                Cursor.visible = true; // 커서 표시
            }
        }
    }
}
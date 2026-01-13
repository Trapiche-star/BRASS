using UnityEngine;
using UnityEngine.InputSystem;

namespace BRASS
{
    /// 플레이어의 실제 동작(이동, 회전, 자동 이동)을 처리하는 컨트롤러
    public class PlayerController : MonoBehaviour
    {
        #region Variables

        [SerializeField] private float moveSpeed = 5f; // 수동/자동 이동 속도
        [SerializeField] private float rotationSpeed = 10f; // 회전 보간 속도
        [SerializeField] private LayerMask groundLayer; // 자동 이동 Raycast 대상 레이어
        [SerializeField] private float clickMoveStopDistance = 0.1f; // 목표 지점 도착 판정 거리

        private CharacterController controller; // 캐릭터 이동을 담당
        private PlayerInputHandler input; // 입력 값을 제공

        private Vector3 moveDirection; // 현재 이동 방향
        private Vector3 targetPosition; // 자동 이동 목표 지점
        private bool isAutoMoving; // 자동 이동 상태 여부

        #endregion

        #region Unity Event Method

        private void Awake()
        {
            controller = GetComponent<CharacterController>(); // 같은 오브젝트의 CharacterController 참조
            input = GetComponent<PlayerInputHandler>(); // 같은 오브젝트의 InputHandler 참조
        }

        private void Update()
        {
            HandleMovement(); // 수동 이동/자동 이동 전환 및 이동 처리
            HandleRotation(); // 회전 처리
        }

        #endregion

        #region Custom Method

        // 입력 상태에 따라 수동 이동 또는 자동 이동을 처리한다
        private void HandleMovement()
        {
            if (input == null || controller == null) return; // 필수 컴포넌트가 없으면 동작하지 않는다

            Vector2 moveInput = input.MoveInput;
            bool hasManualInput = moveInput.sqrMagnitude > 0.01f; // WASD 입력이 존재하는지 판단

            if (hasManualInput)
            {
                isAutoMoving = false; // 수동 입력이 들어오면 자동 이동을 즉시 해제한다
                ManualMove(moveInput);
                return; // 수동 이동을 우선 적용하고 자동 이동 로직은 실행하지 않는다
            }

            if (input.ClickMovePressed)
            {
                SetClickMoveTarget(); // 좌클릭 시 자동 이동 목표 지점을 설정한다
            }

            if (isAutoMoving)
            {
                AutoMove(); // 자동 이동 상태일 때 목표 지점으로 이동한다
            }
        }

        // WASD 입력 기반으로 이동한다
        private void ManualMove(Vector2 moveInput)
        {
            Vector3 inputDir = new Vector3(moveInput.x, 0f, moveInput.y).normalized; // 입력값을 월드 이동 벡터로 변환
            moveDirection = inputDir * moveSpeed; // 이동 속도를 적용한다

            controller.Move(moveDirection * Time.deltaTime); // CharacterController를 통해 이동한다
        }

        // 좌클릭 위치를 Raycast로 계산하여 자동 이동 목표 지점을 설정한다
        private void SetClickMoveTarget()
        {
            if (Camera.main == null) return; // 메인 카메라가 없으면 Raycast를 수행하지 않는다

            Vector2 mousePos = Mouse.current.position.ReadValue(); // New Input System 기준 마우스 좌표 읽기
            Ray ray = Camera.main.ScreenPointToRay(mousePos); // 화면 좌표를 월드 Ray로 변환

            if (Physics.Raycast(ray, out RaycastHit hit, 100f, groundLayer) == false) return; // 지면을 맞추지 못하면 목표를 설정하지 않는다

            targetPosition = hit.point; // 맞은 지점을 자동 이동 목표로 설정한다
            isAutoMoving = true; // 자동 이동 상태로 전환한다
        }


        // 목표 지점까지 자동으로 이동한다
        private void AutoMove()
        {
            Vector3 toTarget = targetPosition - transform.position; // 현재 위치에서 목표까지의 방향 벡터 계산
            toTarget.y = 0f; // 수직 이동을 제거한다

            if (toTarget.magnitude <= clickMoveStopDistance)
            {
                isAutoMoving = false; // 목표 지점에 도달하면 자동 이동을 종료한다
                return; // 더 이상 이동하지 않는다
            }

            moveDirection = toTarget.normalized * moveSpeed; // 목표 방향으로 이동 벡터를 설정한다
            controller.Move(moveDirection * Time.deltaTime); // CharacterController를 통해 이동한다
        }

        // 이동 방향 또는 마우스 입력에 따라 회전한다
        private void HandleRotation()
        {
            if (input == null) return; // 입력 정보가 없으면 회전하지 않는다

            if (input.RotatePressed)
            {
                RotateByMouse(input.LookInput);
                return; // 우클릭 회전이 활성화되면 이동 방향 회전은 적용하지 않는다
            }

            if (moveDirection.sqrMagnitude > 0.01f)
            {
                RotateToMoveDirection(moveDirection);
            }
        }

        // 우클릭 상태에서 마우스 이동량으로 회전한다
        private void RotateByMouse(Vector2 lookInput)
        {
            float yaw = lookInput.x * rotationSpeed; // 마우스 X 이동량을 회전 값으로 변환
            transform.Rotate(0f, yaw * Time.deltaTime, 0f); // Y축 기준으로 회전한다
        }

        // 이동 방향을 바라보도록 회전한다
        private void RotateToMoveDirection(Vector3 direction)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction); // 이동 방향을 향하는 목표 회전 계산
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime); // 부드럽게 회전한다
        }

        #endregion
    }
}

using UnityEngine;

namespace BRASS
{
    /// 플레이어의 수동/자동 이동 및 캐릭터 회전 제어 클래스
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private float moveSpeed = 5f; // 지상 이동 속도
        [SerializeField] private float gravity = -9.81f; // 중력 가속도 수치
        [SerializeField] private Transform cameraPivot; // 이동 방향 기준이 될 카메라 피벗
        [SerializeField] private float rotationSpeed = 15f; // 캐릭터 몸체 회전 감도

        private PlayerInputHandler input; // 입력 정보 참조 변수
        private CharacterController controller; // 이동 컴포넌트 참조 변수
        private Vector3 velocity; // 수직 속도 누적값
        private Vector3 destination; // 좌클릭 이동 목적지 좌표
        private bool isAutoMoving = false; // 자동 이동 활성화 여부

        private void Awake()
        {
            input = GetComponent<PlayerInputHandler>();
            controller = GetComponent<CharacterController>();
        }

        private void Update()
        {
            HandleClickToMove(); // 클릭 이동 감지
            HandleMovement(); // 이동 및 회전 처리
        }

        // 클릭 위치로 자동 이동 목적지 설정
        private void HandleClickToMove()
        {
            if (input.ClickMovePressed) // 만약 [좌클릭이 눌리면]
            {
                Ray ray = Camera.main.ScreenPointToRay(input.MousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit)) // 만약 [지면에 레이가 닿으면]
                {
                    destination = hit.point;
                    isAutoMoving = true;
                }
            }

            if (input.MoveInput.sqrMagnitude > 0.01f) isAutoMoving = false; // 만약 [수동 입력 감지 시] [자동 이동 즉시 해제]
        }

        // 이동 로직 보정
        private void HandleMovement()
        {
            if (input == null || cameraPivot == null) return; // 만약 [참조 없으면] [중단]

            Vector3 moveDir = Vector3.zero;

            if (isAutoMoving) // 만약 [자동 이동 중이면]
            {
                Vector3 dir = (destination - transform.position);
                dir.y = 0;
                if (dir.magnitude > 0.1f) moveDir = dir.normalized;
                else isAutoMoving = false;
            }
            else // 만약 [수동 이동 중이면]
            {
                Vector2 m = input.MoveInput;

                // [핵심 보정] MainCamera의 방향을 직접 참조하여 카메라 시점 기준 이동 구현
                Transform camTransform = Camera.main.transform;
                Vector3 forward = camTransform.forward;
                Vector3 right = camTransform.right;

                forward.y = 0f; // 수직 성분 제거
                right.y = 0f;
                forward.Normalize();
                right.Normalize();

                moveDir = (forward * m.y + right * m.x).normalized; // 최종 이동 방향 계산
            }

            ApplyGravity();

            if (moveDir != Vector3.zero) // 만약 [이동 방향이 있으면]
            {
                controller.Move((moveDir * moveSpeed + velocity) * Time.deltaTime); // 이동 적용

                // 캐릭터가 이동 방향을 바라보게 회전
                Quaternion targetRot = Quaternion.LookRotation(moveDir);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
            }
            else controller.Move(velocity * Time.deltaTime); // 만약 [정지 시] [중력만 적용]
        }

        private void ApplyGravity()
        {
            if (controller.isGrounded && velocity.y < 0f) velocity.y = -2f;
            velocity.y += gravity * Time.deltaTime;
        }
    }
}
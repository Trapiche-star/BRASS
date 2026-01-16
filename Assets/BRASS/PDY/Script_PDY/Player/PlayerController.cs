using UnityEngine;

namespace BRASS
{
    /// 플레이어의 이동, 중력, 회전 및 슬라이딩 이동을 제어한다
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : MonoBehaviour
    {
        #region Variables
        [SerializeField] private float moveSpeed = 5f;           // 기본 이동 속도
        [SerializeField] private float fastRunSpeed = 8f;       // 패스트런 이동 속도
        [SerializeField] private float slideSpeed = 10f;        // 슬라이딩 초기 속도
        [SerializeField] private float slideDeceleration = 15f; // 슬라이딩 감속 속도

        [SerializeField] private float gravity = -9.81f;        // 중력 가속도
        [SerializeField] private Transform cameraPivot;         // 이동 방향 기준 카메라 피벗
        [SerializeField] private float rotationSpeed = 15f;     // 회전 보간 속도

        private PlayerInputHandler input;        // 플레이어 입력 담당
        private MovementAnimation movementAnim;  // 이동 상태(패스트런/슬라이딩) 판정 담당
        private CharacterController controller;  // 캐릭터 이동 컴포넌트

        private Vector3 velocity;                // 중력 누적 속도
        private Vector3 currentMoveDir;          // 현재 이동 방향 캐싱
        private Vector3 slideVelocity;           // 슬라이딩 이동 벡터

        private Vector3 destination;             // 자동 이동 목적지
        private bool isAutoMoving = false;       // 자동 이동 여부
        private bool isSliding = false;          // 현재 슬라이딩 상태 여부
        #endregion

        #region Property
        public bool IsAutoMoving => isAutoMoving; // 외부에서 자동 이동 여부 확인용
        #endregion

        #region Unity Event Method
        private void Awake()
        {
            input = GetComponent<PlayerInputHandler>();
            controller = GetComponent<CharacterController>();
            movementAnim = GetComponent<MovementAnimation>();
        }

        private void Update()
        {
            HandleClickToMove(); // 클릭 이동 처리
            HandleMovement();    // 이동 및 회전 처리
        }
        #endregion

        #region Custom Method
        // 좌클릭 위치로 자동 이동 목적지를 설정한다
        private void HandleClickToMove()
        {
            if (input == null) return; // 만약 [입력 참조가 없으면] [이 메서드에서는 더 이상 처리하지 않는다]

            if (input.ClickMovePressed) // 만약 [좌클릭 이동 입력이 들어오면] [자동 이동을 시작한다]
            {
                Ray ray = Camera.main.ScreenPointToRay(input.MousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit)) // 만약 [지면에 레이가 닿으면] [목적지를 설정한다]
                {
                    destination = hit.point;
                    isAutoMoving = true;
                }
            }

            if (input.MoveInput.sqrMagnitude > 0.01f) // 만약 [WASD 입력이 들어오면] [자동 이동을 해제한다]
                isAutoMoving = false;
        }

        // 이동 방향 계산, 속도 적용, 회전을 처리한다
        private void HandleMovement()
        {
            if (input == null || cameraPivot == null) return; // 만약 [필수 참조가 없으면] [이 메서드에서는 더 이상 처리하지 않는다]

            Vector3 moveDir = Vector3.zero;

            if (isAutoMoving) // 만약 [자동 이동 중이면] [목적지 방향으로 이동한다]
            {
                Vector3 dir = destination - transform.position;
                dir.y = 0f;

                if (dir.magnitude > 0.1f) moveDir = dir.normalized;
                else isAutoMoving = false; // 만약 [목적지에 도달했으면] [자동 이동을 종료한다]
            }
            else // 만약 [수동 이동 중이면] [카메라 기준 방향으로 이동한다]
            {
                Vector2 m = input.MoveInput;

                Transform cam = Camera.main.transform; // 실제 카메라 방향 기준
                Vector3 forward = cam.forward;
                Vector3 right = cam.right;

                forward.y = 0f;
                right.y = 0f;
                forward.Normalize();
                right.Normalize();

                moveDir = (forward * m.y + right * m.x).normalized;
            }

            currentMoveDir = moveDir; // 현재 이동 방향을 캐싱한다
            ApplyGravity();           // 중력을 누적 적용한다

            // 1. 슬라이딩 최우선 처리 (패스트런/일반 이동 전부 무시)
            if (isSliding)
            {
                if (slideVelocity != Vector3.zero)
                {
                    controller.Move((slideVelocity + velocity) * Time.deltaTime);
                    slideVelocity = Vector3.Lerp(slideVelocity, Vector3.zero, slideDeceleration * Time.deltaTime); // 만약 [슬라이딩 중이면] [점점 감속한다]
                }
                else
                {
                    controller.Move(velocity * Time.deltaTime); // 만약 [슬라이딩 이동량이 없으면] [중력만 적용한다]
                }

                return; // 슬라이딩 중에는 다른 이동 로직을 전부 차단한다
            }

            // 2. 일반/패스트런 이동
            float speed = moveSpeed; // 기본 이동 속도를 적용한다

            if (movementAnim != null && movementAnim.IsFastRun) // 만약 [패스트런 상태이면] [더 빠른 속도를 적용한다]
                speed = fastRunSpeed;

            if (moveDir != Vector3.zero) // 만약 [이동 방향이 있으면] [이동과 회전을 수행한다]
            {
                controller.Move((moveDir * speed + velocity) * Time.deltaTime);

                Quaternion targetRot = Quaternion.LookRotation(moveDir);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
            }
            else // 만약 [이동 입력이 없으면] [중력만 적용한다]
            {
                controller.Move(velocity * Time.deltaTime);
            }
        }

        // 중력을 누적하여 수직 속도를 계산한다
        private void ApplyGravity()
        {
            if (controller.isGrounded && velocity.y < 0f) velocity.y = -2f; // 만약 [지면에 붙어 있으면] [낙하 속도를 초기화한다]
            velocity.y += gravity * Time.deltaTime; // 만약 [매 프레임] [중력을 누적한다]
        }

        // 슬라이딩 애니메이션 시작 시 실제 이동을 개시한다
        public void OnSlideStart()
        {
            isSliding = true; // 슬라이딩 상태 진입

            if (currentMoveDir == Vector3.zero) return; // 만약 [이동 방향이 없으면] [슬라이딩을 시작하지 않는다]
            slideVelocity = currentMoveDir * slideSpeed; // 슬라이딩 초기 속도를 설정한다
        }

        // 슬라이딩 애니메이션 종료 시 슬라이딩 이동을 중단한다
        public void OnSlideEnd()
        {
            isSliding = false;     // 슬라이딩 상태 종료
            slideVelocity = Vector3.zero; // 슬라이딩 이동을 즉시 종료한다
        }

        // 슬라이딩 이동을 시작한다 (애니메이션 이벤트용)
        public void OnSlideMoveStart()
        {
            isSliding = true; // 슬라이딩 상태 진입

            if (currentMoveDir == Vector3.zero) return; // 만약 [이동 방향이 없으면] [슬라이딩을 시작하지 않는다]
            slideVelocity = currentMoveDir * slideSpeed; // 슬라이딩 이동을 개시한다
        }

        // 슬라이딩 이동을 종료한다 (애니메이션 이벤트용)
        public void OnSlideMoveEnd()
        {
            isSliding = false;     // 슬라이딩 상태 종료
            slideVelocity = Vector3.zero; // 슬라이딩 이동을 중단한다
        }
        #endregion
    }
}

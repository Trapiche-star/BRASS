using UnityEngine;

namespace BRASS
{
    /// <summary>
    /// 점프 입력을 처리하고
    /// 점프 횟수(JumpIndex)를 기준으로 2단 점프까지만 허용한다
    /// 1단 점프에는 짧은 입력 쿨타임을 적용하여 연타를 방지하고
    /// 2단 점프 이후에는 별도의 회복 쿨타임을 적용하여
    /// 착지 직후 애니메이션이 씹히는 현상을 방지한다
    /// </summary>
    public class PlayerJump : MonoBehaviour
    {
        #region Variables
        [SerializeField] private int maxJumpCount = 2;            // 허용되는 최대 점프 횟수 (1단 + 2단)
        [SerializeField] private float jumpForce = 5f;            // 점프 시 적용할 수직 속도

        [Header("Cooldown")]
        [SerializeField] private float jumpInputCooldown = 0.15f;
        // 1단 점프 포함 공통 입력 연타 방지용 쿨타임

        [SerializeField] private float secondJumpCooldown = 0.25f;
        // 2단 점프 사용 이후 적용되는 착지 애니메이션 보호용 쿨타임

        private PlayerController controller;                      // 수직 속도 적용을 담당하는 컨트롤러 참조
        private PlayerState state;                                 // 점프 단계 및 접지 상태를 관리하는 상태 컨테이너

        private float lastJumpInputTime;                           // 마지막 점프 입력이 처리된 시각
        private float secondJumpTime;                              // 마지막 2단 점프가 실행된 시각
        private bool isSecondJumpCooldownActive;                   // 2단 점프 쿨타임 활성 여부
        #endregion

        #region Unity Event Method
        private void Awake()
        {
            controller = GetComponentInParent<PlayerController>();
            // 부모 계층에서 PlayerController를 탐색하여 캐싱한다

            state = GetComponentInParent<PlayerState>();
            // 부모 계층에서 PlayerState를 탐색하여 캐싱한다
        }

        private void Update()
        {
            CheckSecondJumpCooldown();
            // 2단 점프 쿨타임 종료 여부를 감시한다
        }
        #endregion

        #region Custom Method
        // 점프 입력이 들어왔을 때 호출되어 점프 가능 여부를 판단한다
        public void TryJump()
        {
            if (state == null || controller == null)
                return;
            // 필수 참조가 없으면 점프 처리를 수행하지 않는다

            // 슬라이딩 중에는 점프 입력 자체를 무시한다
            if (state.IsSliding)
                return;

            // 공격 중이라면 점프 입력으로 즉시 공격을 캔슬한다
            if (state.IsAttacking)
            {
                MeleeSkill melee = GetComponentInParent<MeleeSkill>();

                if (melee != null)
                {
                    state.IsAttacking = false;
                    state.IsInputMovementLocked = false;
                    melee.OnSkill3MoveEnd();
                    melee.StopAllCoroutines();
                }

                GetComponent<PlayerCombat>()?.CancelAttack();

                Debug.Log("[Jump] 점프 입력으로 공격 상태 강제 종료");
            }

            if (Time.time < lastJumpInputTime + jumpInputCooldown)
                return;

            if (state.JumpIndex == 0 &&
                isSecondJumpCooldownActive &&
                Time.time < secondJumpTime + secondJumpCooldown)
                return;

            if (state.JumpIndex >= maxJumpCount)
                return;

            state.JumpIndex++;
            state.IsJumping = true;

            controller.SetVerticalVelocity(jumpForce);

            lastJumpInputTime = Time.time;

            if (state.JumpIndex == maxJumpCount)
            {
                secondJumpTime = Time.time;
                isSecondJumpCooldownActive = true;
            }
        }


        // 2단 점프 보호 쿨타임 종료 시점을 감지한다
        private void CheckSecondJumpCooldown()
        {
            if (!isSecondJumpCooldownActive) return;
            // 보호 쿨타임이 활성화되지 않았으면 검사하지 않는다

            if (Time.time >= secondJumpTime + secondJumpCooldown)
            {
                isSecondJumpCooldownActive = false;
                // 보호 쿨타임 상태를 종료한다

                Debug.Log("[Jump] 2단 점프 보호 쿨타임 종료");
                // 디버그용: 착지 직후 애니메이션 보호 구간 종료
            }
        }
        #endregion
    }
}

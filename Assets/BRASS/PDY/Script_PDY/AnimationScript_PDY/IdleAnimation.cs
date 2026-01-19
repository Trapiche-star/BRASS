using UnityEngine;

namespace BRASS
{
    /// Idle 상태가 연속 유지되고 있으며 Animator가 기본 Idle 상태일 때만 시간을 누적하여
    /// 일정 시간이 지나면 Idle_Dwarf를 재생하도록 제어하는 모듈
    /// Idle_Dwarf 재생 중에는 타이머가 동작하지 않는다
    public class IdleAnimation : MonoBehaviour
    {
        #region Variables
        [SerializeField] private float idleAltInterval = 5f; // Idle_Dwarf 재생까지 필요한 연속 Idle 시간
        [SerializeField] private string baseIdleStateName = "Idle"; // 기본 Idle 애니메이션 스테이트 이름

        private Animator animator; // 캐릭터 Animator 참조
        private float idleTimer;   // 기본 Idle 상태가 연속으로 유지된 시간
        private bool isIdle;       // 논리적 Idle 상태 여부
        #endregion

        #region Unity Event Method
        private void Awake()
        {
            animator = GetComponent<Animator>(); // 동일 오브젝트의 Animator를 자동으로 참조한다
        }

        private void Update()
        {
            if (!isIdle) return; // 만약 논리적으로 Idle 상태가 아니면 시간을 누적하지 않는다.

            AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);
            if (!state.IsName(baseIdleStateName)) return;
            // 만약 Animator가 기본 Idle 상태가 아니면(Idle_Dwarf 포함) 시간을 누적하지 않는다.

            idleTimer += Time.deltaTime; // 기본 Idle 상태에서만 시간을 누적한다.
        }
        #endregion

        #region Custom Method
        // Idle 상태 진입/이탈 시에만 호출된다
        public void SetIdleState(bool value)
        {
            if (isIdle == value) return; // 만약 상태 변화가 없다면 아무 처리도 하지 않는다.

            isIdle = value;  // 논리적 Idle 상태 여부를 갱신한다.
            idleTimer = 0f;  // 상태가 바뀌는 순간 연속 Idle 조건이 끊기므로 시간을 초기화한다.
        }

        // Idle_Dwarf를 재생해야 하는 시점인지 판단한다
        public bool ShouldPlayIdleAlt()
        {
            if (!isIdle) return false; // 논리적으로 Idle 상태가 아니면 실행하지 않는다.

            AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);
            if (!state.IsName(baseIdleStateName)) return false;
            // 기본 Idle 상태가 아닐 경우 재생 조건을 만족하지 않는다.

            if (idleTimer < idleAltInterval) return false;
            // 지정된 Idle 유지 시간이 충족되지 않았으면 실행하지 않는다.

            idleTimer = 0f; // 한 번 실행했으므로 다음 주기를 위해 타이머를 초기화한다.
            return true;    // Idle_Dwarf 재생을 허용한다.
        }
        #endregion
    }
}

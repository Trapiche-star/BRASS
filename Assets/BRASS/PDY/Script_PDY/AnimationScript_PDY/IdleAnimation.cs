using UnityEngine;

namespace BRASS
{
    /// 플레이어 Idle 상태가 일정 시간(연속) 유지되면 대체 Idle(Idle_Dwarf)을 1회 재생하는 모듈
    /// 쿨타임 없이 "Idle 연속 유지 시간"만으로 주기를 제어한다
    public class IdleAnimation : MonoBehaviour
    {
        #region Variables
        [SerializeField] private float idleAltInterval = 5f; // Idle 연속 유지 후 대체 Idle을 재생하기까지 대기 시간(초)

        private float idleTimer = 0f; // Idle 연속 유지 시간 누적
        private bool isIdle = false;  // 현재 Idle 상태 여부
        #endregion

        #region Unity Event Method
        private void Update()
        {
            if (!isIdle) return; // 만약 [현재 Idle 상태가 아니면] [Idle 타이머를 누적하지 않는다]

            idleTimer += Time.deltaTime; // Idle 연속 유지 시간을 누적한다
        }
        #endregion

        #region Custom Method
        // 외부(컨트롤러)에서 Idle 상태 진입/이탈을 전달한다
        public void SetIdleState(bool value)
        {
            if (isIdle == value) return; // 만약 [상태 변화가 없으면] [아무 것도 하지 않는다]

            isIdle = value;  // Idle 상태 갱신
            idleTimer = 0f;  // 만약 [상태가 바뀌면] [연속 유지 시간이 끊겼으므로 타이머를 초기화한다]
        }

        // Idle_Alt(Idle_Dwarf)를 실행할 수 있는지 판단한다
        public bool ShouldPlayIdleAlt()
        {
            if (!isIdle) return false; // 만약 [Idle 상태가 아니면] [실행하지 않는다]
            if (idleTimer < idleAltInterval) return false; // 만약 [연속 Idle 시간이 부족하면] [실행하지 않는다]

            idleTimer = 0f; // 실행 직후 연속 Idle 시간을 초기화한다(다시 5초부터 재계측)
            return true;    // Idle_Alt 실행을 허용한다
        }
        #endregion
    }
}

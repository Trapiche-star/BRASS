using UnityEngine;

namespace BRASS
{
    /// <summary>
    /// 점프 입력을 처리하며
    /// 프레임 소모와 조건 분리를 통해 와다다 연타를 차단하고
    /// 공중 점프 간격 제어로 2단 점프 타이밍을 보장한다
    /// </summary>
    public class PlayerJump : MonoBehaviour
    {
        #region Variables
        [SerializeField] private int maxJumpCount = 2;
        // 최대 점프 횟수 (1단 + 2단)

        [SerializeField] private float jumpForce = 5f;
        // 점프 시 적용할 수직 속도

        [SerializeField] private float groundInputDebounce = 0.12f;
        // 지면 점프 연타 방지를 위한 최소 입력 간격

        [SerializeField] private float airJumpInterval = 0.25f;
        // 공중 점프 간 최소 간격 (2단 점프용)

        private int currentJumpCount;
        // 현재 점프 사용 횟수

        private float lastGroundJumpTime = -999f;
        // 마지막 지면 점프 시점

        private float lastAirJumpTime = -999f;
        // 마지막 공중 점프 시점

        private int lastProcessedFrame = -1;
        // 같은 프레임 내 중복 점프 처리를 방지하기 위한 프레임 기록

        private PlayerController controller;
        // 수직 속도 적용 및 애니메이션 트리거 전달 담당

        private PlayerState state;
        // 지면 접촉 여부 및 점프 단계 상태 참조
        #endregion

        #region Unity Event Method
        private void Awake()
        {
            controller = GetComponentInParent<PlayerController>();
            // 부모 계층에서 PlayerController를 캐싱한다

            state = GetComponentInParent<PlayerState>();
            // 부모 계층에서 PlayerState를 캐싱한다
        }

        private void Update()
        {
            ResetJumpCountIfGrounded();
            // 착지 시 점프 관련 상태를 초기화한다
        }
        #endregion

        #region Custom Method
        // 점프 입력이 들어왔을 때 호출되어 조건을 만족하면 점프를 실행한다
        public void TryJump()
        {
            float now = Time.time;

            // 같은 프레임에 이미 점프를 처리했다면 추가 입력을 무시한다
            if (Time.frameCount == lastProcessedFrame) return;

            // 최대 점프 횟수 초과 방지
            if (currentJumpCount >= maxJumpCount) return;

            // 1단 점프 (지면)
            if (currentJumpCount == 0)
            {
                // 지면 점프 연타 방지
                if (now - lastGroundJumpTime < groundInputDebounce) return;

                lastGroundJumpTime = now;
            }
            // 2단 점프 (공중)
            else
            {
                // 공중 점프 간 최소 간격 보장
                if (now - lastAirJumpTime < airJumpInterval) return;

                lastAirJumpTime = now;
            }

            lastProcessedFrame = Time.frameCount;
            // 이번 프레임의 점프 처리를 소비 처리한다

            controller.SetVerticalVelocity(jumpForce);
            // 수직 속도를 설정하여 점프를 수행한다

            currentJumpCount++;
            // 점프 사용 횟수를 증가시킨다

            state.JumpIndex = currentJumpCount;
            // 현재 점프 단계를 상태값으로 기록한다

            controller.TriggerJumpAnimation(state.JumpIndex);
            // 점프 단계에 맞는 애니메이션 트리거를 발생시킨다
        }

        // 지면에 착지했을 때 점프 상태를 초기화한다
        private void ResetJumpCountIfGrounded()
        {
            // 공중 상태라면 초기화를 수행하지 않는다
            if (!state.IsGrounded) return;

            // 이미 초기화된 상태라면 중복 실행을 방지한다
            if (currentJumpCount == 0) return;

            currentJumpCount = 0;
            // 점프 사용 횟수를 초기화한다

            state.JumpIndex = 0;
            // 점프 단계 상태값을 리셋한다

            controller.ResetJumpIndex();
            // 애니메이터의 JumpIndex 파라미터를 초기화한다
        }
        #endregion
    }
}

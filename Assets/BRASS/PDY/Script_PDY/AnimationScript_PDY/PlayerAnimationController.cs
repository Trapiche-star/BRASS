using UnityEngine;

namespace BRASS
{
    /// PlayerState를 기반으로 애니메이터 파라미터를 갱신하여 결과만 재생하는 클래스
    public class PlayerAnimationController : MonoBehaviour
    {
        #region Variables
        public Transform parent;
        [SerializeField] private Animator animator; // 애니메이션 재생을 담당하는 컴포넌트
        [SerializeField] private PlayerState state; // 플레이어의 현재 상태 데이터 참조
        [SerializeField] private IdleAnimation idleAnimation; // 대기 상태 변형 애니메이션 제어기

        private int hashIsMoving; // 이동 상태 파라미터 해시
        private int hashIsSliding; // 슬라이딩 상태 파라미터 해시
        private int hashFastRun; // 고속 달리기 배율 파라미터 해시
        private int hashIsGrounded; // 접지 여부 파라미터 해시

        private int hashIsJumping; // 점프 중 여부 파라미터 해시
        private int hashJumpIndex; // 점프 타수 인덱스 파라미터 해시

        private int hashIdleDwarf; // 대기 변형 동작 트리거 해시

        private int hashAttack; // 공격 동작 트리거 해시

        private int hashIsEquipped; // 추가: 무기 장착 해시
        #endregion

        #region Unity Event Method
        private void Awake()
        {
            if (animator == null) // 애니메이터 참조가 할당되지 않은 경우
                animator = GetComponentInChildren<Animator>(); // 자식에서 애니메이터를 찾아 연결한다

            if (state == null) // 상태 참조가 할당되지 않은 경우
                state = GetComponentInParent<PlayerState>(); // 부모에서 상태 컴포넌트를 찾아 연결한다

            // 문자열 비교 오버헤드를 방지하기 위해 파라미터 이름을 해시로 변환하여 저장
            hashIsMoving = Animator.StringToHash("IsMoving");
            hashIsSliding = Animator.StringToHash("IsSliding");
            hashFastRun = Animator.StringToHash("FastRun");
            hashIsGrounded = Animator.StringToHash("IsGrounded");
            hashIsJumping = Animator.StringToHash("IsJumping");
            hashJumpIndex = Animator.StringToHash("JumpIndex");
            hashIdleDwarf = Animator.StringToHash("Idle_Dwarf");
            hashAttack = Animator.StringToHash("Attack");
            hashIsEquipped = Animator.StringToHash("isEquipped");
        }

        private void Update()
        {
            UpdateAnimator(); // 매 프레임 논리 상태를 애니메이터 파라미터에 투영한다
        }

        private void OnAnimatorMove()
        {
            //
            Vector3 position = animator.rootPosition;
            position.y = parent.position.y;
            parent.position = position;
        }
        #endregion

        #region Custom Method
        // 현재 플레이어의 상태 값을 읽어 애니메이터의 각 파라미터를 동기화함
        private void UpdateAnimator()
        {
            if (animator == null || state == null) return; // 필수 컴포넌트가 누락되었다면 업데이트를 중단한다

            animator.SetBool(hashIsEquipped, state.IsEquipped);  // 무기 장착 상태 전달
            animator.SetBool(hashIsMoving, state.IsMoving);      // 이동 플래그 전달
            animator.SetBool(hashIsSliding, state.IsSliding);    // 슬라이딩 플래그 전달
            animator.SetBool(hashIsGrounded, state.IsGrounded);  // 접지 플래그 전달
            animator.SetBool(hashIsJumping, state.IsJumping);    // 점프 플래그 전달
            animator.SetInteger(hashJumpIndex, state.JumpIndex); // 점프 단계 정수 전달

            animator.SetFloat(
                hashFastRun,
                state.IsFastRun ? 1.2f : 1f
            ); // 고속 주행 시에는 애니메이션 재생 속도를 1.2배로 높여 적용한다

            if (idleAnimation != null) // 대기 애니메이션 제어 객체가 존재하는 경우
            {
                idleAnimation.SetIdleState(state.IsIdle); // 현재 대기 여부를 제어기에 전달한다

                if (idleAnimation.ShouldPlayIdleAlt()) // 변형 대기 동작을 재생할 타이밍이라면
                    animator.SetTrigger(hashIdleDwarf); // 애니메이터에 변형 대기 트리거를 발생시킨다
            }
        }

        // 공격 시작 시 애니메이터에 공격 트리거를 전달함
        public void PlayAttack()
        {
            if (animator == null) return; // 애니메이터가 없다면 명령을 수행하지 않는다
            animator.SetTrigger(hashAttack); // 설정된 공격 트리거 파라미터를 활성화한다
        }

        // 현재 진행 중인 공격 연출을 즉시 중단하고 대기 상태로 되돌림
        public void StopAttack()
        {
            if (animator == null) return; // 참조가 유효하지 않으면 로직을 종료한다

            animator.ResetTrigger(hashAttack); // 대기 중인 공격 트리거를 초기화한다
            animator.CrossFade("Idle", 0.1f); // 0.1초의 전환 시간을 거쳐 기본 대기 상태로 강제 전이한다
        }
        #endregion
    }
}
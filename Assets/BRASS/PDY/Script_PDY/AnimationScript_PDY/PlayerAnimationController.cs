using UnityEngine;

namespace BRASS
{
    /// PlayerState를 기반으로 애니메이터 파라미터를 갱신하여 결과만 재생하는 클래스
    public class PlayerAnimationController : MonoBehaviour
    {
        #region Variables
        public Transform parent; // 루트 모션 적용 시 이동시킬 부모 트랜스폼

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
        private int hashIsEquipped; // 무기 장착 상태 파라미터 해시
        private int hashIsBattleAxeEquipped; // 배틀액스 장착 상태 파라미터 해시
        #endregion

        #region Unity Event Method
        private void Awake()
        {
            if (animator == null) // 만약 애니메이터 참조가 없다면
                animator = GetComponentInChildren<Animator>(); // 자식 객체에서 컴포넌트를 찾아 할당한다

            if (state == null) // 만약 상태 참조가 없다면
                state = GetComponentInParent<PlayerState>(); // 부모 객체에서 컴포넌트를 찾아 할당한다

            // 애니메이터 파라미터 문자열을 해시값으로 미리 변환하여 성능을 최적화한다
            hashIsMoving = Animator.StringToHash("IsMoving");
            hashIsSliding = Animator.StringToHash("IsSliding");
            hashFastRun = Animator.StringToHash("FastRun");
            hashIsGrounded = Animator.StringToHash("IsGrounded");
            hashIsJumping = Animator.StringToHash("IsJumping");
            hashJumpIndex = Animator.StringToHash("JumpIndex");
            hashIdleDwarf = Animator.StringToHash("Idle_Dwarf");
            hashAttack = Animator.StringToHash("Attack");
            hashIsEquipped = Animator.StringToHash("IsEquipped");
            hashIsBattleAxeEquipped = Animator.StringToHash("IsBattleAxeEquipped");
        }

        private void Update()
        {
            UpdateAnimator(); // 매 프레임 플레이어의 논리 상태를 애니메이터에 동기화한다
        }

        private void OnAnimatorMove()
        {
            // 애니메이터의 루트 모션 위치를 계산하여 부모 오브젝트의 위치에 반영한다
            Vector3 position = animator.rootPosition;   // 애니메이션에 의한 루트 위치 추출
            position.y = parent.position.y;             // 수직 위치는 기존 부모의 높이를 유지하여 튀는 현상을 방지한다
            parent.position = position;                 // 최종 계산된 위치를 부모 트랜스폼에 적용한다
        }
        #endregion

        #region Custom Method
        // PlayerState의 변수들을 읽어 애니메이터 파라미터로 값을 전달함
        private void UpdateAnimator()
        {
            if (animator == null || state == null) return; // 참조가 유효하지 않으면 업데이트를 수행하지 않는다

            animator.SetBool(hashIsEquipped, state.IsEquipped);     // 무기 장착 상태 반영
            animator.SetBool(hashIsBattleAxeEquipped, state.IsBattleAxeEquipped); // 배틀액스 장착 상태 반영
            animator.SetBool(hashIsMoving, state.IsMoving);     // 이동 여부 반영
            animator.SetBool(hashIsSliding, state.IsSliding);   // 슬라이딩 여부 반영
            animator.SetBool(hashIsGrounded, state.IsGrounded); // 접지 여부 반영
            animator.SetBool(hashIsJumping, state.IsJumping);   // 점프 여부 반영
            animator.SetInteger(hashJumpIndex, state.JumpIndex); // 현재 점프 단계 반영

            animator.SetFloat(
                hashFastRun,
                state.IsFastRun ? 1.2f : 1f
            ); // 빠른 달리기 중이라면 애니메이션 배율을 1.2배로 높여 적용한다

            if (idleAnimation != null) // 대기 애니메이션 제어 객체가 연결되어 있다면
            {
                idleAnimation.SetIdleState(state.IsIdle); // 현재 논리적 대기 상태를 전달한다

                if (idleAnimation.ShouldPlayIdleAlt()) // 변형 대기 동작을 재생할 조건이 충족되었다면
                    animator.SetTrigger(hashIdleDwarf); // 변형 애니메이션 트리거를 활성화한다
            }
        }

        // 외부에서 호출하여 공격 애니메이션 재생 트리거를 작동시킴
        public void PlayAttack()
        {
            if (animator == null) return; // 애니메이터 참조가 없으면 동작을 수행하지 않는다
            animator.SetTrigger(hashAttack); // 설정된 공격 해시값으로 트리거를 실행한다
        }

        // 강제로 공격 상태를 해제하고 기본 대기 상태로 전환함
        public void StopAttack()
        {
            if (animator == null) return; // 참조가 유효하지 않으면 명령을 무시한다

            animator.ResetTrigger(hashAttack); // 대기 중인 공격 트리거가 있다면 모두 초기화한다
            animator.CrossFade("Idle", 0.1f); // 0.1초 동안 부드럽게 Idle 상태로 애니메이션을 전환한다
        }
        #endregion
    }
}
using UnityEngine;

namespace BRASS
{
    /// <summary>
    /// PlayerState를 기반으로 애니메이터 파라미터를 갱신하여 결과만 재생하는 클래스
    /// </summary>    
    public class PlayerAnimationController : MonoBehaviour
    {
        #region Variables
        public Transform parent; // 루트 모션 적용 시 이동시킬 부모 트랜스폼

        [SerializeField] private Animator animator; // 애니메이션 재생을 담당하는 컴포넌트
        [SerializeField] private PlayerState state; // 플레이어의 현재 상태 데이터 참조
        [SerializeField] private IdleAnimation idleAnimation; // 대기 상태 변형 애니메이션 제어기       
        
        private int hashIsMoving;   // 이동 상태 파라미터 해시
        private int hashIsSliding;  // 슬라이딩 상태 파라미터 해시
        private int hashFastRun;     // 고속 달리기 배율 파라미터 해시
        private int hashIsGrounded; // 접지 여부 파라미터 해시
        private int hashIsJumping; // 점프 중 여부 파라미터 해시
        private int hashJumpIndex; // 점프 타수 인덱스 파라미터 해시
        private int hashIdleDwarf; // 대기 변형 동작 트리거 해시
        private int hashAttack;     // 공격 동작 트리거 해시
        private int hashIsEquipped; // 무기 장착 상태 파라미터 해시
        private int hashIsBattleAxeEquipped; // 배틀액스 장착 상태 파라미터 해시
        private int hashIsGunEquipped;      // 하푼건 장착 상태 파라미터 해시
        private int hashGunFire;    // 하푼건 발사 트리거 해시
        private int hashIsShooting; // 사격 상태(불형) 파라미터 해시
        private int hashSkill2Fire;     // 스킬2 발사 트리거 해시
        private int hashSkill3Fire;     // 스킬3 발사 트리거 해시
        private int hashHit;   // 피격 트리거 해시
        private int hashIsDead;   // 사망 불형 해시
        #endregion

        #region Unity Event Method
        private void Awake()
        {   
            if (animator == null) animator = GetComponentInChildren<Animator>();    // 만약 애니메이터 참조가 없다면 자식 객체에서 컴포넌트를 찾아 할당한다
            if (state == null) state = GetComponentInParent<PlayerState>();     // 만약 상태 참조가 없다면 부모 객체에서 컴포넌트를 찾아 할당한다            

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
            hashIsGunEquipped = Animator.StringToHash("IsGunEquipped");
            hashGunFire = Animator.StringToHash("GunFire");
            hashIsShooting = Animator.StringToHash("IsShooting");
            hashSkill2Fire = Animator.StringToHash("Skill2Fire");
            hashSkill3Fire = Animator.StringToHash("Skill3Fire");
            hashHit = Animator.StringToHash("Hit");
            hashIsDead = Animator.StringToHash("IsDead");          

            // PlayerState 이벤트 구독
            if (state != null)
            {
                state.OnHit += PlayHit;
                state.OnDead += SetDead;
            }
        }

        // 매 프레임 호출되어 애니메이터 파라미터를 갱신함
        private void Update()
        {
            UpdateAnimator(); // 매 프레임 플레이어의 논리 상태를 애니메이터에 동기화한다
        }

        /*오리지널 루트 모션 사용 시 활성화
        private void OnAnimatorMove()
        {
            //애니매이션 자체 루트 모션 사용시 부모 오브젝트에 위치를 반영하기 위한 처리
            // 애니메이터의 루트 모션 위치를 계산하여 부모 오브젝트의 위치에 반영한다
            Vector3 position = animator.rootPosition;   // 애니메이션에 의한 루트 위치 추출
            position.y = parent.position.y;             // 수직 위치는 기존 부모의 높이를 유지하여 튀는 현상을 방지한다
            parent.position = position;                 // 최종 계산된 위치를 부모 트랜스폼에 적용한다

        2026/02/05 수정 전 코드
        // 스킬 중에는 유니티가 애니메이션 좌표를 물리 엔진에 적용하지 못하게 원천 차단
            if (state != null && state.IsAttacking) return; // 공격 중이면 루트 모션 적용을 건너뜀

            if (animator == null || parent == null) return; //  참조가 유효하지 않으면 아무 처리도 하지 않는다

            // 평상시 이동 (CharacterController를 쓸 때는 보통 아래 코드가 필요 없거나 다르게 짜여있을 겁니다)
            // 만약 평소에 루트 모션으로 걷는 게 아니라면 이 함수 전체를 주석 처리해도 됩니다.
            parent.position = animator.rootPosition;    // 애니메이션에 의한 루트 위치를 부모 오브젝트에 반영한다
            parent.rotation = animator.rootRotation;    //  애니메이션에 의한 루트 회전을 부모 오브젝트에 반영한다
        }*/

        private void OnAnimatorMove()
        {
            if (state != null && state.IsDead) return; // 사망 상태이면 루트 모션 적용을 건너뜀
            //스킬 중에는 유니티가 애니메이션 좌표를 물리 엔진에 적용하지 못하게 원천 차단
            if (state != null && state.IsAttacking) return; // 공격 중이면 루트 모션 적용을 건너뜀

            if (animator == null || parent == null) return; //  참조가 유효하지 않으면 아무 처리도 하지 않는다

            // 평상시 이동 (CharacterController를 쓸 때는 보통 아래 코드가 필요 없거나 다르게 짜여있을 겁니다)
            // 만약 평소에 루트 모션으로 걷는 게 아니라면 이 함수 전체를 주석 처리해도 됩니다.
            parent.position = animator.rootPosition;    // 애니메이션에 의한 루트 위치를 부모 오브젝트에 반영한다
            parent.rotation = animator.rootRotation;    //  애니메이션에 의한 루트 회전을 부모 오브젝트에 반영한다
        }
        #endregion

        #region Custom Method
        // PlayerState의 변수들을 읽어 애니메이터 파라미터로 값을 전달함
        private void UpdateAnimator()
        {
            if (animator == null || state == null) return; // 참조가 유효하지 않으면 업데이트를 수행하지 않는다

            // 사망 중이면 Death 상태만 유지
            if (state.IsDead)
            {
                animator.SetBool(hashIsDead, true);
                return;
            }

            animator.SetBool(hashIsEquipped, state.IsEquipped);     // 무기 장착 상태 반영
            animator.SetBool(hashIsBattleAxeEquipped, state.IsBattleAxeEquipped); // 배틀액스 장착 상태 반영
            animator.SetBool(hashIsMoving, state.IsMoving);     // 이동 여부 반영
            animator.SetBool(hashIsSliding, state.IsSliding);   // 슬라이딩 여부 반영
            animator.SetBool(hashIsGrounded, state.IsGrounded); // 접지 여부 반영
            animator.SetBool(hashIsJumping, state.IsJumping);   // 점프 여부 반영
            animator.SetInteger(hashJumpIndex, state.JumpIndex); // 현재 점프 단계 반영
            animator.SetBool(hashIsGunEquipped, state.IsGunEquipped);   // 건 장착 상태 반영

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

        // 외부에서 호출하여 하푼건 발사 애니메이션 재생 트리거를 작동시킴
        public void PlayGunFire()
        {
            if (animator == null) return;
            animator.SetTrigger(hashGunFire);
        }       
        
        // 사격 상태(연사 불형) 제어
        public void SetIsShooting(bool isShooting)
        {
            if (animator == null) return;
            animator.SetBool(hashIsShooting, isShooting);
        }

        // 스킬2 발사 애니메이션 재생 트리거를 작동시킴
        public void PlaySkill2Fire()
        {
            if (animator == null) return;
            animator.SetTrigger(hashSkill2Fire);
        }

        // 스킬3 발사 애니메이션 재생 트리거를 작동시킴
        public void PlaySkill3Fire()
        {
            if (animator == null) return;
            animator.SetTrigger(hashSkill3Fire);
        }

        // 피격 애니메이션 재생 트리거를 작동시킴
        private void OnDestroy()
        {
            if (state != null)
            {
                state.OnHit -= PlayHit;
                state.OnDead -= SetDead;
            }
        }

        // 피격 애니메이션 재생
        private void PlayHit()
        {
            if (animator == null) return;
            if (state != null && state.IsDead) return;
            animator.SetTrigger(hashHit);
        }

        // 사망 애니메이션 재생
        private void SetDead()
        {
            if (animator == null) return;

            // 이미 죽은 상태면 다시 세팅하지 않음
            if (animator.GetBool(hashIsDead)) return;

            animator.SetBool(hashIsDead, true);
        }       
        #endregion
    }
}
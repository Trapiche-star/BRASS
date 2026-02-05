using UnityEngine;

namespace BRASS
{
    /// 애니메이션 이벤트를 각 로직 컴포넌트로 전달하는 공용 이벤트 허브 클래스
    public class AnimationEventRelay : MonoBehaviour
    {
        #region Variables
        [SerializeField] private PlayerAnimationController animationController; // 애니메이션 제어 컴포넌트

        [SerializeField] private PlayerSlide slide;         // 슬라이드 동작 제어 컴포넌트
        [SerializeField] private PlayerJump jump;           // 점프 동작 제어 컴포넌트
        [SerializeField] private PlayerCombat combat;       // 근접 기본 공격 처리 컴포넌트
        [SerializeField] private PlayerState state;         // 플레이어 상태 데이터 컨테이너
        [SerializeField] private MeleeSkill meleeSkill;     // 근접 스킬 처리 컴포넌트 
        [SerializeField] private RangeCombat rangeCombat;   // 원거리 기본 공격 처리 컴포넌트                                                          
        #endregion

        #region Unity Event Method
        private void Awake()
        {
            if (slide == null) slide = GetComponentInParent<PlayerSlide>();
            if (jump == null) jump = GetComponentInParent<PlayerJump>();
            if (combat == null) combat = GetComponentInParent<PlayerCombat>();
            if (state == null) state = GetComponentInParent<PlayerState>();
            if (meleeSkill == null) meleeSkill = GetComponentInParent<MeleeSkill>();
            if (rangeCombat == null) rangeCombat = GetComponentInParent<RangeCombat>();
        }
        #endregion

        #region Custom Method
        // 슬라이딩 이동 시작
        public void OnSlideMoveStart()
        {
            if (slide == null) return;
            slide.BeginSlide(transform.forward);
        }

        // 슬라이딩 이동 종료
        public void OnSlideMoveEnd()
        {
            if (slide == null) return;
            slide.EndSlide();
        }

        // 기본 공격 판정 시작
        public void OnAttackHitStart()
        {
            if (state == null || combat == null)
                return;

            if (!combat.IsAttackSequenceActive)
                return;

            state.IsAttacking = true;
            state.IsInputMovementLocked = true;

            combat.OnAttackHitStart();
        }

        // 기본 공격 판정 종료
        public void OnAttackHitEnd()
        {
            if (state == null || combat == null)
                return;

            state.IsAttacking = false;
            state.IsInputMovementLocked = false;

            combat.OnAttackHitEnd();
        }

        // 콤보 입력 판정 (1타 종료)
        public void OnComboSection1End()
        {
            if (combat == null) return;
            combat.OnComboSectionReached(1);
        }

        // 콤보 입력 판정 (2타 종료)
        public void OnComboSection2End()
        {
            if (combat == null) return;
            combat.OnComboSectionReached(2);
        }

        // 콤보 전체 종료
        public void OnComboAnimationEnd()
        {
            if (combat == null) return;
            combat.OnComboAnimationFinished();
        }

        // 스킬 공격 시작
        public void OnSkillAttackStart()
        {
            if (state == null) return;

            state.IsAttacking = true;
            state.IsInputMovementLocked = true;
        }

        // 스킬 공격 종료
        public void OnSkillAttackEnd()
        {
            if (state == null) return;

            state.IsAttacking = false;
            state.IsInputMovementLocked = false;

            if (meleeSkill != null)
                meleeSkill.OnSkillAttackEnd();
        }

        // 스킬 3 이동 시작
        public void OnSkill3MoveStart()
        {
            if (meleeSkill != null)
                meleeSkill.OnSkill3MoveStart();
        }

        // 스킬 3 이동 종료
        public void OnSkill3MoveEnd()
        {
            if (meleeSkill != null)
                meleeSkill.OnSkill3MoveEnd();
        }

        // 스킬 데미지 이벤트
        public void OnSkill1Damage()
        {
            if (meleeSkill != null)
                meleeSkill.OnSkill1Damage();
        }

        public void OnSkill2Damage()
        {
            if (meleeSkill != null)
                meleeSkill.OnSkill2Damage();
        }

        public void OnSkill3Damage()
        {
            if (meleeSkill != null)
                meleeSkill.OnSkill3Damage();
        }

        // 콤보 1타 미세 전진
        public void OnComboStep1Move()
        {
            if (combat == null)
                return;

            combat.ApplyComboStep(0);
        }

        // 콤보 2타 미세 전진
        public void OnComboStep2Move()
        {
            if (combat == null)
                return;

            combat.ApplyComboStep(1);
        }

        // 콤보 3타 미세 전진
        public void OnComboStep3Move()
        {
            if (combat == null)
                return;

            combat.ApplyComboStep(2);
        }

        // 총 발사 타이밍
        public void OnGunFire()
        {
            //if (animationController != null) animationController.SetGunMaskWeight(true);            

            if (rangeCombat != null)
                rangeCombat.Fire();
        }

        // 발사 애니메이션 종료
        public void OnGunFireEnd()
        {
            //if (animationController != null) animationController.SetGunMaskWeight(false);           

            if (rangeCombat != null)
                rangeCombat.OnFireEnd();
        }
        #endregion
    }
}

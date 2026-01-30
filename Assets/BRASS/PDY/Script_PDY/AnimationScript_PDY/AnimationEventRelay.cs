using UnityEngine;

namespace BRASS
{
    /// 애니메이션 이벤트를 각 로직 컴포넌트로 전달하는 공용 이벤트 허브 클래스
    public class AnimationEventRelay : MonoBehaviour
    {
        #region Variables
        [SerializeField] private PlayerController controller; // 이동 로직 제어 컴포넌트
        [SerializeField] private PlayerJump jump; // 점프 로직 제어 컴포넌트
        [SerializeField] private PlayerCombat combat; // 전투 및 콤보 로직 제어 컴포넌트
        [SerializeField] private PlayerState state; // 캐릭터 상태 플래그 관리 컴포넌트
        [SerializeField] private MeleeSkill meleeSkill; // 현재 실행 중인 근접 스킬 로직
                                                       
        #endregion

        #region Unity Event Method
        private void Awake()
        {
            //  각 컴포넌트 참조가 할당되어 있지 않다면 부모 오브젝트에서 찾아 할당한다

            if (controller == null) // 만약 이동 컨트롤러 참조가 없다면
                controller = GetComponentInParent<PlayerController>(); // 부모 오브젝트에서 해당 컴포넌트를 찾아 할당한다

            if (jump == null) // 만약 점프 컴포넌트 참조가 없다면
                jump = GetComponentInParent<PlayerJump>(); // 부모 오브젝트에서 해당 컴포넌트를 찾아 할당한다

            if (combat == null) // 만약 전투 컴포넌트 참조가 없다면
                combat = GetComponentInParent<PlayerCombat>(); // 부모 오브젝트에서 해당 컴포넌트를 찾아 할당한다

            if (state == null) // 만약 상태 컴포넌트 참조가 없다면
                state = GetComponentInParent<PlayerState>(); // 부모 오브젝트에서 해당 컴포넌트를 찾아 할당한다

            if (meleeSkill == null)
                meleeSkill = GetComponentInParent<MeleeSkill>();
        }
        #endregion

        #region Custom Method
        // 슬라이딩 이동이 시작될 때 호출
        public void OnSlideMoveStart()
        {
            if (controller == null) return; // 컨트롤러가 없으면 이 메서드의 로직을 수행하지 않는다
            controller.StartSlide(transform.forward); // 현재 모델의 정면 방향으로 슬라이딩 물리 이동을 시작한다
        }

        // 슬라이딩 이동이 종료될 때 호출
        public void OnSlideMoveEnd()
        {
            if (controller == null) return; // 참조가 유효하지 않으면 동작을 취소한다
            controller.EndSlide(); // 이동 로직의 슬라이딩 상태를 해제한다
        }

        // 캐릭터가 공격 판정을 시작하는 프레임에 호출
        public void OnAttackHitStart()
        {
            Debug.Log("이벤트가 정상적으로 호출되었습니다!"); // 이게 콘솔에 안 찍히면 이벤트 마커 설정 문제임

            if (state == null) return; // 상태 관리자가 없으면 다음 코드를 실행하지 않는다

            //  공격 시퀀스가 이미 끝났다면 무시
            if (!combat || !combat.IsAttackSequenceActive)
                return;

            state.IsAttacking = true; // 상태 플래그를 공격 중으로 변경하여 이동 등을 제한한다
            state.IsInputMovementLocked = true;

            if (combat != null)
                combat.OnAttackHitStart(); // Combat 스크립트에게 무기 판정을 시작하라고 전달
        }

        // 캐릭터의 공격 판정이 완전히 끝나는 프레임에 호출
        public void OnAttackHitEnd()
        {
            Debug.Log("앤드이벤트가 호출되었습니다"); // 이게 콘솔에 안 찍히면 이벤트 마커 설정 문제임

            if (state == null) return; // 참조가 없으면 상호작용을 중단한다
            state.IsAttacking = false; // 공격 중 상태를 해제하여 일반 상태로 복귀시킨다
            state.IsInputMovementLocked = false;
            Debug.Log($"2. IsAttacking 값 변경 완료: {state.IsAttacking}");            

            if (combat != null)
                combat.OnAttackHitEnd();    //Combat 스크립트에게 무기 판정을 종료하라고 전달
        }

        // 콤보 1타 애니메이션 중 미세 전진이 필요한 시점에 호출
        public void OnComboStep1Move()
        {
            if (combat == null) return; // 전투 로직 참조가 없으면 전진 처리를 하지 않는다
            combat.ApplyComboStep(0); // 0번 인덱스에 설정된 거리만큼 캐릭터를 전진시킨다
        }

        // 콤보 2타 애니메이션 중 미세 전진이 필요한 시점에 호출
        public void OnComboStep2Move()
        {
            if (combat == null) return; // 참조가 유효하지 않으면 메서드를 종료한다
            combat.ApplyComboStep(1); // 1번 인덱스에 설정된 거리만큼 캐릭터를 전진시킨다
        }

        // 콤보 3타 애니메이션 중 미세 전진이 필요한 시점에 호출
        public void OnComboStep3Move()
        {
            if (combat == null) return; // 참조가 유효하지 않으면 동작을 무시한다
            combat.ApplyComboStep(2); // 2번 인덱스에 설정된 거리만큼 캐릭터를 전진시킨다
        }

        // 1타 애니메이션 종료 시점에 다음 콤보 입력 여부를 확인
        public void OnComboSection1End()
        {
            if (combat == null) return; // 전투 컴포넌트가 없으면 시퀀스 연계를 확인하지 않는다
            combat.OnComboSectionReached(1); // 1타 섹션의 입력 유효성을 검사하여 연계 여부를 결정한다
        }

        // 2타 애니메이션 종료 시점에 다음 콤보 입력 여부를 확인
        public void OnComboSection2End()
        {
            if (combat == null) return; // 참조가 없으면 로직을 더 이상 진행하지 않는다
            combat.OnComboSectionReached(2); // 2타 섹션의 입력 유효성을 검사하여 연계 여부를 결정한다
        }

        // 전체 공격 콤보 애니메이션 시퀀스가 완전히 끝났을 때 호출
        public void OnComboAnimationEnd()
        {
            if (combat == null) return; // 참조가 유효하지 않으면 종료 처리를 수행하지 않는다
            combat.OnComboAnimationFinished(); // 누적된 콤보 입력과 공격 상태를 모두 초기화한다
        }

        // 1,2스킬 공격이 시작되는 프레임에 호출되어 공격 상태 전환과 이동 차단을 처리한다
        public void OnSkillAttackStart()
        {
            Debug.Log("스킬어택 스타트 호출 성공");            

            if (state == null) return;
            // 상태 컨테이너가 없으면 처리하지 않는다

            state.IsAttacking = true;
            // 스킬 공격 중 상태로 전환한다

            state.IsInputMovementLocked = true;
            // 스킬 공격 중에는 입력 기반 이동을 차단한다

            if (meleeSkill != null) meleeSkill.OnSkillAttackStart();     
            // 어떤 스킬이든 공통 시작 알림
        }

        // 3번 스킬 전용 중계 시작
        public void OnSkill3MoveStart()
        {
            if (meleeSkill != null)
                meleeSkill.OnSkill3MoveStart();
        }

        // 3번 스킬 전용 중계 종료
        public void OnSkill3MoveEnd()
        {
            if (meleeSkill != null)
                meleeSkill.OnSkill3MoveEnd();
        }


        // 1,2스킬 공격이 완전히 종료되는 프레임에 호출되어 공격 상태 해제와 이동 복구를 처리한다
        public void OnSkillAttackEnd()
        {
            Debug.Log("스킬어택 앤드 호출 성공!");          
    
            if (state == null) return;
            // 상태 컨테이너가 없으면 처리하지 않는다

            state.IsAttacking = false;
            // 스킬 공격 상태를 해제한다

            state.IsInputMovementLocked = false;
            // 입력 기반 이동을 다시 허용한다

            if (meleeSkill != null) meleeSkill.OnSkillAttackEnd();
            // 스킬 종료 후 펄스/후처리 실행
        }

        // 스킬 1 데미지 이벤트 중계
        public void OnSkill1Damage()
        {
            if (meleeSkill != null)
                meleeSkill.OnSkill1Damage();
        }

        // 스킬 2 데미지 이벤트 중계
        public void OnSkill2Damage()
        {
            if (meleeSkill != null)
                meleeSkill.OnSkill2Damage();
        }

        // 스킬 3 데미지 이벤트 중계
        public void OnSkill3Damage()
        {
            if (meleeSkill != null)
                meleeSkill.OnSkill3Damage();
        }
        #endregion
    }
}
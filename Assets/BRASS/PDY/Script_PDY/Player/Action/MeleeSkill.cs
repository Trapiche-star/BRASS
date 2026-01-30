using UnityEngine;

namespace BRASS
{
    /// <summary>
    /// 플레이어의 근접 스킬 입력을 처리하고 애니메이션 이벤트에 맞춰 공격 판정을 수행하는 클래스
    /// </summary>
    public class MeleeSkill : MonoBehaviour
    {
        #region Variables
        [Header("References")]
        [SerializeField] private PlayerState state; // 플레이어의 현재 상태 데이터 참조
        [SerializeField] private PlayerAnimationController animController; // 애니메이션 재생 및 제어 담당
        private Animator animator; // 애니메이션 파라미터 전달을 위한 컴포넌트

        [Header("Skill 3 Movement")]
        [SerializeField] private float skill3Speed = 12f; // 스킬 3 사용 시 전진하는 속도 수치
        private bool isSkill3Moving; // 현재 스킬 3에 의한 강제 이동 상태 여부

        [Header("Skill Damage - Bare Hand")]
        [SerializeField] private float bareSkill1Damage = 15f; // 맨손 상태 스킬 1 데미지
        [SerializeField] private float bareSkill2Damage = 20f; // 맨손 상태 스킬 2 데미지
        [SerializeField] private float bareSkill3Damage = 25f; // 맨손 상태 스킬 3 데미지

        [Header("Skill Damage - BattleAxe")]
        [SerializeField] private float axeSkill1Damage = 40f; // 도끼 장착 상태 스킬 1 데미지
        [SerializeField] private float axeSkill2Damage = 60f; // 도끼 장착 상태 스킬 2 데미지
        [SerializeField] private float axeSkill3Damage = 80f; // 도끼 장착 상태 스킬 3 데미지

        [Header("Skill Hit")]
        [SerializeField] private float skillRange = 2.5f; // 공격 판정을 시도할 구체 범위 반경
        [SerializeField] private LayerMask damageLayer; // 데미지 계산에 포함할 레이어 마스크
        #endregion

        #region Unity Event Method
        private void Awake()
        {
            if (state == null) state = GetComponentInParent<PlayerState>(); // 부모 객체에서 상태 스크립트를 찾아 할당
            if (animController == null) animController = GetComponentInParent<PlayerAnimationController>(); // 부모 객체에서 애니메이션 컨트롤러 탐색
            animator = animController.GetComponentInChildren<Animator>(); // 하위 오브젝트의 실질적인 애니메이터 컴포넌트 참조
        }

        private void Update()
        {
            if (!isSkill3Moving) return; // 이동 플래그가 거짓일 경우 이후 이동 로직을 실행하지 않는다

            CharacterController cc = GetComponentInParent<CharacterController>(); // 이동 처리를 위해 부모의 캐릭터 컨트롤러를 가져온다
            if (cc == null) return; // 컨트롤러가 존재하지 않으면 이동 처리를 중단한다

            Vector3 dir = transform.root.forward; // 최상단 루트의 전방 방향을 기준으로 설정
            dir.y = 0f; // 수직 이동을 방지하기 위해 Y축 값을 제거
            dir.Normalize(); // 일정한 속도 유지를 위해 방향 벡터를 단위화

            cc.Move(dir * skill3Speed * Time.deltaTime); // 계산된 방향과 속도로 캐릭터를 물리 이동시킨다
        }
        #endregion

        #region Custom Method
        // 근접 스킬 1 애니메이션 재생 시도
        public void ExecuteSkill01()
        {
            if (!CanUseSkill()) return; // 사용 불가능 상태라면 실행하지 않고 메서드를 종료한다
            animator.SetTrigger("Skill_1"); // 애니메이터의 스킬 1 트리거를 활성화한다
        }

        // 근접 스킬 2 애니메이션 재생 시도
        public void ExecuteSkill02()
        {
            if (!CanUseSkill()) return; // 사용 조건 미충족 시 메서드 흐름을 차단한다
            animator.SetTrigger("Skill_2"); // 애니메이터의 스킬 2 트리거를 활성화한다
        }

        // 근접 스킬 3 애니메이션 재생 시도
        public void ExecuteSkill03()
        {
            if (!CanUseSkill()) return; // 현재 공격 중이거나 공중에 있다면 실행을 취소한다
            animator.SetTrigger("Skill_3"); // 애니메이터의 스킬 3 트리거를 활성화한다
        }

        // 애니메이션 이벤트: 스킬 3 이동 물리 시작
        public void OnSkill3MoveStart()
        {
            if (state == null || !state.IsAttacking) return; // 상태가 비정상적이거나 공격 중이 아니면 무시한다
            isSkill3Moving = true; // Update에서 이동 로직이 작동하도록 플래그를 참으로 설정한다
        }

        // 애니메이션 이벤트: 스킬 3 이동 물리 종료
        public void OnSkill3MoveEnd()
        {
            isSkill3Moving = false; // 이동 플래그를 해제하여 강제 전진을 멈춘다
        }

        // 애니메이션 이벤트: 공격 액션의 시작 시점 처리
        public void OnSkillAttackStart()
        {
            // 공격 개시 시 필요한 초기화나 효과 처리를 위한 지점
        }

        // 애니메이션 이벤트: 공격 액션의 전체 종료 시점 처리
        public void OnSkillAttackEnd()
        {
            // 공격 시퀀스가 끝난 후 상태를 복구하거나 이펙트를 제거하는 지점
        }

        // 애니메이션 이벤트: 스킬 1 데미지 연산 호출
        public void OnSkill1Damage()
        {
            DealSkillDamage(GetSkillDamage(1)); // 1번 스킬의 데미지 수치를 계산하여 판정을 실행한다
        }

        // 애니메이션 이벤트: 스킬 2 데미지 연산 호출
        public void OnSkill2Damage()
        {
            DealSkillDamage(GetSkillDamage(2)); // 2번 스킬의 데미지 수치를 계산하여 판정을 실행한다
        }

        // 애니메이션 이벤트: 스킬 3 데미지 연산 호출
        public void OnSkill3Damage()
        {
            DealSkillDamage(GetSkillDamage(3)); // 3번 스킬의 데미지 수치를 계산하여 판정을 실행한다
        }

        // 무기 장착 여부에 따라 해당 스킬의 데미지 수치 결정
        private float GetSkillDamage(int skillIndex)
        {
            bool hasAxe = state != null && state.IsBattleAxeEquipped; // 도끼 장착 여부를 상태 데이터에서 확인

            if (hasAxe) // 도끼를 장착한 상태일 경우
            {
                return skillIndex switch // 스킬 인덱스에 따라 도끼 전용 데미지 값을 반환한다
                {
                    1 => axeSkill1Damage,
                    2 => axeSkill2Damage,
                    3 => axeSkill3Damage,
                    _ => 0f
                };
            }

            return skillIndex switch // 도끼 미장착 시 맨손 데미지 수치를 반환한다
            {
                1 => bareSkill1Damage,
                2 => bareSkill2Damage,
                3 => bareSkill3Damage,
                _ => 0f
            };
        }

        // 구체 범위를 감지하여 대상에게 데미지 전달
        private void DealSkillDamage(float damage)
        {
            Vector3 origin = transform.root.position + transform.root.forward * 1.2f; // 플레이어 앞쪽으로 판정 중심점 계산
            Collider[] hits = Physics.OverlapSphere(origin, skillRange, damageLayer); // 특정 반경 내 대상 레이어의 콜라이더들을 수집한다

            foreach (Collider hit in hits) // 감지된 모든 콜라이더를 순회하며 처리한다
            {
                if (hit.transform.root == transform.root) continue; // 감지된 대상이 자기 자신이라면 반복문의 다음 단계로 건너뛴다

                IDamageable target = hit.GetComponentInParent<IDamageable>(); // 대상으로부터 데미지 인터페이스를 추출한다
                if (target == null) continue; // 인터페이스가 존재하지 않는 대상이라면 무시하고 다음으로 넘어간다

                target.TakeDamage(damage); // 인터페이스를 통해 최종 데미지를 전달한다
            }
        }

        // 스킬 사용이 가능한 논리적 상태인지 확인
        private bool CanUseSkill()
        {
            return !state.IsAttacking && state.IsGrounded; // 공격 중이 아니고 땅에 딛고 있는 상태에서만 참을 반환한다
        }
        #endregion
    }
}
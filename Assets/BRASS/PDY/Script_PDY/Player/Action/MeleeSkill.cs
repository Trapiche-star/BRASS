using UnityEngine;
using System.Collections;

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

        [Header("Attack Hit Area")]
        [SerializeField] private float attackHitDistance = 2.5f; // 공격 판정 최대 거리
        [SerializeField] private float attackHitAngle = 45f;     // 공격 판정 허용 각도

        [Header("Auto Approach - Skill")]
        [SerializeField] private PlayerController playerController; // 자동 접근 호출용

        private bool skill1Pending;
        private bool skill2Pending;
        private bool skill3Pending;
        #endregion

        #region Unity Event Method
        private void Awake()
        {
            // 자동 참조 할당
            if (state == null) state = GetComponentInParent<PlayerState>();                                          // 부모 객체에서 상태 스크립트를 찾아 할당
            if (animController == null) animController = GetComponentInParent<PlayerAnimationController>();          // 부모 객체에서 애니메이션 컨트롤러 탐색            
            if (playerController == null) playerController = GetComponentInParent<PlayerController>();               // 부모 객체에서 플레이어 컨트롤러 탐색

            animator = animController.GetComponentInChildren<Animator>();    // 하위 오브젝트의 실질적인 애니메이터 컴포넌트 참조
        }

        private void Update()
        {
            UpdateSkill3Movement();
            TryExecutePendingSkills();

            /*if (!isSkill3Moving) return; // 이동 플래그가 거짓일 경우 이후 이동 로직을 실행하지 않는다

            CharacterController cc = GetComponentInParent<CharacterController>(); // 이동 처리를 위해 부모의 캐릭터 컨트롤러를 가져온다
            if (cc == null) return; // 컨트롤러가 존재하지 않으면 이동 처리를 중단한다

            Vector3 dir = transform.root.forward; // 최상단 루트의 전방 방향을 기준으로 설정
            dir.y = 0f; // 수직 이동을 방지하기 위해 Y축 값을 제거
            dir.Normalize(); // 일정한 속도 유지를 위해 방향 벡터를 단위화

            cc.Move(dir * skill3Speed * Time.deltaTime); // 계산된 방향과 속도로 캐릭터를 물리 이동시킨다*/
        }

        // 스킬 3 사용 중 강제 전진 이동 처리
        private void UpdateSkill3Movement() 
        {
            if (!isSkill3Moving)
                return;

            CharacterController cc = GetComponentInParent<CharacterController>();
            if (cc == null)
                return;

            Vector3 dir = transform.root.forward;
            dir.y = 0f;
            dir.Normalize();

            cc.Move(dir * skill3Speed * Time.deltaTime);
        }

        // 대기 중인 스킬이 있다면 적중 판정 범위에 들어왔을 때 실행 시도
        private void TryExecutePendingSkills()
        {
            if (state == null)
                return;

            if (state.CurrentTarget == null)
            {
                skill1Pending = false;
                skill2Pending = false;
                skill3Pending = false;
                return;
            }

            if (!IsInAttackHitArea(state.CurrentTarget.position))
                return;

            if (skill1Pending)
            {
                skill1Pending = false;

                state.IsAttacking = true;
                state.IsInputMovementLocked = true;

                FaceTargetIfExists();
                animator.SetTrigger("Skill_1");
                return;
            }

            if (skill2Pending)
            {
                skill2Pending = false;

                state.IsAttacking = true;
                state.IsInputMovementLocked = true;

                FaceTargetIfExists();
                animator.SetTrigger("Skill_2");
                return;
            }

            if (skill3Pending)
            {
                skill3Pending = false;

                state.IsAttacking = true;
                state.IsInputMovementLocked = true;

                FaceTargetIfExists();
                animator.SetTrigger("Skill_3");
                return;
            }
        }
        #endregion

        #region Custom Method        

        // 근접 스킬 1 애니메이션 재생 시도
        public void ExecuteSkill01()
        {
            if (!CanUseSkill()) return;

            if (state != null && state.CurrentTarget != null)
            {
                if (!IsInAttackHitArea(state.CurrentTarget.position))
                {
                    skill1Pending = true;

                    if (playerController != null)
                        playerController.StartAutoApproach(state.CurrentTarget, attackHitDistance);

                    return;
                }
            }

            FaceTargetIfExists();

            state.IsAttacking = true;          // 공격 상태 진입
            state.IsInputMovementLocked = true; // 이동 잠금

            animator.SetTrigger("Skill_1");

            /*if (!CanUseSkill()) return; // 사용 불가능 상태라면 실행하지 않고 메서드를 종료한다

            FaceTargetIfExists();
            animator.SetTrigger("Skill_1"); // 애니메이터의 스킬 1 트리거를 활성화한다*/
        }

        // 근접 스킬 2 애니메이션 재생 시도
        public void ExecuteSkill02()
        {
            if (!CanUseSkill()) return;

            if (state != null && state.CurrentTarget != null)
            {
                if (!IsInAttackHitArea(state.CurrentTarget.position))
                {
                    skill2Pending = true;

                    if (playerController != null)
                        playerController.StartAutoApproach(state.CurrentTarget, attackHitDistance);

                    return;
                }
            }

            FaceTargetIfExists();

            state.IsAttacking = true;          // 공격 상태 진입
            state.IsInputMovementLocked = true; // 이동 잠금

            animator.SetTrigger("Skill_2");
            /*if (!CanUseSkill()) return; // 사용 조건 미충족 시 메서드 흐름을 차단한다

            FaceTargetIfExists();
            animator.SetTrigger("Skill_2"); // 애니메이터의 스킬 2 트리거를 활성화한다*/
        }

        // 근접 스킬 3 애니메이션 재생 시도
        public void ExecuteSkill03()
        {
            if (!CanUseSkill()) return;

            if (state != null && state.CurrentTarget != null)
            {
                if (!IsInAttackHitArea(state.CurrentTarget.position))
                {
                    skill3Pending = true;

                    if (playerController != null)
                        playerController.StartAutoApproach(state.CurrentTarget, attackHitDistance);

                    return;
                }
            }

            FaceTargetIfExists();

            state.IsAttacking = true;          // 공격 상태 진입
            state.IsInputMovementLocked = true; // 이동 잠금

            animator.SetTrigger("Skill_3");
        }

        // 애니메이션 이벤트: 스킬 3 이동 물리 시작
        public void OnSkill3MoveStart()
        {
            if (state == null || !state.IsAttacking)
                return;

            isSkill3Moving = true;
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
            if (state == null)
                return;

            state.IsAttacking = false;      //공격 종료
            state.IsInputMovementLocked = false;
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
        // 공격 시작 시 타겟이 있다면 해당 방향으로 캐릭터를 회전시킨다
        private void FaceTargetIfExists()
        {
            if (state == null || state.CurrentTarget == null) return;

            Vector3 dir =
                state.CurrentTarget.position - transform.root.position;
            dir.y = 0f;

            if (dir.sqrMagnitude < 0.01f) return;

            transform.root.rotation =
                Quaternion.LookRotation(dir.normalized);
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
            Vector3 origin = transform.root.position;

            // 지정된 범위 내의 충돌체를 모두 감지한다
            Collider[] hits =
                Physics.OverlapSphere(origin, skillRange, damageLayer);

            foreach (Collider hit in hits)
            {
                // 자기 자신 제외
                if (hit.transform.root == transform.root)
                    continue;

                // 공격 판정 기즈모 영역 검사 (여기가 맞는 위치)
                if (!IsInAttackHitArea(hit.transform.position))
                    continue;

                IDamageable target =
                    hit.GetComponentInParent<IDamageable>();

                if (target == null)
                    continue;

                target.TakeDamage(damage);

                // 히트 스톱
                StartCoroutine(HitStop(0.06f));
            }
        }

        // 스킬 사용이 가능한 논리적 상태인지 확인
        private bool CanUseSkill()
        {
            return !state.IsAttacking && state.IsGrounded; // 공격 중이 아니고 땅에 딛고 있는 상태에서만 참을 반환한다
        }

        // 스킬 적중 시 짧은 멈칫(히트 스톱)을 발생시킨다
        private IEnumerator HitStop(float duration)
        {
            if (state == null)
                yield break;

            state.IsInputMovementLocked = true;
            state.IsMoving = false;

            yield return new WaitForSeconds(duration);  // 지정된 시간 동안 대기

            state.IsInputMovementLocked = false;
        }

        // 특정 월드 좌표가 근접 공격 판정 영역 안에 있는지 검사
        private bool IsInAttackHitArea(Vector3 worldPos)
        {
            Vector3 toTarget = worldPos - transform.root.position;
            toTarget.y = 0f;

            // 거리 체크
            if (toTarget.magnitude > attackHitDistance)
                return false;

            // 각도 체크 (플레이어 전방 기준)
            float angle =
                Vector3.Angle(transform.root.forward, toTarget);

            return angle <= attackHitAngle;
        }

        // 공격 적중 판정 영역을 씬 뷰에 시각화한다
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;

            Transform root = transform.root;
            if (root == null) return;

            Vector3 origin = root.position;

            Vector3 left =
                Quaternion.Euler(0, -attackHitAngle, 0) * root.forward;
            Vector3 right =
                Quaternion.Euler(0, attackHitAngle, 0) * root.forward;

            Gizmos.DrawLine(origin, origin + left * attackHitDistance);
            Gizmos.DrawLine(origin, origin + right * attackHitDistance);
            Gizmos.DrawWireSphere(origin, attackHitDistance);
        }

        // 외부에서 공격 상태를 강제 종료할 때 호출
        public void ForceEndAttack()
        {
            isSkill3Moving = false; // 스킬 3 이동 중단

            CancelPendingSkills();  // 대기 중인 스킬 요청 취소

            if (state != null)
            {
                state.IsAttacking = false;
                state.IsInputMovementLocked = false;
            }

            // 혹시 실행 중일지 모르는 코루틴 중단
            StopAllCoroutines();

            Debug.Log("[MeleeSkill] 점프로 인해 공격 상태가 강제 해제되었습니다.");
        }

        // 대기 중인 스킬 사용 요청을 모두 취소
        public void CancelPendingSkills()
        {
            skill1Pending = false;
            skill2Pending = false;
            skill3Pending = false;

            isSkill3Moving = false;
        }
        #endregion
    }
}
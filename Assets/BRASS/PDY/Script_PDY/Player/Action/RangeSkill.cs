using UnityEngine;

namespace BRASS
{
    /// <summary>
    /// 원거리 무기 전용 스킬 클래스
    /// 1: 연사 / 2: 단발 / 3: 연사(다른 속도)
    /// </summary>
    public class RangeSkill : MonoBehaviour
    {
        #region Variables

        [Header("References")]
        [SerializeField] private PlayerState state; // 플레이어 상태
        [SerializeField] private PlayerAnimationController anim; // 애니메이션 제어
        [SerializeField] private WeaponHandler weaponHandler; // 무기 핸들러

        [Header("Skill 1 - Auto Fire")]
        [SerializeField] private float skill1Damage = 8f;
        [SerializeField] private float skill1BulletSpeed = 30f;
        [SerializeField] private float skill1FireRate = 0.15f; // 1번 연사 

        [Header("Skill 2 - Single Shot")]
        [SerializeField] private float skill2Damage = 10f;
        [SerializeField] private float skill2BulletSpeed = 35f;
        [SerializeField] private float skill2Cooldown = 0.2f;  // 2번 단발 쿨

        [Header("Skill 3 - Single Shot (Strong)")]
        [SerializeField] private float skill3Damage = 18f;
        [SerializeField] private float skill3BulletSpeed = 28f;
        [SerializeField] private float skill3Cooldown = 0.8f;

        private bool isFiring;          // 연사 유지 여부
        private float nextFireTime;     // 다음 발사 가능 시점
        private float currentFireRate;  // 현재 연사 속도
        private float currentSingleDamage;   // 현재 단발 데미지
        private float currentSingleSpeed;   // 현재 단발 탄속
        #endregion

        #region Unity Event Method

        private void Awake()
        {
            if (state == null)
                state = GetComponentInParent<PlayerState>();

            if (anim == null)
                anim = GetComponentInParent<PlayerAnimationController>();

            if (weaponHandler == null)
                weaponHandler = GetComponentInParent<WeaponHandler>();
        }

        private void Update()
        {
            UpdateAutoFire();
        }

        #endregion

        #region Custom Method

        // 1번 스킬: 연사
        public void ExecuteSkill01()
        {
            BeginFire(skill1FireRate);
        }

        // 연사 시작
        private void BeginFire(float fireRate)
        {
            // 조건 검사
            if (!CanFire()) return;

            // 연사 상태 설정
            currentFireRate = fireRate;
            isFiring = true;

            // 공격 상태 진입
            state.IsAttacking = true;
            state.IsInputMovementLocked = true;

            // 타겟이 있으면 회전
            FaceTargetIfExists();
            anim.SetIsShooting(true);

            // 즉시 1발 발사
            FireBullet(skill1Damage, skill1BulletSpeed); // 첫 발은 즉시 발사
            nextFireTime = Time.time + currentFireRate; //  다음 발사 시점 설정
        }        

        // 연사 처리
        private void UpdateAutoFire()
        {
            if (!isFiring) return;  // 연사 중이 아니면 무시
            if (Time.time < nextFireTime) return;   // 발사 가능 시점이 아니면 무시

            FireBullet(skill1Damage, skill1BulletSpeed);       // 총알 발사
            nextFireTime = Time.time + currentFireRate;        // 다음 발사 시점 갱신
        }

        // 연사 중단 (입력 해제 시 호출)
        public void StopFire()
        {
            if (!isFiring) return;      // 연사 중이 아니면 무시

            isFiring = false;       // 연사 상태 해제 

            state.IsAttacking = false;  // 공격 상태 해제
            state.IsInputMovementLocked = false;    //  이동 잠금 해제

            anim.SetIsShooting(false);  // 연사 애니메이션 종료
        }

        // 2번 스킬: 단발
        public void ExecuteSkill02()
        {
            if (!CanFire()) return;

            currentSingleDamage = skill2Damage;
            currentSingleSpeed = skill2BulletSpeed;

            state.IsAttacking = true;
            state.IsInputMovementLocked = true;

            FaceTargetIfExists();
            anim.PlaySkill2Fire();

            nextFireTime = Time.time + skill2Cooldown;
        }

        // 3번 스킬: 강한 단발
        public void ExecuteSkill03()
        {
            if (!CanFire()) return;

            currentSingleDamage = skill3Damage;
            currentSingleSpeed = skill3BulletSpeed;

            state.IsAttacking = true;
            state.IsInputMovementLocked = true;

            FaceTargetIfExists();
            anim.PlaySkill3Fire();

            nextFireTime = Time.time + skill3Cooldown;
        } 
        
        // 타겟의 조준 지점 반환 (Collider 중심 우선)
        private Vector3 GetTargetAimPoint(Transform target)
        {
            // Collider가 있으면 중심을 조준
            Collider col = target.GetComponentInChildren<Collider>();
            if (col != null)
                return col.bounds.center;

            // 없으면 Transform 위치
            return target.position;
        }

        // 발사 가능 여부 검사
        private bool CanFire()
        {
            if (state == null) return false;
            if (!state.IsGunEquipped) return false;

            // 연사 중이면 허용
            if (isFiring) return true;

            // 다른 공격 중이면 차단
            return !state.IsAttacking;
        }

        // 타겟이 있으면 타겟 방향으로 회전

        private void FaceTargetIfExists()
        {
            if (state.CurrentTarget == null) return;

            Vector3 dir = state.CurrentTarget.position - transform.root.position;
            dir.y = 0f;

            if (dir.sqrMagnitude < 0.01f) return;
            transform.root.rotation = Quaternion.LookRotation(dir.normalized);
        }

        // 실제 총알 발사 처리
        private void FireBullet(float damage, float speed)
        {
            Transform firePoint = weaponHandler.GetFirePoint();
            if (firePoint == null) return;

            WeaponData weapon = weaponHandler.CurrentWeapon;
            if (weapon == null || weapon.bulletPrefab == null) return;

            Vector3 dir = firePoint.forward;

            if (state.CurrentTarget != null)
            {
                Vector3 targetPos = GetTargetAimPoint(state.CurrentTarget);
                dir = (targetPos - firePoint.position).normalized;
            }

            GameObject bulletObj = Instantiate(
                weapon.bulletPrefab,
                firePoint.position,
                Quaternion.LookRotation(dir)
            );

            Bullet bullet = bulletObj.GetComponent<Bullet>();
            if (bullet != null)
                bullet.Init(dir, speed, damage);
        }

        // 단발 발사 애니메이션 이벤트에서 호출
        public void OnSingleShotFire()
        {
            FireBullet(currentSingleDamage, currentSingleSpeed);
        }

        //  단발 발사 애니메이션 종료 이벤트에서 호출
        public void OnSingleShotEnd()
        {
            Debug.Log("Skill single shot end");
            state.IsAttacking = false;
            state.IsInputMovementLocked = false;
        }

        /*뷸렛 발사 수정전
        private void FireOnce(float damage, float speed)
        {
            Transform firePoint = weaponHandler.GetFirePoint();
            if (firePoint == null) return;

            WeaponData weapon = weaponHandler.CurrentWeapon;
            if (weapon == null || weapon.bulletPrefab == null) return;

            Vector3 dir = firePoint.forward;

            if (state.CurrentTarget != null)
            {
                // 타겟 조준 지점 (Collider 중심 기준)
                Vector3 targetPos = GetTargetAimPoint(state.CurrentTarget);
                dir = (targetPos - firePoint.position).normalized;
            }

            GameObject bulletObj = Instantiate(
                weapon.bulletPrefab,
                firePoint.position,
                Quaternion.LookRotation(dir)
            );

            Bullet bullet = bulletObj.GetComponent<Bullet>();
            if (bullet != null)
            {
                bullet.Init(dir, speed, damage);
            }
        }*/

        #endregion
    }
}

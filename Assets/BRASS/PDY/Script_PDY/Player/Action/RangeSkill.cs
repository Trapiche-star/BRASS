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

        [Header("Skill Settings")]
        [SerializeField] private float skill1FireRate = 0.15f; // 1번 연사
        [SerializeField] private float skill2Cooldown = 0.4f;  // 2번 단발 쿨
        [SerializeField] private float skill3FireRate = 0.35f; // 3번 연사

        private bool isFiring;          // 연사 유지 여부
        private float nextFireTime;     // 다음 발사 가능 시점
        private float currentFireRate;  // 현재 연사 속도

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

        // 2번 스킬: 단발
        public void ExecuteSkill02()
        {
            if (!CanFire()) return;
            FireOnce(skill1Damage, skill1BulletSpeed);
            nextFireTime = Time.time + skill2Cooldown;
        }

        // 3번 스킬: 느린 연사
        public void ExecuteSkill03()
        {
            BeginFire(skill3FireRate);
        }

        // 연사 시작
        private void BeginFire(float fireRate)
        {
            if (!CanFire()) return;

            currentFireRate = fireRate;
            isFiring = true;

            state.IsAttacking = true;
            state.IsInputMovementLocked = true;

            FaceTargetIfExists();
            anim.SetIsShooting(true);
        }

        // 연사 중단 (입력 해제 시 호출)
        public void StopFire()
        {
            isFiring = false;

            state.IsAttacking = false;
            state.IsInputMovementLocked = false;

            anim.SetIsShooting(false);
        }

        // 연사 처리
        private void UpdateAutoFire()
        {
            if (!isFiring) return;            

            if (Time.time < nextFireTime) return;

            FireOnce(skill1Damage, skill1BulletSpeed);
            nextFireTime = Time.time + currentFireRate;
        }        

        // 실제 발사 처리
        private void FireOnce(float damage, float speed)
        {
            Transform firePoint = weaponHandler.GetFirePoint();
            if (firePoint == null) return;

            WeaponData weapon = weaponHandler.CurrentWeapon;
            if (weapon == null || weapon.bulletPrefab == null) return;

            Vector3 dir = firePoint.forward;

            if (state.CurrentTarget != null)
            {
                // 🎯 타겟 조준 지점 (Collider 중심 기준)
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

        private bool CanFire()
        {
            if (state == null) return false;
            if (!state.IsGunEquipped) return false;

            // 연사 중이면 허용
            if (isFiring) return true;

            // 다른 공격 중이면 차단
            return !state.IsAttacking;
        }

        private void FaceTargetIfExists()
        {
            if (state.CurrentTarget == null) return;

            Vector3 dir = state.CurrentTarget.position - transform.root.position;
            dir.y = 0f;

            if (dir.sqrMagnitude < 0.01f) return;
            transform.root.rotation = Quaternion.LookRotation(dir.normalized);
        }

        #endregion
    }
}

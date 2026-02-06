using UnityEngine;

namespace BRASS
{
    /// 원거리 무기의 기본 공격 실행을 담당하는 클래스
    public class RangeCombat : MonoBehaviour
    {
        #region Variables

        [SerializeField] private PlayerState state;                 // 플레이어 상태 참조
        [SerializeField] private PlayerAnimationController anim;    // 애니메이션 제어기
        [SerializeField] private GameObject harpoonBulletPrefab;     // 발사할 탄환 프리팹  
        [SerializeField] private WeaponHandler weaponHandler; // 무기 핸들러
            
        [SerializeField] private float bulletDamage = 15f; // 단발 고정 데미지
        [SerializeField] private float bulletSpeed = 22f;  // 단발 탄속 (연사보다 느림)
        [SerializeField] private float fireCooldown = 0.2f;         // 연사 간격
        private float lastFireTime; // 마지막 발사 시점

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

        #endregion

        #region Custom Method

        // 발사 시도
        public void TryFire()
        {
            if (state == null || !state.IsGunEquipped)
                return;

            if (Time.time < lastFireTime + fireCooldown)
                return;

            lastFireTime = Time.time;

            // 공격 상태 진입
            state.IsAttacking = true;
            state.IsInputMovementLocked = true;

            FaceTargetIfExists(); // 타겟이 있으면 발사 전에 회전

            // Gun Layer의 Fire 트리거 실행
            anim?.PlayGunFire();
        }


        // 애니메이션 이벤트에서 호출되는 실제 발사
        public void Fire()
        {
            if (weaponHandler == null || harpoonBulletPrefab == null)
                return;

            Transform firePoint = weaponHandler.GetFirePoint();
            if (firePoint == null)
                return;

            Debug.Log($"FirePoint.forward = {firePoint.forward}");

            Vector3 dir;

            if (state != null && state.CurrentTarget != null)
            {
                Vector3 targetPos = GetTargetAimPoint(state.CurrentTarget);
                dir = (targetPos - firePoint.position);
            }
            else
            {
                dir = firePoint.forward;
            }

            dir = Vector3.ProjectOnPlane(dir, Vector3.up).normalized;

            //firePoint.rotation = Quaternion.LookRotation(dir);

            GameObject bulletObj = Instantiate(
                harpoonBulletPrefab,
                firePoint.position,
                Quaternion.LookRotation(dir)
            );

            Bullet bullet = bulletObj.GetComponent<Bullet>();
            if (bullet != null)
            {
                // 새 Init 시그니처에 맞게 수정
                bullet.Init(dir, bulletSpeed, bulletDamage);
            }

            Debug.Log("HarpoonGun_SingleShot fired");
        }

        // 타겟 조준 지점 반환 (Collider 중심 우선)
        private Vector3 GetTargetAimPoint(Transform target)
        {
            Collider col = target.GetComponentInChildren<Collider>();
            if (col != null)
                return col.bounds.center;

            return target.position;
        }

        // 발사 애니메이션 종료 시 호출
        public void OnFireEnd()
        {
            state.IsAttacking = false;
            state.IsInputMovementLocked = false;
        }

        // 타겟이 존재하면 즉시 해당 방향을 바라보게 한다
        private void FaceTargetIfExists()
        {
            if (state == null || state.CurrentTarget == null)
                return;

            Vector3 dir = state.CurrentTarget.position - transform.root.position;
            dir.y = 0f;

            if (dir.sqrMagnitude < 0.01f)
                return;

            transform.root.rotation = Quaternion.LookRotation(dir.normalized);
        }


        #endregion
    }
}

using UnityEngine;
using System.Collections.Generic;

namespace BRASS
{
    public class WeaponDamage : MonoBehaviour
    {
        [SerializeField] private float damage = 20f;
        private Collider _weaponCollider;

        // 한 번 휘두를 때 여러 번 데미지 입는 걸 방지하는 리스트
        private List<IDamageable> _hitTargets = new List<IDamageable>();

        void Awake()
        {
            _weaponCollider = GetComponent<Collider>();
            // 시작할 때는 공격 판정을 꺼둡니다.
            StopAttack();
        }

        // [애니메이션 이벤트용] 공격 휘두르기 시작할 때 호출
        public void StartAttack()
        {
            _hitTargets.Clear();      // 맞았던 대상 목록 초기화
            _weaponCollider.enabled = true; // 콜라이더 켜기
        }

        // [애니메이션 이벤트용] 공격 휘두르기가 끝날 때 호출
        public void StopAttack()
        {
            _weaponCollider.enabled = false; // 콜라이더 끄기
        }

        private void OnTriggerEnter(Collider other)
        {
            // 자해 방지: 부딪힌 대상이 무기를 들고 있는 나 자신(최상위 부모)이라면 무시합니다.
            if (other.transform.root == transform.root)
            {
                return;
            }

            IDamageable target = other.GetComponent<IDamageable>();

            // 대미지를 줄 수 있는 대상이고, 이번 휘두르기에서 아직 안 맞았다면
            if (target != null && !_hitTargets.Contains(target))
            {
                target.TakeDamage(damage);
                _hitTargets.Add(target); // 맞은 목록에 추가
                Debug.Log($"{other.name}에게 데미지를 줌!");
            }
        }
    }
}
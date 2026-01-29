using UnityEngine;

namespace BRASS
{
    public class EnemyHealth : MonoBehaviour, IDamageable
    {
        [SerializeField] private float hp = 50f;

        public void TakeDamage(float damageAmount)
        {
            if (hp <= 0) return;

            hp -= damageAmount;
            Debug.Log($"[Enemy] 적이 {damageAmount} 대미지를 입음. 남은 피: {hp}");

            if (hp <= 0)
            {
                Die();
            }
        }

        private void Die()
        {
            Debug.Log("적이 처치되었습니다.");
            Destroy(gameObject); // 적은 그냥 파괴
        }
    }
}
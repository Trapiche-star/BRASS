using UnityEngine;

public class RobotBossHealth : MonoBehaviour, IDamageable
{
    [SerializeField] private float maxHp = 1000.0f;
    private float currentHealth;
    private bool isDead = false;

    public float Hp => currentHealth;

    private void Awake()
    {
        currentHealth = maxHp;
    }

    public void TakeDamage(float damageAmount)
    {
        if (isDead) return;

        currentHealth = Mathf.Clamp(currentHealth - damageAmount, 0f, maxHp);

        Debug.Log($"보스 데미지 입음! 남은 체력: {currentHealth}");

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    private void Die()
    {
        isDead = true;
        Debug.Log("보스가 파괴되었습니다!");
        // 여기에 폭발 이펙트나 애니메이션 실행 코드 추가
    }
}
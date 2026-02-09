using BRASS;
using UnityEngine;

public class RobotBossHealth : MonoBehaviour, IDamageable
{
    private bool isDead = false;
    [SerializeField]
    private float maxHp = 1000.0f;

    private float currentHealth;
    public float Hp
    {
        get => currentHealth;
        private set
        {
            currentHealth = Mathf.Clamp(value, 0f, maxHp);

            if (currentHealth <= 0f && !isDead)
            {
                Die();
            }
        }
    }
    
    public void TakeDamage(float damageAmount)
    {
        Hp -= damageAmount;
    }

    void Die()
    {
        isDead = true;
    }
}

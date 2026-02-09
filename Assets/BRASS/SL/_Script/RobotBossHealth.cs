using BRASS;
using UnityEngine;

public class RobotBossHealth : MonoBehaviour, IDamageable
{
    private bool isDead = false;
    [SerializeField]
    private float maxHp = 1000.0f;

    // [추가] 연결할 UI 스크립트 참조
    [SerializeField] private TargetHealthBar healthBarUI;
    [SerializeField] private string bossName = "Robot Boss";


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


    void Awake()
    {
        // 시작 체력 설정
        currentHealth = maxHp;
    }

    void Start()
    {
        // [추가] 시작하자마자 UI에 보스 이름과 체력 정보 세팅
        if (healthBarUI != null)
        {
            healthBarUI.SetTarget(bossName, maxHp, currentHealth);
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

/*
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
*/
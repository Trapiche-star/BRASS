using UnityEngine;

namespace BRASS
{
<<<<<<< HEAD
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
=======
    public class RobotBossHealth : MonoBehaviour, IDamageable
>>>>>>> 70f14c16f79d1f1ec83b08eab92cc8934f412ce5
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
<<<<<<< HEAD
}
*/
=======
}
>>>>>>> 70f14c16f79d1f1ec83b08eab92cc8934f412ce5

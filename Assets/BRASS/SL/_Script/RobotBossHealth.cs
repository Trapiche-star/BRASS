using BRASS;
using UnityEngine;

public class RobotBossHealth : MonoBehaviour, IDamageable
{
    private bool isDead = false;
    [SerializeField] private float maxHp = 1000.0f;

    [Header("UI 연동")]
    [SerializeField] private TargetHealthBar healthBarUI;
    [SerializeField] private string bossName = "Robot Boss";

    private float currentHealth;
    public float Hp
    {
        get => currentHealth;
        private set
        {
            // 체력을 0과 maxHp 사이로 제한
            currentHealth = Mathf.Clamp(value, 0f, maxHp);

            // 체력이 변할 때마다 UI 업데이트
            if (healthBarUI != null)
            {
                healthBarUI.UpdateHealth(currentHealth);
            }

            if (currentHealth <= 0f && !isDead)
            {
                Die();
            }
        }
    }

    void Awake()
    {
        // 시작 시 체력을 최대치로 설정
        currentHealth = maxHp;
    }

    // 보스 오브젝트가 켜질 때 실행
    void OnEnable()
    {
        if (healthBarUI != null)
        {
            // 꺼져있던 UI를 활성화하고 정보 전달
            healthBarUI.SetTarget(bossName, maxHp, currentHealth);
        }
    }

    // [테스트용] 스페이스바를 누르면 데미지 100
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TakeDamage(100f);
        }
    }

    public void TakeDamage(float damageAmount)
    {
        Hp -= damageAmount;
    }

    void Die()
    {
        isDead = true;
        if (healthBarUI != null)
        {
            // 죽었을 때 UI 숨기기
            healthBarUI.ClearTarget();
        }
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

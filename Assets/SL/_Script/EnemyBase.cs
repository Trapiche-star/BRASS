using UnityEngine;
using UnityEngine.Events;

public enum BossState
{
    Idle,
    Chasing,
    Attacking,
    PhaseChange,
    Stun,
    Dead
}

public class EnemyBase : MonoBehaviour
{
    [Header("Basic Stats")]
    [SerializeField] protected string bossName = "Boss";
    [SerializeField] protected float maxHealth = 1000f;
    protected float currentHealth;
    [SerializeField] protected float moveSpeed = 3f;

    [Header("State Settings")]
    [SerializeField] protected BossState currentState;
    protected Transform target;

    [Header("Events")]
    public UnityEvent OnTakeDamage;
    public UnityEvent OnDead;

    protected Animator animator;
    protected bool isInvulnerable = false;

    protected virtual void Awake()
    {
        animator = GetComponent<Animator>();
        currentHealth = maxHealth;
        currentState = BossState.Idle;
    }

    protected virtual void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            target = playerObj.transform;
    }

    protected virtual void Update()
    {
        if (currentState == BossState.Dead) return;

        HandleState();
    }

    protected virtual void HandleState()
    {
        switch (currentState)
        {
            case BossState.Idle:
                LogicIdle();
                break;
            case BossState.Chasing:
                LogicChasing();
                break;
            case BossState.Attacking:
                LogicAttacking();
                break;
            case BossState.PhaseChange:
                LogicPhaseChange();
                break;
        }
    }

    protected virtual void LogicIdle() { }
    protected virtual void LogicChasing() { }
    protected virtual void LogicAttacking() { }
    protected virtual void LogicPhaseChange() { }

    public virtual void TakeDamage(float damage)
    {
        if (currentState == BossState.Dead || isInvulnerable) return;

        currentHealth -= damage;

        OnTakeDamage?.Invoke();

        if (currentHealth <= 0)
        {
            Die();
            return;
        }

        CheckPhaseChange();
    }

    protected virtual void CheckPhaseChange()
    {
    }

    protected virtual void Die()
    {
        currentState = BossState.Dead;

        if (animator != null) animator.SetTrigger("Die");

        OnDead?.Invoke();

        Destroy(gameObject, 5f);
    }

    public void ChangeState(BossState newState)
    {
        currentState = newState;
    }
}
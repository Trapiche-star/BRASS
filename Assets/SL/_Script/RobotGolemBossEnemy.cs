using UnityEngine;
using System.Collections.Generic;

public class RobotGolemBossEnemy : EnemyBase
{
    [Header("Combat Settings")]
    public float attackRange = 2f;
    public string[] attackTypes = { "Type01", "Type02", "Type03", "Type04" };

    [Header("AOE Settings")]
    public float aoeRadius = 5f;  // 광역 공격 범위
    public float aoeDamage = 20f; // 광역 공격 데미지
    public LayerMask targetLayer; // 플레이어 레이어 (Inspector에서 Player 설정)
    public GameObject aoeEffectPrefab; // 터지는 이펙트 (선택사항)

    private string lastAttackType = "";
    private float attackCooldown = 0f;

    protected override void Start()
    {
        base.Start();
        moveSpeed = 0f;
    }

    protected override void LogicIdle()
    {
        if (target != null && Vector3.Distance(transform.position, target.position) <= attackRange)
        {
            ChangeState(BossState.Attacking);
        }
    }

    protected override void LogicChasing()
    {
    }

    protected override void LogicAttacking()
    {
        if (attackCooldown > 0)
        {
            attackCooldown -= Time.deltaTime;
            if (attackCooldown <= 0)
            {
                ChangeState(BossState.Idle);
            }
            return;
        }

        List<string> candidates = new List<string>();

        for (int i = 0; i < attackTypes.Length; i++)
        {
            if (attackTypes[i] != lastAttackType)
            {
                candidates.Add(attackTypes[i]);
            }
        }

        if (candidates.Count == 0)
        {
            candidates.AddRange(attackTypes);
        }

        int randIndex = Random.Range(0, candidates.Count);
        string selectedAttack = candidates[randIndex];

        animator.SetTrigger(selectedAttack);
        lastAttackType = selectedAttack;

        attackCooldown = 2.0f;
    }

    protected override void CheckPhaseChange()
    {
        if (currentHealth <= maxHealth * 0.5f)
        {
            ChangeState(BossState.PhaseChange);
        }
    }

    public void OnAreaAttack()
    {
        if (aoeEffectPrefab != null)
        {
            Instantiate(aoeEffectPrefab, transform.position, Quaternion.identity);
        }

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, aoeRadius, targetLayer);

        foreach (Collider hit in hitColliders)
        {
            if (hit.CompareTag("Player"))
            {
                // 플레이어 스크립트의 TakeDamage 함수 호출 추가
                Debug.Log($"Player Hit! Damage: {aoeDamage}");
            }
        }
    }

    // 에디터에서 공격 범위를 눈으로 확인하기 위한 기즈모
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, aoeRadius);
    }
}
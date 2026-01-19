using UnityEngine;
using System.Collections.Generic;

public class RobotGolemBossEnemy : EnemyBase
{
    public float attackRange = 10f;
    public string[] attackTypes = { "Type01", "Type02"};

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
}
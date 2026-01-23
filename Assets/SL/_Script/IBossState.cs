using UnityEngine;

public interface IBossState
{
    void Enter();
    void Execute();
    void Exit();
}

public class BossIdleState : IBossState
{
    private RobotBoss boss;
    public BossIdleState(RobotBoss boss) => this.boss = boss;
    public void Enter() { if (boss.anim) boss.anim.SetTrigger("Idle"); }
    public void Execute()
    {
        if (boss.player && Vector3.Distance(boss.transform.position, boss.player.position) <= boss.detectionRange)
            boss.ChangeState(boss.StateTracking);
    }
    public void Exit() { }
}


// [조준 및 패턴 선택 상태]
public class BossTrackingState : IBossState
{
    private RobotBoss boss;

    public BossTrackingState(RobotBoss boss)
    {
        this.boss = boss;
    }

    public void Enter() { }

    public void Execute()
    {
        if (boss.player == null) return;

        // 회전 로직
        Vector3 direction = (boss.player.position - boss.transform.position).normalized;
        direction.y = 0;
        if (direction != Vector3.zero)
        {
            boss.transform.rotation = Quaternion.Slerp(boss.transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * boss.rotationSpeed);
        }

        // 공격 사거리 진입 시
        float dist = Vector3.Distance(boss.transform.position, boss.player.position);
        if (dist <= boss.attackRange)
        {
            float angle = Vector3.Angle(boss.transform.forward, direction);
            if (angle < 15f)
            {
                // 중복 없는 랜덤 선택 로직
                int totalPatterns = boss.attackPatterns.Length;
                int nextIndex;

                // 1. 일단 랜덤하게 뽑음
                // 2. 만약 방금 쓴 거(lastAttackIndex)랑 똑같으면 다를 때까지 계속 다시 뽑음
                do
                {
                    nextIndex = Random.Range(0, totalPatterns);
                }
                while (nextIndex == boss.lastAttackIndex && totalPatterns > 1);

                // 이번에 뽑은 번호를 기억
                boss.lastAttackIndex = nextIndex;

                // 해당 인덱스의 상태로 전환
                boss.ChangeState(boss.attackPatterns[nextIndex]);
            }
        }
        else if (dist > boss.detectionRange)
        {
            boss.ChangeState(boss.StateIdle);
        }
    }
    public void Exit() { }
}

// ---------------------------------------------------------
// 각 패턴별 상태 클래스들 (1, 2, 3, 4)
// ---------------------------------------------------------

// [패턴 1] 휩쓸기
public class BossSweepState : IBossState
{
    private RobotBoss boss;
    private float timer;
    public BossSweepState(RobotBoss boss) => this.boss = boss;
    public void Enter() { boss.anim.SetTrigger("DoSweep"); timer = 0; }
    public void Execute() { timer += Time.deltaTime; if (timer >= 2.0f) boss.ChangeState(boss.StateRecover); }
    public void Exit() { }
}

// [패턴 2] 박수
public class BossClapState : IBossState
{
    private RobotBoss boss;
    private float timer;
    public BossClapState(RobotBoss boss) => this.boss = boss;
    public void Enter() { boss.anim.SetTrigger("DoClap"); timer = 0; }
    public void Execute() { timer += Time.deltaTime; if (timer >= 2.0f) boss.ChangeState(boss.StateRecover); }
    public void Exit() { }
}

// [패턴 3] 미사일
public class BossMissileState : IBossState
{
    private RobotBoss boss;
    private float timer;
    public BossMissileState(RobotBoss boss) => this.boss = boss;
    public void Enter()
    {
        boss.anim.SetTrigger("DoMissile");
        timer = 0;
    }
    public void Execute() { timer += Time.deltaTime; if (timer >= 2.0f) boss.ChangeState(boss.StateRecover); }
    public void Exit() { }
}

// [패턴 4] 레이저
public class BossLaserState : IBossState
{
    private RobotBoss boss;
    private float timer;
    public BossLaserState(RobotBoss boss) => this.boss = boss;
    public void Enter()
    {
        boss.anim.SetTrigger("DoLaser");
        timer = 0;
    }
    public void Execute() { timer += Time.deltaTime; if (timer >= 3.0f) boss.ChangeState(boss.StateRecover); }
    public void Exit() { }
}

// [회복 상태]
public class BossRecoverState : IBossState
{
    private RobotBoss boss;
    private float timer;
    public BossRecoverState(RobotBoss boss) => this.boss = boss;
    public void Enter() { boss.anim.SetTrigger("Overheat"); timer = 0; }
    public void Execute()
    {
        timer += Time.deltaTime;
        if (timer >= boss.recoverTime) boss.ChangeState(boss.StateTracking);
    }
    public void Exit() { }
}
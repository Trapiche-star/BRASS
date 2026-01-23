using UnityEngine;

public class RobotBoss : MonoBehaviour
{
    [Header("Components")]
    public Animator anim;
    public Transform player;

    [Header("Attack Settings")]
    public Transform sweepCenter;
    public Vector3 sweepSize = new Vector3(5, 2, 3);
    public GameObject clapEffect;
    public Transform clapPoint;

    public GameObject missilePrefab;
    public Transform missilePort;
    public GameObject laserEffect;
    public Transform laserPort;

    [Header("Stats")]
    public float detectionRange = 20f;
    public float attackRange = 10f;
    public float recoverTime = 2f;
    public float rotationSpeed = 5f;

    // --- 상태 관리 ---
    private IBossState currentState;

    public IBossState StateIdle { get; private set; }
    public IBossState StateTracking { get; private set; }
    public IBossState StateRecover { get; private set; }

    // 모든 공격 패턴을 담을 배열
    public IBossState[] attackPatterns;

    // 방금 사용한 패턴 번호를 기억하는 변수 (-1은 처음이라는 뜻)
    public int lastAttackIndex = -1;

    void Start()
    {
        if (anim == null) anim = GetComponent<Animator>();

        // 기본 상태 생성
        StateIdle = new BossIdleState(this);
        StateTracking = new BossTrackingState(this);
        StateRecover = new BossRecoverState(this);

        // ★ 공격 패턴 배열 초기화 (순서대로 0, 1, 2, 3번)
        attackPatterns = new IBossState[]
        {
            new BossSweepState(this),   // [0] 휩쓸기 (1번 패턴)
            new BossClapState(this),    // [1] 박수 (2번 패턴)
            new BossMissileState(this), // [2] 미사일 (3번 패턴)
            new BossLaserState(this)    // [3] 레이저 (4번 패턴)
        };

        ChangeState(StateIdle);
    }

    void Update()
    {
        if (currentState != null) currentState.Execute();
    }

    public void ChangeState(IBossState newState)
    {
        if (currentState != null) currentState.Exit();
        currentState = newState;
        currentState.Enter();
    }

    // --- 애니메이션 이벤트 연결 함수들 ---

    public void OnEvent_Sweep() // 패턴 1
    {
        Collider[] hits = Physics.OverlapBox(sweepCenter.position, sweepSize / 2, sweepCenter.rotation);
        foreach (var hit in hits) if (hit.transform == player) Debug.Log("휩쓸기 적중!");
    }

    public void OnEvent_Clap() // 패턴 2
    {
        if (clapEffect) Instantiate(clapEffect, clapPoint.position, clapPoint.rotation);
        Debug.Log("박수 짝!");
    }

    public void OnEvent_Missile() // 패턴 3
    {
        if (missilePrefab) Instantiate(missilePrefab, missilePort.position, missilePort.rotation);
        Debug.Log("미사일 발사!");
    }

    public void OnEvent_Laser() // 패턴 4
    {
        if (laserEffect) Instantiate(laserEffect, laserPort.position, laserPort.rotation);
        Debug.Log("레이저 발사!");
    }

    void OnDrawGizmos()
    {
        if (sweepCenter)
        {
            Gizmos.color = new Color(1, 0, 0, 0.5f);
            Gizmos.matrix = Matrix4x4.TRS(sweepCenter.position, sweepCenter.rotation, Vector3.one);
            Gizmos.DrawCube(Vector3.zero, sweepSize);
        }
    }
}
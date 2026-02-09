using BRASS;
using System.Collections;
using UnityEngine;

public class RobotBoss : MonoBehaviour
{
    [Header("Components")]
    public Animator anim;
    public Transform player;

    Vector3 dir;

    [Header("Attack Settings")]
    public Transform sweepCenter;
    public Vector3 sweepSize = new Vector3(5, 2, 3);
    public GameObject clapEffect;
    public Transform clapPoint;

    public GameObject missilePrefab;
    public Transform missilePort;
    public GameObject laserEffect;
    public Transform laserPort;
    LineRenderer laserLine;

    [Header("Stats")]
    public float detectionRange = 20f;
    public float attackRange = 10f;
    public float recoverTime = 2f;
    public float rotationSpeed = 5f;
    public float laserDamage = 5.0f;
    public float sweepDamage = 10.0f;
    public float clapDamage = 3.0f;

    // --- 상태 관리 ---
    private IBossState currentState;

    public IBossState StateIdle { get; private set; }
    public IBossState StateTracking { get; private set; }
    public IBossState StateRecover { get; private set; }

    // 모든 공격 패턴을 담을 배열
    public IBossState[] attackPatterns;

    // 방금 사용한 패턴 번호를 기억하는 변수 (-1은 처음이라는 뜻)
    public int lastAttackIndex = -1;

    private void Awake()
    {
        player = FindAnyObjectByType<PlayerController>().transform;
    }
    void Start()
    {
        if (anim == null) anim = GetComponent<Animator>();
        laserLine = GetComponent<LineRenderer>();
        dir = (player.position - laserPort.position).normalized;
        // 기본 상태 생성
        StateIdle = new BossIdleState(this);
        StateTracking = new BossTrackingState(this);
        StateRecover = new BossRecoverState(this);

        // 공격 패턴 배열 초기화 (순서대로 0, 1, 2, 3번)
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
        dir = (player.position - laserPort.position).normalized;
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
        foreach (var hit in hits)
        {
            var targetHealth = hit.transform.GetComponent<PlayerState>();
            if (targetHealth != null)
            {
                targetHealth.TakeDamage(sweepDamage);
            }
            Debug.Log("휩쓸기 적중!");
        }
    }

    public void OnEvent_Clap()  // 패턴 2
    {

        if (clapEffect) Instantiate(clapEffect, clapPoint.position, clapPoint.rotation);

        Debug.Log("박수 짝!");

        RaycastHit hit;
        if (Physics.Raycast(clapPoint.position, clapPoint.forward, out hit, 100f))
        {
            if (hit.collider.CompareTag("Player"))
            {
                var targetHealth = hit.collider.GetComponent<PlayerState>();
                if (targetHealth != null)
                {
                    targetHealth.TakeDamage(clapDamage);
                    Debug.Log("플레이어에게 데미지를 입혔습니다!");
                }
            }
        }

        Debug.DrawRay(clapPoint.position, clapPoint.forward * 100f, Color.red, 0.5f);
    }

    public void OnEvent_Missile() // 패턴 3
    {
        if (missilePrefab) Instantiate(missilePrefab, missilePort.position, missilePort.rotation);
        Debug.Log("미사일 발사!");
    }

    public void OnEvent_Laser() // 패턴 4
    {
        if (laserEffect) Instantiate(laserEffect, laserPort.position, laserPort.rotation);
        StartCoroutine(LaserShoot());
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
    IEnumerator LaserShoot()
    {
        if (laserLine == null) yield break;

        laserLine.enabled = true;

        // 1. 추적 및 대기 단계 (예: 2초 동안 플레이어를 따라감)
        float trackingDuration = 2.0f;
        float warningTime = 0.5f; // 공격 전 노란색으로 변할 시간
        float timer = 0f;

        while (timer < trackingDuration)
        {
            timer += Time.deltaTime;

            // 레이저 시작점과 끝점 업데이트 (플레이어를 계속 조준)
            Vector3 startPos = laserPort.position;
            // 플레이어의 중심(허리 위쪽 등)을 조준하도록 약간 보정 (Vector3.up * 1f)
            Vector3 targetPos = player.position + Vector3.up * 1f;
            Vector3 direction = (targetPos - startPos).normalized;

            // 색상 변경 로직 (공격 0.5초 전부터 노란색)
            if (timer >= (trackingDuration - warningTime))
            {
                laserLine.startColor = Color.yellow;
                laserLine.endColor = Color.yellow;
            }
            else
            {
                laserLine.startColor = Color.white; // 평상시 흰색 혹은 투명한 붉은색
                laserLine.endColor = Color.white;
            }

            laserLine.SetPosition(0, startPos);
            // 추적 단계에서는 레이캐스트 없이 플레이어 위치까지 라인을 그림
            laserLine.SetPosition(1, targetPos);

            yield return null;
        }

        // 2. 공격 단계 (레이캐스트 시도)
        // 공격 시점에는 색을 빨간색으로 변경하여 강렬하게 연출
        laserLine.startColor = Color.red;
        laserLine.endColor = Color.red;

        Vector3 finalDir = (player.position + Vector3.up * 1f - laserPort.position).normalized;
        RaycastHit hit;
        float maxDistance = 50f;

        if (Physics.Raycast(laserPort.position, finalDir, out hit, maxDistance))
        {
            laserLine.SetPosition(1, hit.point);

            if (hit.transform == player)
            {
                Debug.Log("laser hit player");
                player.GetComponent<PlayerState>().TakeDamage(laserDamage);
            }
        }
        else
        {
            laserLine.SetPosition(1, laserPort.position + finalDir * maxDistance);
        }

        // 3. 공격 후 잔상 유지 (잠시 보여줬다가 사라짐)
        yield return new WaitForSeconds(0.2f);
        laserLine.enabled = false;
    }
}
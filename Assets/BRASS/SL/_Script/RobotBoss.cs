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
    public Vector3 sweepSize = new Vector3(5, 3, 5);
    public GameObject clapEffect;
    public Transform clapPoint;

    public GameObject missilePrefab;
    public Transform missilePort;
    public GameObject laserEffect;
    public Transform laserPort;
    LineRenderer laserLine;
    public LayerMask playerLayer;
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

    public void OnEvent_Sweep()
    {
        Collider[] hits = Physics.OverlapBox(sweepCenter.position, sweepSize / 2, sweepCenter.rotation, playerLayer);
        // [디버그용] 몇 개나 잡혔는지 숫자를 먼저 찍어봅니다.
        Debug.Log($"{hits.Length}개의 콜라이더가 감지됨");
        foreach (var hit in hits)
        {
            if (hit.TryGetComponent<PlayerState>(out var targetHealth))
            {
                targetHealth.TakeDamage(sweepDamage);
                Debug.Log("휩쓸기 적중: " + hit.name);
            }
        }
    }

    public void OnEvent_Clap()  // 패턴 2
    {

        if (clapEffect) Instantiate(clapEffect, clapPoint.position, clapPoint.rotation);

        Debug.Log("박수 짝!");
        Vector3 finalDir = (player.position + Vector3.up * 1f - laserPort.position).normalized;

        RaycastHit hit;
        if (Physics.Raycast(laserPort.position, finalDir, out hit, 50f))
        {
            Debug.Log("실행됨");
            if (hit.collider.CompareTag("Player"))
            {
                PlayerState targetHealth = hit.collider.GetComponent<PlayerState>();
                if (targetHealth != null)
                {
                    targetHealth.TakeDamage(clapDamage);
                    Debug.Log("플레이어에게 데미지를 입혔습니다!");
                }
            }
        }

        Debug.DrawRay(clapPoint.position, dir, Color.red, 0.5f);
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

    private void OnDrawGizmos()
    {
        // sweepCenter가 할당되지 않았으면 에러 방지를 위해 리턴
        if (sweepCenter == null) return;

        // 기즈모 색상 설정 (원하는 색으로 변경 가능)
        Gizmos.color = Color.red;

        // Physics.OverlapBox와 동일한 위치와 회전값을 기즈모 행렬에 적용
        Matrix4x4 rotationMatrix = Matrix4x4.TRS(sweepCenter.position, sweepCenter.rotation, sweepCenter.localScale);
        Gizmos.matrix = rotationMatrix;

        // OverlapBox는 size를 사용하지만, Gizmos.DrawWireCube도 전체 크기를 받으므로 sweepSize를 그대로 사용
        // (만약 sweepSize가 로컬 스케일 영향을 받는다면 계산이 달라질 수 있습니다)
        Gizmos.DrawWireCube(Vector3.zero, sweepSize);

        // 내부를 약간 불투명하게 보고 싶다면 아래 주석 해제
        // Gizmos.color = new Color(1, 0, 0, 0.2f);
        // Gizmos.DrawCube(Vector3.zero, sweepSize);
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
    public void InstantKill()
    {
        Debug.Log("보스 치트 작동: 즉시 사망");

        // 1. 현재 진행 중인 모든 코루틴 정지 (레이저 등)
        StopAllCoroutines();
        if (laserLine != null) laserLine.enabled = false;

        // 2. 사망 애니메이션 실행 (애니메이터에 Die 트리거가 있다고 가정)
        if (anim != null) anim.SetTrigger("Die");

        // 3. 상태를 중지시키기 위해 null로 변경하거나 사망 상태로 전환
        currentState = null;

        // 4. 보상 UI 호출 (BossRewardCheater가 직접 호출해도 되지만, 여기서 처리하는 게 구조상 깔끔합니다)
        // 만약 보스 스크립트에서 직접 보상 UI를 참조하고 있다면 여기서 호출하세요.
    }
}
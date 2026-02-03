using UnityEngine;
using UnityEngine.InputSystem;
using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// 통합 타겟팅 시스템
/// - Tab 키: 다음 타겟으로 순환 (거리순)
/// - 마우스 클릭: 클릭한 적을 타겟팅
/// - ESC: 타겟 해제
/// </summary>
public class TargetingSystem : MonoBehaviour
{
    [Header("Targeting Settings")]
    [SerializeField] private float maxTargetingDistance = 50f;
    [SerializeField] private float targetSearchRadius = 30f; // Tab 키로 찾을 범위
    [SerializeField] private LayerMask targetableLayer;

    [Header("Input")]
    [SerializeField] private InputActionReference mouseClickAction;  // 마우스 좌클릭
    [SerializeField] private InputActionReference nextTargetAction; // Tab 키
    [SerializeField] private InputActionReference cancelAction;     // ESC 키

    private ITargetable currentTarget;
    private Camera mainCamera;
    private List<ITargetable> nearbyTargets = new List<ITargetable>();

    // 이벤트
    public event Action<ITargetable> OnTargetChanged;
    public event Action OnTargetCleared;

    // 프로퍼티
    public ITargetable CurrentTarget => currentTarget;
    public bool HasTarget => currentTarget != null && currentTarget.IsValidTarget();
    public int NearbyTargetCount => nearbyTargets.Count;

    private void Awake()
    {
        mainCamera = Camera.main;

        if (mainCamera == null)
        {
            Debug.LogError("Main Camera를 찾을 수 없습니다!");
        }
    }

    private void OnEnable()
    {
        // 마우스 클릭
        if (mouseClickAction != null)
        {
            mouseClickAction.action.Enable();
            mouseClickAction.action.performed += OnMouseClick;
        }

        // Tab 키 (다음 타겟)
        if (nextTargetAction != null)
        {
            nextTargetAction.action.Enable();
            nextTargetAction.action.performed += OnNextTarget;
        }

        // ESC 키 (타겟 해제)
        if (cancelAction != null)
        {
            cancelAction.action.Enable();
            cancelAction.action.performed += OnCancelPerformed;
        }
    }

    private void OnDisable()
    {
        if (mouseClickAction != null)
        {
            mouseClickAction.action.performed -= OnMouseClick;
            mouseClickAction.action.Disable();
        }

        if (nextTargetAction != null)
        {
            nextTargetAction.action.performed -= OnNextTarget;
            nextTargetAction.action.Disable();
        }

        if (cancelAction != null)
        {
            cancelAction.action.performed -= OnCancelPerformed;
            cancelAction.action.Disable();
        }
    }

    private void Update()
    {
        // 현재 타겟이 무효화되면 자동 해제
        if (currentTarget != null && !currentTarget.IsValidTarget())
        {
            ClearTarget();
        }

        // 주변 타겟 목록 갱신 (0.5초마다)
        if (Time.frameCount % 30 == 0)
        {
            UpdateNearbyTargets();
        }
    }

    /// <summary>
    /// 마우스 클릭으로 타겟 선택 (적이 1개든 여러 개든 상관없이)
    /// </summary>
    private void OnMouseClick(InputAction.CallbackContext context)
    {
        if (mainCamera == null) return;

        // 마우스 위치에서 레이캐스트
        Vector2 mousePosition = Mouse.current.position.ReadValue();
        Ray ray = mainCamera.ScreenPointToRay(mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, maxTargetingDistance, targetableLayer))
        {
            ITargetable targetable = hit.collider.GetComponent<ITargetable>();

            if (targetable != null && targetable.IsValidTarget())
            {
                SetTarget(targetable);
                Debug.Log($"마우스 클릭으로 타겟 선택: {targetable.GetTargetTransform().name}");
            }
        }
    }

    /// <summary>
    /// Tab 키로 다음 타겟 순환 (거리순)
    /// </summary>
    private void OnNextTarget(InputAction.CallbackContext context)
    {
        if (nearbyTargets.Count == 0)
        {
            return;
        }

        // 현재 타겟이 없으면 첫 번째 타겟 선택
        if (currentTarget == null)
        {
            SetTarget(nearbyTargets[0]);
            return;
        }

        // 현재 타겟의 다음 타겟 찾기
        int currentIndex = nearbyTargets.IndexOf(currentTarget);
        if (currentIndex >= 0)
        {
            int nextIndex = (currentIndex + 1) % nearbyTargets.Count;
            SetTarget(nearbyTargets[nextIndex]);
        }
        else
        {
            // 현재 타겟이 목록에 없으면 첫 번째 선택
            SetTarget(nearbyTargets[0]);
        }
    }

    /// <summary>
    /// ESC 키로 타겟 해제
    /// </summary>
    private void OnCancelPerformed(InputAction.CallbackContext context)
    {
        ClearTarget();
    }

    /// <summary>
    /// 주변 타겟 목록 갱신 (Tab 키 순환용)
    /// </summary>
    private void UpdateNearbyTargets()
    {
        nearbyTargets.Clear();

        // 플레이어 주변의 모든 타겟 찾기
        Collider[] colliders = Physics.OverlapSphere(transform.position, targetSearchRadius, targetableLayer);

        foreach (Collider col in colliders)
        {
            ITargetable targetable = col.GetComponent<ITargetable>();

            if (targetable != null && targetable.IsValidTarget())
            {
                nearbyTargets.Add(targetable);
            }
        }

        // 거리순 정렬 (가까운 순)
        nearbyTargets = nearbyTargets
            .OrderBy(t => Vector3.Distance(transform.position, t.GetTargetTransform().position))
            .ToList();
    }

    /// <summary>
    /// 타겟 직접 설정
    /// </summary>
    public void SetTarget(ITargetable target)
    {
        if (target == null || !target.IsValidTarget())
        {
            ClearTarget();
            return;
        }

        // 같은 타겟이면 무시
        if (currentTarget == target) return;

        currentTarget = target;
        OnTargetChanged?.Invoke(currentTarget);
    }

    /// <summary>
    /// 타겟 해제
    /// </summary>
    public void ClearTarget()
    {
        if (currentTarget == null) return;

        currentTarget = null;
        OnTargetCleared?.Invoke();
    }

    /// <summary>
    /// 가장 가까운 타겟 가져오기
    /// </summary>
    public ITargetable GetNearestTarget()
    {
        return nearbyTargets.Count > 0 ? nearbyTargets[0] : null;
    }

    /// <summary>
    /// 디버그: 타겟팅 범위 시각화
    /// </summary>
    private void OnDrawGizmos()
    {
        // Tab 키 타겟 검색 범위
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, targetSearchRadius);

        // 현재 타겟 표시
        if (currentTarget != null && currentTarget.IsValidTarget())
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, currentTarget.GetTargetTransform().position);
        }

        // 주변 타겟들 표시
        Gizmos.color = Color.green;
        foreach (var target in nearbyTargets)
        {
            if (target != null && target.IsValidTarget())
            {
                Gizmos.DrawWireSphere(target.GetTargetTransform().position, 1f);
            }
        }
    }
}
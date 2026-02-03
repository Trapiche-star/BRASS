using UnityEngine;

/// <summary>
/// 타겟 가능한 적 예시 클래스
/// 적, NPC 등 타겟팅 가능한 오브젝트에 부착
/// </summary>
public class EnemyTarget : MonoBehaviour, ITargetable
{
    [Header("Target Settings")]
    [SerializeField] private TargetType targetType = TargetType.Enemy;
    [SerializeField] private Transform targetPoint; // 마커가 표시될 위치 (null이면 자신)

    [Header("Display")]
    [SerializeField] private string displayName = ""; // 마커에 표시할 이름 (비어있으면 GameObject 이름)

    [Header("Status")]
    [SerializeField] private bool isAlive = true;
    [SerializeField] private bool canBeTargeted = true;

    private void Awake()
    {
        // targetPoint가 없으면 자신을 사용
        if (targetPoint == null)
        {
            targetPoint = transform;
        }

        // displayName이 비어있으면 GameObject 이름 사용
        if (string.IsNullOrEmpty(displayName))
        {
            displayName = gameObject.name;
        }
    }

    public Transform GetTargetTransform()
    {
        return targetPoint;
    }

    public bool IsValidTarget()
    {
        return isAlive && canBeTargeted && gameObject.activeInHierarchy;
    }

    public TargetType GetTargetType()
    {
        return targetType;
    }

    /// <summary>
    /// 마커에 표시될 이름 가져오기
    /// </summary>
    public string GetDisplayName()
    {
        return displayName;
    }

    /// <summary>
    /// 표시 이름 변경 (런타임에서도 가능)
    /// </summary>
    public void SetDisplayName(string newName)
    {
        displayName = newName;
    }

    /// <summary>
    /// 사망 처리 (예시)
    /// </summary>
    public void Die()
    {
        isAlive = false;
        // 사망 시 타겟 마커가 자동으로 제거됨 (TargetingSystem의 Update에서 체크)
    }

    /// <summary>
    /// 타겟 가능 여부 설정
    /// </summary>
    public void SetTargetable(bool targetable)
    {
        canBeTargeted = targetable;
    }

    /// <summary>
    /// 타겟 타입 변경 (적 -> 아군 등)
    /// </summary>
    public void SetTargetType(TargetType newType)
    {
        targetType = newType;
    }
}
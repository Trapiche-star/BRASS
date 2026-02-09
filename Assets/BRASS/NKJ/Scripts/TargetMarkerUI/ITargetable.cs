using UnityEngine;

/// <summary>
/// 타겟팅 가능한 오브젝트가 구현해야 하는 인터페이스
/// 적, NPC, 상호작용 오브젝트 등에 부착
/// </summary>
public interface ITargetable
{
    /// <summary>
    /// 타겟의 월드 좌표 (마커가 표시될 위치)
    /// </summary>
    Transform GetTargetTransform();

    /// <summary>
    /// 타겟이 유효한지 확인 (사망, 비활성화 등)
    /// </summary>
    bool IsValidTarget();

    /// <summary>
    /// 타겟 타입 (적, 아군, 중립 등 - 마커 색상 변경 시 사용)
    /// </summary>
    TargetType GetTargetType();

    /// <summary>
    /// 마커에 표시할 이름 (선택적 - 기본 구현 제공)
    /// </summary>
    string GetDisplayName()
    {
        return GetTargetTransform().gameObject.name;
    }
}

/// <summary>
/// 타겟 타입 열거형
/// </summary>
public enum TargetType
{
    Enemy,      // 적
    Ally,       // 아군
    Neutral,    // 중립
    Interactive // 상호작용 가능 오브젝트
}
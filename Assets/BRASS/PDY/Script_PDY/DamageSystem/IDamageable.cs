namespace BRASS
{
    /// <summary>
    /// 대미지를 입을 수 있는 모든 오브젝트(플레이어, 적, 구조물 등)가 상속받아야 하는 인터페이스
    /// </summary>
    public interface IDamageable
    {
        // 이 함수를 구현하는 쪽에서 체력 감소, 피격 이펙트, 사망 처리 등을 작성하게 됩니다.
        void TakeDamage(float damageAmount);
    }
}
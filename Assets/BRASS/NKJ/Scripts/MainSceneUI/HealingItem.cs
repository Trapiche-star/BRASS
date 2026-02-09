using UnityEngine;
using Team1;

public class HealingItem : ConsumableItem
{
    public float healAmount = 20f;

    public override void Use(GameObject user)
    {
        // GaugeController의 싱글톤 인스턴스를 통해 HP 회복
        if (GaugeController.Instance != null)
        {
            GaugeController.Instance.HealHp(healAmount);
            Debug.Log($"{ItemName} 사용! HP {healAmount} 회복");
        }
    }

    public override ConsumableItem Clone() => (ConsumableItem)this.MemberwiseClone();
}
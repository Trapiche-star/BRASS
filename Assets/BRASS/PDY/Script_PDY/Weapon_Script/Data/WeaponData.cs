using UnityEngine;

namespace BRASS
{
    [CreateAssetMenu(menuName = "GameData/Weapon")]
    public class WeaponData : ScriptableObject
    {
        public int itemId;
        public string itemName;

        public WeaponType weaponType;        // 어떤 무기 타입인지 (애니메이션/공격 분기용)
        public GameObject weaponPrefab;        // 손에 붙을 실제 모델

        // 원거리 전용
        public GameObject bulletPrefab;
        public string firePointName = "FirePoint";
    }
}

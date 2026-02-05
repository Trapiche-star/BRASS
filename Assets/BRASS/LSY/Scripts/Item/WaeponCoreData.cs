
using UnityEngine;

namespace Team1
{
    [CreateAssetMenu(fileName = "WeaponCoreData", menuName = "Scriptable Objects/WeaponCoreData")]
    public class WaeponCoreData : ConsumableItemData
    {
        public override ConsumableItem CreateItem()
        {
            return new WaeponCore(
                itemName,
                icon,
                price
            );
        }
    }
}
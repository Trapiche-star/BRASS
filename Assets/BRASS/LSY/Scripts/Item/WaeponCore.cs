using Team1;
using UnityEngine;

namespace Team1
{

    public class WaeponCore : ConsumableItem
    {
        public WaeponCore(string name, Sprite icon, int price)
        {
            ItemName = name;
            Icon = icon;
            Price = price;

        }

        public override ConsumableItem Clone()
        {
            throw new System.NotImplementedException();
        }

        public override void Use(GameObject user)
        {
            throw new System.NotImplementedException();
        }
    }
}
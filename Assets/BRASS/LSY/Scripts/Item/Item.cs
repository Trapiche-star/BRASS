using UnityEngine;

namespace Team1
{
    public interface IItem
    {
        string ItemName { get; }
        Sprite Icon { get; }     // ⭐ 추가
        void Use(GameObject user);
    }
}

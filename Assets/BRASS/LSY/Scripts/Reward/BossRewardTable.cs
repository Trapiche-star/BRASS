using System.Collections.Generic;
using Team1;
using UnityEngine;

[CreateAssetMenu(fileName = "BossRewardTable", menuName = "Scriptable Objects/BossRewardTable")]
public class BossRewardTable : ScriptableObject
{
    [Header("보스 드랍 아이템 목록")]
    public List<ConsumableItemData> possibleItems;
}

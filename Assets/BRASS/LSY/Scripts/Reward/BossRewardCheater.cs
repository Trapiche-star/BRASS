using UnityEngine;
using UnityEngine.InputSystem;

namespace Team1
{
    public class BossRewardCheater : MonoBehaviour
    {
        [SerializeField] private BossClearUI bossClearUI;

        void Update()
        {
            // 신형 방식만 사용해서 에러 해결
            if (Keyboard.current != null && Keyboard.current.rightBracketKey.wasPressedThisFrame)
            {
                if (bossClearUI != null)
                {
                    bossClearUI.ShowReward();
                }
            }
        }
    }
}
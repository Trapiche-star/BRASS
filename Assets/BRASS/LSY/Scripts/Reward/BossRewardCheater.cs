using UnityEngine;
using UnityEngine.InputSystem;

namespace Team1
{
    public class BossRewardCheater : MonoBehaviour
    {
        [SerializeField] private RobotBoss robotBoss; // 보스 스크립트 참조
        [SerializeField] private BossClearUI bossClearUI; // 보상 UI 참조

        void Update()
        {
            if (Keyboard.current != null && Keyboard.current.minusKey.wasPressedThisFrame)
            {
                Debug.Log("- 키 눌림: 치트 발동");

                // 1. 보스 스크립트의 사망 함수 실행 (동작 중지 등)
                if (robotBoss != null) robotBoss.InstantKill();
                if (bossClearUI != null)
                {
                    bossClearUI.ShowReward();
                }

                // 2. 보상 UI의 ShowReward 실행 (여기서 마켓/지도를 꺼야 함)
                if (bossClearUI != null)
                {
                    bossClearUI.ShowReward();
                }
            }
        }
    }
}
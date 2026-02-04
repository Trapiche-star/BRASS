using UnityEngine;
using UnityEngine.InputSystem; // 신형 입력 시스템 사용 시 필요

namespace Team1
{
    public class BossRewardCheater : MonoBehaviour
    {
        [Header("연결할 보상 UI")]
        [SerializeField] private BossClearUI bossClearUI;

        void Update()
        {
            // --- 1. 신형 Input System 방식 (Keyboard.current) ---
            if (Keyboard.current != null && Keyboard.current.rightBracketKey.wasPressedThisFrame)
            {
                TriggerCheat();
            }

            // --- 2. 혹시 모르니 구형 Input 방식도 같이 체크 (KeyCode.RightBracket) ---
            if (Input.GetKeyDown(KeyCode.RightBracket))
            {
                TriggerCheat();
            }
        }

        private void TriggerCheat()
        {
            if (bossClearUI != null)
            {
                Debug.Log("<color=yellow><b>[CHEAT]</b> ']' 키 입력 감지! 보상 UI를 호출합니다.</color>");
                bossClearUI.ShowReward();
            }
            else
            {
                Debug.LogError("❌ [CHEAT] BossClearUI가 할당되지 않았습니다! 인스펙터에서 보상 패널을 드래그해서 넣어주세요.");
            }
        }
    }
}
using UnityEngine;
using System.Collections;

namespace BRASS
{
    /// 플레이어의 현재 상태를 저장하는 단일 상태 컨테이너
    public class PlayerState_SY : MonoBehaviour
    {
        #region Variables
        public bool IsMoving;   // 현재 이동 중인지 여부
        public bool IsFastRun;  // 패스트런 상태 여부
        public bool IsSliding;  // 슬라이딩 상태 여부
        public bool IsGrounded;  // 지면에 닿아 있는 상태 여부
        public bool IsJumping;   // 점프가 시작된 프레임 여부
        public int JumpIndex;   // 0 = 점프 아님, 1 = 1단 점프, 2 = 2단 점프
        #endregion

        #region Property
        public bool IsIdle => !IsMoving && !IsSliding; // 이동·슬라이딩이 아니면 Idle로 간주한다
        #endregion

        public int maxHP = 100;
        public int currentHP = 100;

        public float moveSpeed = 5f;

        public void Heal(int amount)
        {
            currentHP = Mathf.Min(currentHP + amount, maxHP);
            Debug.Log($"현재 체력: {currentHP}");
        }

        public IEnumerator SpeedBuff(float multiplier, float duration)
        {
            moveSpeed *= multiplier;

            yield return new WaitForSeconds(duration);

            moveSpeed /= multiplier;
            Debug.Log("⚡ 이동속도 버프 종료");
        }
    }
}
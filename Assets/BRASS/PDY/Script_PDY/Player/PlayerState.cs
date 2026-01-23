using UnityEngine;

namespace BRASS
{
    /// <summary>
    /// 플레이어의 이동·점프·접지 상태를 저장하는 순수 상태 컨테이너
    /// 판단이나 계산 없이 결과값만 보관한다
    /// </summary>
    public class PlayerState : MonoBehaviour
    {
        #region Variables
        public bool IsMoving;    // 현재 이동 입력 또는 이동 벡터가 존재하는 상태
        public bool IsFastRun;   // 패스트런 조건이 충족된 상태
        public bool IsSliding;   // 슬라이딩 동작이 진행 중인 상태
        public bool IsGrounded;  // 캐릭터가 지면에 접촉해 있는 상태
        public bool IsJumping;   // 점프 입력으로 인해 공중 상태로 판정된 상태
        public int JumpIndex;    // 0 = 지면, 1 = 1단 점프, 2 = 2단 점프
        
        #endregion      

        #region Property
        public bool IsIdle => !IsMoving && !IsSliding;
        // 이동도 슬라이딩도 아닌 경우를 논리적 Idle 상태로 간주한다
        #endregion
    }
}

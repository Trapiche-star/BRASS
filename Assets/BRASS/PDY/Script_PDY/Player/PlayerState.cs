using UnityEngine;

namespace BRASS
{
    /// 플레이어의 현재 상태를 저장하는 단일 상태 컨테이너
    public class PlayerState : MonoBehaviour
    {
        #region Variables
        public bool IsMoving;   // 현재 이동 중인지 여부
        public bool IsFastRun;  // 패스트런 상태 여부
        public bool IsSliding;  // 슬라이딩 상태 여부
        #endregion

        #region Property
        public bool IsIdle => !IsMoving && !IsSliding; // 이동·슬라이딩이 아니면 Idle로 간주한다
        #endregion
    }
}

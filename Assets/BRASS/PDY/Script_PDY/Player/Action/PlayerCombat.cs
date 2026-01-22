using UnityEngine;

namespace BRASS
{
    /// <summary>
    /// 플레이어의 기본공격(평타) 전투 로직을 담당
    /// </summary>
    public class PlayerCombat : MonoBehaviour
    {
        #region Variables
        [SerializeField] private float attackCooldown = 0.6f;       // 기본공격 쿨타임

        private bool isBasicAttackHeld;   // 공격 버튼 눌림 상태
        private float lastAttackTime;     // 마지막 공격 시점
        #endregion

        #region Unity Event Method
        private void Update()
        {
            HandleBasicAttackLoop();        // 매 프레임 기본공격 반복 여부를 확인한다
        }
        #endregion

        #region Custom Method
        // BasicAttack 버튼 눌림 시작
        public void OnBasicAttackStarted()
        {
            isBasicAttackHeld = true;     // 버튼이 눌린 상태로 변경한다
        }

        // BasicAttack 버튼 눌림 종료
        public void OnBasicAttackCanceled()
        {
            isBasicAttackHeld = false;      // 버튼을 뗀 상태로 변경한다
        }

        // 기본공격 반복 처리 루프
        private void HandleBasicAttackLoop()
        {
            if (!isBasicAttackHeld)       // 공격 버튼이 눌려있지 않으면 이후 로직을 실행하지 않는다
                return;

            if (Time.time < lastAttackTime + attackCooldown)    // 마지막 공격 후 쿨타임이 지나지 않았으면 실행을 중단한다
                return;

            ExecuteBasicAttack();      // 공격 조건이 충족되면 실제 공격을 실행한다
        }

        // 실제 기본공격 실행
        private void ExecuteBasicAttack()
        {
            lastAttackTime = Time.time;    // 현재 시간을 마지막 공격 시점으로 기록한다

            Debug.Log("Basic Attack!");
        }
        #endregion
    }
}
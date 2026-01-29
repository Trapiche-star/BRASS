using UnityEngine;

namespace BRASS
{
    /// 플레이어의 모든 이동 상태 플래그와 체력 수치를 관리하는 통합 상태 컨테이너 클래스
    public class PlayerState : MonoBehaviour, IDamageable
    {
        #region Variables
        [Header("Action States")]
        public bool IsMoving; // 현재 캐릭터가 이동 입력 중인지 여부
        public bool IsFastRun; // 캐릭터가 고속 달리기 상태인지 여부
        public bool IsSliding; // 슬라이딩 동작을 수행 중인지 여부
        public bool IsGrounded; // 캐릭터가 지면에 닿아 있는지 여부
        public bool IsJumping; // 현재 점프 상승 또는 낙하 중인지 여부
        public int JumpIndex; // 연속 점프 중 현재 몇 번째 점프인지 기록
        public bool IsAttacking; // 캐릭터가 공격 동작을 수행 중인지 여부
        public bool IsEquipped; // 어떠한 무기라도 장착 중인지 여부
        public bool IsBattleAxeEquipped; // 현재 배틀액스를 주 무기로 장착했는지 여부
        public bool IsInputMovementLocked; // 외부 요인으로 인해 이동 입력을 차단해야 하는지 여부

        [Header("Health Settings")]
        [SerializeField] private float maxHealth = 100f; // 캐릭터가 가질 수 있는 최대 체력
        [SerializeField] private float currentHealth; // 현재 캐릭터의 남은 체력 수치
        #endregion

        #region Property
        public float MaxHealth => maxHealth; // 외부에서 최대 체력을 읽기 위한 프로퍼티
        public float CurrentHealth => currentHealth; // 외부에서 현재 체력을 읽기 위한 프로퍼티
        public bool IsIdle => !IsMoving && !IsSliding; // 이동과 슬라이딩이 모두 없을 때의 대기 상태 여부
        #endregion

        #region Unity Methods
        private void Awake()
        {
            currentHealth = maxHealth; // 게임 시작 시 현재 체력을 최대치로 설정하여 초기화한다
        }
        #endregion

        #region Custom Methods
        // 대미지를 수신하여 체력을 삭감하고 사망 여부를 판단함
        public void TakeDamage(float damageAmount)
        {
            if (currentHealth <= 0) return; // 이미 사망한 상태라면 대미지 계산을 수행하지 않고 중단한다

            currentHealth -= damageAmount; // 전달받은 수치만큼 현재 체력에서 차감한다
            Debug.Log($"[Player] 대미지 발생! 남은 체력: {currentHealth}");

            // 체력이 0 이하로 떨어졌을 경우 사망 프로세스를 가동한다
            if (currentHealth <= 0)
            {
                Die(); // 사망 메서드 호출
            }
        }

        // 체력이 소진되었을 때의 처리 로직을 실행함
        private void Die()
        {
            Debug.Log("플레이어 사망!"); // 캐릭터 사망 로그 출력
        }
        #endregion
    }
}
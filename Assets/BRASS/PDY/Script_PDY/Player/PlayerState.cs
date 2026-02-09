using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

namespace BRASS
{
    /// 플레이어의 모든 이동 상태 플래그와 체력 수치를 관리하는 통합 상태 컨테이너 클래스
    public class PlayerState : MonoBehaviour, IDamageable
    {
        #region Variables
        [Header("Action States")]
        public bool IsMoving; // 현재 캐릭터가 이동 입력 중인지 여부
        public bool IsFastRun; // 캐릭터가 고속 달리기 상태인지 여부
        public bool IsInputMovementLocked; // 외부 요인으로 인해 이동 입력을 차단해야 하는지 여부

        public bool IsSliding; // 슬라이딩 동작을 수행 중인지 여부
        public bool SlideRequested; // 슬라이드 입력이 요청되었는지 여부

        public bool IsGrounded; // 캐릭터가 지면에 닿아 있는지 여부
        public bool IsJumping; // 현재 점프 상승 또는 낙하 중인지 여부
        public int JumpIndex; // 연속 점프 중 현재 몇 번째 점프인지 기록
        public bool IsAttacking; // 캐릭터가 공격 동작을 수행 중인지 여부      

        public bool IsBattleAxeEquipped; // 현재 배틀액스를 주 무기로 장착했는지 여부
        public bool IsEquipped; // 어떠한 무기라도 장착 중인지 여부
        public bool IsGunEquipped;  // 하푼건 장착 여부 추가       

        public bool IsEngagedWithTarget; // 타겟과 전투가 시작된 상태인지 여부

        [Header("UI Input States")]
        public bool IsTypingInUI; // 현재 채팅, 닉네임 등 문자 입력 패널에서 타이핑 중인지 여부

        [Header("Health Settings")]
        [SerializeField] private float maxHealth = 100f; // 캐릭터가 가질 수 있는 최대 체력
        [SerializeField] private float currentHealth; // 현재 캐릭터의 남은 체력 수치

        public Transform CurrentTarget; // 현재 플레이어가 조준하고 있는 타겟 오브젝트 참조
        #endregion

        #region Property
        public float MaxHealth => maxHealth; // 외부에서 최대 체력을 읽기 위한 프로퍼티
        public float CurrentHealth => currentHealth; // 외부에서 현재 체력을 읽기 위한 프로퍼티
        public bool IsIdle => !IsMoving && !IsSliding; // 이동과 슬라이딩이 모두 없을 때의 대기 상태 여부

        // ⭐ 게임 입력을 받을 수 있는 상태인지 확인 (문자 입력 중이 아닐 때만 true)
        public bool CanReceiveGameInput => !IsTypingInUI;
        #endregion

        #region Unity Methods
        private void Awake()
        {
            currentHealth = maxHealth; // 게임 시작 시 현재 체력을 최대치로 설정하여 초기화한다           
        }

        private void Update()
        {
            // 매 프레임 InputField 포커스 상태를 자동으로 체크
            UpdateTypingState();
        }
        #endregion

        #region Custom Methods
        /// <summary>
        /// 현재 어떤 InputField라도 포커스 중인지 자동으로 감지하여 IsTypingInUI 플래그 갱신
        /// </summary>
        private void UpdateTypingState()
        {
            // EventSystem이 없으면 타이핑 중이 아님
            if (EventSystem.current == null)
            {
                IsTypingInUI = false;
                return;
            }

            // 현재 선택된 GameObject 가져오기
            GameObject selectedObject = EventSystem.current.currentSelectedGameObject;

            if (selectedObject == null)
            {
                IsTypingInUI = false;
                return;
            }

            // TMP_InputField 체크
            if (selectedObject.TryGetComponent<TMP_InputField>(out var tmpInput))
            {
                IsTypingInUI = tmpInput.isFocused;
                return;
            }

            // 기본 Unity InputField 체크 (혹시 사용 중이라면)
            if (selectedObject.TryGetComponent<UnityEngine.UI.InputField>(out var unityInput))
            {
                IsTypingInUI = unityInput.isFocused;
                return;
            }

            // InputField가 아니면 타이핑 중이 아님
            IsTypingInUI = false;
        }

        /// <summary>
        /// 외부에서 수동으로 타이핑 상태를 설정할 때 사용 (필요시)
        /// </summary>
        public void SetTypingState(bool isTyping)
        {
            IsTypingInUI = isTyping;
        }

        // 대미지를 수신하여 체력을 삭감하고 사망 여부를 판단함
        public void TakeDamage(float damageAmount)
        {
            if (currentHealth <= 0) return;

            currentHealth -= damageAmount;
            Debug.Log($"[Player] 대미지 발생! 남은 체력: {currentHealth}");

            if (currentHealth <= 0)
            {
                Die();
            }
        }

        // 체력이 소진되었을 때의 처리 로직을 실행함
        private void Die()
        {
            Debug.Log("플레이어 사망!");
        }
        #endregion
    }
}

/*
using UnityEngine;

namespace BRASS
{
    /// <summary>
    /// 플레이어의 모든 이동 상태 플래그와 체력 수치를 관리하는 통합 상태 컨테이너 클래스
    /// </summary>   
    public class PlayerState : MonoBehaviour, IDamageable
    {
        #region Variables
        [Header("Action States")]
        public bool IsMoving; // 현재 캐릭터가 이동 입력 중인지 여부
        public bool IsFastRun; // 캐릭터가 고속 달리기 상태인지 여부
        public bool IsInputMovementLocked; // 외부 요인으로 인해 이동 입력을 차단해야 하는지 여부

        public bool IsSliding; // 슬라이딩 동작을 수행 중인지 여부
        public bool SlideRequested; // 슬라이드 입력이 요청되었는지 여부        
        [SerializeField] private bool isDead; // 캐릭터 죽음 인스펙터 표시용
        public bool IsGrounded; // 캐릭터가 지면에 닿아 있는지 여부
        public bool IsJumping; // 현재 점프 상승 또는 낙하 중인지 여부
        public int JumpIndex; // 연속 점프 중 현재 몇 번째 점프인지 기록
        public bool IsAttacking; // 캐릭터가 공격 동작을 수행 중인지 여부      

        public bool IsBattleAxeEquipped; // 현재 배틀액스를 주 무기로 장착했는지 여부
        public bool IsEquipped; // 어떠한 무기라도 장착 중인지 여부
        public bool IsGunEquipped;  // 하푼건 장착 여부 추가       
        
        public bool IsEngagedWithTarget; // 타겟과 전투가 시작된 상태인지 여부

        [Header("Health Settings")]
        [SerializeField] private float maxHealth = 100f; // 캐릭터가 가질 수 있는 최대 체력
        [SerializeField] private float currentHealth; // 현재 캐릭터의 남은 체력 수치        

        public System.Action OnHit;     // 히트 이벤트 콜백
        public System.Action OnDead;    // 사망 이벤트 콜백

        public Transform CurrentTarget; // 현재 플레이어가 조준하고 있는 타겟 오브젝트 참조
        #endregion

        #region Property
        public float MaxHealth => maxHealth; // 외부에서 최대 체력을 읽기 위한 프로퍼티
        public float CurrentHealth => currentHealth; // 외부에서 현재 체력을 읽기 위한 프로퍼티
        public bool IsIdle => !IsMoving && !IsSliding; // 이동과 슬라이딩이 모두 없을 때의 대기 상태 여부
        public bool IsDead { get; private set; } // 캐릭터가 사망 상태인지 여부
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
            if (IsDead) return;

            currentHealth -= damageAmount;
            Debug.Log($"[Player] 대미지 발생! 남은 체력: {currentHealth}");

            if (currentHealth <= 0)
            {
                currentHealth = 0;
                Die();
                return; // ❗ 사망이면 Hit 안 보냄
            }

            // 살아있을 때만 히트
            OnHit?.Invoke();
        }

        // 사망 처리 메서드
        private void Die()
        {
            if (IsDead) return;

            IsDead = true;
            Debug.Log("플레이어 사망!");
            OnDead?.Invoke();
        }
        #endregion
    }
}
*/
using UnityEngine;
using UnityEngine.InputSystem;

namespace BRASS
{
    /// 무기 데이터를 관리하며 타입별(Gun/Melee) 소켓 보정 및 상태 동기화를 담당하는 핸들러
    public class WeaponHandler : MonoBehaviour
    {
        #region Variables

        [Header("Harpoon Gun Tuning")]
        [SerializeField] private Vector3 harpoonPositionOffset; // HandlePivot 기준 정렬 이후 추가 위치 보정값
        [SerializeField] private Vector3 harpoonRotationOffset; // HandlePivot 기준 정렬 이후 추가 회전 보정값 (Euler)

        [SerializeField] private Transform leftHand; // 캐릭터의 실제 왼손 본

        [SerializeField] private WeaponData[] weaponSlots; // 사용 가능한 무기 데이터 슬롯 배열
        [SerializeField] private Transform weaponSocket;   // 무기가 부착될 캐릭터의 손 소켓
        [SerializeField] private PlayerState state;         // 장착 여부 및 타입 정보를 동기화할 상태 데이터
        [SerializeField] private PlayerCombat combat;       // 현재 무기의 대미지 판정 정보를 전달할 컴포넌트

        private GameObject currentWeaponObject;              // 현재 장착되어 씬에 존재하는 무기 오브젝트

        #endregion

        #region Property
        public WeaponData CurrentWeapon { get; private set; } // 현재 논리적으로 장착된 무기 데이터
        #endregion

        #region Unity Event Method
        private void Update()
        {
            // 테스트용: ] 키 입력 시 1번 슬롯 무기 토글
            if (Keyboard.current != null && Keyboard.current.rightBracketKey.wasPressedThisFrame)
            {
                ToggleWeaponByIndex(1);
            }
        }
        #endregion

        #region Custom Methods

        // 지정한 슬롯 인덱스의 무기를 장착하거나, 이미 장착 중이면 해제한다
        public void ToggleWeaponByIndex(int index)
        {
            if (weaponSlots == null || index < 0 || index >= weaponSlots.Length) return;

            WeaponData targetWeapon = weaponSlots[index];
            if (targetWeapon == null || targetWeapon.weaponPrefab == null) return;

            if (CurrentWeapon == targetWeapon)
                UnequipWeapon();
            else
                EquipWeapon(index);
        }

        // 무기 프리팹을 생성하고 타입에 따라 위치 보정을 수행한다
        private void EquipWeapon(int index)
        {
            // 기존 무기 제거
            if (currentWeaponObject != null)
            {
                Destroy(currentWeaponObject);
            }

            CurrentWeapon = weaponSlots[index];
            currentWeaponObject = Instantiate(CurrentWeapon.weaponPrefab, weaponSocket);

            bool isGun = CurrentWeapon.weaponType == WeaponType.Gun;

            if (isGun)
            {
                currentWeaponObject.transform.localPosition = Vector3.zero;
                currentWeaponObject.transform.localRotation = Quaternion.identity;
            }
            else
            {
                // 근접 무기: 기본 정렬
                currentWeaponObject.transform.localPosition = Vector3.zero;
                currentWeaponObject.transform.localRotation = Quaternion.identity;
            }

            // 전투 판정 컴포넌트에 무기 전달
            if (combat != null)
            {
                WeaponDamage weaponDamage = currentWeaponObject.GetComponent<WeaponDamage>();
                combat.SetCurrentWeapon(weaponDamage);
            }

            // 상태 플래그 갱신
            if (state != null)
            {
                state.IsEquipped = true;
                state.IsBattleAxeEquipped = CurrentWeapon.weaponType == WeaponType.BattleAxe;
                state.IsGunEquipped = isGun;
            }

            Debug.Log($"무기 장착: {CurrentWeapon.name} (총기 여부: {isGun})");
        }

        // 현재 장착된 무기를 제거하고 상태를 초기화한다
        private void UnequipWeapon()
        {
            if (currentWeaponObject != null)
            {
                Destroy(currentWeaponObject);
                currentWeaponObject = null;
            }

            if (combat != null)
                combat.SetCurrentWeapon(null);

            CurrentWeapon = null;

            if (state != null)
            {
                state.IsEquipped = false;
                state.IsBattleAxeEquipped = false;
                state.IsGunEquipped = false;
            }

            Debug.Log("무기 해제");
        }

        // 현재 장착된 총기의 발사 지점을 반환한다
        public Transform GetFirePoint()
        {
            if (CurrentWeapon == null)
                return null;

            if (CurrentWeapon.weaponType != WeaponType.Gun)
                return null;

            if (currentWeaponObject == null)
                return null;

            return currentWeaponObject.transform.Find("FirePoint");
        }

        #endregion
    }
}

using UnityEngine;

namespace BRASS
{
    /// 무기 슬롯 데이터를 관리하며, 무기를 소켓(손)에 장착하거나 해제하는 책임을 가진 클래스
    public class WeaponHandler : MonoBehaviour
    {
        #region Variables
        [SerializeField] private WeaponData[] weaponSlots; // 사용 가능한 무기 데이터 자산 배열
        [SerializeField] private Transform weaponSocket; // 무기가 생성되어 부속될 손 위치 트랜스폼
        [SerializeField] private PlayerState state; // 캐릭터의 장착 상태를 갱신하기 위한 상태 데이터 참조

        private GameObject currentWeaponObject; // 씬에 생성된 실제 무기 게임 오브젝트
        #endregion

        #region Property
        public WeaponData CurrentWeapon { get; private set; } // 현재 논리적으로 장착된 무기 정보 (없으면 null)
        #endregion

        #region Custom Methods
        // 인덱스를 기반으로 현재 무기와의 중복 여부를 확인해 장착 혹은 해제함
        public void ToggleWeaponByIndex(int index)
        {
            // 슬롯 배열이 비어있거나 인덱스 범위가 잘못된 경우 처리를 중단한다
            if (weaponSlots == null || index < 0 || index >= weaponSlots.Length)
            {
                Debug.LogWarning("무기 슬롯 인덱스 오류");
                return;
            }

            WeaponData targetWeapon = weaponSlots[index];
            // 대상 무기 정보나 프리팹 데이터가 없는 경우 장착 절차를 진행하지 않는다
            if (targetWeapon == null || targetWeapon.weaponPrefab == null)
            {
                Debug.LogWarning("무기 데이터 또는 프리팹 없음");
                return;
            }

            if (CurrentWeapon == targetWeapon) // 이미 들고 있는 무기를 다시 선택한 경우
            {
                UnequipWeapon(); // 무기를 집어넣는다
            }
            else // 다른 무기를 선택했거나 빈손 상태인 경우
            {
                EquipWeapon(index); // 새로운 무기를 장착한다
            }
        }

        // 기존 무기 모델을 파괴하고 새로운 무기 모델을 생성하여 소켓에 부착함
        private void EquipWeapon(int index)
        {
            if (currentWeaponObject != null) // 기존에 소환된 무기 오브젝트가 있다면
                Destroy(currentWeaponObject); // 즉시 삭제한다

            CurrentWeapon = weaponSlots[index]; // 현재 장착 데이터 갱신

            // 프리팹을 소켓 자식으로 생성하고 위치와 회전값을 초기화한다
            currentWeaponObject = Instantiate(CurrentWeapon.weaponPrefab, weaponSocket);
            currentWeaponObject.transform.localPosition = Vector3.zero;
            currentWeaponObject.transform.localRotation = Quaternion.identity;

            // 플레이어 상태 데이터(PlayerState)에 장착 여부와 종류를 기록한다
            if (state != null)
            {
                state.IsEquipped = true;
                state.IsBattleAxeEquipped = CurrentWeapon.weaponType == WeaponType.BattleAxe;
            }

            Debug.Log($"무기 장착: {CurrentWeapon.name}");
        }

        // 현재 씬의 무기 오브젝트를 제거하고 장착 관련 상태를 초기화함
        private void UnequipWeapon()
        {
            if (currentWeaponObject != null) // 삭제할 무기 실체가 존재하는 경우
            {
                Destroy(currentWeaponObject);
                currentWeaponObject = null;
            }

            CurrentWeapon = null; // 장착 데이터 참조 제거

            // 플레이어 상태 데이터의 장착 관련 플래그를 모두 해제한다
            if (state != null)
            {
                state.IsEquipped = false;
                state.IsBattleAxeEquipped = false;
            }

            Debug.Log("무기 해제");
        }
        #endregion
    }
}
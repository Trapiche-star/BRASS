using UnityEngine;

namespace BRASS
{
    /// 무기 데이터를 슬롯 단위로 관리하고 선택된 무기를 손 소켓에 장착하는 클래스
    public class WeaponHandler : MonoBehaviour
    {
        #region Variables
        [SerializeField] private WeaponData[] weaponSlots; // 장착 가능한 무기 데이터들을 담는 배열 슬롯
        [SerializeField] private Transform weaponSocket; // 무기 모델이 배치될 캐릭터의 손 부위 트랜스폼

        private GameObject currentWeaponObject; // 현재 게임 화면에 생성되어 있는 무기 오브젝트
        #endregion

        #region Property
        public WeaponData CurrentWeapon { get; private set; } // 현재 어떤 무기 데이터를 사용 중인지 나타내는 프로퍼티
        #endregion

        #region Unity Event Methods
        private void Start()
        {
            EquipWeaponByIndex(0); // 게임이 시작되면 인벤토리의 0번 슬롯 무기를 기본으로 장착함
        }
        #endregion

        #region Custom Methods
        // 인덱스 번호를 전달받아 해당 슬롯의 무기를 생성하고 교체함
        public void EquipWeaponByIndex(int index)
        {
            // 무기 슬롯 배열이 비어있거나 할당되지 않았다면 로직을 실행하지 않음
            if (weaponSlots == null || weaponSlots.Length == 0)
                return;

            // 요청한 인덱스가 배열의 범위를 벗어날 경우 비정상 접근으로 판단하여 중단함
            if (index < 0 || index >= weaponSlots.Length)
                return;

            WeaponData newWeapon = weaponSlots[index]; // 지정된 인덱스에서 새로운 무기 데이터를 참조함

            // 데이터가 비어있거나 생성할 프리팹 정보가 없다면 장착 과정을 취소함
            if (newWeapon == null || newWeapon.weaponPrefab == null)
                return;

            CurrentWeapon = newWeapon; // 현재 사용 중인 무기 정보를 새 무기 데이터로 갱신함

            // 기존에 장착되어 있던 무기 오브젝트가 존재하면 메모리에서 제거함
            if (currentWeaponObject != null)
                Destroy(currentWeaponObject);

            // 새 무기 프리팹을 소켓의 자식으로 생성하여 위치를 귀속시킴
            currentWeaponObject = Instantiate(
                newWeapon.weaponPrefab,
                weaponSocket
            );

            currentWeaponObject.transform.localPosition = Vector3.zero; // 생성된 무기 위치를 소켓 정중앙으로 초기화함
            currentWeaponObject.transform.localRotation = Quaternion.identity; // 생성된 무기 회전값을 소켓 기준으로 초기화함
        }
        #endregion
    }
}
using UnityEngine;
using UnityEngine.InputSystem;

namespace BRASS
{
    /// 무기 데이터를 관리하며 타입별(Gun/Melee) 소켓 보정 및 상태 동기화를 담당하는 핸들러
    public class WeaponHandler : MonoBehaviour
    {
        #region Variables
        [SerializeField] private WeaponData[] weaponSlots; // 사용 가능한 무기 데이터 슬롯 배열
        [SerializeField] private Transform weaponSocket; // 무기가 부착될 캐릭터의 손 소켓(Socket)
        [SerializeField] private PlayerState state; // 장착 여부 및 타입 정보를 동기화할 상태 데이터
        [SerializeField] private PlayerCombat combat; // 현재 무기의 대미지 판정 정보를 전달할 컴포넌트

        private GameObject currentWeaponObject; // 씬에 생성되어 현재 쥐고 있는 무기 오브젝트
        #endregion

        #region Property
        public WeaponData CurrentWeapon { get; private set; } // 현재 논리적으로 장착된 무기 자산 정보
        #endregion

        #region Unity Event Method
        private void Update()
        {
            // 테스트용: ] 키를 누르면 1번 슬롯의 무기를 토글한다
            if (Keyboard.current != null && Keyboard.current.rightBracketKey.wasPressedThisFrame)
            {
                ToggleWeaponByIndex(1);
            }
        }
        #endregion

        #region Custom Methods
        // 인덱스를 확인하여 무기를 장착하거나 이미 장착된 경우 해제 처리함
        public void ToggleWeaponByIndex(int index)
        {
            if (weaponSlots == null || index < 0 || index >= weaponSlots.Length) return; // 슬롯 범위 초과 시 중단

            WeaponData targetWeapon = weaponSlots[index];
            if (targetWeapon == null || targetWeapon.weaponPrefab == null) return; // 데이터 누락 시 중단

            if (CurrentWeapon == targetWeapon) // 이미 들고 있는 무기라면
                UnequipWeapon(); // 무기 해제 실행
            else // 새로운 무기라면
                EquipWeapon(index); // 무기 장착 실행
        }

        // 선택한 슬롯의 무기 프리팹을 생성하고 타입에 따른 위치 보정을 수행함
        private void EquipWeapon(int index)
        {
            if (currentWeaponObject != null) // 기존 무기가 있다면
                Destroy(currentWeaponObject); // 이전 무기를 파괴하여 정리한다

            CurrentWeapon = weaponSlots[index]; // 현재 무기 정보 갱신
            currentWeaponObject = Instantiate(CurrentWeapon.weaponPrefab, weaponSocket); // 소켓 자식으로 무기 생성

            // 총기(Gun) 타입 판별 (상태 플래그와 연결됨)
            bool isGun = CurrentWeapon.weaponType == WeaponType.HarpoonGun;

            if (isGun) // 총기 타입인 경우 핸들 위치 보정 로직을 실행한다
            {
                Transform handle = currentWeaponObject.transform.Find("HandlePivot"); // 무기 내부의 Handle 오브젝트 탐색
                if (handle != null) // 핸들이 존재한다면
                {
                    // 핸들이 소켓 좌표의 원점에 오도록 무기 루트의 상대 위치를 역계산한다
                    currentWeaponObject.transform.localPosition = -handle.localPosition;
                    currentWeaponObject.transform.localRotation = Quaternion.Inverse(handle.localRotation);
                }
                else // 핸들이 없다면 기본 정렬을 수행한다
                {
                    currentWeaponObject.transform.localPosition = Vector3.zero;
                    currentWeaponObject.transform.localRotation = Quaternion.identity;
                }
            }
            else // 총기가 아닐 경우 일반적인 원점 정렬을 수행한다
            {
                currentWeaponObject.transform.localPosition = Vector3.zero;
                currentWeaponObject.transform.localRotation = Quaternion.identity;
            }

            // 전투 컴포넌트에 현재 무기의 대미지 스크립트를 등록한다
            if (combat != null)
            {
                WeaponDamage weaponDamage = currentWeaponObject.GetComponent<WeaponDamage>();
                combat.SetCurrentWeapon(weaponDamage); // 판정 시스템에 무기 연결
            }

            // 플레이어 상태 데이터를 갱신하여 애니메이터와 레이어 시스템에 반영한다
            if (state != null)
            {
                state.IsEquipped = true; // 장착 플래그 활성
                state.IsBattleAxeEquipped = CurrentWeapon.weaponType == WeaponType.BattleAxe;
                state.IsGunEquipped = isGun; // 총기 레이어(Layer) 활성화를 위해 전달
            }

            Debug.Log($"무기 장착: {CurrentWeapon.name} (총기 여부: {isGun})");
        }

        // 현재 장착된 무기를 파괴하고 모든 상태 플래그를 초기화함
        private void UnequipWeapon()
        {
            if (currentWeaponObject != null) // 제거할 무기 실체가 있다면
            {
                Destroy(currentWeaponObject); // 오브젝트 제거
                currentWeaponObject = null; // 참조 변수 비우기
            }

            if (combat != null) combat.SetCurrentWeapon(null); // 전투 컴포넌트 참조 초기화

            CurrentWeapon = null; // 데이터 참조 초기화

            if (state != null) // 상태 데이터의 모든 장착 관련 플래그를 거짓으로 변경한다
            {
                state.IsEquipped = false;
                state.IsBattleAxeEquipped = false;
                state.IsGunEquipped = false; // 총기 레이어 비활성화
            }

            Debug.Log("무기 해제");
        }
        #endregion
    }
}
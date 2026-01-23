using UnityEngine;

namespace Team1
{
    public class TestItemSpawner : MonoBehaviour
    {
        [SerializeField] private ConsumableItemData testItemData;
        private Inventory inventory;

        private void Start()
        {
            inventory = GetComponent<Inventory>();

            if (inventory == null)
            {
                Debug.LogError("❌ Inventory 컴포넌트 없음");
                return;
            }

            if (testItemData == null)
            {
                Debug.LogError("❌ Test Item Data 연결 안됨");
                return;
            }

            // ✅ 템플릿 기반으로 아이템 생성
            inventory.AddItem(testItemData.CreateItem());
            inventory.AddItem(testItemData.CreateItem());   // 수량 증가 테스트
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                inventory.UseItem(0);
            }
        }
    }
}

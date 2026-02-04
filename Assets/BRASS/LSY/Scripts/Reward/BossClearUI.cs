using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Collections;

namespace Team1
{
    public class BossClearUI : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private GameObject rewardPanel;
        [SerializeField] private TextMeshProUGUI goldResultText;
        [SerializeField] private Transform itemSlotParent; // 아이템 아이콘들이 생성될 부모 오브젝트
        [SerializeField] private GameObject itemIconPrefab; // 아이템 아이콘 표시용 프리팹 (Image 컴포넌트 포함)

        [Header("연출 설정")]
        [SerializeField] private float goldCountDuration = 1.5f; // 숫자가 올라가는 총 시간

        [Header("Reward Data")]
        [SerializeField] private BossRewardTable rewardTable;
        [SerializeField] private Inventory playerInventory;

        public void ShowReward()
        {
            rewardPanel.SetActive(true);
            ClearPreviousItems();

            // 1. 랜덤 골드 결정
            int randomGold = Random.Range(1000, 1501);
            GoldManager.Instance.AddGold(randomGold);

            // 2. 숫자 올라가는 연출 시작
            StopAllCoroutines(); // 혹시 실행 중인 연출이 있다면 중지
            StartCoroutine(CountGoldRoutine(randomGold));

            // 2. 전리품 랜덤 생성 (1 ~ 2개)
            int itemAmount = Random.Range(1, 3);
            ClearPreviousItems(); // 기존에 떠있던 아이콘 삭제

            for (int i = 0; i < itemAmount; i++)
            {
                if (rewardTable.possibleItems.Count > 0)
                {
                    // 랜덤 아이템 선택
                    int randomIndex = Random.Range(0, rewardTable.possibleItems.Count);
                    ConsumableItemData selectedData = rewardTable.possibleItems[randomIndex];

                    // 실제 아이템 생성 및 인벤토리 추가
                    ConsumableItem newItem = selectedData.CreateItem();
                    playerInventory.AddItem(newItem);

                    // UI에 아이콘 표시
                    CreateItemIcon(selectedData.icon);
                }
            }

            Debug.Log($"보스 보상: {randomGold}G와 아이템 {itemAmount}개 획득!");
        }

        private void CreateItemIcon(Sprite icon)
        {
            if (itemIconPrefab != null && itemSlotParent != null)
            {
                GameObject iconObj = Instantiate(itemIconPrefab, itemSlotParent);
                // 아이콘 이미지 설정 (프리팹에 아이콘 이미지를 담당하는 컴포넌트가 있다고 가정)
                var image = iconObj.GetComponent<UnityEngine.UI.Image>();
                if (image != null) image.sprite = icon;
            }
        }

        private void ClearPreviousItems()
        {
            foreach (Transform child in itemSlotParent)
            {
                Destroy(child.gameObject);
            }
        }

        // UI 닫기 버튼용
        public void CloseUI()
        {
            rewardPanel.SetActive(false);
        }
        private IEnumerator CountGoldRoutine(int targetGold)
        {
            float elapsed = 0f;
            int currentDisplayGold = 0;

            while (elapsed < goldCountDuration)
            {
                elapsed += Time.unscaledDeltaTime; // UI이므로 게임 정지 상태에서도 흐르게 unscaled 사용

                // 경과 시간에 따라 0부터 targetGold까지 보간
                float progress = elapsed / goldCountDuration;
                currentDisplayGold = (int)Mathf.Lerp(0, targetGold, progress);

                // 텍스트 업데이트 (N0는 천 단위 콤마 추가)
                goldResultText.text = $"{currentDisplayGold:N0} G";

                yield return null; // 다음 프레임까지 대기
            }

            // 마지막에 정확한 수치로 고정
            goldResultText.text = $"{targetGold:N0} G";
        }
    }
}
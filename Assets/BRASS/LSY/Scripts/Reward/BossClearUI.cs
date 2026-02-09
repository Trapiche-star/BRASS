using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using System.Collections;

namespace Team1
{
    public class BossClearUI : MonoBehaviour
    {
        [Header("실제 끄고 켤 UI 판넬")]
        public GameObject uiPanel; // 보상 창 부모 오브젝트

        [Header("보상 데이터 설정")]
        public BossRewardTable rewardTable;    // 보상 아이템 목록 에셋
        public Image itemIcon;                 // 보상 아이템 아이콘 표시
        public TextMeshProUGUI goldText;       // 보상 골드 텍스트 (다라라락 연출용)

        [Header("제어할 마을/보스 오브젝트")]
        public GameObject marketObject;        // 마을 마켓
        public GameObject mapObject;           // 마을 지도
        public GameObject bossObject;          // 현재 전투 중인 보스

        private Inventory inventory;           // 인벤토리 참조

        private void Awake()
        {
            // 씬 내에서 인벤토리 스크립트를 자동으로 찾습니다.
            inventory = Object.FindFirstObjectByType<Inventory>();

            // 시작할 때는 보상 UI가 꺼져 있어야 합니다.
            if (uiPanel != null) uiPanel.SetActive(false);
        }

        /// <summary>
        /// 보스가 죽거나 치트키를 누를 때 호출되는 메인 함수
        /// </summary>
        public void ShowReward()
        {
            Debug.Log("🏆 BossClearUI: 보상 프로세스 시작!");

            // 1. UI 판넬 활성화
            if (uiPanel != null) uiPanel.SetActive(true);

            // 2. 마을 UI는 보상 확인 중에는 꺼둡니다.
            if (marketObject) marketObject.SetActive(false);
            if (mapObject) mapObject.SetActive(false);

            // 3. 랜덤 보상 생성 및 실제 데이터 연동
            SetRandomReward();
        }

        private void SetRandomReward()
        {
            if (rewardTable == null || rewardTable.possibleItems.Count == 0) return;

            // 1. 랜덤 데이터 선택
            int randomIndex = Random.Range(0, rewardTable.possibleItems.Count);
            ConsumableItemData selectedData = rewardTable.possibleItems[randomIndex];

            if (selectedData != null)
            {
                // UI 아이콘 표시
                if (itemIcon != null) itemIcon.sprite = selectedData.icon;

                // ⭐ 핵심: abstract 구조에 맞게 아이템 생성 호출
                // CreateItem()은 이미 하위 클래스에서 ConsumableItem을 리턴하도록 설계되어 있습니다.
                ConsumableItem realItem = selectedData.CreateItem();

                if (inventory != null && realItem != null)
                {
                    // Inventory.AddItem(IItem item)은 ConsumableItem(IItem 상속)을 받을 수 있습니다.
                    inventory.AddItem(realItem);
                    Debug.Log($"🎒 보상 획득: {realItem.ItemName}");
                }
            }

            // 2. 골드 처리 (기존과 동일)
            int rewardGold = Random.Range(100, 501);
            if (GoldManager.Instance != null) GoldManager.Instance.AddGold(rewardGold);

            StopAllCoroutines();
            StartCoroutine(CountGoldRoutine(rewardGold));
        }

        /// <summary>
        /// 골드 숫자가 0에서 목표치까지 다라라락 올라가는 연출
        /// </summary>
        private IEnumerator CountGoldRoutine(int targetGold)
        {
            float duration = 0.8f; // 연출 시간 (0.8초)
            float timer = 0;

            while (timer < duration)
            {
                timer += Time.deltaTime;
                // 선형 보간을 사용하여 숫자 계산
                int currentDisplay = (int)Mathf.Lerp(0, targetGold, timer / duration);

                if (goldText != null)
                    goldText.text = currentDisplay.ToString("N0") + " G";

                yield return null;
            }

            // 마지막 값 고정
            if (goldText != null)
                goldText.text = targetGold.ToString("N0") + " G";
        }

        /// <summary>
        /// 보상 UI의 Exit 버튼에 연결할 함수
        /// </summary>
        public void OnExitButtonClicked()
        {
            Debug.Log("🚪 전투 종료: 마을로 복귀 및 보스 제거");

            // 1. 보스 오브젝트 비활성화 또는 파괴
            if (bossObject != null) bossObject.SetActive(false);

            // 2. 마을 마켓과 지도 UI 다시 켜기
            if (marketObject) marketObject.SetActive(true);
            if (mapObject) mapObject.SetActive(true);

            // 3. 보상 UI 자신을 비활성화
            if (uiPanel != null) uiPanel.SetActive(false);
        }
    }
}
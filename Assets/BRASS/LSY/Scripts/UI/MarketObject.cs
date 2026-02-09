using UnityEngine;
using BRASS;
using Team1;

public class MarketObject : MonoBehaviour, IInteractable
{
    public void Interact()
    {
        Debug.Log("<color=yellow>MarketObject: Interact 시작</color>");

        if (UIManager_SY.Instance != null)
        {
            // 1. 일단 켜기 시도
            UIManager_SY.Instance.OpenShop();

            // 2. 켜졌는지 확인 로그
            GameObject shop = UIManager_SY.Instance.shopPanel;
            if (shop != null)
            {
                Debug.Log($"[체크] 상점 패널 이름: {shop.name} | 활성화 상태: {shop.activeSelf}");

                // 3. 만약 켜졌는데 안 보인다면 위치 확인
                RectTransform rect = shop.GetComponent<RectTransform>();
                if (rect != null)
                {
                    Debug.Log($"[위치체크] 좌표: {rect.anchoredPosition} | 크기: {rect.sizeDelta}");
                }
            }
        }
        else
        {
            Debug.LogError("UIManager_SY 인스턴스가 비어있습니다!");
        }
    }
}
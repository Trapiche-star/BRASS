using UnityEngine;
using TMPro;
using System;

public class GoldManager : MonoBehaviour
{
    [Header("UI References (HUD)")]
    [SerializeField] private TextMeshProUGUI mainGoldText;

    [Header("Gold Settings")]
    [SerializeField] private int maxGold = 999999;

#if UNITY_EDITOR
    [SerializeField] private int currentGold = 0;
#else
    private int currentGold = 0;
#endif

    private const string GOLD_SAVE_KEY = "PlayerGold";

    // ✅ 방송국 역할: 골드가 변할 때마다 등록된 모든 UI에게 신호를 보냄
    public static event Action<int> OnGoldChanged;

    public static GoldManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        LoadGold();
        // UI 시스템이 완전히 로드된 후 갱신되도록 약간의 지연 호출
        Invoke(nameof(UpdateGoldUI), 0.1f);
    }

    public void AddGold(int amount)
    {
        if (amount < 0) return;
        currentGold = Mathf.Min(currentGold + amount, maxGold);
        SaveAndRefresh();
    }

    public bool RemoveGold(int amount)
    {
        if (amount < 0 || currentGold < amount) return false;
        currentGold -= amount;
        SaveAndRefresh();
        return true;
    }

    // ✅ GoldTester와 연동을 위해 SetGold 함수 유지
    public void SetGold(int amount)
    {
        currentGold = Mathf.Clamp(amount, 0, maxGold);
        SaveAndRefresh();
    }

    // ✅ GoldTester와 연동을 위해 ResetGold 함수 유지
    public void ResetGold()
    {
        currentGold = 0;
        SaveAndRefresh();
        Debug.Log("<color=white>골드가 초기화되었습니다.</color>");
    }

    private void SaveAndRefresh()
    {
        SaveGold();
        UpdateGoldUI();
    }

    public void UpdateGoldUI()
    {
        // 1. 메인 HUD(항상 보이는 화면) 갱신
        if (mainGoldText != null)
        {
            mainGoldText.text = currentGold.ToString("N0");
        }

        // 2. 이 방송을 듣고 있는 인벤토리 등 모든 UI에게 전달
        OnGoldChanged?.Invoke(currentGold);
    }

    private void SaveGold()
    {
        PlayerPrefs.SetInt(GOLD_SAVE_KEY, currentGold);
        PlayerPrefs.Save();
    }

    private void LoadGold() => currentGold = PlayerPrefs.GetInt(GOLD_SAVE_KEY, 0);

    public int GetCurrentGold() => currentGold;
    public bool HasEnoughGold(int amount) => currentGold >= amount;
}
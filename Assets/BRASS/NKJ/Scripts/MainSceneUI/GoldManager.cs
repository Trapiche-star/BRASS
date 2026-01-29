using UnityEngine;
using TMPro;

public class GoldManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI goldText;

    [Header("Gold Settings")]
    [SerializeField] private int maxGold = 999999;

#if UNITY_EDITOR
    [SerializeField] private int currentGold = 0;
#else
    private int currentGold = 0;
#endif

    private const string GOLD_SAVE_KEY = "PlayerGold";

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
        UpdateGoldUI();
    }

    public void AddGold(int amount)
    {
        if (amount < 0)
        {
            Debug.LogWarning("골드는 음수로 추가할 수 없습니다.");
            return;
        }

        currentGold = Mathf.Min(currentGold + amount, maxGold);
        SaveGold();
        UpdateGoldUI();
    }

    public bool RemoveGold(int amount)
    {
        if (amount < 0)
        {
            Debug.LogWarning("차감 금액은 양수여야 합니다.");
            return false;
        }

        if (currentGold < amount)
        {
            Debug.Log("골드가 부족합니다!");
            return false;
        }

        currentGold -= amount;
        SaveGold();
        UpdateGoldUI();
        return true;
    }

    public void SetGold(int amount)
    {
        currentGold = Mathf.Clamp(amount, 0, maxGold);
        SaveGold();
        UpdateGoldUI();
    }

    private void UpdateGoldUI()
    {
        if (goldText != null)
        {
            goldText.text = currentGold.ToString("N0");
        }
    }

    private void SaveGold()
    {
        PlayerPrefs.SetInt(GOLD_SAVE_KEY, currentGold);
        PlayerPrefs.Save();
    }

    private void LoadGold()
    {
        currentGold = PlayerPrefs.GetInt(GOLD_SAVE_KEY, 0);
    }

    public void ResetGold()
    {
        currentGold = 0;
        SaveGold();
        UpdateGoldUI();
        Debug.Log("골드 초기화 완료");
    }

    public int GetCurrentGold() => currentGold;
    public int GetMaxGold() => maxGold;
    public bool HasEnoughGold(int amount) => currentGold >= amount;
}
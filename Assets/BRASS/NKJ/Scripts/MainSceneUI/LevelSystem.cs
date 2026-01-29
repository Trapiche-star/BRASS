//로컬저장 추가
using UnityEngine;
using TMPro;
// using UnityEngine.UI;

public class LevelSystem : MonoBehaviour
{
    [Header("Level Settings")]
    [SerializeField] private int currentLevel = 1;
    [SerializeField] private int maxLevel = 99;
    [SerializeField] private int currentExp = 0;
    [SerializeField] private int expToLevelUp = 100;

    [Header("UI Reference")]
    [SerializeField] private TextMeshProUGUI levelText;
    // [SerializeField] private Text levelText;

    [Header("Gauge Reference")]
    [SerializeField] private GaugeController gaugeController;

    [Header("Save Settings")]
    [SerializeField] private bool autoSave = true; // 자동 저장 활성화
    [SerializeField] private float autoSaveInterval = 5f; // 5초마다 자동 저장

    private float saveTimer = 0f;

    // PlayerPrefs 키 이름
    private const string SAVE_KEY_LEVEL = "PlayerLevel";
    private const string SAVE_KEY_EXP = "PlayerExp";

    void Start()
    {
        // 저장된 데이터 불러오기
        LoadGameData();

        // 게임 시작시 게이지를 즉시 설정
        if (gaugeController != null)
        {
            gaugeController.currentExp = currentExp;
            if (gaugeController.expFill != null)
            {
                gaugeController.expFill.fillAmount = (float)currentExp / expToLevelUp;
            }
        }

        UpdateLevelUI();
        UpdateExpGauge();

        Debug.Log($"Game Started - Level: {currentLevel}, Exp: {currentExp}");
    }

    void OnValidate()
    {
        currentLevel = Mathf.Clamp(currentLevel, 1, maxLevel);
        currentExp = Mathf.Max(0, currentExp);
        expToLevelUp = Mathf.Max(1, expToLevelUp);

        if (Application.isPlaying)
        {
            UpdateLevelUI();
            UpdateExpGauge();
        }
    }

    void Update()
    {
        CheckLevelUp();

        // 자동 저장
        if (autoSave)
        {
            saveTimer += Time.deltaTime;
            if (saveTimer >= autoSaveInterval)
            {
                SaveGameData();
                saveTimer = 0f;
            }
        }
    }

    private void CheckLevelUp()
    {
        bool leveledUp = false;

        while (currentExp >= expToLevelUp && currentLevel < maxLevel)
        {
            currentExp -= expToLevelUp;
            currentLevel++;
            leveledUp = true;
            Debug.Log($"Level Up! Current Level: {currentLevel}");
            UpdateLevelUI();

            // 레벨업시 즉시 저장
            SaveGameData();
        }

        if (leveledUp)
        {
            ResetExpGauge();
        }
        else
        {
            UpdateExpGauge();
        }

        if (currentLevel >= maxLevel)
        {
            currentLevel = maxLevel;
            currentExp = Mathf.Min(currentExp, expToLevelUp - 1);
            UpdateExpGauge();
        }

        if (currentLevel < 1)
        {
            currentLevel = 1;
            currentExp = 0;
            UpdateLevelUI();
            UpdateExpGauge();
        }
    }

    public void AddExperience(int amount)
    {
        if (currentLevel >= maxLevel)
        {
            Debug.Log("Max level reached! No more experience gained.");
            return;
        }

        currentExp += amount;
        currentExp = Mathf.Max(0, currentExp);

        Debug.Log($"Experience added: {amount}, Current Exp: {currentExp}/{expToLevelUp}");
    }

    private void UpdateLevelUI()
    {
        if (levelText != null)
        {
            levelText.text = currentLevel.ToString();
            Debug.Log($"UI Updated: Level {currentLevel}, Exp: {currentExp}/{expToLevelUp}");
        }
        else
        {
            Debug.LogError("Level Text is not assigned!");
        }
    }

    private void UpdateExpGauge()
    {
        if (gaugeController != null)
        {
            gaugeController.maxExp = expToLevelUp;
            gaugeController.SetExp(currentExp);
        }
    }

    private void ResetExpGauge()
    {
        if (gaugeController != null)
        {
            gaugeController.maxExp = expToLevelUp;
            gaugeController.currentExp = 0;
            if (gaugeController.expFill != null)
            {
                gaugeController.expFill.fillAmount = 0;
            }
            gaugeController.SetExp(currentExp);
        }
    }

    // 저장 기능
    public void SaveGameData()
    {
        PlayerPrefs.SetInt(SAVE_KEY_LEVEL, currentLevel);
        PlayerPrefs.SetInt(SAVE_KEY_EXP, currentExp);
        PlayerPrefs.Save(); // 즉시 디스크에 저장

        Debug.Log($"Game Saved - Level: {currentLevel}, Exp: {currentExp}");
    }

    // 불러오기 기능
    public void LoadGameData()
    {
        if (PlayerPrefs.HasKey(SAVE_KEY_LEVEL))
        {
            currentLevel = PlayerPrefs.GetInt(SAVE_KEY_LEVEL, 1);
            currentExp = PlayerPrefs.GetInt(SAVE_KEY_EXP, 0);

            // 불러온 데이터 검증
            currentLevel = Mathf.Clamp(currentLevel, 1, maxLevel);
            currentExp = Mathf.Max(0, currentExp);

            Debug.Log($"Game Loaded - Level: {currentLevel}, Exp: {currentExp}");
        }
        else
        {
            // 저장된 데이터가 없으면 기본값
            currentLevel = 1;
            currentExp = 0;
            Debug.Log("No save data found. Starting new game.");
        }
    }

    // 데이터 초기화 (새 게임 시작)
    public void ResetGameData()
    {
        currentLevel = 1;
        currentExp = 0;

        PlayerPrefs.DeleteKey(SAVE_KEY_LEVEL);
        PlayerPrefs.DeleteKey(SAVE_KEY_EXP);
        PlayerPrefs.Save();

        UpdateLevelUI();
        ResetExpGauge();

        Debug.Log("Game data reset! Starting from Level 1.");
    }

    // 게임 종료시 저장
    void OnApplicationQuit()
    {
        SaveGameData();
    }

    // 게임 일시정지시 저장 (모바일용)
    void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            SaveGameData();
        }
    }

    public void SetLevel(int level)
    {
        currentLevel = Mathf.Clamp(level, 1, maxLevel);
        UpdateLevelUI();
        UpdateExpGauge();
        SaveGameData();
    }

    public void SetExperience(int exp)
    {
        if (currentLevel >= maxLevel)
            return;

        currentExp = Mathf.Max(0, exp);
        UpdateExpGauge();
    }

    public int GetCurrentLevel() => currentLevel;
    public int GetCurrentExp() => currentExp;
    public int GetExpToLevelUp() => expToLevelUp;
    public int GetMaxLevel() => maxLevel;
}
/*
 * 로컬저장 없는 기본

using UnityEngine;
using TMPro;
// using UnityEngine.UI;

public class LevelSystem : MonoBehaviour
{
    [Header("Level Settings")]
    [SerializeField] private int currentLevel = 1;
    [SerializeField] private int maxLevel = 99;
    [SerializeField] private int currentExp = 0;
    [SerializeField] private int expToLevelUp = 100;

    [Header("UI Reference")]
    [SerializeField] private TextMeshProUGUI levelText;
    // [SerializeField] private Text levelText;

    [Header("Gauge Reference")]
    [SerializeField] private GaugeController gaugeController;

    void Start()
    {
        currentLevel = 1;
        currentExp = 0;

        // 게임 시작시 게이지를 즉시 0으로 설정
        if (gaugeController != null)
        {
            gaugeController.currentExp = 0;
            if (gaugeController.expFill != null)
            {
                gaugeController.expFill.fillAmount = 0;
            }
        }

        UpdateLevelUI();
        UpdateExpGauge();
    }

    void OnValidate()
    {
        currentLevel = Mathf.Clamp(currentLevel, 1, maxLevel);
        currentExp = Mathf.Max(0, currentExp);
        expToLevelUp = Mathf.Max(1, expToLevelUp);

        if (Application.isPlaying)
        {
            UpdateLevelUI();
            UpdateExpGauge();
        }
    }

    void Update()
    {
        CheckLevelUp();
    }

    private void CheckLevelUp()
    {
        bool leveledUp = false;

        while (currentExp >= expToLevelUp && currentLevel < maxLevel)
        {
            currentExp -= expToLevelUp;
            currentLevel++;
            leveledUp = true;
            Debug.Log($"Level Up! Current Level: {currentLevel}");
            UpdateLevelUI();
        }

        if (leveledUp)
        {
            ResetExpGauge();
        }
        else
        {
            UpdateExpGauge();
        }

        if (currentLevel >= maxLevel)
        {
            currentLevel = maxLevel;
            currentExp = Mathf.Min(currentExp, expToLevelUp - 1);
            UpdateExpGauge();
        }

        if (currentLevel < 1)
        {
            currentLevel = 1;
            currentExp = 0;
            UpdateLevelUI();
            UpdateExpGauge();
        }
    }

    public void AddExperience(int amount)
    {
        if (currentLevel >= maxLevel)
        {
            Debug.Log("Max level reached! No more experience gained.");
            return;
        }

        currentExp += amount;
        currentExp = Mathf.Max(0, currentExp);

        Debug.Log($"Experience added: {amount}, Current Exp: {currentExp}/{expToLevelUp}");
    }

    private void UpdateLevelUI()
    {
        if (levelText != null)
        {
            levelText.text = currentLevel.ToString();
            Debug.Log($"UI Updated: Level {currentLevel}, Exp: {currentExp}/{expToLevelUp}");
        }
        else
        {
            Debug.LogError("Level Text is not assigned!");
        }
    }

    private void UpdateExpGauge()
    {
        if (gaugeController != null)
        {
            gaugeController.maxExp = expToLevelUp;
            gaugeController.SetExp(currentExp);
        }
    }

    private void ResetExpGauge()
    {
        if (gaugeController != null)
        {
            gaugeController.maxExp = expToLevelUp;
            gaugeController.currentExp = 0;
            if (gaugeController.expFill != null)
            {
                gaugeController.expFill.fillAmount = 0;
            }
            gaugeController.SetExp(currentExp);
        }
    }

    public void SetLevel(int level)
    {
        currentLevel = Mathf.Clamp(level, 1, maxLevel);
        UpdateLevelUI();
        UpdateExpGauge();
    }

    public void SetExperience(int exp)
    {
        if (currentLevel >= maxLevel)
            return;

        currentExp = Mathf.Max(0, exp);
        UpdateExpGauge();
    }

    public int GetCurrentLevel() => currentLevel;
    public int GetCurrentExp() => currentExp;
    public int GetExpToLevelUp() => expToLevelUp;
    public int GetMaxLevel() => maxLevel;
}
*/
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

    void Start()
    {
        currentLevel = 1;
        currentExp = 0;
        UpdateLevelUI();
    }

    void OnValidate()
    {
        // Inspector에서 값 조절할 때 제한 적용
        currentLevel = Mathf.Clamp(currentLevel, 1, maxLevel);
        currentExp = Mathf.Max(0, currentExp);
        expToLevelUp = Mathf.Max(1, expToLevelUp);

        if (Application.isPlaying)
        {
            UpdateLevelUI();
        }
    }

    void Update()
    {
        // 경험치가 레벨업 수치에 도달하면 레벨업
        CheckLevelUp();
    }

    private void CheckLevelUp()
    {
        // 최대 레벨 아니고, 경험치가 충분하면 레벨업
        while (currentExp >= expToLevelUp && currentLevel < maxLevel)
        {
            currentExp -= expToLevelUp;
            currentLevel++;
            Debug.Log($"Level Up! Current Level: {currentLevel}");
            UpdateLevelUI();
        }

        // 최대 레벨 도달시 경험치 초과분 제거
        if (currentLevel >= maxLevel)
        {
            currentLevel = maxLevel; // 확실하게 최대 레벨로 고정
            currentExp = Mathf.Min(currentExp, expToLevelUp - 1); // 경험치를 최대치 미만으로 제한
        }

        // 레벨이 1 미만으로 내려가지 않도록
        if (currentLevel < 1)
        {
            currentLevel = 1;
            currentExp = 0;
            UpdateLevelUI();
        }
    }

    public void AddExperience(int amount)
    {
        // 최대 레벨이면 경험치 추가 안함
        if (currentLevel >= maxLevel)
        {
            Debug.Log("Max level reached! No more experience gained.");
            return;
        }

        currentExp += amount;
        currentExp = Mathf.Max(0, currentExp); // 음수 방지

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

    // 외부에서 값을 설정할 때도 제한 적용
    public void SetLevel(int level)
    {
        currentLevel = Mathf.Clamp(level, 1, maxLevel);
        UpdateLevelUI();
    }

    public void SetExperience(int exp)
    {
        if (currentLevel >= maxLevel)
            return;

        currentExp = Mathf.Max(0, exp);
    }

    // Getter 메서드들
    public int GetCurrentLevel() => currentLevel;
    public int GetCurrentExp() => currentExp;
    public int GetExpToLevelUp() => expToLevelUp;
    public int GetMaxLevel() => maxLevel;
}
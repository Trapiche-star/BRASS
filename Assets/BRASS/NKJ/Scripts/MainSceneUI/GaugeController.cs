using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class GaugeController : MonoBehaviour
{
    [Header("Gauge Fill Objects")]
    public Image expFill;
    public Image hpFill;
    public Image mpFill;

    [Header("Current Values (For Testing)")]
    [Range(0, 100)] public float currentExp = 100f;
    [Range(0, 100)] public float currentHp = 100f;
    [Range(0, 100)] public float currentMp = 100f;

    [Header("Max Values")]
    public float maxExp = 100f;
    public float maxHp = 100f;
    public float maxMp = 100f;

    [Header("Gauge Animation Speed")]
    public float fillSpeed = 2f;

    private void Start()
    {
        // Check if Fill objects are assigned
        Debug.Log("=== GaugeController Start ===");
        Debug.Log($"HP Fill assigned: {hpFill != null}");
        Debug.Log($"MP Fill assigned: {mpFill != null}");
        Debug.Log($"EXP Fill assigned: {expFill != null}");

        if (hpFill != null) Debug.Log($"HP Fill Amount: {hpFill.fillAmount}");
        if (mpFill != null) Debug.Log($"MP Fill Amount: {mpFill.fillAmount}");
        if (expFill != null) Debug.Log($"EXP Fill Amount: {expFill.fillAmount}");

        // Initial gauge setup
        UpdateGauges();

        // Show initial status
        Debug.Log("=== Initial Gauge Status ===");
        Debug.Log($"[HP] {currentHp}/{maxHp} ({(currentHp / maxHp) * 100:F1}%)");
        Debug.Log($"[MP] {currentMp}/{maxMp} ({(currentMp / maxMp) * 100:F1}%)");
        Debug.Log($"[EXP] {currentExp}/{maxExp} ({(currentExp / maxExp) * 100:F1}%)");
    }

    private void Update()
    {
        // Real-time update when values change in inspector (for testing)
        UpdateGauges();

        // Test input
        TestInput();
    }

    // Update gauges (method to call from actual game)
    public void UpdateGauges()
    {
        if (expFill != null)
            expFill.fillAmount = Mathf.Lerp(expFill.fillAmount, currentExp / maxExp, Time.deltaTime * fillSpeed);

        if (hpFill != null)
            hpFill.fillAmount = Mathf.Lerp(hpFill.fillAmount, currentHp / maxHp, Time.deltaTime * fillSpeed);

        if (mpFill != null)
            mpFill.fillAmount = Mathf.Lerp(mpFill.fillAmount, currentMp / maxMp, Time.deltaTime * fillSpeed);
    }

    // Set individual gauges (for character stat integration later)
    public void SetExp(float value)
    {
        currentExp = Mathf.Clamp(value, 0, maxExp);
        Debug.Log($"[EXP] {currentExp}/{maxExp} ({(currentExp / maxExp) * 100:F1}%)");
    }

    public void SetHp(float value)
    {
        currentHp = Mathf.Clamp(value, 0, maxHp);
        Debug.Log($"[HP] {currentHp}/{maxHp} ({(currentHp / maxHp) * 100:F1}%)");
    }

    public void SetMp(float value)
    {
        currentMp = Mathf.Clamp(value, 0, maxMp);
        Debug.Log($"[MP] {currentMp}/{maxMp} ({(currentMp / maxMp) * 100:F1}%)");
    }

    // Test input (New Input System)
    private void TestInput()
    {
        var keyboard = Keyboard.current;
        if (keyboard == null)
        {
            Debug.LogWarning("Keyboard not detected!");
            return;
        }

        // HP test (F1, F2 keys)
        if (keyboard.f1Key.wasPressedThisFrame)
        {
            Debug.Log("F1 Key Pressed!");
            SetExp(currentExp - 10);
        }
        if (keyboard.f2Key.wasPressedThisFrame)
        {
            Debug.Log("F2 Key Pressed!");
            SetExp(currentExp + 10);
        }

        // MP test (F3, F4 keys)
        if (keyboard.f3Key.wasPressedThisFrame)
        {
            Debug.Log("F3 Key Pressed!");
            SetHp(currentHp - 10);
            
        }
        if (keyboard.f4Key.wasPressedThisFrame)
        {
            Debug.Log("F4 Key Pressed!");
            SetHp(currentHp + 10);
            
        }

        // EXP test (F5, F6 keys)
        if (keyboard.f5Key.wasPressedThisFrame)
        {
            Debug.Log("F5 Key Pressed!");
            SetMp(currentMp - 10);
            
        }
        if (keyboard.f6Key.wasPressedThisFrame)
        {
            Debug.Log("F6 Key Pressed!");
            SetMp(currentMp + 10);
            
        }
    }
}

// Example of integration with character stats later
/*
public class PlayerStats : MonoBehaviour
{
    public GaugeController gaugeController;
    
    private float exp, hp, mp;

    private void Update()
    {
        // Update gauges whenever stats change
        gaugeController.SetExp(exp);
        gaugeController.SetHp(hp);
        gaugeController.SetMp(mp);
    }
}
*/
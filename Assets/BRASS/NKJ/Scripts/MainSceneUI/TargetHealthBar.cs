using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TargetHealthBar : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Slider healthSlider;
    [SerializeField] private Image fillImage;
    [SerializeField] private TextMeshProUGUI targetText;

    [Header("Health Settings")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float currentHealth = 100f;

    [Header("Color Settings")]
    [SerializeField] private Color normalColor = Color.red;        // 빨강
    [SerializeField] private Color lowHealthColor = Color.yellow;   // 노랑
    [SerializeField] private float lowHealthThreshold = 30f;        // 30% 기준
    [SerializeField] private float transitionDuration = 0.5f;       // 전환 시간 (초) - 작을수록 빠름!

    private float colorTransitionTimer = 0f;
    private Color startColor;
    private Color targetColor;
    private bool isTransitioning = false;

    void Start()
    {
        // 초기화
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }

        if (fillImage != null)
        {
            fillImage.color = normalColor;
        }

        targetColor = normalColor;
    }

    void Update()
    {
        // Slider value 실시간 업데이트 (가장 중요!)
        if (healthSlider != null)
        {
            healthSlider.value = currentHealth;
        }

        // 현재 체력 퍼센트 계산
        float healthPercent = (currentHealth / maxHealth) * 100f;

        // 목표 색상 결정
        Color newTargetColor;
        if (healthPercent <= lowHealthThreshold)
        {
            newTargetColor = lowHealthColor;  // 30% 이하면 노랑
        }
        else
        {
            newTargetColor = normalColor;  // 30% 초과면 빨강
        }

        // 목표 색상이 바뀌면 전환 시작
        if (newTargetColor != targetColor)
        {
            startColor = fillImage.color;
            targetColor = newTargetColor;
            colorTransitionTimer = 0f;
            isTransitioning = true;
            Debug.Log($"색상 전환 시작! 체력: {healthPercent}% → {(healthPercent <= lowHealthThreshold ? "노랑" : "빨강")}");
        }

        // 색상 전환 중
        if (isTransitioning && fillImage != null)
        {
            colorTransitionTimer += Time.deltaTime;
            float progress = colorTransitionTimer / transitionDuration;

            if (progress >= 1f)
            {
                // 전환 완료
                fillImage.color = targetColor;
                isTransitioning = false;
                Debug.Log("색상 전환 완료!");
            }
            else
            {
                // 부드럽게 전환
                fillImage.color = Color.Lerp(startColor, targetColor, progress);
            }
        }
    }

    // 타겟 설정
    public void SetTarget(string targetName, float maxHP, float currentHP)
    {
        if (targetText != null)
        {
            targetText.text = targetName;
        }

        maxHealth = maxHP;
        currentHealth = currentHP;

        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }

        // 색상 초기화
        if (fillImage != null)
        {
            fillImage.color = normalColor;
        }
        targetColor = normalColor;
        isTransitioning = false;

        gameObject.SetActive(true);
    }

    // 체력 업데이트
    public void UpdateHealth(float newHealth)
    {
        currentHealth = Mathf.Clamp(newHealth, 0f, maxHealth);

        if (healthSlider != null)
        {
            healthSlider.value = currentHealth;
        }
    }

    // 타겟 해제
    public void ClearTarget()
    {
        if (targetText != null)
        {
            targetText.text = "";
        }

        gameObject.SetActive(false);
    }
}


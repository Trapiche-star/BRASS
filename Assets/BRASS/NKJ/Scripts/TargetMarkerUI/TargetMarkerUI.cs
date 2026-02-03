using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 타겟 마커 UI 표시 및 위치 업데이트 담당
/// TargetingSystem과 연동하여 동작
/// </summary>
public class TargetMarkerUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TargetingSystem targetingSystem;
    [SerializeField] private RectTransform markerContainer;

    [Header("Marker Visuals")]
    [SerializeField] private GameObject markerPrefab;
    [SerializeField] private Vector3 markerOffset = new Vector3(0f, 2f, 0f); // 타겟 위 오프셋

    [Header("Marker Colors (Optional)")]
    [SerializeField] private Color enemyColor = Color.red;
    [SerializeField] private Color allyColor = Color.green;
    [SerializeField] private Color neutralColor = Color.yellow;
    [SerializeField] private Color interactiveColor = Color.cyan;

    [Header("Animation Settings")]
    [SerializeField] private bool enableAnimation = true;
    [SerializeField] private float pulseSpeed = 2f;
    [SerializeField] private float pulseScale = 1.2f;

    private GameObject currentMarkerInstance;
    private Image markerImage;
    private TextMeshProUGUI markerText;
    private ITargetable activeTarget;
    private Camera mainCamera;
    private Canvas parentCanvas;
    private Vector3 originalScale;

    private void Awake()
    {
        mainCamera = Camera.main;

        // Canvas 찾기
        parentCanvas = GetComponentInParent<Canvas>();
        if (parentCanvas == null)
        {
            Debug.LogError("TargetMarkerUI는 Canvas 하위에 있어야 합니다!");
        }

        // 마커 컨테이너 설정
        if (markerContainer == null)
        {
            markerContainer = transform as RectTransform;
        }

        // TargetingSystem 자동 찾기
        if (targetingSystem == null)
        {
            targetingSystem = Object.FindFirstObjectByType<TargetingSystem>();
        }
    }

    private void OnEnable()
    {
        if (targetingSystem != null)
        {
            targetingSystem.OnTargetChanged += ShowMarker;
            targetingSystem.OnTargetCleared += HideMarker;
        }
    }

    private void OnDisable()
    {
        if (targetingSystem != null)
        {
            targetingSystem.OnTargetChanged -= ShowMarker;
            targetingSystem.OnTargetCleared -= HideMarker;
        }
    }

    private void Update()
    {
        // 타겟이 있으면 마커 위치 업데이트
        if (activeTarget != null && activeTarget.IsValidTarget())
        {
            UpdateMarkerPosition();

            if (enableAnimation)
            {
                AnimateMarker();
            }
        }
        else if (activeTarget != null)
        {
            // 타겟이 무효화되면 마커 숨김
            HideMarker();
        }
    }

    /// <summary>
    /// 마커 표시
    /// </summary>
    private void ShowMarker(ITargetable target)
    {
        if (target == null || !target.IsValidTarget())
        {
            HideMarker();
            return;
        }

        activeTarget = target;

        // 기존 마커 제거
        if (currentMarkerInstance != null)
        {
            Destroy(currentMarkerInstance);
        }

        // 새 마커 생성
        if (markerPrefab != null)
        {
            currentMarkerInstance = Instantiate(markerPrefab, markerContainer);
            markerImage = currentMarkerInstance.GetComponentInChildren<Image>();
            markerText = currentMarkerInstance.GetComponentInChildren<TextMeshProUGUI>();

            // 원본 스케일 저장
            if (currentMarkerInstance.transform is RectTransform rectTransform)
            {
                originalScale = rectTransform.localScale;
            }

            // 타겟 타입에 따라 색상 변경
            UpdateMarkerColor(target.GetTargetType());

            // 타겟 이름 표시
            if (markerText != null)
            {
                markerText.text = target.GetDisplayName();
            }

            currentMarkerInstance.SetActive(true);
        }
    }

    /// <summary>
    /// 마커 숨김
    /// </summary>
    private void HideMarker()
    {
        activeTarget = null;

        if (currentMarkerInstance != null)
        {
            Destroy(currentMarkerInstance);
            currentMarkerInstance = null;
        }
    }

    /// <summary>
    /// 마커 위치를 타겟에 맞춰 업데이트 (월드 -> 스크린 좌표 변환)
    /// </summary>
    private void UpdateMarkerPosition()
    {
        if (currentMarkerInstance == null || activeTarget == null || mainCamera == null)
            return;

        Transform targetTransform = activeTarget.GetTargetTransform();
        if (targetTransform == null) return;

        // 월드 좌표에 오프셋 적용
        Vector3 worldPosition = targetTransform.position + markerOffset;

        // 월드 좌표를 스크린 좌표로 변환
        Vector3 screenPosition = mainCamera.WorldToScreenPoint(worldPosition);

        // 타겟이 카메라 뒤에 있으면 숨김
        if (screenPosition.z < 0)
        {
            currentMarkerInstance.SetActive(false);
            return;
        }

        currentMarkerInstance.SetActive(true);

        // Canvas가 Screen Space - Overlay인 경우
        if (parentCanvas.renderMode == RenderMode.ScreenSpaceOverlay)
        {
            currentMarkerInstance.transform.position = screenPosition;
        }
        // Canvas가 Screen Space - Camera인 경우
        else if (parentCanvas.renderMode == RenderMode.ScreenSpaceCamera)
        {
            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                markerContainer,
                screenPosition,
                parentCanvas.worldCamera,
                out localPoint
            );
            currentMarkerInstance.transform.localPosition = localPoint;
        }
    }

    /// <summary>
    /// 타겟 타입에 따라 마커 색상 변경
    /// </summary>
    private void UpdateMarkerColor(TargetType targetType)
    {
        if (markerImage == null) return;

        Color color = targetType switch
        {
            TargetType.Enemy => enemyColor,
            TargetType.Ally => allyColor,
            TargetType.Neutral => neutralColor,
            TargetType.Interactive => interactiveColor,
            _ => Color.white
        };

        markerImage.color = color;
    }

    /// <summary>
    /// 마커 애니메이션 (펄스 효과)
    /// </summary>
    private void AnimateMarker()
    {
        if (currentMarkerInstance == null) return;

        float scale = 1f + Mathf.Sin(Time.time * pulseSpeed) * (pulseScale - 1f) * 0.5f;
        currentMarkerInstance.transform.localScale = originalScale * scale;
    }

    /// <summary>
    /// 외부에서 마커 오프셋 변경 가능
    /// </summary>
    public void SetMarkerOffset(Vector3 offset)
    {
        markerOffset = offset;
    }
}
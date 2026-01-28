using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

/// <summary>
/// 채팅 패널 - 고정 위치 방식
/// 메시지를 위에서 아래로 순서대로 배치
/// </summary>
public class ChatPanel : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject chatMessagePrefab;
    [SerializeField] private RectTransform contentContainer;    // Content의 RectTransform
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private Button sendButton;
    [SerializeField] private ScrollRect scrollRect;

    [Header("Message Settings")]
    [SerializeField] private float messageHeight = 30f;         // 각 메시지 높이
    [SerializeField] private float messageSpacing = 5f;         // 메시지 간격
    [SerializeField] private float messagePadding = 10f;        // 양옆 여백
    [SerializeField] private int maxMessages = 50;
    [SerializeField] private Color systemMessageColor = Color.yellow;

    [Header("Optional Buttons")]
    [SerializeField] private Button clearButton;
    [SerializeField] private Button toggleChatButton;          // "채팅 숨기기" 버튼
    [SerializeField] private Button showChatButton;            // "채팅 보기" 버튼
    [SerializeField] private GameObject controlButtons;        // ControlButtons 오브젝트
    [SerializeField] private GameObject backgroundImage;       // BackgroundImage 오브젝트
    [SerializeField] private Slider transparencySlider;
    [SerializeField] private CanvasGroup chatCanvasGroup;
    [SerializeField] private GameObject chatScrollView;        // ChatScrollView 오브젝트

    private List<GameObject> messageObjects = new List<GameObject>();
    private float currentYPosition = 0f;
    private bool isChatVisible = true;

    private void Start()
    {
        // 필수 체크
        if (chatMessagePrefab == null)
        {
            Debug.LogError("ChatMessage Prefab이 없습니다!");
            return;
        }

        if (contentContainer == null)
        {
            Debug.LogError("Content Container가 없습니다!");
            return;
        }

        // 버튼 이벤트
        if (sendButton != null)
            sendButton.onClick.AddListener(OnSendButtonClick);

        if (inputField != null)
            inputField.onSubmit.AddListener(OnInputSubmit);

        if (clearButton != null)
            clearButton.onClick.AddListener(ClearAllMessages);

        if (toggleChatButton != null)
            toggleChatButton.onClick.AddListener(ToggleChatVisibility);

        // "채팅 보기" 버튼 (새로 추가!)
        if (showChatButton != null)
            showChatButton.onClick.AddListener(ShowChat);

        // 초기 상태: 채팅 보임
        UpdateChatVisibility();

        if (transparencySlider != null)
        {
            transparencySlider.onValueChanged.AddListener(OnTransparencyChanged);
            transparencySlider.value = 1f;
        }

        // Content 크기 초기화
        ResetContentSize();

        // 테스트 메시지 - 실제 화면에 보일 거예요!
        AddSystemMessage("=== 채팅 시작 ===");
        AddMessage("첫 번째 메시지");
        AddMessage("두 번째 메시지입니다");
        AddMessage("세 번째 메시지 - 긴 텍스트 테스트입니다. 이렇게 길면 어떻게 될까요?");
        AddMessage("네 번째");
        AddSystemMessage("시스템 메시지");

        Debug.Log($"[ChatPanel] 초기화 완료. Content 위치: {contentContainer.anchoredPosition}");
    }

    private void OnSendButtonClick()
    {
        SendMessage();
    }

    private void OnInputSubmit(string text)
    {
        SendMessage();
        if (inputField != null)
            inputField.ActivateInputField();
    }

    private void SendMessage()
    {
        if (inputField == null || string.IsNullOrWhiteSpace(inputField.text))
            return;

        AddMessage(inputField.text);
        inputField.text = "";
        inputField.ActivateInputField();
    }

    /// <summary>
    /// 일반 메시지 추가
    /// </summary>
    public void AddMessage(string message)
    {
        CreateMessage(message, Color.white);
    }

    /// <summary>
    /// 시스템 메시지 추가
    /// </summary>
    public void AddSystemMessage(string message)
    {
        CreateMessage("[시스템] " + message, systemMessageColor);
    }

    /// <summary>
    /// 메시지 생성 및 배치
    /// </summary>
    private void CreateMessage(string message, Color textColor)
    {
        // 프리팹 생성
        GameObject newMessageObj = Instantiate(chatMessagePrefab, contentContainer);

        // RectTransform 가져오기
        RectTransform messageRect = newMessageObj.GetComponent<RectTransform>();
        if (messageRect == null)
        {
            Debug.LogError("프리팹에 RectTransform이 없습니다!");
            Destroy(newMessageObj);
            return;
        }

        // 위치 설정
        messageRect.anchorMin = new Vector2(0, 1);
        messageRect.anchorMax = new Vector2(1, 1);
        messageRect.pivot = new Vector2(0, 1);

        messageRect.localPosition = new Vector3(0, 0, -1);
        messageRect.localScale = Vector3.one;
        messageRect.localRotation = Quaternion.identity;

        float contentWidth = contentContainer.rect.width;

        // 텍스트 설정 (높이 계산 전에 먼저!)
        TextMeshProUGUI textComponent = newMessageObj.GetComponentInChildren<TextMeshProUGUI>();
        if (textComponent != null)
        {
            textComponent.text = message;
            textComponent.color = textColor;
            textComponent.fontSize = 20;
            textComponent.fontStyle = FontStyles.Bold;
            textComponent.enableAutoSizing = false;
            textComponent.raycastTarget = true;
        }
        else
        {
            Debug.LogError("프리팹에 TextMeshProUGUI가 없습니다!");
        }

        // 임시 크기 설정 후 텍스트 높이 계산
        messageRect.sizeDelta = new Vector2(contentWidth - messagePadding * 2, 100); // 임시 큰 높이

        // Canvas 업데이트로 텍스트 렌더링
        Canvas.ForceUpdateCanvases();

        // 실제 텍스트 높이 가져오기
        float actualHeight = messageHeight; // 기본값
        if (textComponent != null)
        {
            actualHeight = Mathf.Max(messageHeight, textComponent.preferredHeight + 10); // 여유 공간 10
        }

        // 실제 높이로 재설정
        messageRect.sizeDelta = new Vector2(contentWidth - messagePadding * 2, actualHeight);

        // 위치 설정
        messageRect.anchoredPosition = new Vector2(messagePadding, -currentYPosition);

        Debug.Log($"메시지 생성: '{message}' 위치: {messageRect.anchoredPosition}, 크기: {messageRect.sizeDelta}, 실제높이: {actualHeight}");

        // 리스트에 추가
        messageObjects.Add(newMessageObj);

        // 다음 메시지 위치 계산 (실제 높이 사용!)
        currentYPosition += actualHeight + messageSpacing;

        // Content 높이 업데이트
        UpdateContentSize();

        // 메시지 개수 제한
        if (messageObjects.Count > maxMessages)
        {
            Destroy(messageObjects[0]);
            messageObjects.RemoveAt(0);
            RecalculateAllPositions();
        }

        // 스크롤 맨 아래로
        ScrollToBottom();
    }

    /// <summary>
    /// Content 크기를 메시지 개수에 맞게 조정
    /// </summary>
    private void UpdateContentSize()
    {
        float totalHeight = Mathf.Max(currentYPosition, 100f); // 최소 높이 100
        contentContainer.sizeDelta = new Vector2(contentContainer.sizeDelta.x, totalHeight);

        Debug.Log($"Content 높이 업데이트: {totalHeight}");
    }

    /// <summary>
    /// Content 초기화
    /// </summary>
    private void ResetContentSize()
    {
        currentYPosition = messagePadding;
        contentContainer.sizeDelta = new Vector2(contentContainer.sizeDelta.x, 100f);
        contentContainer.anchoredPosition = new Vector2(0, 0);
    }

    /// <summary>
    /// 모든 메시지 위치 재계산
    /// </summary>
    private void RecalculateAllPositions()
    {
        currentYPosition = messagePadding;

        foreach (GameObject msgObj in messageObjects)
        {
            if (msgObj != null)
            {
                RectTransform rect = msgObj.GetComponent<RectTransform>();
                float msgHeight = rect.sizeDelta.y; // 각 메시지의 실제 높이 사용

                rect.anchoredPosition = new Vector2(messagePadding, -currentYPosition);
                currentYPosition += msgHeight + messageSpacing;
            }
        }

        UpdateContentSize();
    }

    /// <summary>
    /// 스크롤 맨 아래로
    /// </summary>
    private void ScrollToBottom()
    {
        if (scrollRect != null)
        {
            Canvas.ForceUpdateCanvases();
            scrollRect.verticalNormalizedPosition = 0f;
        }
    }

    /// <summary>
    /// 모든 메시지 삭제
    /// </summary>
    public void ClearAllMessages()
    {
        foreach (GameObject msg in messageObjects)
        {
            if (msg != null)
                Destroy(msg);
        }

        messageObjects.Clear();
        ResetContentSize();

        AddSystemMessage("채팅이 삭제되었습니다.");
    }

    /// <summary>
    /// 채팅창 숨기기 (토글)
    /// </summary>
    public void ToggleChatVisibility()
    {
        isChatVisible = !isChatVisible;
        UpdateChatVisibility();
    }

    /// <summary>
    /// 채팅창 보이기 (ShowChatButton용)
    /// </summary>
    public void ShowChat()
    {
        isChatVisible = true;
        UpdateChatVisibility();
    }

    /// <summary>
    /// 채팅 표시 상태 업데이트
    /// </summary>
    private void UpdateChatVisibility()
    {
        // 채팅이 켜져 있을 때
        if (isChatVisible)
        {
            // 채팅 관련 UI 모두 보이기
            if (backgroundImage != null)
                backgroundImage.SetActive(true);

            if (chatScrollView != null)
                chatScrollView.SetActive(true);

            if (inputField != null)
                inputField.gameObject.SetActive(true);

            if (sendButton != null)
                sendButton.gameObject.SetActive(true);

            if (controlButtons != null)
                controlButtons.SetActive(true);

            // "채팅 보기" 버튼은 숨기기
            if (showChatButton != null)
                showChatButton.gameObject.SetActive(false);
        }
        // 채팅이 꺼져 있을 때
        else
        {
            // 채팅 관련 UI 모두 숨기기
            if (backgroundImage != null)
                backgroundImage.SetActive(false);

            if (chatScrollView != null)
                chatScrollView.SetActive(false);

            if (inputField != null)
                inputField.gameObject.SetActive(false);

            if (sendButton != null)
                sendButton.gameObject.SetActive(false);

            if (controlButtons != null)
                controlButtons.SetActive(false);

            // "채팅 보기" 버튼만 보이기
            if (showChatButton != null)
                showChatButton.gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// 투명도 변경
    /// </summary>
    private void OnTransparencyChanged(float value)
    {
        if (chatCanvasGroup != null)
            chatCanvasGroup.alpha = value;
    }

    public void SetTransparency(float alpha)
    {
        if (chatCanvasGroup != null)
            chatCanvasGroup.alpha = Mathf.Clamp01(alpha);

        if (transparencySlider != null)
            transparencySlider.value = Mathf.Clamp01(alpha);
    }
}


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
    [SerializeField] private Button showChatButton;            // "채팅 보기" 버튼 (새로 추가!)
    [SerializeField] private GameObject controlButtons;        // ControlButtons 전체 오브젝트
    [SerializeField] private Slider transparencySlider;
    [SerializeField] private CanvasGroup chatCanvasGroup;
    [SerializeField] private GameObject chatScrollView;

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

        // 위치 설정 - 완전히 수동으로!
        messageRect.anchorMin = new Vector2(0, 1);      // 왼쪽 위 기준
        messageRect.anchorMax = new Vector2(1, 1);      // 가로는 늘어남
        messageRect.pivot = new Vector2(0, 1);          // 왼쪽 위 기준

        // localPosition을 명시적으로 0으로 (Content 좌표계 기준)
        messageRect.localPosition = new Vector3(0, 0, -1);  // Z를 -1로 (앞으로)
        messageRect.localScale = Vector3.one;
        messageRect.localRotation = Quaternion.identity;

        // Content 너비에 맞춰서 가로 크기 설정
        float contentWidth = contentContainer.rect.width;
        messageRect.sizeDelta = new Vector2(contentWidth - messagePadding * 2, messageHeight);

        // 위치: 위에서부터 currentYPosition만큼 내려간 곳
        messageRect.anchoredPosition = new Vector2(messagePadding, -currentYPosition);

        Debug.Log($"메시지 생성: '{message}' 위치: {messageRect.anchoredPosition}, 크기: {messageRect.sizeDelta}");

        // 텍스트 설정
        TextMeshProUGUI textComponent = newMessageObj.GetComponentInChildren<TextMeshProUGUI>();
        if (textComponent != null)
        {
            textComponent.text = message;
            textComponent.color = textColor;

            // 강제 설정 - 확실하게!
            textComponent.fontSize = 20;
            textComponent.fontStyle = FontStyles.Bold;
            textComponent.enableAutoSizing = false;
            textComponent.raycastTarget = true;

            Debug.Log($"텍스트 설정 완료: '{message}', 색상: {textComponent.color}, 폰트: {textComponent.font?.name}");
        }
        else
        {
            Debug.LogError("프리팹에 TextMeshProUGUI가 없습니다!");
        }

        // 리스트에 추가
        messageObjects.Add(newMessageObj);

        // 다음 메시지 위치 계산
        currentYPosition += messageHeight + messageSpacing;

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
                rect.anchoredPosition = new Vector2(messagePadding, -currentYPosition);
                currentYPosition += messageHeight + messageSpacing;
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
        // 채팅 관련 UI 토글
        if (chatScrollView != null)
            chatScrollView.SetActive(isChatVisible);

        if (inputField != null)
            inputField.gameObject.SetActive(isChatVisible);

        if (sendButton != null)
            sendButton.gameObject.SetActive(isChatVisible);

        // ControlButtons 전체 토글
        if (controlButtons != null)
            controlButtons.SetActive(isChatVisible);

        // ShowChatButton은 반대로 (채팅 꺼지면 보이기)
        if (showChatButton != null)
            showChatButton.gameObject.SetActive(!isChatVisible);
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

/*
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
    [SerializeField] private Button toggleChatButton;
    [SerializeField] private Slider transparencySlider;
    [SerializeField] private CanvasGroup chatCanvasGroup;
    [SerializeField] private GameObject chatScrollView;

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

        // 위치 설정 - 완전히 수동으로!
        messageRect.anchorMin = new Vector2(0, 1);      // 왼쪽 위 기준
        messageRect.anchorMax = new Vector2(1, 1);      // 가로는 늘어남
        messageRect.pivot = new Vector2(0, 1);          // 왼쪽 위 기준

        // Content 너비에 맞춰서 가로 크기 설정
        float contentWidth = contentContainer.rect.width;
        messageRect.sizeDelta = new Vector2(-messagePadding * 2, messageHeight);

        // 위치: 위에서부터 currentYPosition만큼 내려간 곳
        messageRect.anchoredPosition = new Vector2(messagePadding, -currentYPosition);

        Debug.Log($"메시지 생성: '{message}' 위치: {messageRect.anchoredPosition}, 크기: {messageRect.sizeDelta}");

        // 텍스트 설정
        TextMeshProUGUI textComponent = newMessageObj.GetComponentInChildren<TextMeshProUGUI>();
        if (textComponent != null)
        {
            textComponent.text = message;
            textComponent.color = textColor;
        }
        else
        {
            Debug.LogError("프리팹에 TextMeshProUGUI가 없습니다!");
        }

        // 리스트에 추가
        messageObjects.Add(newMessageObj);

        // 다음 메시지 위치 계산
        currentYPosition += messageHeight + messageSpacing;

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
                rect.anchoredPosition = new Vector2(messagePadding, -currentYPosition);
                currentYPosition += messageHeight + messageSpacing;
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
    /// 채팅창 토글
    /// </summary>
    public void ToggleChatVisibility()
    {
        isChatVisible = !isChatVisible;

        if (chatScrollView != null)
            chatScrollView.SetActive(isChatVisible);

        if (inputField != null)
            inputField.gameObject.SetActive(isChatVisible);

        if (sendButton != null)
            sendButton.gameObject.SetActive(isChatVisible);
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
*/
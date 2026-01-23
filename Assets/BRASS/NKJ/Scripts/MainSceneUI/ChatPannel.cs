using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

/// <summary>
/// 채팅 패널 관리 스크립트
/// - SendButton/Enter로 메시지 전송
/// - 채팅창 보이기/숨기기
/// - 전체 채팅 삭제
/// - 투명도 조절
/// - 시스템 메시지 추가
/// </summary>
public class ChatPanel : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject chatMessagePrefab;      // 채팅 메시지 프리팹
    [SerializeField] private Transform contentContainer;        // ScrollView의 Content
    [SerializeField] private TMP_InputField inputField;         // 메시지 입력 필드
    [SerializeField] private Button sendButton;                 // 전송 버튼
    [SerializeField] private ScrollRect scrollRect;             // ScrollView의 ScrollRect

    [Header("Control Buttons")]
    [SerializeField] private Button toggleChatButton;           // 채팅창 보이기/숨기기 버튼
    [SerializeField] private Button clearButton;                // 전체 삭제 버튼
    [SerializeField] private Slider transparencySlider;         // 투명도 조절 슬라이더

    [Header("Chat Container")]
    [SerializeField] private CanvasGroup chatContainer;         // 투명도 조절할 컨테이너 (Background, ScrollView 등 포함)
    [SerializeField] private GameObject chatContent;            // 숨김/보임 처리할 오브젝트 (ScrollView, InputField 등)

    [Header("Toggle Button Texts")]
    [SerializeField] private TextMeshProUGUI toggleButtonText;  // 토글 버튼 텍스트 (선택사항)

    [Header("Settings")]
    [SerializeField] private int maxMessages = 50;              // 최대 메시지 개수
    [SerializeField] private Color systemMessageColor = Color.yellow; // 시스템 메시지 색상

    private List<GameObject> messageObjects = new List<GameObject>();
    private bool isChatVisible = true;                          // 채팅창 보임 상태

    private void Start()
    {
        // SendButton 클릭 이벤트
        sendButton.onClick.AddListener(OnSendButtonClick);

        // InputField에서 Enter 이벤트
        inputField.onSubmit.AddListener(OnInputSubmit);

        // 채팅창 토글 버튼
        if (toggleChatButton != null)
        {
            toggleChatButton.onClick.AddListener(ToggleChatVisibility);
            UpdateToggleButtonText();
        }

        // 전체 삭제 버튼
        if (clearButton != null)
        {
            clearButton.onClick.AddListener(ClearAllMessages);
        }

        // 투명도 슬라이더
        if (transparencySlider != null)
        {
            transparencySlider.onValueChanged.AddListener(OnTransparencyChanged);
            // 초기값 설정 (1 = 불투명, 0 = 완전 투명)
            transparencySlider.value = 1f;
        }

        // 시작 시 테스트 메시지들
        AddSystemMessage("채팅 시스템이 시작되었습니다.");
        AddMessage("테스트 메시지 1");
        AddMessage("테스트 메시지 2 - 이것은 조금 긴 메시지입니다.");
        AddMessage("테스트 메시지 3");
        AddSystemMessage("시스템 메시지 테스트");
    }

    /// <summary>
    /// SendButton 클릭 시 호출
    /// </summary>
    private void OnSendButtonClick()
    {
        SendMessage();
    }

    /// <summary>
    /// InputField에서 Enter 눌렀을 때 호출
    /// </summary>
    private void OnInputSubmit(string text)
    {
        SendMessage();
        inputField.ActivateInputField();
    }

    /// <summary>
    /// 메시지 전송 처리
    /// </summary>
    private void SendMessage()
    {
        if (string.IsNullOrWhiteSpace(inputField.text))
            return;

        AddMessage(inputField.text);
        inputField.text = "";
        inputField.ActivateInputField();
    }

    /// <summary>
    /// 일반 채팅 메시지 추가
    /// </summary>
    public void AddMessage(string message)
    {
        AddMessageInternal(message, false);
    }

    /// <summary>
    /// 시스템 메시지 추가 (색상 다름)
    /// </summary>
    public void AddSystemMessage(string message)
    {
        AddMessageInternal("[시스템] " + message, true);
    }

    /// <summary>
    /// 내부 메시지 추가 로직
    /// </summary>
    private void AddMessageInternal(string message, bool isSystemMessage)
    {
        // 프리팹 체크
        if (chatMessagePrefab == null)
        {
            Debug.LogError("ChatMessage Prefab이 설정되지 않았습니다!");
            return;
        }

        if (contentContainer == null)
        {
            Debug.LogError("Content Container가 설정되지 않았습니다!");
            return;
        }

        GameObject newMessage = Instantiate(chatMessagePrefab, contentContainer);
        Debug.Log($"메시지 생성됨: {message}, 위치: {newMessage.transform.position}");

        TextMeshProUGUI textComponent = newMessage.GetComponentInChildren<TextMeshProUGUI>();
        if (textComponent != null)
        {
            textComponent.text = message;

            // 시스템 메시지면 색상 변경
            if (isSystemMessage)
            {
                textComponent.color = systemMessageColor;
            }
        }
        else
        {
            Debug.LogError("ChatMessage 프리팹에 TextMeshProUGUI 컴포넌트가 없습니다!");
        }

        messageObjects.Add(newMessage);

        // 메시지 개수 제한
        if (messageObjects.Count > maxMessages)
        {
            Destroy(messageObjects[0]);
            messageObjects.RemoveAt(0);
        }

        ScrollToBottom();
    }

    /// <summary>
    /// 스크롤을 맨 아래로 이동
    /// </summary>
    private void ScrollToBottom()
    {
        // 코루틴으로 다음 프레임에 스크롤 (튕김 방지)
        StartCoroutine(ScrollToBottomCoroutine());
    }

    /// <summary>
    /// 스크롤을 부드럽게 아래로 이동하는 코루틴
    /// </summary>
    private System.Collections.IEnumerator ScrollToBottomCoroutine()
    {
        // Layout이 완전히 재계산될 때까지 대기
        yield return null; // 1프레임 대기

        Canvas.ForceUpdateCanvases();
        scrollRect.verticalNormalizedPosition = 0f;

        // 한 번 더 보정 (확실하게)
        yield return null;
        scrollRect.verticalNormalizedPosition = 0f;
    }

    /// <summary>
    /// 모든 메시지 삭제
    /// </summary>
    public void ClearAllMessages()
    {
        foreach (GameObject msg in messageObjects)
        {
            Destroy(msg);
        }
        messageObjects.Clear();

        AddSystemMessage("모든 채팅이 삭제되었습니다.");
    }

    /// <summary>
    /// 채팅창 보이기/숨기기 토글
    /// </summary>
    public void ToggleChatVisibility()
    {
        isChatVisible = !isChatVisible;

        if (chatContent != null)
        {
            chatContent.SetActive(isChatVisible);
        }

        UpdateToggleButtonText();
    }

    /// <summary>
    /// 토글 버튼 텍스트 업데이트
    /// </summary>
    private void UpdateToggleButtonText()
    {
        if (toggleButtonText != null)
        {
            toggleButtonText.text = isChatVisible ? "채팅 숨기기" : "채팅 보기";
        }
    }

    /// <summary>
    /// 투명도 변경 (슬라이더 값: 0~1)
    /// </summary>
    private void OnTransparencyChanged(float value)
    {
        if (chatContainer != null)
        {
            // value = 1: 완전 불투명, value = 0: 완전 투명
            chatContainer.alpha = value;
        }
    }

    /// <summary>
    /// 투명도를 코드로 설정 (0~1)
    /// </summary>
    public void SetTransparency(float alpha)
    {
        alpha = Mathf.Clamp01(alpha); // 0~1 사이로 제한

        if (chatContainer != null)
        {
            chatContainer.alpha = alpha;
        }

        if (transparencySlider != null)
        {
            transparencySlider.value = alpha;
        }
    }
}


/////////////////////////////////////////
///


/*using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

/// <summary>
/// 채팅 패널 관리 스크립트
/// SendButton 클릭 또는 Enter키로 메시지 전송
/// Scrollbar로 이전 메시지 확인 가능
/// </summary>
public class ChatPanel : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject chatMessagePrefab;  // 채팅 메시지 프리팹
    [SerializeField] private Transform contentContainer;    // ScrollView의 Content
    [SerializeField] private TMP_InputField inputField;     // 메시지 입력 필드
    [SerializeField] private Button sendButton;             // 전송 버튼
    [SerializeField] private ScrollRect scrollRect;         // ScrollView의 ScrollRect

    [Header("Settings")]
    [SerializeField] private int maxMessages = 50;          // 최대 메시지 개수 제한

    private List<GameObject> messageObjects = new List<GameObject>();

    private void Start()
    {
        // SendButton 클릭 시 메시지 전송
        sendButton.onClick.AddListener(OnSendButtonClick);

        // InputField에서 Enter 눌렀을 때 메시지 전송
        inputField.onSubmit.AddListener(OnInputSubmit);
    }

    /// <summary>
    /// SendButton 클릭 시 호출
    /// </summary>
    private void OnSendButtonClick()
    {
        SendMessage();
    }

    /// <summary>
    /// InputField에서 Enter 눌렀을 때 호출
    /// </summary>
    private void OnInputSubmit(string text)
    {
        SendMessage();
        // Enter 후에도 InputField 포커스 유지
        inputField.ActivateInputField();
    }

    /// <summary>
    /// 메시지 전송 처리
    /// </summary>
    private void SendMessage()
    {
        // 빈 메시지는 전송하지 않음
        if (string.IsNullOrWhiteSpace(inputField.text))
            return;

        // 메시지 추가
        AddMessage(inputField.text);

        // InputField 초기화
        inputField.text = "";

        // InputField 포커스 다시 활성화 (계속 채팅 가능하게)
        inputField.ActivateInputField();
    }

    /// <summary>
    /// 채팅창에 메시지 추가
    /// </summary>
    public void AddMessage(string message)
    {
        // 프리팹으로 메시지 오브젝트 생성
        GameObject newMessage = Instantiate(chatMessagePrefab, contentContainer);

        // 텍스트 설정
        TextMeshProUGUI textComponent = newMessage.GetComponentInChildren<TextMeshProUGUI>();
        if (textComponent != null)
        {
            textComponent.text = message;
        }

        // 리스트에 추가
        messageObjects.Add(newMessage);

        // 메시지 개수 제한 (오래된 메시지 삭제)
        if (messageObjects.Count > maxMessages)
        {
            Destroy(messageObjects[0]);
            messageObjects.RemoveAt(0);
        }

        // 스크롤을 맨 아래로 이동 (최신 메시지 보이게)
        ScrollToBottom();
    }

    /// <summary>
    /// 스크롤을 맨 아래로 이동
    /// </summary>
    private void ScrollToBottom()
    {
        // Canvas를 강제로 업데이트해서 Layout이 즉시 반영되게 함
        Canvas.ForceUpdateCanvases();

        // verticalNormalizedPosition: 0 = 맨 아래, 1 = 맨 위
        scrollRect.verticalNormalizedPosition = 0f;
    }

    /// <summary>
    /// 모든 메시지 삭제 (필요할 때 사용)
    /// </summary>
    public void ClearAllMessages()
    {
        foreach (GameObject msg in messageObjects)
        {
            Destroy(msg);
        }
        messageObjects.Clear();
    }
}*/
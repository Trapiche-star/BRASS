using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class NicknameManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nicknameDisplay;
    [SerializeField] private Button changeNicknameButton;
    [SerializeField] private GameObject nicknameInputPanel;
    [SerializeField] private TMP_InputField nicknameInputField;
    [SerializeField] private Button confirmButton;
    [SerializeField] private Button cancelButton;

    private string currentNickname = "Player";
    private const string NICKNAME_KEY = "PlayerNickname";
    private bool isInputActive = false;

    private void Start()
    {
        // 저장된 닉네임 불러오기
        if (PlayerPrefs.HasKey(NICKNAME_KEY))
        {
            currentNickname = PlayerPrefs.GetString(NICKNAME_KEY);
        }
        else
        {
            // 처음 시작할 때 기본값 저장
            PlayerPrefs.SetString(NICKNAME_KEY, currentNickname);
        }

        // 초기 설정
        if (nicknameDisplay != null)
            nicknameDisplay.text = currentNickname;

        if (nicknameInputPanel != null)
            nicknameInputPanel.SetActive(false);

        // 버튼 이벤트 연결
        if (changeNicknameButton != null)
            changeNicknameButton.onClick.AddListener(OpenNicknameInput);

        if (confirmButton != null)
            confirmButton.onClick.AddListener(ConfirmNicknameChange);

        if (cancelButton != null)
            cancelButton.onClick.AddListener(CloseNicknameInput);
    }

    private void Update()
    {
        // null 체크 및 활성화 상태 확인
        if (nicknameInputPanel == null || !nicknameInputPanel.activeSelf || !isInputActive)
            return;

        // Keyboard가 null인지 체크
        if (Keyboard.current == null)
            return;

        // InputField가 포커스 상태인지 확인 (다른 UI와 충돌 방지)
        bool isInputFieldFocused = nicknameInputField != null && nicknameInputField.isFocused;

        // ESC 키로 취소 (InputField 포커스 여부와 관계없이)
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            CloseNicknameInput();
            return;
        }

        // Enter 키로 확인 (InputField가 포커스 상태일 때만)
        if (isInputFieldFocused && Keyboard.current.enterKey.wasPressedThisFrame)
        {
            ConfirmNicknameChange();
        }
    }

    private void OpenNicknameInput()
    {
        if (nicknameInputPanel == null)
            return;

        isInputActive = true;
        nicknameInputPanel.SetActive(true);

        // 잠깐 기다렸다가 포커스 설정 (UI 업데이트 후)
        StartCoroutine(FocusInputFieldWithDelay());
    }

    private System.Collections.IEnumerator FocusInputFieldWithDelay()
    {
        yield return new WaitForEndOfFrame();

        if (nicknameInputField != null)
        {
            nicknameInputField.text = currentNickname;
            nicknameInputField.ActivateInputField();
            nicknameInputField.Select();
        }
    }

    private void ConfirmNicknameChange()
    {
        if (nicknameInputField == null)
            return;

        string newNickname = nicknameInputField.text.Trim();

        // 유효성 검사
        if (string.IsNullOrEmpty(newNickname))
        {
            Debug.Log("닉네임을 입력해주세요.");
            return;
        }

        if (newNickname.Length > 20)
        {
            Debug.Log("닉네임은 20자 이하여야 합니다.");
            return;
        }

        // 닉네임 변경
        currentNickname = newNickname;

        if (nicknameDisplay != null)
            nicknameDisplay.text = currentNickname;

        // PlayerPrefs에 저장
        PlayerPrefs.SetString(NICKNAME_KEY, currentNickname);
        PlayerPrefs.Save(); // 즉시 저장

        Debug.Log($"닉네임이 '{currentNickname}'로 변경되었습니다.");
        CloseNicknameInput();

        // 필요하면 서버에 저장
        // SaveNicknameToServer(currentNickname);
    }

    private void CloseNicknameInput()
    {
        isInputActive = false;

        if (nicknameInputPanel != null)
            nicknameInputPanel.SetActive(false);
    }

    public string GetCurrentNickname()
    {
        return currentNickname;
    }

    public void SetNickname(string newNickname)
    {
        currentNickname = newNickname;

        if (nicknameDisplay != null)
            nicknameDisplay.text = currentNickname;
    }

    // 외부에서 입력 패널 활성화 상태 확인용
    public bool IsInputPanelActive()
    {
        return isInputActive && nicknameInputPanel != null && nicknameInputPanel.activeSelf;
    }

    private void OnDestroy()
    {
        // 이벤트 리스너 제거
        if (changeNicknameButton != null)
            changeNicknameButton.onClick.RemoveListener(OpenNicknameInput);

        if (confirmButton != null)
            confirmButton.onClick.RemoveListener(ConfirmNicknameChange);

        if (cancelButton != null)
            cancelButton.onClick.RemoveListener(CloseNicknameInput);
    }
}
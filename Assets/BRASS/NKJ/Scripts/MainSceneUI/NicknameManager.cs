using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

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
        nicknameDisplay.text = currentNickname;
        nicknameInputPanel.SetActive(false);

        // 버튼 이벤트 연결
        changeNicknameButton.onClick.AddListener(OpenNicknameInput);
        confirmButton.onClick.AddListener(ConfirmNicknameChange);
        cancelButton.onClick.AddListener(CloseNicknameInput);
    }

    private void Update()
    {
        // 입력 패널이 활성화된 상태에서만 키 입력 감지
        if (nicknameInputPanel.activeSelf)
        {
            // ESC 키로 취소
            if (Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                CloseNicknameInput();
            }

            // Enter 키로 확인
            if (Keyboard.current.enterKey.wasPressedThisFrame)
            {
                ConfirmNicknameChange();
            }
        }
    }

    private void OpenNicknameInput()
    {
        nicknameInputPanel.SetActive(true);

        // 잠깐 기다렸다가 포커스 설정 (UI 업데이트 후)
        StartCoroutine(FocusInputFieldWithDelay());
    }

    private System.Collections.IEnumerator FocusInputFieldWithDelay()
    {
        yield return new WaitForEndOfFrame();
        nicknameInputField.text = currentNickname;
        nicknameInputField.ActivateInputField();
        nicknameInputField.Select();
    }

    private void ConfirmNicknameChange()
    {
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
        nicknameInputPanel.SetActive(false);
    }

    public string GetCurrentNickname()
    {
        return currentNickname;
    }

    public void SetNickname(string newNickname)
    {
        currentNickname = newNickname;
        nicknameDisplay.text = currentNickname;
    }
}





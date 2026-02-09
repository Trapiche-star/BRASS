using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class ExitMenuManager : MonoBehaviour
{
    [Header("UI Reference")]
    [SerializeField] private GameObject panel;

    [Header("Input Action")]
    [SerializeField] private InputActionReference escAction;

    // 1. 가장 먼저 실행되는 곳
    private void Awake()
    {
        ForceClosePanel();
    }

    // 2. 혹시 몰라서 한 번 더 체크 (방어용)
    private void Start()
    {
        ForceClosePanel();
    }

    private void ForceClosePanel()
    {
        if (panel != null)
        {
            panel.SetActive(false);
            Time.timeScale = 1f; // 시간도 무조건 정상으로 시작
        }
    }

    // --- 이하 버튼 및 키 입력 로직 ---

    private void OnEnable()
    {
        if (escAction != null)
        {
            escAction.action.Enable();
            escAction.action.performed += OnEscPerformed;
        }
    }

    private void OnDisable()
    {
        if (escAction != null)
        {
            escAction.action.performed -= OnEscPerformed;
            escAction.action.Disable();
        }
    }

    private void OnEscPerformed(InputAction.CallbackContext context)
    {
        if (panel != null && panel.activeSelf)
        {
            ClosePanel();
        }
    }

    public void OpenPanel()
    {
        if (panel == null) return;
        panel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void ClosePanel()
    {
        if (panel == null) return;
        panel.SetActive(false);
        Time.timeScale = 1f;
    }

    public void GoToHome()
    {
        // 저장 로직 (필요시)
        if (SaveManager.Instance != null) SaveManager.Instance.SaveGame();

        Time.timeScale = 1f;
        SceneManager.LoadScene("EnterSceneUI");
    }

    public void QuitGame()
    {
        // 종료 전 저장
        if (SaveManager.Instance != null) SaveManager.Instance.SaveGame();

        Time.timeScale = 1f;
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }
}
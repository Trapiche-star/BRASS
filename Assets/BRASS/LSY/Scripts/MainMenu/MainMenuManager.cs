using UnityEngine;
using UnityEngine.SceneManagement;

namespace Team1
{
    public class MainMenuManager : MonoBehaviour
    {
        [Header("연결할 옵션 UI 패널")]
        public GameObject optionsPanel;

        [Header("이동할 메인 게임 씬 이름")]
        public string gameSceneName = "MainTest";

        public void PlayGame()
        {
            // SoundManager가 있고, 그 안에 PlayButtonClick 함수가 있어야 함!
            if (SoundManager.Instance != null)
            {
                SoundManager.Instance.PlayButtonClick();
            }

            // SceneFader가 있으면 페이드 실행, 없으면 즉시 이동
            if (SceneFader.Instance != null)
            {
                SceneFader.Instance.FadeToScene(gameSceneName);
            }
            else
            {
                Debug.LogWarning("SceneFader를 찾을 수 없어 즉시 씬을 이동합니다.");
                SceneManager.LoadScene(gameSceneName);
            }
        }

        public void OpenOptions()
        {
            if (optionsPanel != null)
            {
                optionsPanel.SetActive(true);
            }
            else
            {
                Debug.LogWarning("옵션 패널이 연결되지 않았습니다!");
            }
        }
        public void CloseOptions()
        {
            if (optionsPanel != null)
            {
                optionsPanel.SetActive(false);
            }
            else
            {
                Debug.LogWarning("옵션 패널이 연결되지 않았습니다!");
            }
        }

        public void QuitGame()
        {
            Debug.Log("게임 종료!");

#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false; // 에디터에서 끌 때
#else
                Application.Quit(); // 빌드된 게임에서 끌 때
#endif
        }
    }
}
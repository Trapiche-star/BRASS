using UnityEngine;
using UnityEngine.SceneManagement; // 씬 이동을 위해 필수!

namespace Team1
{
    public class MainMenuManager : MonoBehaviour
    {
        [Header("연결할 옵션 UI 패널")]
        public GameObject optionsPanel;

        [Header("이동할 메인 게임 씬 이름")]
        public string gameSceneName = "Ship_SY";

        public void PlayGame()
        {
            SceneManager.LoadScene(gameSceneName);
        }

        // 2. 옵션 UI 켜기
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

        // 3. 게임 종료
        public void QuitGame()
        {
            Debug.Log("게임 종료!");

                Application.Quit();
        }
    }
}
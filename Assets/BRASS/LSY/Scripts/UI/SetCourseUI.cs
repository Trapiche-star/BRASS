using UnityEngine;
using UnityEngine.UI;

namespace Team1
{
    public class SetCourseUI : MonoBehaviour
    {
        public GameObject towerCircle;
        public GameObject shipCircle;
        public GameObject townCircle;

        void ResetCircles()
        {
            if (towerCircle != null) towerCircle.SetActive(false);
            if (shipCircle != null) shipCircle.SetActive(false);
            if (townCircle != null) townCircle.SetActive(false);
        }

        public void SelectTower()
        {
            ResetCircles();
            if (towerCircle != null) towerCircle.SetActive(true);

            // 직접 이동 대신 페이더를 통해 이동
            if (SceneFader.Instance != null)
                SceneFader.Instance.FadeToScene("Tower");
            else
                UnityEngine.SceneManagement.SceneManager.LoadScene("Tower");
        }

        public void SelectShip()
        {
            ResetCircles();
            if (shipCircle != null) shipCircle.SetActive(true);

            if (SceneFader.Instance != null)
                SceneFader.Instance.FadeToScene("MainTest");
            else
                UnityEngine.SceneManagement.SceneManager.LoadScene("MainTest");
        }

        public void SelectTown()
        {
            ResetCircles();
            if (townCircle != null) townCircle.SetActive(true);

            if (SceneFader.Instance != null)
                SceneFader.Instance.FadeToScene("Town");
            else
                UnityEngine.SceneManagement.SceneManager.LoadScene("Town");
        }
    }
}
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Team1
{
    public class SetCourseUI : MonoBehaviour
    {
        public GameObject towerCircle;
        public GameObject shipCircle;

        // 공통 처리: 둘 다 끄기
        void ResetCircles()
        {
            towerCircle.SetActive(false);
            shipCircle.SetActive(false);
        }

        public void SelectTower()
        {
            ResetCircles();                 // 기존 선택 해제
            towerCircle.SetActive(true);    // 타워 선택 표시
            SceneManager.LoadScene("Tower"); // Tower 씬 이동
        }

        public void SelectShip()
        {
            ResetCircles();                // 기존 선택 해제
            shipCircle.SetActive(true);     // 배 선택 표시
            SceneManager.LoadScene("Ship_SY"); // Ship 씬 이동
        }
    }
}

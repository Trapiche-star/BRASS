using UnityEngine;
using UnityEngine.SceneManagement;

namespace Team1
{
    public class SetCourseUI : MonoBehaviour
    {
        public GameObject towerCircle;
        public GameObject shipCircle;
        public GameObject townCircle;

        // 공통 처리: 하나라도 연결 안 되어 있어도 에러로 코드가 멈추지 않게 방지
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

            // Build Settings에 등록된 정확한 이름 또는 경로 사용
            SceneManager.LoadScene("Tower");
        }

        public void SelectShip()
        {
            ResetCircles();
            if (shipCircle != null) shipCircle.SetActive(true);


            // 사진에 MainTest가 등록되어 있으므로 그대로 사용
            SceneManager.LoadScene("MainTest");
        }

        public void SelectTown()
        {
            ResetCircles();
            if (townCircle != null) townCircle.SetActive(true);


            // 사진에 Town이 등록되어 있으므로 그대로 사용
            SceneManager.LoadScene("Town");
        }
    }
}
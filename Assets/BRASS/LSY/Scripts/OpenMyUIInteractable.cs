using UnityEngine;
using BRASS;

namespace Team1
{
    public class OpenMyUIInteractable : MonoBehaviour, IInteractable
    {
        [SerializeField] private GameObject myUI;

        private static OpenMyUIInteractable currentOpen;

        public void Interact()
        {
            if (myUI == null)
                return;

            // ✅ 다른 UI 열려있으면 닫기
            if (currentOpen != null && currentOpen != this)
            {
                currentOpen.Close();
            }

            // ✅ 토글
            if (myUI.activeSelf)
                Close();
            else
                Open();
        }

        private void Open()
        {
            myUI.SetActive(true);
            Time.timeScale = 0f;
            currentOpen = this;
        }

        private void Close()
        {
            myUI.SetActive(false);
            Time.timeScale = 1f;

            if (currentOpen == this)
                currentOpen = null;
        }

        public bool IsOpen => myUI != null && myUI.activeSelf;
    }
}

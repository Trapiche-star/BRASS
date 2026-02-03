using UnityEngine;

namespace Team1
{
    public class InteractionPromptUI : MonoBehaviour
    {
        [SerializeField] private GameObject root;

        private void Awake()
        {
            if (root != null)
                root.SetActive(false);
        }

        public void Show()
        {
            if (root != null && !root.activeSelf)
            {
                Debug.Log("<color=green>프롬프트 ON</color>"); // 로그 추가
                root.SetActive(true);
            }
        }

        public void Hide()
        {
            if (root != null && root.activeSelf)
            {
                root.SetActive(false);
            }
        }
    }
}

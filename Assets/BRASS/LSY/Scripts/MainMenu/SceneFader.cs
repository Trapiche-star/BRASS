using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

namespace Team1
{
    public class SceneFader : MonoBehaviour
    {
        public static SceneFader Instance;

        [Header("UI 연결")]
        public CanvasGroup faderCanvasGroup;
        public float fadeDuration = 1.0f;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                transform.SetParent(null); // 최상단으로 이동 (에러 방지)
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            // 게임 처음 시작 시 화면을 밝게 만듦
            StartCoroutine(FadeIn());
        }

        // ⭐ 씬 이동 시 호출할 함수
        public void FadeToScene(string sceneName)
        {
            // 코루틴 중첩 실행 방지
            StopAllCoroutines();
            StartCoroutine(FadeOutAndLoad(sceneName));
        }

        IEnumerator FadeOutAndLoad(string sceneName)
        {
            faderCanvasGroup.blocksRaycasts = true; // 클릭 방지
            float timer = 0;

            // 1. 점점 어두워짐 (알파 0 -> 1)
            while (timer < fadeDuration)
            {
                timer += Time.deltaTime;
                faderCanvasGroup.alpha = Mathf.Lerp(0, 1, timer / fadeDuration);
                yield return null;
            }

            faderCanvasGroup.alpha = 1; // 확실히 검게 만듦

            // 2. 실제 씬 로드
            SceneManager.LoadScene(sceneName);

            // ⭐ 씬이 로드될 때까지 한 프레임 대기 (중요!)
            yield return null;

            // 3. 잠시 검은 화면 유지 (로딩 느낌)
            yield return new WaitForSeconds(0.2f);

            // 4. 다시 밝아짐 (알파 1 -> 0)
            yield return StartCoroutine(FadeIn());
        }

        IEnumerator FadeIn()
        {
            float timer = 0;
            while (timer < fadeDuration)
            {
                timer += Time.deltaTime;
                faderCanvasGroup.alpha = Mathf.Lerp(1, 0, timer / fadeDuration);
                yield return null;
            }

            faderCanvasGroup.alpha = 0;
            faderCanvasGroup.blocksRaycasts = false;
        }
    }
}
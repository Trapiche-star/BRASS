using UnityEngine;

namespace Team1
{
    public class SoundManager : MonoBehaviour
    {
        // 어디서든 접근 가능하게 싱글톤으로 만듭니다.
        public static SoundManager Instance;

        public AudioSource bgmSource;
        public AudioSource sfxSource;

        [Header("효과음 리스트")]
        public AudioClip buttonClickClip;
        public AudioClip itemUseClip;
        public AudioClip errorClip;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                // ⭐ 이 오브젝트는 씬이 바뀌어도 파괴되지 않습니다.
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                // ⭐ 이미 매니저가 있다면 새로 생긴 것은 삭제합니다 (중복 방지)
                Destroy(gameObject);
            }
        }

        // 버튼 클릭 소리 재생 예시
        public void PlayButtonClick() => sfxSource.PlayOneShot(buttonClickClip);

        // 아이템 사용 소리 재생 예시
        public void PlayItemUse() => sfxSource.PlayOneShot(itemUseClip);
    }
}
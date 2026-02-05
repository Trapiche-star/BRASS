using UnityEngine;
using UnityEngine.SceneManagement;

namespace Team1
{
    public class SoundManager : MonoBehaviour
    {
        public static SoundManager Instance;

        [Header("오디오 소스")]
        public AudioSource bgmSource;
        public AudioSource sfxSource;

        [Header("BGM 리스트")]
        public AudioClip introOST;
        public AudioClip shipOST;
        public AudioClip towerOST;

        [Header("SFX 리스트")]
        public AudioClip buttonClickClip;

        [Header("볼륨 설정 (내부 데이터)")]
        [Range(0f, 1f)] public float masterVolume = 1f;
        [Range(0f, 1f)] public float bgmVolume = 1f;
        [Range(0f, 1f)] public float sfxVolume = 1f;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                // 부모가 있다면 해제해야 DontDestroyOnLoad가 작동함
                transform.SetParent(null);
                DontDestroyOnLoad(gameObject);
                SceneManager.sceneLoaded += OnSceneLoaded;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        // ⭐ 볼륨을 최종 적용하는 핵심 함수
        private void ApplyAllVolumes()
        {
            if (bgmSource != null)
            {
                bgmSource.volume = bgmVolume * masterVolume;
            }
            if (sfxSource != null)
            {
                sfxSource.volume = sfxVolume * masterVolume;
            }
        }

        // --- 슬라이더 연결용 함수들 ---

        public void SetMasterVolume(float volume)
        {
            masterVolume = volume;
            ApplyAllVolumes();
        }

        public void SetBGMVolume(float volume)
        {
            bgmVolume = volume;
            ApplyAllVolumes();
        }

        public void SetSFXVolume(float volume)
        {
            sfxVolume = volume;
            ApplyAllVolumes();
        }

        // --- 재생 관련 함수들 ---

        public void PlayButtonClick()
        {
            if (sfxSource != null && buttonClickClip != null)
            {
                sfxSource.PlayOneShot(buttonClickClip);
            }
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            switch (scene.name)
            {
                case "Start":
                    ChangeBGM(introOST);
                    break;
                case "MainTest":
                    ChangeBGM(shipOST);
                    break;
                case "Tower":
                    ChangeBGM(towerOST);
                    break;
                default:
                    Debug.LogWarning($"{scene.name} 씬의 배경음악이 설정되지 않았습니다.");
                    break;
            }
        }

        public void ChangeBGM(AudioClip newClip)
        {
            if (newClip == null || bgmSource.clip == newClip) return;

            bgmSource.Stop();
            bgmSource.clip = newClip;
            bgmSource.Play();

            // 씬이 바뀌어 재생될 때도 현재 설정된 볼륨을 적용
            ApplyAllVolumes();
        }
    }
}
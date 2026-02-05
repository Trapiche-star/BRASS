using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio; // 오디오 믹서 제어를 위해 필수

namespace Team1
{
    public class VolumeControl : MonoBehaviour
    {
        [Header("오디오 믹서")]
        public AudioMixer audioMixer;

        [Header("슬라이더 연결")]
        public Slider masterSlider;
        public Slider backgroundSlider;
        public Slider effectSlider;

        private void Start()
        {
            // 슬라이더 초기값 설정 (0~1 사이의 값 중 1이 최대)
            masterSlider.value = 1f;
            backgroundSlider.value = 1f;
            effectSlider.value = 1f;

            audioMixer.SetFloat("Master", 0f);
            audioMixer.SetFloat("Background", 0f);
            audioMixer.SetFloat("Effect", 0f);

            // 슬라이더 이벤트 연결
            masterSlider.onValueChanged.AddListener(SetMasterVolume);
            backgroundSlider.onValueChanged.AddListener(SetBackgroundVolume);
            effectSlider.onValueChanged.AddListener(SetEffectVolume);
        }

        // 1. 글로벌(마스터) 볼륨 조절
        public void SetMasterVolume(float value)
        {
            // 믹서 파라미터 조절 (로그 스케일 적용)
            audioMixer.SetFloat("Master", Mathf.Log10(value) * 20);

            // ⭐ 핵심: 마스터를 움직이면 하위 슬라이더들도 같이 움직이게 함
            backgroundSlider.value = value;
            effectSlider.value = value;
        }

        // 2. 배경음 볼륨 조절
        public void SetBackgroundVolume(float value)
        {
            audioMixer.SetFloat("Background", Mathf.Log10(value) * 20);
        }

        // 3. 효과음 볼륨 조절
        public void SetEffectVolume(float value)
        {
            audioMixer.SetFloat("Effect", Mathf.Log10(value) * 20);
        }
    }
}
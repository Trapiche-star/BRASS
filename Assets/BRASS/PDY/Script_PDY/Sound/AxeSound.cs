using UnityEngine;

namespace BRASS
{
    /// 배틀액스 전용 사운드 처리 클래스
    public class AxeSound : MonoBehaviour
    {
        #region Variables
        [SerializeField] private AudioSource audioSource;   // 액스 사운드 재생용 오디오 소스
        [SerializeField] private AudioClip swingClip;       // 휘두르는 소리
        [SerializeField] private AudioClip hitClip;         // 적중 시 소리
        #endregion

        #region Public Method
        // 액스 휘두르는 소리 재생
        public void PlaySwing()
        {
            if (audioSource == null || swingClip == null) return;
            audioSource.PlayOneShot(swingClip);
        }

        // 액스 적중 소리 재생
        public void PlayHit()
        {
            if (audioSource == null || hitClip == null) return;
            audioSource.PlayOneShot(hitClip);
        }
        #endregion
    }
}

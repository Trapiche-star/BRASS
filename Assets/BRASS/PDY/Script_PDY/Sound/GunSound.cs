using UnityEngine;

namespace BRASS
{
    /// <summary>
    /// 총기 사운드(발사/리로드) 전용 클래스
    /// </summary>    
    public class GunSound : MonoBehaviour
    {
        #region Variables
        [SerializeField] private AudioSource audioSource;   // 총 사운드 재생용 오디오 소스
        [SerializeField] private AudioClip fireClip;        // 발사 사운드
        [SerializeField] private AudioClip reloadClip;      // 리로드 사운드
        #endregion

        #region Public Method
        // 총 발사 소리 재생
        public void PlayFire()
        {
            if (audioSource == null || fireClip == null)
                return;

            audioSource.PlayOneShot(fireClip);
        }

        // 총 리로드 소리 재생
        public void PlayReload()
        {
            if (audioSource == null || reloadClip == null)
                return;

            audioSource.PlayOneShot(reloadClip);
        }
        #endregion
    }
}

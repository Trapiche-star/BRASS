using UnityEngine;

namespace BRASS
{
    /// 플레이어 발소리 재생 전용 클래스
    public class PlayerFootstepSound : MonoBehaviour
    {
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip[] footstepClips;

        public void PlayFootstep()
        {
            if (footstepClips == null || footstepClips.Length == 0)
                return;

            AudioClip clip = footstepClips[Random.Range(0, footstepClips.Length)];
            audioSource.PlayOneShot(clip);
        }
    }
}

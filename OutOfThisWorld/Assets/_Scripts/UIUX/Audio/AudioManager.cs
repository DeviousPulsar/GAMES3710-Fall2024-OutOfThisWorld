
using UnityEngine;

namespace OutOfThisWorld {
    public class AudioManager : MonoBehaviour {
        [Header("------- Audio Source -------")]
        [SerializeField] AudioSource musicSource;
        [SerializeField] AudioSource SFXSource;

        [Header("------- Audio Clip -------")]
        public AudioClip background;
        public AudioClip iconClick;
        public AudioClip itemInteract;
        public AudioClip startInterface;

        private void Start()
        {
            PlaySFX(startInterface);
        }

        public void StopMusic()
        {
            musicSource.Stop();
        }

        public void PlaySFX(AudioClip clip)
        {
            SFXSource.PlayOneShot(clip);
        }
    }
}

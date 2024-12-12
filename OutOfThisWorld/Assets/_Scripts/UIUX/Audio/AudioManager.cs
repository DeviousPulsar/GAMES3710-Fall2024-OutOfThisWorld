
using UnityEngine;

namespace OutOfThisWorld.Audio {
    public class SingletonAudioManager : MonoBehaviour{
        [Header("------- Audio Source -------")]
        [SerializeField] AudioSource musicSource;
        [SerializeField] AudioSource SFXSource;

        [Header("------- Audio Clip -------")]
        public AudioClip background;
        public AudioClip iconClick;
        public AudioClip itemInteract;
        public AudioClip startInterface;
        public AudioClip breath;
        public AudioClip jumpScare1;
        public AudioClip jumpScare2;
        public AudioClip walk;

        public AudioClip boom;
        public AudioClip doorLocked;
        public AudioClip switch_pull;
        public AudioClip collection_ship;
        public AudioClip collection_drone;
        public AudioClip drone_build;
        public AudioClip drone_wreck;
        public AudioClip item_pickup;
        public AudioClip item_drop;
        public AudioClip endingBGM;
        public AudioClip doorOpen;
        public AudioClip doorClose;

        private static SingletonAudioManager instance;
        public static SingletonAudioManager Get() {
            if (instance is null) {
                Debug.LogError("There is no SingletonAudioManager in the current scene!");
            }
            return instance;
        }

        
        private void Awake()
        {
            if (instance != null)
                Destroy(gameObject);
            else
            {
                instance = this;
            }
        }

        void OnDelete() {
            if (instance == this) {
                instance = null;
            }
        }

        public void StopMusic()
        {
            musicSource.Stop();
        }

        public void PlaySFX(AudioClip clip) {
            SFXSource.PlayOneShot(clip);
        }

        public void PlaySFX(AudioSource source, AudioClip clip) {
            if (source == null) {
                SFXSource.PlayOneShot(clip);
            } else {
                source.volume = SFXSource.volume;
                source.PlayOneShot(clip);
            }
        }

        public void PlayBackgroundMusic(AudioClip clip)
        {
            if (musicSource.clip != clip) {
                musicSource.clip = clip;
                musicSource.loop = true; 
                musicSource.Play();
            }
        }
    }
}
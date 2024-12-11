using UnityEngine;
using UnityEngine.SceneManagement;
using OutOfThisWorld.Audio;

namespace OutOfThisWorld {
    public class MainMenu : MonoBehaviour {
        public AudioClip startBGM;
        public AudioClip buttonClick;

        private SingletonAudioManager _audioManager;

        void Awake() {
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
        }

        void Start() {
            _audioManager = SingletonAudioManager.Get();
            if (_audioManager == null) { Destroy(this); }
            _audioManager.PlayBackgroundMusic(startBGM);
        }

        public void PlayGame()
        {
            _audioManager.StopMusic();
            SceneManager.LoadSceneAsync(1);
        }

        public void QuitGame()
        {
            Application.Quit();
        }

        public void ClickEffect()
        {
            _audioManager.PlaySFX(buttonClick);
        }
    }
}
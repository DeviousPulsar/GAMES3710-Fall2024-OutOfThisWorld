using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace OutOfThisWorld {
    public class MainMenu : MonoBehaviour {
        public AudioManager audioManager;

        private void Awake()
        {
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
            audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
        }

        public void PlayGame()
        {
            audioManager.StopMusic();
            SceneManager.LoadSceneAsync(1);
        }

        public void QuitGame()
        {
            Application.Quit();
        }

        public void ClickEffect()
        {
            audioManager.PlaySFX(audioManager.iconClick);
        }
    }
}
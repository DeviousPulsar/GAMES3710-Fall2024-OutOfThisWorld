using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace OutOfThisWorld {
    public class PauseMenu : MonoBehaviour {
        [SerializeField] GameObject ContinueButton;

        public bool CanContinue {
            get => ContinueButton.activeSelf;
            set => ContinueButton.SetActive(value);
        }

        private bool _isPaused = false;
        public bool IsPaused {
            get => _isPaused;
        }

        public void TogglePause()
        {
            if (_isPaused && CanContinue) {
                Continue();
            } else {
                Pause();
            }
        }

        public void Pause()
        {
            _isPaused = true;
            gameObject.SetActive(true);
            Time.timeScale = 0;

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        public void NoStopPause()
        {
            _isPaused = true;
            gameObject.SetActive(true);

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        public void Continue()
        {
            _isPaused = false;
            gameObject.SetActive(false);
            Time.timeScale = 1;

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

        }

        public void Restart()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            Time.timeScale = 1;
        }

        public void Quit()
        {
            Debug.Log("Quit to Main Menu");
            SceneManager.LoadScene(0);
        }
    }
}
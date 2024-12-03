using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace OutOfThisWorld {
    public class PauseMenu : MonoBehaviour {
        [SerializeField] GameObject pauseMenu;

        private bool isPaused = false;

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape)) {
                if (isPaused) {
                    Continue();
                } else {
                    PauseGame();
                }
            }
        }

        public void PauseGame()
        {
            isPaused = true;
            pauseMenu.gameObject.SetActive(true);
            Time.timeScale = 0;

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        public void Continue()
        {
            isPaused = false;
            pauseMenu.gameObject.SetActive(false);
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
            Debug.Log("Quit");
            Application.Quit();
        }
    }
}
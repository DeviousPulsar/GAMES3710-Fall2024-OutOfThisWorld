using UnityEngine;
using UnityEngine.SceneManagement;
using OutOfThisWorld.Player;

namespace OutOfThisWorld {
    public class GameFlowManager : Triggerable {
        [SerializeField] GameObject HUD;
        [SerializeField] PauseMenu PauseMenu;
        [SerializeField] PlayerInputHandler PlayerIn;

        void Update() {
            if (PauseMenu.CanContinue && Input.GetButtonDown(PlayerIn.Pause)) {
                PauseMenu.TogglePause();
                HUD.SetActive(!PauseMenu.IsPaused);
            }
        }

        public void Lose() {
            PauseMenu.CanContinue = false;
            PauseMenu.NoStopPause();
            HUD.SetActive(false);
        }

        public void Win() {
            SceneManager.LoadScene(2);
        }

        public override void Trigger() {
            Win();
        }
    }
}

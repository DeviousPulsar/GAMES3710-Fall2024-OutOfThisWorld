using UnityEngine;
using UnityEngine.SceneManagement;
using OutOfThisWorld.Player;

namespace OutOfThisWorld {
    public class GameFlowManager : Triggerable {
        [SerializeField] PauseMenu PauseMenu;
        [SerializeField] PlayerInputHandler PlayerIn;

        void Update() {
            if (PauseMenu.CanContinue && Input.GetButtonDown(PlayerIn.Pause)) {
                PauseMenu.TogglePause();
            }
        }

        public void Lose() {
            PauseMenu.CanContinue = false;
            PauseMenu.NoStopPause();
        }

        public void Win() {
            SceneManager.LoadScene(2);
        }

        public override void Trigger() {
            Win();
        }
    }
}

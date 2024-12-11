using UnityEngine;
using OutOfThisWorld.Player;

namespace OutOfThisWorld {
    public class GameFlowManager : MonoBehaviour {
        [SerializeField] GameObject HUD;
        [SerializeField] PauseMenu PauseMenu;
        [SerializeField] PlayerInputHandler PlayerIn;

        void Update() {
            if (PauseMenu.CanContinue && Input.GetButtonDown(PlayerIn.Pause)) {
                PauseMenu.TogglePause();
            }

            HUD.SetActive(!PauseMenu.IsPaused);
        }

        public void Lose() {
            PauseMenu.CanContinue = false;
            PauseMenu.NoStopPause();
            HUD.SetActive(false);
        }

        public void Win() {
            PauseMenu.CanContinue = false;
            PauseMenu.Pause();
            HUD.SetActive(false);
        }
    }
}

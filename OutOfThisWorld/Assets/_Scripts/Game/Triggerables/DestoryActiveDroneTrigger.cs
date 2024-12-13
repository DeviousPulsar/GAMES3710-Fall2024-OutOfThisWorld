using UnityEngine;
using OutOfThisWorld.Player;

namespace OutOfThisWorld {
    public class DestoryActiveDroneTrigger : Triggerable {
        public PlayerController PlayerController;

        public override void Trigger() {
            PlayerController.GetActiveDrone().AttemptDestroy();
        }
    }
}

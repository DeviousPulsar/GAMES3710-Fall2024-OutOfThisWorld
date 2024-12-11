using UnityEngine;
using OutOfThisWorld.Player.HUD;

namespace OutOfThisWorld {
    public class TaskCompletionTrigger : Triggerable {
        [SerializeField] TaskInfoPanel TaskPanel;
        [SerializeField] string task;

        public override void Trigger() {
            TaskPanel.CompleteTask(task);
        }
    }
}

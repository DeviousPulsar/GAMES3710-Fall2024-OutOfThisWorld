using UnityEngine;

namespace OutOfThisWorld {
    public class EnableDisableTrigger : Triggerable {
        public bool Enable = true;
        public bool DoUndo = false;

        public override void Trigger() {
            gameObject.SetActive(Enable);
        }

        public override void Undo() {
            if (DoUndo) {
                gameObject.SetActive(!Enable);
            }
        }
    }
}

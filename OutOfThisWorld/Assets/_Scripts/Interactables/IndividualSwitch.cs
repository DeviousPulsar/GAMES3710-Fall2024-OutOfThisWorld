using UnityEngine;

namespace OutOfThisWorld {
    public class IndividualSwitch : MonoBehaviour
    {
        public int switchIndex;
        public SwitchController switchController;

        private void OnMouseDown()
        {
            switchController.ToggleSwitch(switchIndex);
        }
    }
}


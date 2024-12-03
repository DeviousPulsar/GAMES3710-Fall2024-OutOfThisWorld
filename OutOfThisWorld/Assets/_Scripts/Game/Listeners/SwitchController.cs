using UnityEngine;

namespace OutOfThisWorld {
    public class SwitchController : OnLoadListener {
        public Animator[] switchAnimators; 
        public Triggerable[] triggers;
        public bool[] desiredPositions;
        private bool[] switchStates; 
        private bool doorOpened = false; 

        void Start()
        {
            if (switchAnimators.Length != desiredPositions.Length)
            {
                Debug.LogError("Please assign and equal number of Switch Animators and Desired Positions! ");
                return;
            }

            switchStates = new bool[switchAnimators.Length];
        }

        public void ToggleSwitch(int switchIndex)
        {

            if (doorOpened)
            {
                return;
            }

            if (switchIndex < 0 || switchIndex >= switchAnimators.Length)
            {
                Debug.LogError("Invalid switch index!");
                return;
            }

            
            switchStates[switchIndex] = !switchStates[switchIndex];

            
            if (switchStates[switchIndex])
            {
                switchAnimators[switchIndex].Play("Switch_down_to_up", 0, 0f);
            }
            else
            {
                switchAnimators[switchIndex].Play("Switch_up_to_down", 0, 0f);
            }
        }

        protected override bool CheckTriggered()
        {
            for (int i = 0; i < switchAnimators.Length; i++) {
                if (desiredPositions[i] != switchStates[i]) {
                    return false;
                }
            }
           
            return true;
        }
    }
}
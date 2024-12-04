using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OutOfThisWorld {
    public abstract class BooleanStateMachine : MonoBehaviour {
        public abstract bool GetState();
    }

    public class BooleanStateListener : BooleanStateMachine {
        
        /* ----------| Serialized Variables |---------- */

            [System.Serializable]
            public class MachineValuePair {
                public BooleanStateMachine machine;
                public bool desiredState;
            }

            [SerializeField] List<Triggerable> Triggers;
            [SerializeField] List<MachineValuePair> MachineValuePairs;

        /* ----------| Private Variables |---------- */

            private bool _lastState = false;

        /* ----------| Main Loop |----------- */

        void FixedUpdate() {
            bool state = GetState();
            if (state && !_lastState) {
                foreach (Triggerable t in Triggers) { t.Trigger(); }
            } else if (!state && _lastState) {
                foreach (Triggerable t in Triggers) { t.Undo(); }
            }
            _lastState = state;
        }

        /* ----------| Override Methods |----------- */

            public override bool GetState() {
                foreach (MachineValuePair pair in MachineValuePairs)
                {
                    if (pair.machine.GetState() != pair.desiredState) { 
                        return false; 
                    }
                }

                return true;
            }
    }
}

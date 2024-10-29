using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OutOfThisWorld {
    public class UseCountListener : MonoBehaviour {
        /* ----------| Serialized Variables |---------- */

            [SerializeField] List<Triggerable> Triggers;
            public int TriggerOn = 0;

        /* ----------| Instance Variables |---------- */

            private int _numSpawns = 0;

        /* ----------| Main Loop |----------- */

            public void Count() {
                _numSpawns += 1;
            }

            void FixedUpdate() 
            {
                bool triggered = CheckTriggers();
                if (triggered) {
                    foreach (Triggerable t in Triggers) {
                        t.Trigger();
                    }
                    Destroy(this);
                }
            }

            bool CheckTriggers() {
                return _numSpawns >= TriggerOn;
            }
    }
}

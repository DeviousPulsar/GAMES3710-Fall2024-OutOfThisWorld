using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OutOfThisWorld {
    public class UseCountListener : BooleanStateMachine {

        /* ----------| Serialized Variables |---------- */

            [SerializeField] List<Triggerable> Triggers;
            public int TriggerOn = 0;

        /* ----------| Instance Variables |---------- */

            private int _count = 0;

        /* ----------| Main Loop |----------- */

            public void Count() {
                _count += 1;
                
                if (_count == TriggerOn) {
                    foreach (Triggerable t in Triggers) { t.Trigger(); }
                }
            }

            public override bool GetState() {
                return _count >= TriggerOn;
            }
    }
}

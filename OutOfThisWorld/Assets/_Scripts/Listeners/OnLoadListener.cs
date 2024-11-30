using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OutOfThisWorld {
    public class OnLoadListener : MonoBehaviour {

        /* ----------| Serialized Variables |---------- */

            [SerializeField] List<Triggerable> Triggers;

        /* ----------| Instance Variables |---------- */

            private bool _lastTrigged = false;

        /* ----------| Main Functions |----------- */

            void FixedUpdate() 
            {
                bool triggered = CheckTriggered();
                if (triggered && !_lastTrigged) {
                    foreach (Triggerable t in Triggers) { t.Trigger(); }
                } else if (!triggered && _lastTrigged) {
                    foreach (Triggerable t in Triggers) { t.Undo(); }
                }
                _lastTrigged = triggered;
            }

            public bool Triggered() {
                return _lastTrigged;
            }

        /* ----------| Virtual Methods |---------- */

            protected virtual bool CheckTriggered() {
                return true;
            }
    }
}

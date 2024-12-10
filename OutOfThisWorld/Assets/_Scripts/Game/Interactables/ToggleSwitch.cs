using System.Collections.Generic;
using UnityEngine;

namespace OutOfThisWorld {
    public class ToggleSwitch : BooleanStateMachine {

        /* ----------| Serialized Variables |---------- */

            [SerializeField] List<Triggerable> Triggers;

        /* ----------| Private Variables |---------- */

            private bool _state = false;

        /* ----------| Interaction Functions |---------- */

            public void Toggle()
            {
                _state = !_state;
                if (_state) {
                    foreach (Triggerable t in Triggers) { t.Trigger(); }
                } else {
                    foreach (Triggerable t in Triggers) { t.Undo(); }
                }
            }

            public override bool GetState()
            {
                return _state;
            }

    }
}


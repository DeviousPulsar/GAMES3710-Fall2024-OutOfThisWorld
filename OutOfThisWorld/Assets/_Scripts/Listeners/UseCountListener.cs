using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OutOfThisWorld {
    public class UseCountListener : OnLoadListener {

        /* ----------| Serialized Variables |---------- */

            public int TriggerOn = 0;

        /* ----------| Instance Variables |---------- */

            private int _numSpawns = 0;

        /* ----------| Main Loop |----------- */

            public void Count() {
                _numSpawns += 1;
            }

            protected override bool CheckTriggered() {
                return _numSpawns >= TriggerOn;
            }
    }
}

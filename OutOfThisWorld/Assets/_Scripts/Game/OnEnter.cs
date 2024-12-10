using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OutOfThisWorld {
    [RequireComponent(typeof(Collider))]
    public class OnEnter : MonoBehaviour {

        /* ----------| Serialized Variables |---------- */

            [SerializeField] List<Triggerable> Triggers;
            public bool UndoTriggersOnLeave;

        /* ----------| Main Functions |----------- */

            void OnTriggerEnter() {
                foreach (Triggerable t in Triggers) { t.Trigger(); }
            }

            void OnTriggerExit() {
                if (UndoTriggersOnLeave) {
                    foreach (Triggerable t in Triggers) { t.Undo(); }
                }
            }
    }
}

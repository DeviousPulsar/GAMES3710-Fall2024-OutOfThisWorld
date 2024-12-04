using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OutOfThisWorld {
    public class UseBehavior : MonoBehaviour
    {
        /* ----------| Serialized Variables |---------- */

            [SerializeField] List<Triggerable> Triggers;
            [SerializeField] List<UseCountListener> Listeners;

        /* ----------| Main Functions |---------- */

            private void OnMouseDown() {
                foreach (Triggerable t in Triggers) { t.Trigger(); }
                foreach(UseCountListener l in Listeners) { l.Count(); }
            }
    }
    }
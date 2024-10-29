using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OutOfThisWorld {
    public class DestoryTrigger : Triggerable {
        /* ----------| Trigger Functions |---------- */

            public override void Trigger() {
                Destroy(gameObject);
            }

            public override void Undo() {
                UnityEngine.Debug.Log("Trigger DestoryTrigger cannot be undone!");
            }
    }
}

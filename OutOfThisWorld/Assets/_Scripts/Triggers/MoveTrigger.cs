using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OutOfThisWorld {
    public class MoveTrigger : Triggerable {
        
        /* ----------| Serialized Variables |---------- */

            public Transform InitialTransform;
            public Transform DestinationTransform;

        /* ----------| Trigger Functions |---------- */

            public override void Trigger() {
                transform.position = DestinationTransform.position;
                transform.rotation = DestinationTransform.rotation;
                transform.localScale = DestinationTransform.localScale;
            }

            public override void Undo() {
                transform.position = InitialTransform.position;
                transform.rotation = InitialTransform.rotation;
                transform.localScale = InitialTransform.localScale;
            }
    }
}
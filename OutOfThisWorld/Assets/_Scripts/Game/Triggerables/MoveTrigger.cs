using UnityEngine;

namespace OutOfThisWorld {
    public class MoveTrigger : Triggerable {
        /* ----------| Serialized Variables |---------- */

            public GameObject ObjectToMove;
            public Transform DestinationTransform;
            public Transform UndoTransform;

        /* ----------| Trigger Functions |---------- */

            public override void Trigger() {
                ObjectToMove.transform.position = DestinationTransform.position;
                ObjectToMove.transform.rotation = DestinationTransform.rotation;
                ObjectToMove.transform.localScale = DestinationTransform.localScale;
            }

            public override void Undo() {
                if (UndoTransform) {
                    ObjectToMove.transform.position = UndoTransform.position;
                    ObjectToMove.transform.rotation = UndoTransform.rotation;
                    ObjectToMove.transform.localScale = UndoTransform.localScale;
                }
            }
    }
}

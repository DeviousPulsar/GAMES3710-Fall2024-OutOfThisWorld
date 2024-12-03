using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OutOfThisWorld {
    public abstract class Triggerable : MonoBehaviour {
        public abstract void Trigger();
        public virtual void Undo() {
            Debug.LogWarning("Warning: Triggerable of type " + GetType() + " cannot be undone!");
        }
    }

    public class DestoryTrigger : Triggerable {
        public override void Trigger() {
            Destroy(gameObject);
        }
    }
}

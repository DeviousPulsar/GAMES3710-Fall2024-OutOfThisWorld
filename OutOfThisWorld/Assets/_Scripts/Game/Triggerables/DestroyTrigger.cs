using UnityEngine;

namespace OutOfThisWorld {
    public abstract class Triggerable : MonoBehaviour {
        public abstract void Trigger();
        public virtual void Undo() {
            Debug.LogWarning("Warning: Triggerable of type " + GetType() + " cannot be undone!");
        }
    }

    public class DestroyTrigger : Triggerable {
        public Component ToDestory;
        public bool DestroyGameObject;

        public override void Trigger() {
            if (DestroyGameObject) {
                Destroy(gameObject);
            } else if (ToDestory is not null) {
                Destroy(ToDestory);
            }
        }
    }
}

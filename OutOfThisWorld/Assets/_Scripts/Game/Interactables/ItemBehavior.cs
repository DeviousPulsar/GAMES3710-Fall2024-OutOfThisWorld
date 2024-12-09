using UnityEngine;
using deVoid.Utils;

namespace OutOfThisWorld {
    public class ItemSpawned : ASignal<ItemBehavior> {}
    public class ItemDropped : ASignal<ItemBehavior> {}

    [RequireComponent(typeof(Rigidbody)), RequireComponent(typeof(Renderer))]
    public class ItemBehavior : MonoBehaviour {
        /* ----------| Serialized Variables |---------- */

            public float ItemCost = 1f;
            public string ItemTag;

        /* ----------| Instance Variables |---------- */

            private bool _isHeld = false;
            private Rigidbody _rigidbody;
            private float _initMass;
            private Renderer _renderer;

        /* ----------| Initaliazation Functions |----------- */

            void Start()
            {
                // fetch Rigidbody and MeshRenderer
                _rigidbody = GetComponent<Rigidbody>();
                DebugUtility.HandleErrorIfNullGetComponent<Rigidbody, ItemBehavior>(_rigidbody, this, gameObject);
                _initMass = _rigidbody.mass;

                _renderer = GetComponent<Renderer>();
                DebugUtility.HandleErrorIfNullGetComponent<Renderer, ItemBehavior>(_renderer, this, gameObject);

                //Debug.Log(gameObject + " has colliders with local bounds with " + _bounds);

                Signals.Get<ItemSpawned>().Dispatch(this);
            }

        /* ----------| Getters |-------- */

            public bool IsHeld()
            {
                return _isHeld;
            }

            public Rigidbody GetRigidbody()
            {
                return _rigidbody;
            }

            public Bounds LocalBounds() {
                return _renderer.localBounds;
            }

        /* ----------| Pickup and Dropping Functions |----------- */

            public virtual void Grab(Transform holdPos, float massScale)
            {
                gameObject.tag = "Untagged";

                transform.parent = holdPos;
                transform.rotation = holdPos.rotation;
                Vector3 extents = _renderer.localBounds.max;
                transform.localPosition = new Vector3(0, -extents.y*transform.localScale.y, extents.z*transform.localScale.z);

                _isHeld = true;

                _rigidbody.useGravity = false;
                _rigidbody.mass = massScale*_initMass;
            }

            public virtual void Drop()
            {
                gameObject.layer = LayerMask.NameToLayer("Default");
                gameObject.tag = "Selectable";

                transform.parent = transform.root;

                _isHeld = false;
                _rigidbody.useGravity = true;

                _rigidbody.mass = _initMass;

                Signals.Get<ItemDropped>().Dispatch(this);
            }
    }
}

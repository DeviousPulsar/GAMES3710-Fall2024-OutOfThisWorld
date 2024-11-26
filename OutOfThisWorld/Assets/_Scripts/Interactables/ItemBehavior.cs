using UnityEngine;

namespace OutOfThisWorld {
    [RequireComponent(typeof(Rigidbody))]//, RequireComponent(typeof(Renderer))]
    public class ItemBehavior : MonoBehaviour {
        /* ----------| Component Properties |---------- */

            public float ItemCost = 1f;
            public string ItemTag;

        /* ----------| Instance Variables |---------- */

            private bool _isHeld = false;
            private Rigidbody _rigidbody;
            private float _initMass;
            private Renderer _renderer;

        /* ----------| Initaliazation Functions |----------- */

            void Awake()
            {
                // fetch Rigidbody and MeshRenderer
                _rigidbody = GetComponent<Rigidbody>();
                DebugUtility.HandleErrorIfNullGetComponent<Rigidbody, ItemBehavior>(_rigidbody, this, gameObject);
                _initMass = _rigidbody.mass;

                _renderer = GetComponent<Renderer>();
                DebugUtility.HandleErrorIfNullGetComponent<Renderer, ItemBehavior>(_renderer, this, gameObject);

                //Debug.Log(gameObject + " has colliders with local bounds with " + _bounds);
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

            public Bounds Bounds() {
                return _renderer.bounds;
            }

        /* ----------| Pickup and Dropping Functions |----------- */

            public void Grab(Transform holdPos, float massScale)
            {
                gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
                
                transform.parent = holdPos;
                transform.rotation = holdPos.rotation;
                //transform.LookAt(holdPos);
                Vector3 extents = _renderer.localBounds.max;
                transform.localPosition = new Vector3(0, -extents.y*transform.lossyScale.y, extents.z*transform.lossyScale.z);

                _isHeld = true;

                _rigidbody.useGravity = false;
                _rigidbody.mass = massScale*_initMass;
            }

            public void Drop()
            {
                gameObject.layer = LayerMask.NameToLayer("Default");

                transform.parent = transform.root;

                _isHeld = false;
                _rigidbody.useGravity = true;

                _rigidbody.mass = _initMass;
            }
    }
}

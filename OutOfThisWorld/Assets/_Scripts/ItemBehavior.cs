using UnityEngine;
using OutOfThisWorld.Debug;

namespace OutOfThisWorld
{
    [RequireComponent(typeof(Rigidbody))]
    public class ItemBehavior : MonoBehaviour
    {
        /* ----------| Component Properties |---------- */

            public float ItemCost = 1f;
            public string ItemTag;

        /* ----------| Instance Variables |---------- */

        private bool _isHeld = false;
        private Rigidbody _rigidbody;

        /* ----------| Initaliazation Functions |----------- */

        void Awake()
        {
            // fetch components on the same gameObject
            _rigidbody = GetComponent<Rigidbody>();
            DebugUtility.HandleErrorIfNullGetComponent<Rigidbody, ItemBehavior>(_rigidbody, this, gameObject);
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

        /* ----------| Pickup and Dropping Functions |----------- */

            public void Grab(Vector3 pos, Quaternion rot, float scale, float massScale)
            {
                transform.position = pos;
                transform.rotation = rot;
                transform.localScale = transform.localScale*scale;
                _isHeld = true;

                _rigidbody.useGravity = false;
                _rigidbody.mass = massScale*_rigidbody.mass;
            }

            public void Drop(float scale, float massScale)
            {
                transform.localScale = transform.localScale/scale;
                _isHeld = false;
                _rigidbody.useGravity = true;
                _rigidbody.mass = massScale/_rigidbody.mass;
            }
    }
}

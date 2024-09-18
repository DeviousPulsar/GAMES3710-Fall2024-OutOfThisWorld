using UnityEngine;
using OutOfThisWorld.Debug;

namespace OutOfThisWorld.Player
{
    [RequireComponent(typeof(Rigidbody))]
    public class DroneController : MonoBehaviour
    {
        enum DroneMode { ACTIVE, INACTIVE }

    /* ----------| Component Properties |---------- */

        public float MaxSpeed = 1f;
        public float Acceleration = 10f;
        public float MaxXAxisLook = 90f;

    /* ----------| Instance Variables |---------- */

        private Vector3 _eulers = Vector3.zero;

        private Rigidbody _rigidbody;
        private PlayerInputHandler _playerInputHandler;
        private DroneMode _mode;

    /* ----------| Initalization Functions |---------- */

        void Start()
        {
            // fetch components on the same gameObject
            _rigidbody = GetComponent<Rigidbody>();
            DebugUtility.HandleErrorIfNullGetComponent<Rigidbody, DroneController>(_rigidbody,
                this, gameObject);
        }

    /* ----------| Movement Functions |---------- */

        public void HandleMove(Vector3 move_dir, Vector3 look_dir, float delta) 
        {
            _eulers += look_dir;
            _eulers.x = Mathf.Clamp(_eulers.x, -MaxXAxisLook, MaxXAxisLook);
            transform.eulerAngles = _eulers;
            move_dir = Quaternion.Euler(_eulers.x, _eulers.y, _eulers.z)*move_dir;

            if(move_dir != Vector3.zero)
            {
                _rigidbody.velocity += Acceleration*delta*move_dir;
                _rigidbody.velocity = Vector3.ClampMagnitude(_rigidbody.velocity, MaxSpeed);
            }
        }
    }
}
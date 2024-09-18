using UnityEngine;
using OutOfThisWorld.Debug;

namespace OutOfThisWorld.Player
{
    [RequireComponent(typeof(Rigidbody), typeof(PlayerInputHandler))]
    public class DroneController : MonoBehaviour
    {

    /* ----------| Component Properties |---------- */

        public float MaxSpeed = 1f;
        public float Acceleration = 10f;
        public float Friction = 2f;
        //public float GravityTimeoutLength = 5f;

    /* ----------| Instance Variables |---------- */

        private Vector3 _velocity = Vector3.zero;
        //private float _gravityTimeout;

        private Rigidbody _rigidbody;
        private PlayerInputHandler _playerInputHandler;

    /* ----------| Initalization Functions |---------- */

        void Start()
        {
            // fetch components on the same gameObject
            _rigidbody = GetComponent<Rigidbody>();
            DebugUtility.HandleErrorIfNullGetComponent<Rigidbody, DroneController>(_rigidbody,
                this, gameObject);
            _playerInputHandler = GetComponent<PlayerInputHandler>();
            DebugUtility.HandleErrorIfNullGetComponent<PlayerInputHandler, DroneController>(_playerInputHandler,
                this, gameObject);

            // Set initial values
            //_gravityTimeout = GravityTimeoutLength;
        }

    /* ----------| Movement Functions |---------- */

        void Update()
        {
            float delta = Time.deltaTime;
            //_gravityTimeout -= delta;
            //_rigidbody.useGravity = _gravityTimeout <= 0;

            HandleMove(delta);
        }

        void HandleMove(float delta) 
        {
            Vector3 direction = transform.eulerAngles + _playerInputHandler.GetLookDirection();
            transform.eulerAngles = direction;

            Vector3 move_dir = _playerInputHandler.GetMoveForce();
            move_dir = Quaternion.Euler(direction.x, direction.y, direction.z)*move_dir;

            if(move_dir != Vector3.zero)
            {
                _velocity += Acceleration*delta*move_dir;
                _velocity = Vector3.ClampMagnitude(_velocity, MaxSpeed);

                //_gravityTimeout = GravityTimeoutLength;
            } 
            else if (_velocity.magnitude > 0) { 
                _velocity -= Friction*delta*_velocity; 
            } else {
                _velocity = Vector3.zero;
            }


            transform.position += delta*_velocity;
        }
    }
}
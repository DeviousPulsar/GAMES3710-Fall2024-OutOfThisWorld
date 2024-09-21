using UnityEngine;
using OutOfThisWorld.Debug;
using System.Collections.Generic;
using UnityEngine.UI;

namespace OutOfThisWorld.Player
{
    [RequireComponent(typeof(Rigidbody)), RequireComponent(typeof(CapsuleCollider))]
    public class DroneController : MonoBehaviour
    {
        enum DroneMode { ACTIVE, INACTIVE }

    /* ----------| Component Properties |---------- */

        public float MaxSpeed = 1f;
        public float Acceleration = 10f;
        public float MaxXAxisLook = 90f;
        public int MaxStorageSize = 1;

    /* ----------| Instance Variables |---------- */

        private Vector3 _eulers = Vector3.zero;

        private Rigidbody _rigidbody;
        private PlayerInputHandler _playerInputHandler;
        private DroneMode _mode;


        private CapsuleCollider _interactionRange;
        private ISet<Collider> _occupingBodies;

        private List<ItemBehavior> _droneStorageList;

        /* ----------| Initalization Functions |---------- */

        void Awake()
        {
            // fetch components on the same gameObject
            _rigidbody = GetComponent<Rigidbody>();
            DebugUtility.HandleErrorIfNullGetComponent<Rigidbody, DroneController>(_rigidbody, this, gameObject);

            // Initialize collider set
            _occupingBodies = new HashSet<Collider>();

            _droneStorageList = new List<ItemBehavior> { };

            // fetch components on the same gameObject
            _interactionRange = this.GetComponent<CapsuleCollider>();
            DebugUtility.HandleErrorIfNullGetComponent<CapsuleCollider, PlayerController>(_interactionRange, this, gameObject);

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

        public bool IsOccupied()
        {
            return _occupingBodies.Count > 0;
        }

        public bool interactWithOccupied()
        {
            foreach (Collider collider in _occupingBodies)
            {
                if (collider.gameObject.GetComponent<ItemBehavior>() != null && _droneStorageList.Count < MaxStorageSize)
                {
                    _droneStorageList.Add((collider.gameObject.GetComponent<ItemBehavior>())); // Add item to inventory

                    _occupingBodies.Remove(collider);
                    //Destroy(collider.gameObject);

                    Vector3 position = collider.transform.position;
                    position.y -= 100f;
                    collider.transform.position = position;
                    return true;

                } else if(collider.gameObject.GetComponent<DepositBehavior>() != null && _droneStorageList.Count > 0)
                {

                    collider.gameObject.GetComponent<DepositBehavior>().MakeDeposit(_droneStorageList[0]);
                    _droneStorageList.RemoveAt(0);
                    _droneStorageList.Sort();
                }
            }
            return false;
        }


        /* ----------| Message Processing |---------- */

        private void OnTriggerEnter(Collider other)
        {
            _occupingBodies.Add(other);
        }

        private void OnTriggerExit(Collider other)
        {
            _occupingBodies.Remove(other);
        }
    }
}
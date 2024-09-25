using UnityEngine;
using OutOfThisWorld.Debug;
using System.Collections.Generic;
using UnityEngine.UI;

namespace OutOfThisWorld.Player
{
    [RequireComponent(typeof(Rigidbody))]
    public class DroneController : MonoBehaviour
    {
        enum DroneMode { ACTIVE, INACTIVE }

    /* ----------| Component Properties |---------- */

        [Header("Movement")]
        public float MaxSpeed = 1f;
        public float Acceleration = 10f;
        public float MaxXAxisLook = 90f;
        [Header("Item Pick Up")]
        public int MaxStorageSize = 1;
        public float InteractionRange = 3;

    /* ----------| Instance Variables |---------- */

        private Vector3 _eulers = Vector3.zero;

        private Rigidbody _rigidbody;
        private PlayerInputHandler _playerInputHandler;
        private DroneMode _mode;

        private List<ItemBehavior> _droneStorageList;

        /* ----------| Initalization Functions |---------- */

        void Awake()
        {
            // fetch components on the same gameObject
            _rigidbody = GetComponent<Rigidbody>();
            DebugUtility.HandleErrorIfNullGetComponent<Rigidbody, DroneController>(_rigidbody, this, gameObject);

            // Initialize pickup list
            _droneStorageList = new List<ItemBehavior> { };
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

        public bool Interact()
        {
             // Calculate raycast parameters
            Vector3 raySrc = transform.position;
            Vector3 rayDir = transform.TransformDirection(Vector3.forward);
            //int layermask = 1 << 7;

            // Cast ray and update position if there is a hit
            RaycastHit hit;
            if (Physics.Raycast(raySrc, rayDir, out hit, InteractionRange)) {
                Collider hitCol = hit.collider;
                if (hitCol.gameObject.GetComponent<ItemBehavior>() != null && _droneStorageList.Count < MaxStorageSize)
                {
                    _droneStorageList.Add((hitCol.gameObject.GetComponent<ItemBehavior>())); // Add item to inventory

                    Vector3 position = hitCol.transform.position;
                    position.y -= 100f;
                    hitCol.transform.position = position;
                    return true;
                } else if(hitCol.gameObject.GetComponent<DepositBehavior>() != null && _droneStorageList.Count > 0)
                {

                    hitCol.gameObject.GetComponent<DepositBehavior>().MakeDeposit(_droneStorageList[0]);
                    GameObject desposited = _droneStorageList[0].gameObject;
                    _droneStorageList.RemoveAt(0);
                    Destroy(desposited);
                    return true;
                }
            }


            return false;
        }
    }
}
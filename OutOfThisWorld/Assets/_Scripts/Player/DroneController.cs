using UnityEngine;
using UnityEngine.UI;
using OutOfThisWorld.Debug;
using System.Collections.Generic;

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
        public Transform HoldTransform;
        public float HoldScale = 1f;
        public float BreakHoldForce = 100f;
        public float ThrowForce = 1f;

    /* ----------| Instance Variables |---------- */

        private Vector3 _eulers = Vector3.zero;

        private Rigidbody _rigidbody;
        private PlayerInputHandler _playerInputHandler;

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

        /* ----------| Main Loop |----------- */

        void Update()
        {
            if (_droneStorageList.Count > 0 && gameObject.GetComponent<FixedJoint>() == null)
            {
                RenderInHand(_droneStorageList[0]);
            }
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

            // Cast ray and update position if there is a hit
            RaycastHit hit;
            if (Physics.Raycast(raySrc, rayDir, out hit, InteractionRange)) {
                Collider hitCol = hit.collider;

                ItemBehavior hitItem = hitCol.gameObject.GetComponent<ItemBehavior>();
                DepositBehavior hitDepot = hitCol.gameObject.GetComponent<DepositBehavior>();
                Spawner hitSpawner = hitCol.gameObject.GetComponent<Spawner>();
                ItemSocket hitSocket = hitCol.gameObject.GetComponent<ItemSocket>();

                UnityEngine.Debug.Log("" + hitItem + hitDepot + hitSpawner);
                if (_droneStorageList.Count < MaxStorageSize && hitItem != null && !hitItem.IsHeld())
                {
                    Pickup(hitItem);
                    
                    return true;
                } else if (_droneStorageList.Count > 0)
                {
                    if (hitDepot != null) {
                        hitDepot.MakeDeposit(_droneStorageList[0]);
                        
                        GameObject desposited = _droneStorageList[0].gameObject;
                        _droneStorageList.RemoveAt(0);
                        Destroy(desposited);
                        Destroy(gameObject.GetComponent<FixedJoint>());

                        return true;
                    } else if (hitSocket != null && !hitSocket.HasItem()) {
                        ItemBehavior item = _droneStorageList[0];
                        DropHeld();
                        hitSocket.TakeItem(item);

                        return true;
                    }
                    
                    return false;
                } else if (_droneStorageList.Count < MaxStorageSize && hitSocket != null && hitSocket.HasItem()) {
                    ItemBehavior item = hitSocket.GiveItem();
                    _droneStorageList.Add(item);
                    item.gameObject.SetActive(false);
                    
                    return true;
                }
            }


            return false;
        }

        public void Pickup(ItemBehavior item)
        {
            _droneStorageList.Add(item); // Add item to inventory
            item.gameObject.SetActive(false);
        }

        public void Deposit(DepositBehavior depot)
        {
            depot.MakeDeposit(_droneStorageList[0]);
                    
            GameObject desposited = _droneStorageList[0].gameObject;
            _droneStorageList.RemoveAt(0);
            Destroy(desposited);
            Destroy(gameObject.GetComponent<FixedJoint>());
        }

        public bool DropHeld()
        {
            FixedJoint holdJoint = gameObject.GetComponent<FixedJoint>();
            if (_droneStorageList.Count > 0 && holdJoint != null)
            {
                _droneStorageList[0].Drop(HoldScale);
                _droneStorageList[0].GetComponent<Rigidbody>().AddForce(transform.rotation*(ThrowForce*Vector3.forward), ForceMode.Impulse);
                _droneStorageList.RemoveAt(0);
                Destroy(holdJoint);
                return true;
            }

            return false;
        }

        void RenderInHand(ItemBehavior item)
        {
            item.gameObject.SetActive(true);
            item.Grab(HoldTransform.position, HoldTransform.rotation, HoldScale);

            FixedJoint holdJoint = gameObject.AddComponent<FixedJoint>();
            holdJoint.connectedBody = item.GetRigidbody();
            holdJoint.breakForce = BreakHoldForce;
            holdJoint.breakTorque = BreakHoldForce;
        }

        /* ----------| Message Handling |---------- */

        void OnJointBreak(float breakForce)
        {
            UnityEngine.Debug.Log("A joint has just been broken!, dropping: " + _droneStorageList[0]);
            _droneStorageList[0].Drop(HoldScale);
            _droneStorageList.RemoveAt(0);
        }
    }
}
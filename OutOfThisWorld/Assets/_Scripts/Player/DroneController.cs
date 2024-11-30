using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using OutOfThisWorld.Player.HUD;
using deVoid.Utils;

namespace OutOfThisWorld.Player {
    public class DroneDestroyed : ASignal<DroneController> {}

    [RequireComponent(typeof(Rigidbody))]
    public class DroneController : MonoBehaviour {
        
        /* ----------| Serialized Variables |---------- */


            [Header("Camera")]
            public Transform CameraOffset;
            [Header("Movement")]
            public float MaxSpeed = 1f;
            public float Acceleration = 10f;
            public float MaxXAxisLook = 90f;
            [Header("Item Pick Up")]
            public float InteractionRange = 3;
            public int MaxStorageSize = 1;
            public Transform HoldTransform;
            public LayerMask _raycastMask;
            public float HeldItemMassFactor = 1f;
            public float BreakHoldForce = 100f;
            public float ThrowForce = 1f;

        /* ----------| Private Variables |---------- */

            private Vector3 _eulers = Vector3.zero;

            private Rigidbody _rigidbody;
            private PlayerController _playerController;

            private List<ItemBehavior> _droneStorageList;
            

        /* ----------| Initalization Functions |---------- */

            void Awake() {
                // fetch components on the same gameObject
                _rigidbody = GetComponent<Rigidbody>();
                DebugUtility.HandleErrorIfNullGetComponent<Rigidbody, DroneController>(_rigidbody, this, gameObject);

                // Initialize pickup list
                _droneStorageList = new List<ItemBehavior> { };

                // Fetch nearest PlayerController
                _playerController = FindObjectOfType<PlayerController>();
                DebugUtility.HandleErrorIfNullGetComponent<PlayerController, DroneController>(_playerController, this, gameObject);
            }

        /* ----------| Main Loop |----------- */

            void FixedUpdate() {
                if (_droneStorageList.Count > 0) {
                    ItemBehavior item = _droneStorageList[0];
                    if (gameObject.GetComponent<FixedJoint>() == null) {
                        RenderInHand(item);
                    }
                }
            }

        /* ----------| Movement Functions |---------- */

            public void HandleMove(Vector3 move_dir, Vector3 look_dir, float delta) {
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

            public bool Interact() {
                // Calculate raycast parameters
                Vector3 raySrc = transform.position;
                Vector3 rayDir = transform.TransformDirection(Vector3.forward);

                // Cast ray and update position if there is a hit
                RaycastHit hit;
                if (Physics.Raycast(raySrc, rayDir, out hit, InteractionRange, _raycastMask)) {
                    Collider hitCol = hit.collider;

                    ItemBehavior hitItem = hitCol.gameObject.GetComponent<ItemBehavior>();
                    DepositBehavior hitDepot = hitCol.gameObject.GetComponent<DepositBehavior>();
                    ItemSocket hitSocket = hitCol.gameObject.GetComponent<ItemSocket>();
                    ClickBehavior hitTrigger = hitCol.gameObject.GetComponent<ClickBehavior>();
                    
                    // If Inventory not full
                    if (_droneStorageList.Count < MaxStorageSize) { 
                        if (hitSocket != null && hitSocket.HasItem()) {
                            UnsocketItem(hitSocket);
                            return true;
                        } else if (hitItem != null && !hitItem.IsHeld()) {
                            Pickup(hitItem);
                            return true;
                        }
                    }
                    
                    // If holding at least 1 item
                    if (_droneStorageList.Count > 0) { 
                        if (hitDepot != null) {
                            DepositHeld(hitDepot);
                            return true;
                        } else if (hitSocket != null && !hitSocket.HasItem()) {
                            SocketItem(hitSocket);
                            return true;
                        }
                    }
                    
                    if (hitTrigger != null) {
                        hitTrigger.Interact();
                        return true;
                    }
                }

                return false;
            }

            public void Pickup(ItemBehavior item) {
                _droneStorageList.Add(item); // Add item to inventory
                item.gameObject.SetActive(false);

                _playerController._taskUIPanel.CompleteTask("Pick Up an Object (Left Click)");
            }

            public void UnsocketItem(ItemSocket socket) {
                ItemBehavior item = socket.GiveItem();
                _droneStorageList.Add(item);
                item.gameObject.SetActive(false);
            }

            public void SocketItem(ItemSocket socket) {
                ItemBehavior item = _droneStorageList[0];
                DropHeld();
                socket.TakeItem(item);
            }

            public void DepositHeld(DepositBehavior depot) {
                depot.MakeDeposit(_droneStorageList[0]);
                            
                GameObject desposited = _droneStorageList[0].gameObject;
                _droneStorageList.RemoveAt(0);
                Destroy(desposited);
                Destroy(gameObject.GetComponent<FixedJoint>());

                _playerController._taskUIPanel.CompleteTask("Deposit item in ship (Left Click anywhere on the ship)");
            }

            public bool DropHeld() {
                FixedJoint holdJoint = gameObject.GetComponent<FixedJoint>();
                if (_droneStorageList.Count > 0 && holdJoint != null && CanDropCurrentItem()) {
                    _droneStorageList[0].Drop();
                    _droneStorageList[0].GetComponent<Rigidbody>().AddForce(transform.rotation*(ThrowForce*Vector3.forward), ForceMode.Impulse);
                    _droneStorageList.RemoveAt(0);
                    Destroy(holdJoint);

                    _playerController._taskUIPanel.CompleteTask("Drop an object (Right Click)");
                    return true;
                }

                return false;
            }

            void RenderInHand(ItemBehavior item) {
                item.gameObject.SetActive(true);
                item.Grab(HoldTransform, HeldItemMassFactor);
                item.gameObject.layer = LayerMask.NameToLayer("ActiveHoldLayer");

                // Create a Joint to hold the item in place relative to the drone
                FixedJoint holdJoint = gameObject.AddComponent<FixedJoint>();
                holdJoint.connectedBody = item.GetRigidbody();
                holdJoint.breakForce = BreakHoldForce;
                holdJoint.breakTorque = BreakHoldForce;
            }

        /* ----------| State Change Functions |---------- */

            public void ActivateDrone() {
                if (_droneStorageList.Count > 0) {
                    _droneStorageList[0].gameObject.layer = LayerMask.NameToLayer("ActiveHoldLayer");
                }
            }

            public void DeactivateDrone() {
                if (_droneStorageList.Count > 0) {
                    _droneStorageList[0].gameObject.layer = LayerMask.NameToLayer("InactiveHoldLayer");
                }
            }

        /* ----------| State Check Functions |---------- */

            public bool CanDropCurrentItem() {
                if (_droneStorageList.Count == 0) { return false; }

                ItemBehavior item = _droneStorageList[0];
                Vector3[] testPoints = new[]{
                    new Vector3( 0.5f,  0, 1),
                    new Vector3(-0.5f,  0, 1),
                    new Vector3( 0.5f, -1, 1),
                    new Vector3(-0.5f, -1, 1),
                };

                Vector3 raySrc = transform.position;
                Vector3 extents = item.LocalBounds().size;
                foreach (Vector3 test in testPoints) {
                    Vector3 rayDir = HoldTransform.position - transform.position + item.transform.rotation*Vector3.Scale(test, extents);
                    if(Physics.Raycast(raySrc, rayDir.normalized, rayDir.magnitude, _raycastMask)) {
                        return false;
                    }
                }

                return true;
            }

        /* ----------| Finalization Functions |---------- */

            void OnDestroy() {
                Signals.Get<DroneDestroyed>().Dispatch(this);
                Debug.Log("DroneController " + this + " destroyed");
            }

        /* ----------| Message Handling |---------- */

            void OnJointBreak(float breakForce) {
                UnityEngine.Debug.Log("A joint has just been broken!, dropping: " + _droneStorageList[0]);
                _droneStorageList[0].Drop();
                _droneStorageList.RemoveAt(0);
            }
    }
}
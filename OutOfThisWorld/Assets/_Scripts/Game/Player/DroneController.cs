using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using OutOfThisWorld.Player.HUD;
using OutOfThisWorld.Audio;
using deVoid.Utils;

namespace OutOfThisWorld.Player {
    public class DroneSpawned : ASignal<DroneController> {}
    public class DroneDestroyed : ASignal<DroneController> {}
    public class DronePositionUpdate : ASignal<DroneController> {}

    [RequireComponent(typeof(Rigidbody))]
    public class DroneController : MonoBehaviour {
        
        /* ----------| Serialized Variables |---------- */


            [Header("Camera")]
            public Transform CameraOffset;
            [Header("Movement")]
            public float MaxSpeed = 1f;
            public float Acceleration = 10f;
            public float MaxXAxisLook = 90f;
            public float DroneGravityMultiple = 1f;
            public float MaxHoverTime = 1f;
            public float HoverDistance = 1f;
            [Header("Item Pick Up")]
            public float InteractionRange = 3;
            public int MaxStorageSize = 1;
            public Transform HoldTransform;
            public LayerMask _raycastMask;
            public float HeldItemMassFactor = 1f;
            public float BreakHoldForce = 100f;
            public float ThrowForce = 1f;
            [Header("Death")]
            public List<GameObject> DronePeices;
            public float ExplosionForce = 1f;
            public float ItemSpawnVarience = 1f;

        /* ----------| Properties |--------- */

            public bool Follow { get; set; } = true;

            private bool _active;
            public bool Active { 
                get => _active; 
                set {
                    _active = value;

                    if (HasHeldItem()) {
                        if (_active) {
                            GetCurrentHeldItem().gameObject.layer = LayerMask.NameToLayer("ActiveHoldLayer");
                        } else {
                            GetCurrentHeldItem().gameObject.layer = LayerMask.NameToLayer("InactiveHoldLayer");
                        }
                    }
                }
            }

        /* ----------| Private Variables |---------- */

            private Vector3 _eulers = Vector3.zero;

            private Rigidbody _rigidbody;
            private PlayerController _playerController;
            private SingletonAudioManager _audioManager;

            private List<ItemBehavior> _droneStorageList;

            private float _hoverTime;
            

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

                // Fetch AudioManager
                _audioManager = SingletonAudioManager.Get();

                // Signal that the Drone has been spawned
                Signals.Get<DroneSpawned>().Dispatch(this);
            }

        /* ----------| Main Loop |----------- */

            void FixedUpdate() {
                if (HasHeldItem()) {
                    ItemBehavior item = GetCurrentHeldItem();
                    if (gameObject.GetComponent<FixedJoint>() == null) {
                        RenderInHand(item);
                    }
                }

                if (IsHovering()) {
                    _hoverTime += Time.fixedDeltaTime;
                    if (_hoverTime >= MaxHoverTime) {
                        _rigidbody.velocity += DroneGravityMultiple*Physics.gravity;
                        _rigidbody.velocity = Vector3.ClampMagnitude(_rigidbody.velocity, MaxSpeed);
                        _hoverTime = MaxHoverTime;
                    }
                } else if (_hoverTime > 0) {
                    _hoverTime -= Time.fixedDeltaTime;
                    _hoverTime = Mathf.Clamp(_hoverTime, 0, MaxHoverTime);
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
                    Signals.Get<DronePositionUpdate>().Dispatch(this);
                }
            }

            public void HandleMove(Vector3 move_dir, float delta) {
                if(move_dir != Vector3.zero)
                {
                    _rigidbody.velocity += Acceleration*delta*move_dir;
                    _rigidbody.velocity = Vector3.ClampMagnitude(_rigidbody.velocity, MaxSpeed);
                    Signals.Get<DronePositionUpdate>().Dispatch(this);
                }
            }

         /* ----------| Interaction Functions |---------- */

            public bool Interact() {
                // Calculate raycast parameters
                Vector3 raySrc = transform.position;
                Vector3 rayDir = transform.TransformDirection(Vector3.forward);

                // Cast ray and get hit object Components if there is a hit
                RaycastHit hit;
                if (Physics.Raycast(raySrc, rayDir, out hit, InteractionRange, _raycastMask)) {
                    Collider hitCol = hit.collider;
                    
                    // If Inventory not full
                    if (_droneStorageList.Count < MaxStorageSize) {
                        ItemBehavior hitItem = hitCol.gameObject.GetComponent<ItemBehavior>();
                        ItemSocket hitSocket = hitCol.gameObject.GetComponent<ItemSocket>();
                        if (hitSocket != null && hitSocket.HasItem()) {
                            UnsocketItem(hitSocket);
                            return true;
                        } else if (hitItem != null && !hitItem.IsHeld()) {
                            Pickup(hitItem);
                            return true;
                        }
                    }
                    
                    if (HasHeldItem()) { 
                        DepositBehavior hitDepot = hitCol.gameObject.GetComponent<DepositBehavior>();
                        ItemSocket hitSocket = hitCol.gameObject.GetComponent<ItemSocket>();
                        if (hitDepot != null) {
                            DepositHeld(hitDepot);
                            return true;
                        } else if (hitSocket != null && !hitSocket.HasItem()) {
                            SocketItem(hitSocket);
                            return true;
                        }
                    }

                    UseBehavior hitUse = hitCol.gameObject.GetComponent<UseBehavior>();
                    ToggleSwitch hitToggle = hitCol.gameObject.GetComponent<ToggleSwitch>();
                    if(hitUse != null) {
                        hitUse.Use();
                        return true;
                    } else if(hitToggle != null) {
                        hitToggle.Toggle();
                        return true;
                    }
                }

                return false;
            }

            public void Pickup(ItemBehavior item) {
                _droneStorageList.Add(item); // Add item to inventory
                item.gameObject.SetActive(false);
                
                //_audioManager.PlaySFX(_audioManager.item_pickup);

                _playerController._taskUIPanel.CompleteTask("Pick up an object\n(Left Click)");
            }

            public void UnsocketItem(ItemSocket socket) {
                ItemBehavior item = socket.GiveItem();
                _droneStorageList.Add(item);
                item.gameObject.SetActive(false);

                _audioManager.PlaySFX(_audioManager.item_pickup);
            }

            public void SocketItem(ItemSocket socket) {
                ItemBehavior item = GetCurrentHeldItem();
                DropHeld();
                socket.TakeItem(item);
            }

            public void DepositHeld(DepositBehavior depot) {
                depot.MakeDeposit(GetCurrentHeldItem());
                            
                GameObject desposited = GetCurrentHeldItem().gameObject;
                _droneStorageList.RemoveAt(0);
                Destroy(desposited);
                Destroy(gameObject.GetComponent<FixedJoint>());

                _playerController._taskUIPanel.CompleteTask("Deposit item in blue cabinets\n(Left Click anywhere on the cabinet)");
            }

            public bool DropHeld() {
                FixedJoint holdJoint = gameObject.GetComponent<FixedJoint>();
                if (HasHeldItem() && holdJoint != null && CanDropCurrentItem()) {
                    GetCurrentHeldItem().Drop();
                    GetCurrentHeldItem().GetComponent<Rigidbody>().AddForce(transform.rotation*(ThrowForce*Vector3.forward), ForceMode.Impulse);
                    _droneStorageList.RemoveAt(0);
                    Destroy(holdJoint);

                    _playerController._taskUIPanel.CompleteTask("Drop an object\n(Right Click)");
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

        /* ----------| Held Item Information Passing |---------- */

            public bool HasHeldItem() {
                return _droneStorageList.Count > 0;
            }

            public ItemBehavior GetCurrentHeldItem() {
                if (!HasHeldItem()) { return null; }
                return _droneStorageList[0];
            }

        /* ----------| State Check Functions |---------- */

            public bool CanDropCurrentItem() {
                if (!HasHeldItem()) { return false; }

                ItemBehavior item = GetCurrentHeldItem();
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

            public bool IsHovering() {
                Vector3 raySrc = transform.position;
                Vector3 rayDir = Physics.gravity.normalized;
                return !Physics.Raycast(raySrc, rayDir, HoverDistance, _raycastMask);
            }

            public float GetFuelPercentage() {
                return Mathf.Clamp(1 - _hoverTime/MaxHoverTime, 0, 1);
            }

        /* ----------| Finalization Functions |---------- */

            void OnDestroy() {
                Signals.Get<DroneDestroyed>().Dispatch(this);
                Debug.Log("DroneController " + this + " destroyed");
            }

            public void AttemptDestroy() {
                foreach(GameObject prefab in DronePeices) {
                    GameObject obj = Instantiate(prefab, transform.position + ItemSpawnVarience*Random.insideUnitSphere, transform.rotation);
                    Rigidbody body = obj.GetComponent<Rigidbody>();
                    if (body) {
                        body.AddExplosionForce(ExplosionForce, transform.position, ItemSpawnVarience, 0f, ForceMode.Impulse);
                    }
                }
                _audioManager.PlaySFX(_audioManager.drone_wreck);
                Destroy(gameObject);
            }


        /* ----------| Message Handling |---------- */

            void OnJointBreak(float breakForce) {
                UnityEngine.Debug.Log("A joint has just been broken!, dropping: " + GetCurrentHeldItem());
                GetCurrentHeldItem().Drop();
                _droneStorageList.RemoveAt(0);
            }
    }
}
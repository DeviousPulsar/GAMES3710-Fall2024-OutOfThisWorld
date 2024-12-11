using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using OutOfThisWorld;
using OutOfThisWorld.Player.HUD;
using deVoid.Utils;

namespace OutOfThisWorld.Player {
    public class DroneSwitched : ASignal<DroneController> {}

    public class PlayerController : MonoBehaviour {

        /* ----------| Serialized Variables |---------- */

            [Header("References")]
            public Transform _cameraTransform;
            //public TaskInfoPanel _taskUIPanel;
            public PlayerInputHandler _playerInputHandler;
            public GameFlowManager _gameFlowManager;

            [Header("Drone Information")]
            public GameObject InitalDronePrefab;
            public Transform InitalDroneLocation;
            public int DroneMax = 2;

            [Header("Drone Follow Settings")]
            public float PullDistMult = 1f;
            public float PullMaxDist = 10f;

        /* ----------| Private Variables |---------- */

            
            private List<DroneController> _drones;
            private int _activeDroneIndex = 0;


        /* ----------| Initalization Functions |---------- */

            void Start() {
                // Spawn inital drone
                _drones = new List<DroneController>();
                SpawnDrone(InitalDronePrefab, InitalDroneLocation.position, InitalDroneLocation.rotation);
                GetActiveDrone().Active = true;

                // Set up drone death handling
                Signals.Get<DroneDestroyed>().AddListener(RemoveDrone);
            }

        /* ----------| Main Update Loop |---------- */

            void Update()
            {
                if (_drones.Count < 1) {
                    _gameFlowManager.Lose();
                    Destroy(gameObject);
                } else {
                    if (Input.GetButtonDown(_playerInputHandler.DroneShiftAction)) { SwitchDrone(); }
                    if (Input.GetButtonDown(_playerInputHandler.DroneInteract)) { GetActiveDrone().Interact(); }
                    if (Input.GetButtonDown(_playerInputHandler.DroneDrop)) { GetActiveDrone().DropHeld(); }
                    if (Input.GetButtonDown(_playerInputHandler.DroneModeAction)) { 
                        GetActiveDrone().Follow = !GetActiveDrone().Follow; 
                    }
                }
            }

            void FixedUpdate()
            {
                for(int i = _drones.Count - 1; i >= 0; i--) {
                    DroneController drone = _drones[i];
                    if (drone is null) {
                        _drones.RemoveAt(i);
                    } else if (i == _activeDroneIndex) {
                        drone.HandleMove(_playerInputHandler.GetMoveForce(), _playerInputHandler.GetLookAngles(), Time.fixedDeltaTime);
                        _cameraTransform.transform.position = drone.CameraOffset.position;
                        _cameraTransform.transform.rotation = drone.CameraOffset.rotation;
                    }  else if (drone.Follow) {
                        DroneController activeDrone = GetActiveDrone();
                        Vector3 dist = activeDrone.transform.position - drone.transform.position;
                        if (dist.magnitude > PullMaxDist) {
                            drone.Follow = false;
                        } else if (dist.magnitude > PullDistMult*(i+_drones.Count-_activeDroneIndex)%_drones.Count) {
                            drone.HandleMove(dist.normalized, Time.fixedDeltaTime);
                        }
                        drone.transform.LookAt(activeDrone.transform);
                    }
                }
            }

        /* -----------| Drone Spawning and Manipulation |----------- */

            public GameObject SpawnDrone(GameObject DronePrefab, Vector3 position, Quaternion rotation) {
                if (_drones.Count < DroneMax) {
                    GameObject drone = Instantiate(DronePrefab, position, rotation, transform);
                    DroneController droneController = drone.GetComponent<DroneController>();
                    DebugUtility.HandleErrorIfNullGetComponent<DroneController, PlayerController>(droneController, this, drone);
                    if (droneController) {
                        _drones.Add(droneController);

                        return drone;
                    }
                }

                return null;
            }

            public void RemoveDrone(DroneController drone) {
                int indexToRemove = _drones.FindIndex((DroneController o) => o == drone);
                if (indexToRemove > -1) {
                    Debug.Log(this + " received request to remove DroneController " + drone + " from drone list. Removing drone at position " + indexToRemove);
                    _drones.RemoveAt(indexToRemove);
                    if (_activeDroneIndex > indexToRemove) {
                        _activeDroneIndex -= 1;
                    }
                    if (_drones.Count > 0 && _activeDroneIndex >= _drones.Count) {
                        _activeDroneIndex %= _drones.Count;
                    }
                } else {
                    Debug.Log(this + " received request to remove DroneController " + drone + " from drone list. No such drone in drone list");
                }
            }

            public void SwitchDrone() {
                Signals.Get<DroneSwitched>().Dispatch(GetActiveDrone());

                GetActiveDrone().Active = false;
                _activeDroneIndex += 1;
                if (_activeDroneIndex >= _drones.Count) { _activeDroneIndex = 0; }
                GetActiveDrone().Active = true;

                //_taskUIPanel.CompleteTask("Switch Drones (Tab)");
            }

            public DroneController GetActiveDrone() {
                return _drones[_activeDroneIndex];
            }

        /* ----------| Finalization Functions |---------- */

        void OnDestroy() {
            Signals.Get<DroneDestroyed>().RemoveListener(RemoveDrone);
        }
    }
}

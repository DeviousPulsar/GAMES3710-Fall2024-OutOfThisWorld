using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using OutOfThisWorld;
using OutOfThisWorld.Player.HUD;
using deVoid.Utils;

namespace OutOfThisWorld.Player {
    public class DroneSwitched : ASignal<DroneController> {}

    [RequireComponent(typeof(PlayerInputHandler))]
    public class PlayerController : MonoBehaviour {

        /* ----------| Serialized Variables |---------- */

            [Header("References")]
            public Transform _cameraTransform;
            public DroneInfoPanel _droneUIPanel;
            public TaskInfoPanel _taskUIPanel;

            [Header("Drone Information")]
            public GameObject InitalDronePrefab;
            public Transform InitalDroneLocation;
            public int DroneMax = 2;

        /* ----------| Private Variables |---------- */

            private PlayerInputHandler _playerInputHandler;
            private List<DroneController> _drones;
            private int _activeDroneIndex = 0;


        /* ----------| Initalization Functions |---------- */

            void Start()
            {
                // fetch components from GameObject
                _playerInputHandler = GetComponent<PlayerInputHandler>();
                DebugUtility.HandleErrorIfNullGetComponent<PlayerInputHandler, PlayerController>(_playerInputHandler, this, gameObject);

                // Spawn inital drone
                _drones = new List<DroneController>();
                SpawnDrone(InitalDronePrefab, InitalDroneLocation.position, InitalDroneLocation.rotation);

                // Set up drone death handling
                Signals.Get<DroneDestroyed>().AddListener(RemoveDrone);
            }

        /* ----------| Main Update Loop |---------- */

            void Update()
            {
                if (_drones.Count < 1) {
                    SceneManager.LoadSceneAsync(0);
                    Destroy(gameObject);
                } else {
                    if (Input.GetButtonDown(_playerInputHandler.DroneShiftAction)) { SwitchDrone(); }
                    if (Input.GetButtonDown(_playerInputHandler.DroneInteract)) { DroneInteract(); } // Added by JB
                    if (Input.GetButtonDown(_playerInputHandler.DroneDrop)) { GetActiveDrone().DropHeld(); }

                    _droneUIPanel.SetActiveInfoBar(GetActiveDrone());
                }
            }

            void FixedUpdate()
            {
                DroneController activeDrone = GetActiveDrone();

                activeDrone.HandleMove(_playerInputHandler.GetMoveForce(), _playerInputHandler.GetLookAngles(), Time.fixedDeltaTime);
                _cameraTransform.transform.position = activeDrone.CameraOffset.position;
                _cameraTransform.transform.rotation = activeDrone.CameraOffset.rotation;
            }

        /* -----------| Drone Spawning and Manipulation |----------- */

            public GameObject SpawnDrone(GameObject DronePrefab, Vector3 position, Quaternion rotation) {
                if (_drones.Count < DroneMax) {
                    GameObject drone = Instantiate(DronePrefab, position, rotation, transform);
                    DroneController droneController = drone.GetComponent<DroneController>();
                    DebugUtility.HandleErrorIfNullGetComponent<DroneController, PlayerController>(droneController, this, drone);
                    if (droneController) {
                        _drones.Add(droneController);
                        _droneUIPanel.AddInfoBar(droneController);

                        _taskUIPanel.CompleteTask("Create Second Drone (Left Click the ship to spend 5 RP)");

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
                } else {
                    Debug.Log(this + " received request to remove DroneController " + drone + " from drone list. No such drone in drone list");
                }
            }

            public void SwitchDrone() {
                Signals.Get<DroneSwitched>().Dispatch(GetActiveDrone());

                GetActiveDrone().DeactivateDrone();
                _activeDroneIndex += 1;
                if (_activeDroneIndex >= _drones.Count) { _activeDroneIndex = 0; }
                GetActiveDrone().ActivateDrone();

                _taskUIPanel.CompleteTask("Switch Drones (Tab)");
            }

            public DroneController GetActiveDrone() {
                return _drones[_activeDroneIndex];
            }

        /* ----------| I/O Functions |---------- */

            /// <summary>
            /// Try to interact with what you are looking at from the currently activated drone.
            /// </summary>
            /// <returns></returns> false is there is nothing to interact with.
            bool DroneInteract()
            {
                return GetActiveDrone().Interact();
            }
    }
}

using System.Collections.Generic;
using UnityEngine;
using OutOfThisWorld;
using OutOfThisWorld.Debug;
using OutOfThisWorld.Player.HUD;

namespace OutOfThisWorld.Player {
    [RequireComponent(typeof(PlayerInputHandler))]
    public class PlayerController : MonoBehaviour {

        /* ----------| Serialized Variables |---------- */

            [Header("References")]
            [SerializeField] ResourceSystem _resourceSystem;
            [SerializeField] Camera _mainCamera;
            [SerializeField] DroneInfoPanel _droneUIPanel;

            [Header("Drone Information")]
            public GameObject InitalDronePrefab;
            public Transform InitalDroneSpawn;
            public int DroneMax = 4;

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
                SpawnDrone(InitalDronePrefab, InitalDroneSpawn.position, InitalDroneSpawn.rotation);
            }

        /* ----------| Main Update Loops |---------- */

            void Update()
            {
                if (Input.GetButtonDown(_playerInputHandler.DroneShiftAction)) { _activeDroneIndex += 1; }
                if (_activeDroneIndex >= _drones.Count) { _activeDroneIndex = 0; }
                if (Input.GetButtonDown(_playerInputHandler.DroneInteract1)) { DroneInteract(); } // Added by JB
                if (Input.GetButtonDown(_playerInputHandler.DroneInteract2)) { _drones[_activeDroneIndex].DropHeld(); }

                _droneUIPanel.SetActiveInfoBar(_drones[_activeDroneIndex]);
            }

            void FixedUpdate()
            {
                if (_drones.Count < 1) 
                {
                    SpawnDrone(InitalDronePrefab, InitalDroneSpawn.position, InitalDroneSpawn.rotation);
                    _activeDroneIndex = 0;
                }

                _drones[_activeDroneIndex].HandleMove(_playerInputHandler.GetMoveForce(), _playerInputHandler.GetLookAngles(), Time.fixedDeltaTime);
                _mainCamera.transform.position = _drones[_activeDroneIndex].transform.position;
                _mainCamera.transform.rotation = _drones[_activeDroneIndex].transform.rotation;
            }

        /* -----------| Drone Spawning and Manipulation |----------- */

            public GameObject SpawnDrone(GameObject droneToAdd, Vector3 position, Quaternion rotation)
            {
                if (_drones.Count < DroneMax) {
                    GameObject drone = Instantiate(droneToAdd, position, rotation, transform);
                    DroneController droneController = drone.GetComponent<DroneController>();
                    DebugUtility.HandleErrorIfNullGetComponent<DroneController, PlayerController>(droneController, this, drone);
                    
                    _drones.Add(droneController);
                    _droneUIPanel.AddInfoBar(droneController);

                    return drone;
                }

                return null;
            }

            bool DroneInteract()
            {
                return _drones[_activeDroneIndex].Interact();
            }
    }
}

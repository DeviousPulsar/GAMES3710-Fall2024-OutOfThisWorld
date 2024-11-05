using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OutOfThisWorld;
using OutOfThisWorld.Debug;
using OutOfThisWorld.Player.HUD;

namespace OutOfThisWorld.Player
{
    [RequireComponent(typeof(PlayerInputHandler))]
    public class PlayerController : MonoBehaviour
    {

    /* ----------| Serialized Variables |---------- */

        [Header("References")]
        [SerializeField] ResourceSystem _resourceSystem;
        [SerializeField] Camera _mainCamera;
        [SerializeField] DroneInfoPanel _droneUIPanel;

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
        }

    /* ----------| Main Update Loop |---------- */

        void Update()
        {
            if (Input.GetButtonDown(_playerInputHandler.DroneShiftAction)) { 
                _activeDroneIndex += 1;
                _taskUIPanel.CompleteTask("Switch Drones (Tab)");
            }
            if (_activeDroneIndex >= _drones.Count) { _activeDroneIndex = 0; }
            if (Input.GetButtonDown(_playerInputHandler.DroneInteract)) { DroneInteract(); } // Added by JB
            if (Input.GetButtonDown(_playerInputHandler.DroneDrop)) { _drones[_activeDroneIndex].DropHeld(); }

            _droneUIPanel.SetActiveInfoBar(_drones[_activeDroneIndex]);
        }

        void FixedUpdate()
        {
            /*if (_drones.Count < 1) 
            {
                SpawnDrone();
                _activeDroneIndex = 0;
            }*/

            DroneController activeDrone = _drones[_activeDroneIndex];

            activeDrone.HandleMove(_playerInputHandler.GetMoveForce(), _playerInputHandler.GetLookAngles(), Time.fixedDeltaTime);
            _mainCamera.transform.position = activeDrone.CameraOffset.position;
            _mainCamera.transform.rotation = activeDrone.CameraOffset.rotation;
        }

    /* -----------| Drone Spawning and Manipulation |----------- */

        public GameObject SpawnDrone(GameObject DronePrefab, Vector3 position, Quaternion rotation)
        {
            if (_drones.Count < DroneMax) {
                GameObject drone = Instantiate(DronePrefab, position, rotation, transform);
                DroneController droneController = drone.GetComponent<DroneController>();
                DebugUtility.HandleErrorIfNullGetComponent<DroneController, PlayerController>(droneController, this, drone);
                
                _drones.Add(droneController);
                _droneUIPanel.AddInfoBar(droneController);

                _taskUIPanel.CompleteTask("Create Second Drone (Left Click the ship to spend 5 RP)");
                
                return drone;
            }

            return null;
        }

        /// <summary>
        /// Try to interact with what you are looking at from the currently activated drone.
        /// </summary>
        /// <returns></returns> false is there is nothing to interact with.
        bool DroneInteract()
        {
            return _drones[_activeDroneIndex].Interact();
        }



    }
}

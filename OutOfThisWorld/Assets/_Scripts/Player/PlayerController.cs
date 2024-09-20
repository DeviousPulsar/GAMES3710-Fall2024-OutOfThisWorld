using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OutOfThisWorld;
using OutOfThisWorld.Debug;
using UnityEngine.UI;

namespace OutOfThisWorld.Player
{
    [RequireComponent(typeof(PlayerInputHandler)), RequireComponent(typeof(ResourceSystem))]
    public class PlayerController : MonoBehaviour
    {

    /* ----------| Component Properties |---------- */

        public GameObject DronePrefab;
        public float DroneSpawnCost = 5f;

    /* ----------| Instance Variables |---------- */

        private PlayerInputHandler _playerInputHandler;
        private ResourceSystem _resourceSystem;
        private Camera _mainCamera;
        private List<DroneController> _drones;
        private SpawnArea[] _spawnLocations;
        private int _activeDroneIndex = 0;


        /* ----------| Initalization Functions |---------- */

        void Start()
        {
            // fetch components from GameObject
            _playerInputHandler = GetComponent<PlayerInputHandler>();
            DebugUtility.HandleErrorIfNullGetComponent<PlayerInputHandler, PlayerController>(_playerInputHandler, this, gameObject);
            _resourceSystem = GetComponent<ResourceSystem>();
            DebugUtility.HandleErrorIfNullGetComponent<ResourceSystem, PlayerController>(_resourceSystem, this, gameObject);


            
            // fetch components from child nodes
            _mainCamera =  GetComponentInChildren<Camera>();
            DebugUtility.HandleWarningIfNoComponentsFoundAmongChildren<Camera, PlayerController>(_mainCamera != null ? 1 : 0, this);
            _spawnLocations = GetComponentsInChildren<SpawnArea>();
            DebugUtility.HandleWarningIfNoComponentsFoundAmongChildren<SpawnArea, PlayerController>(_spawnLocations.Length, this);

            // Spawn inital drone
            _drones = new List<DroneController>();
            SpawnDrone();
        }

    /* ----------| Main Update Loop |---------- */

        void Update()
        {
            if (_drones.Count < 1) 
            {
                SpawnDrone();
                _activeDroneIndex = 0;
            }
            
            if (Input.GetButtonDown(_playerInputHandler.SpawnNewDroneAction)) { SpawnDrone(); }
            if (Input.GetButtonDown(_playerInputHandler.DroneShiftAction)) 
            {
                _activeDroneIndex += 1;
                GameObject.Find("CurrentDroneID").GetComponent<Text>().text = "Drone ID: " + _activeDroneIndex;
            }
            if (_activeDroneIndex >= _drones.Count) { _activeDroneIndex = 0; }


            // Added by JB
            if (Input.GetButtonDown(_playerInputHandler.DroneInteraction))
            {
                DroneInteract();
            }
        }

        void FixedUpdate()
        {
            _drones[_activeDroneIndex].HandleMove(_playerInputHandler.GetMoveForce(), _playerInputHandler.GetLookAngles(), Time.fixedDeltaTime);
            _mainCamera.transform.position = _drones[_activeDroneIndex].transform.position;
            _mainCamera.transform.rotation = _drones[_activeDroneIndex].transform.rotation;
        }

    /* -----------| Drone Spawning and Manipulation |----------- */

        bool SpawnDrone()
        {
            foreach (SpawnArea location in _spawnLocations)
            {
                if (!location.IsOccupied() && _resourceSystem.SpendResources(DroneSpawnCost)) {
                    GameObject drone = Instantiate(DronePrefab, location.GetRandomizedSpawnLocation(), location.GetRandomizedSpawnAngle(), transform);
                    DroneController droneController = drone.GetComponent<DroneController>();
                    DebugUtility.HandleErrorIfNullGetComponent<DroneController, PlayerController>(droneController, this, drone);
                    _drones.Add(droneController);
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Try to interact with what you are looking at from the currently activated drone.
        /// </summary>
        /// <returns></returns> false is there is nothing to interact with.
        bool DroneInteract()
        {
            if (_drones[_activeDroneIndex].IsOccupied() )
            {
                return _drones[_activeDroneIndex].interactWithOccupied();
            }

            return false;
        }

    }
}

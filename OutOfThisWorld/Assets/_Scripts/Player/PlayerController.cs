using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OutOfThisWorld;
using OutOfThisWorld.Debug;

namespace OutOfThisWorld.Player
{
    [RequireComponent(typeof(PlayerInputHandler))]
    public class PlayerController : MonoBehaviour
    {

    /* ----------| Component Properties |---------- */

    public GameObject DronePrefab;

    /* ----------| Instance Variables |---------- */

    private PlayerInputHandler _playerInputHandler;
    private List<DroneController> _drones;
    private SpawnArea[] _spawnLocations;
    private int _activeDroneIndex = 0;
    
    /* ----------| Initalization Functions |---------- */

        void Start()
        {
            // fetch PlayerInputHandler
            _playerInputHandler = GetComponent<PlayerInputHandler>();
            DebugUtility.HandleErrorIfNullGetComponent<PlayerInputHandler, PlayerController>(_playerInputHandler, this, gameObject);

            // fetch spawn locations from child nodes
            _spawnLocations = this.GetComponentsInChildren<SpawnArea>();
            DebugUtility.HandleWarningIfNoComponentsFoundAmongChildren<SpawnArea, PlayerController>(_spawnLocations.Length, this);

            // Spawn inital drone
            _drones = new List<DroneController>();
            SpawnDrone();
        }

    /* ----------| Main Update Loop |---------- */

        void FixedUpdate()
        {
            if (_drones.Count < 1) 
            {
                SpawnDrone();
                _activeDroneIndex = 0;
            }
            else if (_activeDroneIndex >= _drones.Count)
            {
                _activeDroneIndex = 0;
            }

            _drones[_activeDroneIndex].HandleMove(_playerInputHandler.GetMoveForce(), _playerInputHandler.GetLookAngles(), Time.fixedDeltaTime);
        }

    /* -----------| Drone Spawning and Manipulation |----------- */

        bool SpawnDrone()
        {
            foreach (SpawnArea location in _spawnLocations)
            {
                if (!location.IsOccupied()) {
                    GameObject drone = Instantiate(DronePrefab, location.GetRandomizedSpawnLocation(), location.GetRandomizedSpawnAngle(), transform);
                    DroneController droneController = drone.GetComponent<DroneController>();
                    DebugUtility.HandleErrorIfNullGetComponent<DroneController, PlayerController>(droneController, this, drone);
                    _drones.Add(droneController);
                    return true;
                }
            }

            return false;
        }

    }
}

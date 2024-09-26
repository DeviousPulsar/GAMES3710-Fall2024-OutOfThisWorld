using System.Collections.Generic;
using UnityEngine;
using OutOfThisWorld;
using OutOfThisWorld.Debug;
using OutOfThisWorld.Player.HUD;
using deVoid.Utils;

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
        public Spawner DroneSpawner;
        public int DroneMax = 4;

    /* ----------| Private Variables |---------- */

        private PlayerInputHandler _playerInputHandler;
        private List<DroneController> _drones;
        private int _activeDroneIndex = 0;


    /* ----------| Initalization Functions |---------- */

        void Awake()
        {
            // fetch components from GameObject
            _playerInputHandler = GetComponent<PlayerInputHandler>();
            DebugUtility.HandleErrorIfNullGetComponent<PlayerInputHandler, PlayerController>(_playerInputHandler, this, gameObject);

            // Spawn inital drone
            _drones = new List<DroneController>();
            Signals.Get<DroneSpawned>().AddListener(AddDroneToList);
            //Instantiate(DronePrefab, position, rotation, transform);
        }

    /* ----------| Main Update Loop |---------- */

        void Update()
        {         
            //if (Input.GetButtonDown(_playerInputHandler.SpawnNewDroneAction)) { SpawnDrone(); }
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
                DroneSpawner.Spawn();
                _activeDroneIndex = 0;
            }

            _drones[_activeDroneIndex].HandleMove(_playerInputHandler.GetMoveForce(), _playerInputHandler.GetLookAngles(), Time.fixedDeltaTime);
            _mainCamera.transform.position = _drones[_activeDroneIndex].transform.position;
            _mainCamera.transform.rotation = _drones[_activeDroneIndex].transform.rotation;
        }

    /* -----------| Drone Spawning and Manipulation |----------- */

        public void AddDroneToList(DroneController drone)
        {
            if(_drones.Count < DroneMax) {
                _drones.Add(drone);
                _droneUIPanel.AddInfoBar(drone);
            }

            //Destroy(drone);
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

using System.Collections.Generic;
using UnityEngine;
using OutOfThisWorld.Debug;
using OutOfThisWorld.Player;

namespace OutOfThisWorld {
    public class DroneSpawner : AbstractSpawner {

        /* ----------| Serialized Variables |---------- */

            public GameObject DronePrefab;
            [SerializeField] PlayerController _playerControler;
            [SerializeField] ResourceSystem _resourceSystem;
            public float Cost = 1f;

        /* ----------| Spawning Functions |---------- */

            protected override GameObject AbsSpawn()
            {
                DroneController drone = DronePrefab.GetComponent<DroneController>();
                DebugUtility.HandleErrorIfNullGetComponent<DroneController, GameObject>(drone, this, DronePrefab);

                UnityEngine.Debug.Log(""+  DronePrefab + drone );
                
                if (drone == null) { return null; }
                
                foreach (SpawnArea area in _spawnLocations)
                {
                    if (!area.IsOccupied() && _resourceSystem.SpendResources(Cost))
                    {
                        return _playerControler.SpawnDrone(DronePrefab, area.GetRandomizedSpawnLocation(), area.GetRandomizedSpawnAngle());
                    }
                }

                return null;
            }
    }
}

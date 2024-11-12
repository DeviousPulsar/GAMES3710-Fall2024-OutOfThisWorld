using System.Collections.Generic;
using UnityEngine;
using OutOfThisWorld.Player;

namespace OutOfThisWorld {
    public class DroneSpawner : AbstractSpawner {

        /* ----------| Serialized Variables |---------- */

            public GameObject DronePrefab;
            [SerializeField] PlayerController _playerControler;

        /* ----------| Spawning Functions |---------- */

            protected override GameObject AbsSpawn()
            {
                DroneController drone = DronePrefab.GetComponent<DroneController>();
                DebugUtility.HandleErrorIfNullGetComponent<DroneController, GameObject>(drone, this, DronePrefab);
                
                if (drone == null) { return null; }
                
                foreach (SpawnArea area in _spawnLocations)
                {
                    if (!area.IsOccupied())
                    {
                        return _playerControler.SpawnDrone(DronePrefab, area.GetSpawnLocation(), area.GetSpawnAngle());
                    }
                }

                return null;
            }
    }
}

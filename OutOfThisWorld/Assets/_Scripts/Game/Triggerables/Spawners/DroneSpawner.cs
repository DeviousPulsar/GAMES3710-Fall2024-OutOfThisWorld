using System.Collections.Generic;
using UnityEngine;
using OutOfThisWorld.Player;

namespace OutOfThisWorld {
    public class DroneSpawner : AbstractSpawner {

        /* ----------| Serialized Variables |---------- */

            [Header("Drone Spawn Information")]
            public GameObject DronePrefab;
            [SerializeField] PlayerController PlayerControler;
            public float SpawnForce;
            [Header("Payment Information")]
            [SerializeField] ResourceSystem ResourceSystem;
            public float UseCost = 1f;

        /* ----------| Spawning Functions |---------- */

            protected override GameObject AbsSpawn()
            {              
                foreach (SpawnArea area in _spawnLocations)
                {
                    if (!area.IsOccupied() && ResourceSystem.SpendResources(UseCost))
                    {
                        Quaternion spawnAngle = area.GetSpawnAngle();
                        GameObject obj = PlayerControler.SpawnDrone(DronePrefab, area.GetSpawnLocation(), area.GetSpawnAngle());
                        Rigidbody body = obj.GetComponent<Rigidbody>();
                        if (body) {
                            body.AddForce(SpawnForce*(spawnAngle*Vector3.forward), ForceMode.Impulse);
                        }
                        return obj;
                    }
                }

                return null;
            }
    }
}

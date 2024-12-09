using System.Collections.Generic;
using UnityEngine;
using OutOfThisWorld.Player;

namespace OutOfThisWorld {
    public class RandomizedSpawner : AbstractSpawner {

        /* ----------| Serialized Variables |---------- */

            [Header("Item Spawn Information")]
            [SerializeField] List<GameObject> _spawnables;
            public float SpawnForce;
            [Header("Payment Information")]
            [SerializeField] ResourceSystem ResourceSystem;
            public float UseCost = 1f;

        /* ----------| Spawning Functions |---------- */

            protected override GameObject AbsSpawn()
            {
                GameObject objToSpawn = _spawnables[(int) Random.Range(0, _spawnables.Count - 0.001f)];
                
                foreach (SpawnArea area in _spawnLocations)
                {
                    if (!area.IsOccupied() && ResourceSystem.SpendResources(UseCost))
                    {
                        Quaternion spawnAngle = area.GetSpawnAngle();
                        GameObject obj = Instantiate(objToSpawn, area.GetSpawnLocation(), spawnAngle);
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
using System.Collections.Generic;
using UnityEngine;
using OutOfThisWorld.Player;

namespace OutOfThisWorld {
    public class RandomizedSpawner : AbstractSpawner {

        /* ----------| Serialized Variables |---------- */

            [SerializeField] List<GameObject> _spawnables;

        /* ----------| Spawning Functions |---------- */

            protected override GameObject AbsSpawn()
            {
                GameObject objToSpawn = _spawnables[(int) Random.Range(0, _spawnables.Count - 0.001f)];
                
                foreach (SpawnArea area in _spawnLocations)
                {
                    if (!area.IsOccupied())
                    {
                        return Instantiate(objToSpawn, area.GetSpawnLocation(), area.GetSpawnAngle());
                    }
                }

                return null;
            }
    }
}
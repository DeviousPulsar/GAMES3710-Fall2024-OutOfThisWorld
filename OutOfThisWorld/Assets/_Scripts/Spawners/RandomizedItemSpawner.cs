using System.Collections.Generic;
using UnityEngine;
using OutOfThisWorld.Debug;
using OutOfThisWorld.Player;

namespace OutOfThisWorld {
    public class RandomizedItemSpawner : AbstractSpawner {

        /* ----------| Serialized Variables |---------- */

            [SerializeField] List<GameObject> _spawnables;
            [SerializeField] ResourceSystem _resourceSystem;
            public float Cost = 1f;

        /* ----------| Spawning Functions |---------- */

            public override GameObject Spawn()
            {
                GameObject objToSpawn = _spawnables[(int) Random.Range(0, _spawnables.Count - 0.001f)];
                ItemBehavior itemInfo = objToSpawn.GetComponent<ItemBehavior>();
                DebugUtility.HandleErrorIfNullGetComponent<ItemBehavior, GameObject>(itemInfo, this, objToSpawn);

                UnityEngine.Debug.Log(""+  objToSpawn + itemInfo );
                
                if (itemInfo == null) { return null; }
                
                foreach (SpawnArea area in _spawnLocations)
                {
                    if (!area.IsOccupied() && (Cost <= 0 || _resourceSystem.SpendResources(Cost)))
                    {
                        return Instantiate(objToSpawn, area.GetRandomizedSpawnLocation(), area.GetRandomizedSpawnAngle());
                    }
                }

                return null;
            }
    }
}
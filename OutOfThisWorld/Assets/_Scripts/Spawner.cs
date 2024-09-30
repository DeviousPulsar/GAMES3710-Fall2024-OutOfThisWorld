using System.Collections.Generic;
using UnityEngine;
using OutOfThisWorld.Debug;

namespace OutOfThisWorld {
    public class Spawner : MonoBehaviour
    {
        [SerializeField] List<GameObject> _spawnables;
        [SerializeField] ResourceSystem _resourceSystem;
        public float CostMultiplier = 1f;
        public float SpawnForce = 1f;

        /* ----------| Private Variables |---------- */

        private SpawnArea[] _spawnLocations;

        /* ----------| Initalization Functions |---------- */

        void Awake()
        {
            // fetch components from child GameObject
            _spawnLocations = GetComponentsInChildren<SpawnArea>();
            DebugUtility.HandleWarningIfNoComponentsFoundAmongChildren<SpawnArea, Spawner>(_spawnLocations.Length, this);
        }

        /* ----------| Spawner Functions |---------- */

        public GameObject Spawn()
        {
            GameObject objToSpawn = _spawnables[(int) Random.Range(0, _spawnables.Count - 0.001f)];
            ItemBehavior itemInfo = objToSpawn.GetComponent<ItemBehavior>();
            DebugUtility.HandleErrorIfNullGetComponent<ItemBehavior, GameObject>(itemInfo, this, objToSpawn);
            UnityEngine.Debug.Log("" + objToSpawn + itemInfo);
            
            if (itemInfo == null) { return null; }
            
            foreach (SpawnArea area in _spawnLocations)
            {
                UnityEngine.Debug.Log(!area.IsOccupied() + " "+ _resourceSystem.SpendResources(CostMultiplier*itemInfo.ItemCost));
                if (!area.IsOccupied() && _resourceSystem.SpendResources(CostMultiplier*itemInfo.ItemCost))
                {
                    return Instantiate(objToSpawn, area.GetRandomizedSpawnLocation(), area.GetRandomizedSpawnAngle());
                }
            }

            return null;
        }
    }
}

using System.Collections.Generic;
using UnityEngine;
using OutOfThisWorld.Debug;
using OutOfThisWorld.Player;

namespace OutOfThisWorld {
    public class AbstractSpawner : MonoBehaviour {

        /* ----------| Private Variables |---------- */

            protected SpawnArea[] _spawnLocations;

        /* ----------| Initalization Functions |---------- */

            void Awake()
            {
                // fetch components from child GameObject
                _spawnLocations = GetComponentsInChildren<SpawnArea>();
                DebugUtility.HandleWarningIfNoComponentsFoundAmongChildren<SpawnArea, AbstractSpawner>(_spawnLocations.Length, this);
            }
        
        /* ----------| Abstract Methods |---------- */

        public virtual GameObject Spawn() {
            UnityEngine.Debug.Log("AbstractSpawner's Spawn() method is nonfunctional and should not be used!");

            return null;
        }
    }
}
using System.Collections.Generic;
using UnityEngine;
using OutOfThisWorld.Player;

namespace OutOfThisWorld {
    public class AbstractSpawner : Triggerable {

        /* ----------| Private Variables |---------- */

            protected SpawnArea[] _spawnLocations;

        /* ----------| Initalization Functions |---------- */

            void Awake()
            {
                // fetch components from child GameObject
                _spawnLocations = GetComponentsInChildren<SpawnArea>();
                DebugUtility.HandleWarningIfNoComponentsFoundAmongChildren<SpawnArea, AbstractSpawner>(_spawnLocations.Length, this);
            }
        
        /* -----------| Spawn & Trigger Functions |----------- */

            public GameObject Spawn() {
                GameObject spawned = AbsSpawn();

                return spawned; 
            }

            public override void Trigger() {
                Spawn();
            }

        /* ----------| Abstract Methods |---------- */

            protected virtual GameObject AbsSpawn() {
                UnityEngine.Debug.Log("AbstractSpawner's Spawn() method is nonfunctional and should not be used!");

                return null;
            }
    }
}
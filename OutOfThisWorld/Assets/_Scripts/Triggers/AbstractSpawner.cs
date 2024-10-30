using System.Collections.Generic;
using UnityEngine;
using OutOfThisWorld.Player;

namespace OutOfThisWorld {
    public class AbstractSpawner : Triggerable {

        /* ----------| Serialized Variables |---------- */

            [SerializeField] List<UseCountListener> Listeners;

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

                if (spawned is not null) {
                    foreach(UseCountListener l in Listeners) {
                        l.Count();
                    }
                }

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

    [RequireComponent(typeof(BoxCollider), typeof(Rigidbody))]
    public class SpawnArea : MonoBehaviour {

        /* ----------| Serialized Variables |---------- */

            public float Margin = 0.5f;
            public float MaxAngleVariation = 0f;

        /* ----------| Instance Variables |---------- */

            private BoxCollider _spawnArea;
            private ISet<Collider> _occupingBodies ;
        
        /* ----------| Initalization Functions |---------- */

            void Start()
            {
                // Initialize collider set
                _occupingBodies = new HashSet<Collider>();

                // fetch components on the same gameObject
                _spawnArea = GetComponent<BoxCollider>();
                DebugUtility.HandleErrorIfNullGetComponent<BoxCollider, SpawnArea>(_spawnArea, this, gameObject);
            }

        /* ----------| Message Processing |---------- */

            private void OnTriggerEnter(Collider other)
            {
                _occupingBodies.Add(other);
            }

            private void OnTriggerExit(Collider other)
            {
                _occupingBodies.Remove(other);
            }

        /* ----------| Functions |---------- */

            public bool IsOccupied()
            {
                return _occupingBodies.Count > 0;
            }

            public Vector3 GetRandomizedSpawnLocation ()
            {
                Vector3 min = _spawnArea.center - 0.5f*_spawnArea.size + Margin*Vector3.one;
                Vector3 max = _spawnArea.center + 0.5f*_spawnArea.size - Margin*Vector3.one;
                Vector3 result = new Vector3(Random.Range(min.x, max.x), Random.Range(min.y, max.y), Random.Range(min.z, max.z));

                return transform.position+result;
            }

            public Quaternion GetRandomizedSpawnAngle()
            {
                Vector3 eulers = new Vector3(Random.Range(-1,1), Random.Range(-1,1), 0);
                eulers = Random.Range(0,MaxAngleVariation)*Vector3.ClampMagnitude(eulers, 1);
                return transform.rotation*Quaternion.Euler(eulers.x, eulers.y, eulers.z);
            }
        
    }
}
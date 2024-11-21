using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using OutOfThisWorld.Player;
using deVoid.Utils;

namespace OutOfThisWorld.Monster {
    [RequireComponent(typeof(NavMeshAgent))]
    public class MonsterAI : MonoBehaviour {
        /* ----------| Serialized Variables |---------- */

            public float DespawnTimeout = 1000f;
            public float InaccessableRelogDistance = 1f;
            public float MinRelogDistance = 0.1f;
            public float WanderRange = 1f;
            public float WanderTimeout = 1f;
            public float EatTimeout = 1f;
            public float InaccessableTimeout = 1f;
            public float StoppedSpeed = 0.01f;

        /* ----------| Instance Variables |---------- */

            private NavMeshAgent _navAgent;
            private Dictionary<MonsterTarget, MonsterMemUnit> _memory;
            private float _spawnTimestamp;
            private float _wanderTimestamp;
            private float _eatTimestamp;
            private float _timeStopped;

        /* ----------| Initialization Functions |---------- */

            void Awake()
            {
                _navAgent = GetComponent<NavMeshAgent>();
                DebugUtility.HandleErrorIfNullGetComponent<MonsterAI, NavMeshAgent>(_navAgent, this, gameObject);

                _memory = new Dictionary<MonsterTarget, MonsterMemUnit>();
                _spawnTimestamp = Time.fixedTime;
            }

        /* ----------| Main Loop |---------- */

            void FixedUpdate() {
                if (_eatTimestamp + EatTimeout < Time.fixedTime) {
                    UpdatePath();
                }

                if (IsStopped()) {
                    _timeStopped += Time.fixedDeltaTime;
                } else {
                    _timeStopped = 0;
                }

                if (_timeStopped > InaccessableTimeout) {
                    if (GetClosestAccessibleTarget()) {
                        _memory[GetClosestAccessibleTarget()].accessible = false;
                    }
                    _timeStopped = 0;
                }
            }

            void UpdatePath() {
                MonsterTarget closest_target = GetClosestAccessibleTarget();
                if (closest_target) {
                    _navAgent.destination = _memory[closest_target].position;
                } else if (_wanderTimestamp + WanderTimeout < Time.fixedTime) {
                    _navAgent.destination += WanderRange*Random.onUnitSphere;
                }
            }

            bool IsStopped() {
                return _navAgent.velocity.magnitude < StoppedSpeed;
            }

            MonsterTarget GetClosestAccessibleTarget() {
                float min_dist = float.PositiveInfinity;
                MonsterTarget closest_target = null;
                foreach (MonsterTarget target in _memory.Keys) {
                    MonsterMemUnit mem = _memory[target];
                    if (mem.accessible) {
                        float heu_dist = (mem.position - transform.position).magnitude/target.Value;
                        if (heu_dist < min_dist) {
                            closest_target = target;
                            min_dist = heu_dist;
                        }
                    }
                }

                return closest_target;
            }

        /* ----------| Collision Handling |---------- */

            void OnCollisionEnter(Collision coll) {
                MonsterTarget target = coll.gameObject.GetComponent<MonsterTarget>();
                if (target && _eatTimestamp + EatTimeout < Time.fixedTime) {
                    _memory.Remove(target);
                    Destroy(coll.gameObject);
                    _eatTimestamp = Time.fixedTime;
                }
            }

            public void Spot(MonsterTarget target) {
                if (!target) { return; }

                if (_memory.ContainsKey(target)) {
                    MonsterMemUnit mem = _memory[target];
                    float dist = (mem.position - target.transform.position).magnitude;
                    if ((mem.accessible && dist > MinRelogDistance) || dist > InaccessableRelogDistance) {
                        _memory[target] = new MonsterMemUnit(target.transform.position, Time.fixedTime);
                    }
                } else {
                    _memory[target] = new MonsterMemUnit(target.transform.position, Time.fixedTime);
                }
            }
     }

    class MonsterMemUnit {
        public readonly Vector3 position;
        public readonly float last_changed;
        public bool accessible;

        public MonsterMemUnit(Vector3 pos, float timestamp) {
            position = pos;
            last_changed = timestamp;
            accessible = true;
        }
    }
}

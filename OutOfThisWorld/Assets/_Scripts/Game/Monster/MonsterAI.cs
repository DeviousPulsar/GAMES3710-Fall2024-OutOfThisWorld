using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using OutOfThisWorld.Player;
using deVoid.Utils;

namespace OutOfThisWorld.Monster {
    public enum MonsterState {
        Disabled,
        Wander,
        Persue,
        Rage
    }

    public class MonsterAI : MonoBehaviour {
        /* ----------| Serialized Variables |---------- */

            [Header("References")]
            public NavMeshAgent NavAgent;
            public List<MonsterDetectionArea> DetectionAreas;
            public List<Transform> WanderDestinations;

            [Header("Behavior Timeouts")]
            public float DespawnTimeout = 1000f;
            public float EatTimeout = 1f;
            public float StoppedTimeout = 1f;
            //public float WanderTimeout = 1f;
            //public float RageTimeout = 30f;

            [Header("Behavior Thresholds")]
            public float StoppedSpeed = 0.01f;
            public float RelogDist = 0.1f;

        /* ----------| Properties |---------- */

            public MonsterState CurrentState { get; set; }

        /* ----------| Instance Variables |---------- */

            private Dictionary<DroneController, bool> _memory;
            private Transform _wanderDest;
            private DroneController _target;

            private float _spawnTimestamp;
            private float _eatTimestamp;
            private float _timeStopped;
            private float _rageTimestamp;

            private float _debugPrintTimestamp;
            private MonsterState _lastState;
            private Vector3 _lastDest;

        /* ----------| Initialization Functions |---------- */

            void Awake() {
                _spawnTimestamp = Time.fixedTime;
                _memory = new Dictionary<DroneController, bool>();

                UpdateWanderDest();
                CurrentState = MonsterState.Disabled;

                // Signal Registration
                Signals.Get<DronePositionUpdate>().AddListener(UpdateMemory);
                Signals.Get<DroneSpawned>().AddListener(AddDroneToMemory);
                Signals.Get<DroneDestroyed>().AddListener(RemoveDroneFromMemory);
                Signals.Get<DroneEnteredDetectionArea>().AddListener(SetTarget);
            }

        /* ----------| Main Loop |---------- */

            void FixedUpdate() {
                if (CurrentState != MonsterState.Disabled && _eatTimestamp + EatTimeout < Time.fixedTime) {
                    UpdatePath();

                    if (IsStopped()) {
                        _timeStopped += Time.fixedDeltaTime;
                    } else {
                        _timeStopped = 0;
                    }

                    if (_timeStopped > StoppedTimeout) {
                        if (CurrentState == MonsterState.Persue || CurrentState == MonsterState.Rage) {
                            _memory[_target] = false;
                            Debug.Log("Alien memory updates noting that " + _target + " cannot be chased.");
                        }
                        if (CurrentState != MonsterState.Rage) {
                            UpdateWanderDest();
                        }
                    }
                }

                if (_lastState != CurrentState || (_lastDest - NavAgent.destination).magnitude > 1) {
                    Debug.Log("Alien in moving into state " + CurrentState + " w/ destination " + NavAgent.destination.ToString() + "." );
                }
                _lastState = CurrentState;
                _lastDest = NavAgent.destination;
            }

            void UpdatePath() {
                switch (CurrentState) {
                    case MonsterState.Wander:
                        NavAgent.destination = _wanderDest.position;
                        break;

                    case MonsterState.Rage:
                        if (_memory.Keys.Count == 0) {
                            UpdateWanderDest();
                            break;
                        }

                        float minDist = float.PositiveInfinity;
                        foreach (DroneController drone in _memory.Keys) {
                            float dist = (transform.position - drone.transform.position).magnitude;
                            if (dist < minDist) {
                                SetTarget(drone);
                            }
                        }

                        SetDestinationToTargetPos();
                        break;

                    case MonsterState.Persue:
                        SetDestinationToTargetPos();
                        break;
                }
            }

            void SetDestinationToTargetPos() {
                Vector3 currentDest = NavAgent.destination;
                Vector3 targetDest = _target.transform.position;
                if (Mathf.Pow(currentDest.x - targetDest.x, 2) + Mathf.Pow(currentDest.y - targetDest.y, 2) 
                        > Mathf.Pow(RelogDist, 2)) {
                    NavAgent.destination = _target.transform.position;
                }
            }

        /* ----------| State Accessors |---------- */

            bool IsStopped() {
                return NavAgent.velocity.magnitude < StoppedSpeed;
            }

        /* ----------| Memory and Target Update Functions |---------- */

            void UpdateWanderDest() {
                _wanderDest = WanderDestinations[Random.Range(0, WanderDestinations.Count)];
                _target = null;
                CurrentState = MonsterState.Wander;

            }

            void UpdateMemory(DroneController drone) {
                if (!_memory[drone]) {
                    _memory[drone] = true;
                    Debug.Log("Alien memory updates so " + drone + " can be chased.");
                }

                foreach (MonsterDetectionArea detect in DetectionAreas) {
                    if (detect.InDetectionArea(drone)) {
                        SetTarget(drone);
                        return;
                    }
                }
            }

            void SetTarget(DroneController drone) {
                if (_memory[drone] && _target != drone) {
                    _target = drone;
                    if (CurrentState != MonsterState.Rage) {
                        CurrentState = MonsterState.Persue;
                    }
                }
            }

            void AddDroneToMemory(DroneController drone) {
                _memory[drone] = true;
            }

            void RemoveDroneFromMemory(DroneController drone) {
                _memory.Remove(drone);
            }

        /* ----------| Collision Handling |---------- */

            void OnCollisionEnter(Collision coll) {
                DroneController target = coll.gameObject.GetComponent<DroneController>();
                if (target && _eatTimestamp + EatTimeout < Time.fixedTime) {
                    target.AttemptDestroy();
                    _eatTimestamp = Time.fixedTime;

                    if(CurrentState != MonsterState.Rage) {
                        UpdateWanderDest();
                    }
                }
            }
        
        /* ----------| Finalization Functions |---------- */
        
            void OnDestory() {
                Signals.Get<DronePositionUpdate>().RemoveListener(UpdateMemory);
                Signals.Get<DroneSpawned>().RemoveListener(AddDroneToMemory);
                Signals.Get<DroneDestroyed>().RemoveListener(RemoveDroneFromMemory);
                Signals.Get<DroneEnteredDetectionArea>().RemoveListener(SetTarget);
            }
     }
}

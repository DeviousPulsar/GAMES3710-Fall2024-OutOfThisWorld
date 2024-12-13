using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using OutOfThisWorld.Player;
using deVoid.Utils;

namespace OutOfThisWorld.Monster {
    public enum MonsterState {
        Disabled,
        Idle,
        Wander,
        Persue,
        Rage
    }

    public class MonsterAI : MonoBehaviour {
        /* ----------| Serialized Variables |---------- */

            [Header("References")]
            public NavMeshAgent NavAgent;
            public AlienAnimationController Animator;
            public CapsuleCollider Collider;
            public List<MonsterDetectionArea> DetectionAreas;
            public List<Transform> WanderDestinations;

            [Header("Behavior Timeouts")]
            public float DespawnTimeout = 1000f;
            public float EatTimeout = 1f;
            public float PersueTimeout = 1f;
            public float WanderTimeout = 1f;
            //public float RageTimeout = 30f;

            [Header("Behavior Thresholds")]
            public float StoppedSpeed = 0.01f;
            public float RelogDist = 0.1f;
            public float PersueWaitDist = 1f;

        /* ----------| Properties |---------- */

            public MonsterState CurrentState { get; set; }

        /* ----------| Instance Variables |---------- */

            private Dictionary<DroneController, bool> _memory;
            private Transform _wanderDest;
            private DroneController _target;

            private float _spawnTimestamp;
            private float _waitUntilTimestamp;
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
                if (CurrentState != MonsterState.Disabled && Time.fixedTime >_waitUntilTimestamp) {
                    if (CurrentState == MonsterState.Idle) {
                        UpdateWanderDest();
                    } else if (CurrentState == MonsterState.Persue && InWaitRange() && !IsTargetAccessable()) {
                        _memory[_target] = false;
                        StartWaitState(PersueTimeout);
                    }

                    UpdatePath();
                }

                if (_lastState != CurrentState || (_lastDest - NavAgent.destination).magnitude > 1) {
                    Debug.Log("Alien in moving into state " + CurrentState + " w/ destination " + 
                            NavAgent.destination.ToString() + " at t=" + Time.fixedTime + "."  + 
                            (CurrentState == MonsterState.Idle? " Idling until " + _waitUntilTimestamp : ""));
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

                        if (_target == null) {
                            float minDist = float.PositiveInfinity;
                            foreach (DroneController drone in _memory.Keys) {
                                float dist = (transform.position - drone.transform.position).magnitude;
                                if (dist < minDist) {
                                    SetTarget(drone);
                                }
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

        /* ----------| State Updators |---------- */

            public void ReachedWanderDest(Transform dest) {
                if (CurrentState == MonsterState.Wander && _wanderDest == dest) {
                    StartWaitState(WanderTimeout);
                }
            }

            bool IsTargetAccessable() {
                Vector3 dest = NavAgent.destination;
                Vector3 targetPos = _target.transform.position;
                return targetPos.z >= dest.z && targetPos.z <= dest.z + Collider.height &&
                    Mathf.Pow(dest.x - targetPos.x, 2) + Mathf.Pow(dest.y - targetPos.y, 2) <= Collider.radius;

            }

            bool InWaitRange() {
                if (!NavAgent.hasPath) { return false; }

                float dist = 0;
                Vector3 _lastPoint = NavAgent.path.corners[0];
                foreach(Vector3 point in NavAgent.path.corners) {
                    dist += (_lastPoint - point).magnitude;
                    _lastPoint = point;
                }
                return dist < PersueWaitDist;
            }

            public void StartWaitState(float deltaTime) {
                CurrentState = MonsterState.Idle;
                _waitUntilTimestamp = Time.fixedTime + deltaTime;
            }

        /* ----------| Memory and Target Update Functions |---------- */

            void UpdateWanderDest() {
                Transform newDest = null;
                while (newDest == null || newDest == _wanderDest) {
                    newDest = WanderDestinations[Random.Range(0, WanderDestinations.Count)];
                }
                _wanderDest = newDest;
                _target = null;
                CurrentState = MonsterState.Wander;

            }

            void UpdateMemory(DroneController drone) {
                if (!_memory[drone]) {
                    _memory[drone] = true;
                    Debug.Log("Alien memory updates so " + drone + " can be chased.");
                }

                if (CurrentState == MonsterState.Rage) {
                    SetTarget(drone);
                } else {
                    foreach (MonsterDetectionArea detect in DetectionAreas) {
                        if (detect.InDetectionArea(drone)) {
                            SetTarget(drone);
                            return;
                        }
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
                if (target){ //&& _waitUntilTimestamp < Time.fixedTime) {
                    target.AttemptDestroy();
                    _waitUntilTimestamp = Time.fixedTime + EatTimeout;
                    Animator.TriggerAttack();

                    if(CurrentState != MonsterState.Rage) {
                        CurrentState = MonsterState.Idle;
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

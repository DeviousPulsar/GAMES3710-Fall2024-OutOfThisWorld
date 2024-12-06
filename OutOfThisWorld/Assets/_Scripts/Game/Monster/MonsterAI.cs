using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using OutOfThisWorld.Player;
using deVoid.Utils;

namespace OutOfThisWorld.Monster {
    public enum MonsterState {
        WANDER,
        PERSUE,
        RAGE
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

            public float StoppedSpeed = 0.01f;

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

                CurrentState = MonsterState.WANDER;
                UpdateWanderDest();

                // Signal Registration
                Signals.Get<DronePositionUpdate>().AddListener(UpdateMemory);
                Signals.Get<DroneSpawned>().AddListener(AddDroneToMemory);
                Signals.Get<DroneDestroyed>().AddListener(RemoveDroneFromMemory);
                Signals.Get<DroneEnteredDetectionArea>().AddListener(SetTarget);
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

                if (_timeStopped > StoppedTimeout) {
                    if (CurrentState == MonsterState.PERSUE || CurrentState == MonsterState.RAGE) {
                        _memory[_target] = false;
                        Debug.Log("Alien memory updates noting that " + _target + " cannot be chased.");
                    }
                    UpdateWanderDest();
                }

                if(_debugPrintTimestamp + 1000000 < Time.fixedTime) {
                    Debug.Log(
                        "Alien in state " + CurrentState + " w/ destination " + NavAgent.destination.ToString() + "\n" +
                        NavAgent.velocity + " " + _timeStopped + "\n" + 
                        new System.Func<string>(() => {
                            string result = "_memory = [";
                            foreach (DroneController drone in _memory.Keys) {
                                result += drone + ":" + _memory[drone] + ",";
                            }
                            result += "]";
                            return result;
                        })()
                    );
                    _debugPrintTimestamp = Time.fixedTime;
                }

                if (_lastState != CurrentState || (_lastDest - NavAgent.destination).magnitude > 1) {
                    Debug.Log("Alien in moving into state " + CurrentState + " w/ destination " + NavAgent.destination.ToString() + "." );
                }
                _lastState = CurrentState;
                _lastDest = NavAgent.destination;
            }

            void UpdatePath() {
                switch (CurrentState) {
                    case MonsterState.WANDER:
                        NavAgent.destination = _wanderDest.position;
                        break;

                    case MonsterState.RAGE:
                        float minDist = float.PositiveInfinity;
                        foreach (DroneController drone in _memory.Keys) {
                            float dist = (transform.position - drone.transform.position).magnitude;
                            if (dist < minDist) {
                                SetTarget(drone);
                            }
                        }
                        NavAgent.destination = _target.transform.position;
                        break;

                    case MonsterState.PERSUE:
                        NavAgent.destination = _target.transform.position;
                        break;
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
                if (CurrentState != MonsterState.RAGE) {
                    CurrentState = MonsterState.WANDER;
                }
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
                    if (CurrentState != MonsterState.RAGE) {
                        CurrentState = MonsterState.PERSUE;
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
                    Destroy(coll.gameObject);
                    _eatTimestamp = Time.fixedTime;

                    UpdateWanderDest();
                }
            }
     }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using OutOfThisWorld.Player;
using deVoid.Utils;

namespace OutOfThisWorld.Monster {
    [RequireComponent(typeof(NavMeshAgent))]
    public class MonsterAI : MonoBehaviour {

        /* ----------| Instance Variables |---------- */

            private NavMeshAgent _navAgent;

        /* ----------| Initialization Functions |---------- */

            void Start()
            {
                _navAgent = GetComponent<NavMeshAgent>();
                DebugUtility.HandleErrorIfNullGetComponent<MonsterAI, NavMeshAgent>(_navAgent, this, gameObject);

                Signals.Get<DronePositionsUpdate>().AddListener(UpdatePath);
            }

        /* ----------| Main Loop |---------- */

            void UpdatePath(List<Vector3> targets) {
                float min_dist = float.PositiveInfinity;
                Vector3 closest_target = _navAgent.destination;
                foreach (Vector3 dest in targets) {
                    float heu_dist = (dest - transform.position).magnitude;
                    if (heu_dist < min_dist) {
                        closest_target = dest;
                        min_dist = heu_dist;
                    }
                }

                _navAgent.destination = closest_target;
            }

        /* ----------| Collision Handling |---------- */

            void OnCollisionEnter(Collision coll) {
                if (coll.gameObject.GetComponent<DroneController>() is not null) {
                    Destroy(coll.gameObject);
                }
            }
    }
}

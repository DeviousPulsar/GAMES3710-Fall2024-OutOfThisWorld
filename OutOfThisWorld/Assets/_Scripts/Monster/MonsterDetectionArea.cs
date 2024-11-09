using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OutOfThisWorld.Monster {
    [RequireComponent(typeof(Rigidbody))]
    public class MonsterDetectionArea : MonoBehaviour {
        /* ----------| Serialized Variables |---------- */

            public MonsterAI Monster;

        /* ----------| Instance Variables |---------- */


        /* ----------| Collision Detection Functions |---------- */

            void OnTriggerStay(Collider other) {
                MonsterTarget target = other.GetComponent<MonsterTarget>();
                if (target) {
                    Monster.Spot(target);
                }
            }
    }
}

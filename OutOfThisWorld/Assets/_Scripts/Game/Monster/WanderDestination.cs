using UnityEngine;

namespace OutOfThisWorld.Monster {
    [RequireComponent(typeof(Collider))]
    public class WanderDestination : MonoBehaviour {
        public MonsterAI Monster;

        void OnTiggerEnter() {
            Monster.ReachedWanderDest(transform);
        }
    }
}

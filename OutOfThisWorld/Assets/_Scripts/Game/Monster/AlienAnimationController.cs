using UnityEngine;
using UnityEngine.AI;

namespace OutOfThisWorld.Monster {
    public class AlienAnimationController : MonoBehaviour {
        [SerializeField] Animator Animations;
        [SerializeField] NavMeshAgent NavAgent;
        public float WalkAnimMult = 1f;

        void Update() {
            Animations.SetFloat("Speed", WalkAnimMult*NavAgent.velocity.magnitude);
        }

        void TriggerAttack() {
            Animations.SetTrigger("Attack");
        }
    }
}

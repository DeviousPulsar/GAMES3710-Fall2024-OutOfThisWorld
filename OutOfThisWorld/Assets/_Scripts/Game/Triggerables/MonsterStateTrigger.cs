using UnityEngine;
using OutOfThisWorld.Monster;

namespace OutOfThisWorld {
    public class MonsterStateTrigger : Triggerable {
        public MonsterAI Monster;
        public MonsterState State;

        public override void Trigger() {
            Monster.CurrentState = State;
        }
    }
}
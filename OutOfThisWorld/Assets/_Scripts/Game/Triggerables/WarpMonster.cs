using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OutOfThisWorld.Monster;

namespace OutOfThisWorld {
    public class WarpMonster : Triggerable {
        /* ----------| Serialized Variables |---------- */

            public MonsterAI Monster;
            public Transform WarpDestination;

        /* ----------| Trigger Functions |---------- */

            public override void Trigger() {
                Monster.NavAgent.Warp(WarpDestination.position);
            }
    }
}

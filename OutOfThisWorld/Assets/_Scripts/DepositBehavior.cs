using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OutOfThisWorld.Debug;


namespace OutOfThisWorld {
    public class DepositBehavior : MonoBehaviour {
        /* ----------| Serialized Variables |---------- */

            public float cashInMultiplier = 1f;
            public ResourceSystem resourceSystem;

        /* ----------| Functions |---------- */

            public void MakeDeposit(ItemBehavior item)
            {
                if (item != null)
                {
                    resourceSystem.AddResources(item.ItemCost);
                }
            }
    }
}

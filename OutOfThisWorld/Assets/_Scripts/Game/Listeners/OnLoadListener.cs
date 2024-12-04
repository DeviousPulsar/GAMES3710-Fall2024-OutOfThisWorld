using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OutOfThisWorld {
    public class OnLoadListener : MonoBehaviour {

        /* ----------| Serialized Variables |---------- */

            [SerializeField] List<Triggerable> Triggers;

        /* ----------| Main Functions |----------- */

            void Start() 
            {
                foreach (Triggerable t in Triggers) { t.Trigger(); }
            }
    }
}

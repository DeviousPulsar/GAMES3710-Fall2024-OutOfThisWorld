using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OutOfThisWorld {
    public abstract class Triggerable : MonoBehaviour {
        public abstract void Trigger();
        public abstract void Undo();
    }

    public class SocketListener : MonoBehaviour{
        
        /* ----------| Serialized Variables |---------- */

            [SerializeField] List<ItemSocket> Sockets;
            [SerializeField] List<Triggerable> Triggers;

        /* ----------| Instance Variables |---------- */

            private bool LastTriggered = false;

        /* ----------| Main Loop |----------- */

            void FixedUpdate() 
            {
                bool triggered = CheckTriggers();
                if (triggered && !LastTriggered) {
                    foreach (Triggerable t in Triggers) { t.Trigger(); }
                } else if (!triggered && LastTriggered) {
                    foreach (Triggerable t in Triggers) { t.Undo(); }
                }
                LastTriggered = triggered;
            }

            bool CheckTriggers() {
                foreach (ItemSocket sock in Sockets)
                {
                    if (!sock.HasCorrectItem()) { 
                        return false; 
                    }
                }

                return true;
            }
    }
}

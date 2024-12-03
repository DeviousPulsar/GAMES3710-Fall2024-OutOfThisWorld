using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OutOfThisWorld {
    public class SocketListener : OnLoadListener {
        
        /* ----------| Serialized Variables |---------- */

            [SerializeField] List<ItemSocket> Sockets;

        /* ----------| Main Loop |----------- */

            protected override bool CheckTriggered() {
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

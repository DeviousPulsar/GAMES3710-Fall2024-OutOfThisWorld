using System.Collections.Generic;
using UnityEngine;
using OutOfThisWorld.Player;
using deVoid.Utils;

namespace OutOfThisWorld.Monster {
    public class DroneEnteredDetectionArea : ASignal<DroneController> {}

    [RequireComponent(typeof(Rigidbody))]
    public class MonsterDetectionArea : MonoBehaviour {
        /* ----------| Instance Variables |---------- */

            private ISet<DroneController> _occupingBodies;

        /* ----------| Initialization Functions |---------- */

            void Awake() {
                _occupingBodies = new HashSet<DroneController>();
            }

        /* ----------| Access Methods |---------- */

            public bool InDetectionArea(DroneController drone) {
                return _occupingBodies.Contains(drone);
            }

        /* ----------| Collision Detection Functions |---------- */

            void OnTriggerEnter(Collider other) {
                DroneController drone = other.GetComponent<DroneController>();
                if (drone != null) {
                    _occupingBodies.Add(drone);
                    Signals.Get<DroneEnteredDetectionArea>().Dispatch(drone);
                    print("Drone " + drone + " detected entering MonsterDetectionArea " + this); 
                }
            }

            void OnTriggerExit(Collider other) {
                DroneController drone = other.GetComponent<DroneController>();
                if (drone != null) {
                    _occupingBodies.Remove(drone);
                    print("Drone " + drone + " detected leaving MonsterDetectionArea " + this);
                }
            }
    }
}

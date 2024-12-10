using UnityEngine;
using UnityEngine.UI;

namespace OutOfThisWorld.Player.HUD {
    //[RequireComponent(typeof(Image))]
    public class DroneInfoBar : MonoBehaviour {
        private DroneController _drone;
        public DroneController Drone {
            get => _drone;
            set {
                if (_drone == null) {
                    _drone = value;
                }
            }
        }

        public FuelGauge FuelGauge;
        public GameObject Paused;
        public GameObject Active;
        public GameObject Alarm;
        public GameObject Warning;

        void Update() {
            if(Drone != null) {
                FuelGauge.UpdateGauge(Drone.GetFuelPercentage());
                Paused.SetActive(!Drone.Follow);
                Active.SetActive(Drone.Active);
            }
        }
    }
}
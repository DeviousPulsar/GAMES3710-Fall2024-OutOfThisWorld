using System.Collections.Generic;
using UnityEngine;
using deVoid.Utils;

namespace OutOfThisWorld.Player.HUD
{
    public class DroneInfoPanel : MonoBehaviour
    {
        public GameObject InfoBarPrefab;

        private Dictionary<DroneController,DroneInfoBar> _drones;
        private DroneController _activeDrone;
        private DroneInfoBar _defaultInfoBar;

        void Awake() 
        {
            _drones = new Dictionary<DroneController,DroneInfoBar>();

            _defaultInfoBar = InfoBarPrefab.GetComponent<DroneInfoBar>();
            DebugUtility.HandleErrorIfNullGetComponent<DroneInfoBar, GameObject>(_defaultInfoBar, this, InfoBarPrefab);

            Signals.Get<DroneSpawned>().AddListener(AddInfoBar);
            Signals.Get<DroneDestroyed>().AddListener(RemoveInfoBar);
        }

        public bool InfoBarPrefabValid() { return _defaultInfoBar != null; }

        public void AddInfoBar(DroneController drone)
        {
            if (!InfoBarPrefabValid()) { return; }

            GameObject obj = Instantiate(InfoBarPrefab, transform);
            DroneInfoBar infobar = obj.GetComponent<DroneInfoBar>();
            DebugUtility.HandleErrorIfNullGetComponent<DroneInfoBar, GameObject>(infobar, this, obj);
            if(!infobar || !_drones.TryAdd(drone, infobar)) { 
                Destroy(obj);
            } else {
                infobar.Drone = drone;
            }
        }

        public void RemoveInfoBar(DroneController drone)
        {
            DroneInfoBar infobar = _defaultInfoBar;
            if (!_drones.TryGetValue(drone, out infobar)) { return; }
            if (!_drones.Remove(drone)) { return; }
            Destroy(infobar.gameObject);
        }

        void OnDestroy() {
            Signals.Get<DroneSpawned>().RemoveListener(AddInfoBar);
            Signals.Get<DroneDestroyed>().RemoveListener(RemoveInfoBar);
        }
    }
}

using System.Collections.Generic;
using UnityEngine;
using OutOfThisWorld.Debug;

namespace OutOfThisWorld.Player.HUD
{
    public class DroneInfoPanel : MonoBehaviour
    {
        public GameObject InfoBarPrefab;

        private Dictionary<DroneController,DroneInfoBar> _drones;
        private DroneController _activeDrone;
        private DroneInfoBar _defaultInfoBar;

        void Start() 
        {
            _drones = new Dictionary<DroneController,DroneInfoBar>();

            _defaultInfoBar = InfoBarPrefab.GetComponent<DroneInfoBar>();
            DebugUtility.HandleErrorIfNullGetComponent<DroneInfoBar, GameObject>(_defaultInfoBar, this, InfoBarPrefab);
        }

        public bool InfoBarPrefabValid() { return _defaultInfoBar; }

        public bool AddInfoBar(DroneController drone)
        {
            if (!InfoBarPrefabValid()) { return false; }

            GameObject obj = Instantiate(InfoBarPrefab, transform);
            DroneInfoBar infobar = obj.GetComponent<DroneInfoBar>();
            DebugUtility.HandleErrorIfNullGetComponent<DroneInfoBar, GameObject>(infobar, this, obj);
            if(!infobar || !_drones.TryAdd(drone, infobar)) 
            { 
                Destroy(obj);
                return false;
            }

            return true;
        }

        public bool RemoveInfoBar(DroneController drone)
        {
            DroneInfoBar infobar = _defaultInfoBar;
            if (!_drones.TryGetValue(drone, out infobar)) { return false; }
            if (!_drones.Remove(drone)) { return false; }
            Destroy(_defaultInfoBar.gameObject);

            return true;
        }

        public bool SetActiveInfoBar(DroneController drone)
        {
            if (drone == _activeDrone) { return false; }
            if (!_drones.ContainsKey(drone)) { return false; }

            foreach(DroneController d in _drones.Keys)
            {
                if (d != drone) {
                    _drones[d].SetAsInactive();
                } else {
                    _drones[d].SetAsActive();
                }
            }

            _activeDrone = drone;
            return true;
        }
    }
}

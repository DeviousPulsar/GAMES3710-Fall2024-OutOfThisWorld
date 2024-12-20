using UnityEngine;
using deVoid.Utils;
using UnityEngine.UI;
using OutOfThisWorld.Player.HUD;

namespace OutOfThisWorld
{
    public class ResourceSystem : MonoBehaviour
    {
        /* ----------| Component Properties |---------- */

            public float InitalResourceCount = 100f;
            [SerializeField] TaskInfoPanel _taskUIPanel;

        /* ----------| Instance Variables |---------- */

            private float _currentResourceCount = 0;
            

        /* ----------| Initalization Functions |---------- */

            void Start()
            {
                _currentResourceCount = InitalResourceCount;
            }

        /* -----------| Resource Count Modification Functions |---------- */

            public float GetResourceCount() { return _currentResourceCount; }
            
            public bool AddResources(float delta)
            {
                if (delta <= 0) { return false; }

                _currentResourceCount += delta;
                if(_currentResourceCount >= 5) { _taskUIPanel.CompleteTask("Accumulate 5 RP"); }
                return true;
            }

            public bool SpendResources(float delta)
            {
                if (delta < 0) { return false; }
                if (delta > _currentResourceCount) { return false; }

                _currentResourceCount -= delta;
                return true;
            }
    }
}

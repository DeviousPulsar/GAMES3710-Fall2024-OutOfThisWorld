using UnityEngine;
using deVoid.Utils;

namespace OutOfThisWorld
{
    public class ResourceSystem : MonoBehaviour
    {
        /* ----------| Component Properties |---------- */

            public float InitalResourceCount = 100f;

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
                return true;
            }

            public bool SpendResources(float delta)
            {
                if (delta <= 0) { return false; }
                if (delta > _currentResourceCount) { return false; }

                _currentResourceCount -= delta;
                return true;
            }
    }
}

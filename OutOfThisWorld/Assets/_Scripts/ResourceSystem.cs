using UnityEngine;
using deVoid.Utils;
using UnityEngine.UI;

namespace OutOfThisWorld
{
    public class ResourceCountChanged : ASignal<float> {}

    public class ResourceSystem : MonoBehaviour
    {
        /* ----------| Component Properties |---------- */

            public float InitalResourceCount = 100f;
            public bool DoResourceChangeSignals = false;

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
                if(DoResourceChangeSignals) { Signals.Get<ResourceCountChanged>().Dispatch(_currentResourceCount); }

                GameObject.Find("ResourcesAmount").GetComponent<Text>().text = "Resources: " + _currentResourceCount.ToString();

                return true;
            }

            public bool SpendResources(float delta)
            {
                if (delta <= 0) { return false; }
                if (delta > _currentResourceCount) { return false; }

                _currentResourceCount -= delta;
                if(DoResourceChangeSignals) { Signals.Get<ResourceCountChanged>().Dispatch(_currentResourceCount); }
                return true;
            }
    }
}

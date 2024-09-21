using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OutOfThisWorld.Debug;


namespace OutOfThisWorld
{
    [RequireComponent(typeof(ResourceSystem))]
    public class DepositBehavior : MonoBehaviour
    {
        /* ----------| Component Properties |---------- */
        public float cashInMultiplier = 1f;

        /* ----------| Instance Variables |---------- */
        private ResourceSystem _resourceSystem;

        // Start is called before the first frame update
        void Start()
        {
            _resourceSystem = GetComponent<ResourceSystem>();
            DebugUtility.HandleErrorIfNullGetComponent<ResourceSystem, DepositBehavior>(_resourceSystem, this, gameObject);

        }

        // Update is called once per frame
        void Update()
        {

        }


        /* ----------| Functions |---------- */

        public void MakeDeposit(ItemBehavior item)
        {
            if (item != null)
            {
                _resourceSystem.AddResources(item.getItemCost());
            }

        }






    }
}

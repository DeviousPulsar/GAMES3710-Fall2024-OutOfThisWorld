using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OutOfThisWorld
{    
    public class ItemBehavior : MonoBehaviour
    {
        /* ----------| Component Properties |---------- */

        public float ItemCost = 1f;


        /* ----------| Instance Variables |---------- */






        /* ----------| Initalization Functions |---------- */

        // Start is called before the first frame update
        void Start()
        {





        }



        /* ----------| Main Update Loop |---------- */

        // Update is called once per frame
        void Update()
        {

        }

        public float getItemCost()
        {
            return ItemCost;
        }

        public void setItemCost(float cost)
        {
            ItemCost = cost;
        }

    }
}

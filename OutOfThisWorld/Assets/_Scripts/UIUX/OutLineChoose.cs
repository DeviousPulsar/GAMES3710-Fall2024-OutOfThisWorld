using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace OutOfThisWorld {
    public class OutLineChoose : MonoBehaviour {
        private Transform highlight; 
        private RaycastHit raycastHit;

        public Color defaultOutlineColor = Color.gray; 
        public Color hoverOutlineColor = Color.white; 

        public float outlineWidth = 8f;
        public float outlineRange = 2f;

        void Update()
        {
            
            if (highlight != null)
            {
                var outline = highlight.gameObject.GetComponent<Outline>();
                if (outline != null)
                {
                    outline.OutlineColor = defaultOutlineColor; 
                }
                highlight = null;
            }

            
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out raycastHit, outlineRange))
            {
                highlight = raycastHit.transform;

                if (highlight.CompareTag("Selectable"))
                {
                    var outline = highlight.gameObject.GetComponent<Outline>();
                    if (outline == null)
                    {
                        outline = highlight.gameObject.AddComponent<Outline>();
                        outline.OutlineWidth = outlineWidth;
                    }
                    outline.OutlineColor = hoverOutlineColor; 
                }
            }
        }
    }
}



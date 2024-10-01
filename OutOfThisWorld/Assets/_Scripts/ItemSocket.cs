using System.Collections.Generic;
using System.Collections.Immutable;
using UnityEngine;

namespace OutOfThisWorld {
    public class ItemSocket : MonoBehaviour {
        
        /* ----------| Serialized Variables |---------- */

            public List<string> DesiredItemTags;
            [Header("Item Hold Information")]
            public Transform HoldTransform;
            public float HoldScale = 1f;
            public float BreakHoldForce = float.PositiveInfinity;

        /* ----------| Instance Variables |---------- */

            private ItemBehavior _heldItem;
            private FixedJoint _holdJoint;
            private ISet<string> _fixedDesiredSet;

        /* ----------| Initalizatino Functions |---------- */

            void Start()
            {
                _fixedDesiredSet = new HashSet<string>(DesiredItemTags);
            }

        /* ----------| Socket Functions |----------- */

            public void TakeItem (ItemBehavior item)
            {
                _heldItem = item;
                _heldItem.gameObject.SetActive(true);
                _heldItem.Grab(HoldTransform.position, HoldTransform.rotation, HoldScale);

                _holdJoint = gameObject.AddComponent<FixedJoint>();
                _holdJoint.connectedBody = _heldItem.GetRigidbody();
                _holdJoint.breakForce = BreakHoldForce;
                _holdJoint.breakTorque = BreakHoldForce;
            }

            public ItemBehavior GiveItem ()
            {
                _heldItem.Drop(HoldScale);

                Destroy(_holdJoint);

                return _heldItem;
            }

            public bool HasItem ()
            {
                return _holdJoint != null;
            }

            public bool HasCorrectItem ()
            {
                return _holdJoint != null && _fixedDesiredSet.Contains(_heldItem.ItemTag);
            }
    }
}
using UnityEngine;
using UnityEngine.EventSystems;
using OutOfThisWorld.Player;
using deVoid.Utils;

namespace OutOfThisWorld {
    [RequireComponent(typeof(PlayerController))]
    public class OutlineHandler : MonoBehaviour {

        /* ----------| Serialized Variables |---------- */

            public Color defaultColor = Color.clear; 
            public Color hoverColor = Color.white; 
            public Color canDropColor = Color.white; 
            public Color noDropColor = Color.red; 

            public float outlineWidth = 8f;
            public float outlineRange = 2f;

        /* ----------| Private Variables |---------- */

            private Outline _lastHover; 
            private PlayerController _playerController;

        /* ----------| Initialization Functions |---------- */

            void Start() {
                // fetch components on the same gameObject
                _playerController =  GetComponent<PlayerController>();
                DebugUtility.HandleErrorIfNullGetComponent<PlayerController, OutlineHandler>(_playerController, this, gameObject);

                //Find all selectable objects
                GameObject[] selectables = GameObject.FindGameObjectsWithTag("Selectable");
                foreach (GameObject obj in selectables) {
                    InitializeOutline(obj);
                }

                // Signal Registration
                Signals.Get<ItemSpawned>().AddListener(InitializeOutline);
                Signals.Get<ItemDropped>().AddListener(ResetOutline);
                Signals.Get<DroneSwitched>().AddListener(ResetHeldItemOutline);
            }

        /* ----------| Main Update Loop |---------- */

            void FixedUpdate() {             
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, outlineRange)) {
                    if (hit.collider.CompareTag("Selectable")) {
                        Outline outline = hit.collider.GetComponent<Outline>();
                        if (_lastHover != null && outline != _lastHover) {
                            _lastHover.OutlineColor = defaultColor;
                        }

                        if (outline != null) {
                            outline.OutlineColor = hoverColor; 
                            _lastHover = outline;
                        }
                    }
                } else if (_lastHover != null) {
                    _lastHover.OutlineColor = defaultColor;
                    _lastHover = null;
                }

                DroneController activeDrone = _playerController.GetActiveDrone();
                if (activeDrone.HasHeldItem()) {
                    Outline outline = activeDrone.GetCurrentHeldItem().GetComponent<Outline>();

                    if (outline != null) {
                        if (activeDrone.CanDropCurrentItem()) {
                            outline.OutlineColor = canDropColor;
                        } else {
                            outline.OutlineColor = noDropColor;
                        }
                    }
                }
            }

        /* ----------| Signal Processing |---------- */

            void InitializeOutline(GameObject obj) {
                if (obj == null) { return; }

                Outline outline = obj.GetComponent<Outline>();
                if (outline == null) { outline = obj.gameObject.AddComponent<Outline>(); }
                outline.OutlineWidth = outlineWidth;
                outline.OutlineColor = defaultColor;
            }

            void InitializeOutline(Component comp) {
                if (comp == null) { return; }

                InitializeOutline(comp.gameObject);
            }

            void ResetOutline(GameObject obj) {
                if (obj == null) { return; }

                Outline outline = obj.GetComponent<Outline>();
                if (outline != null) {
                    outline.OutlineColor = defaultColor;
                }
            }

            void ResetOutline(Component comp) {
                if (comp == null) { return; }

                ResetOutline(comp.gameObject);
            }

            void ResetHeldItemOutline(DroneController drone) {
                ResetOutline(drone.GetCurrentHeldItem());
            }
    }
}



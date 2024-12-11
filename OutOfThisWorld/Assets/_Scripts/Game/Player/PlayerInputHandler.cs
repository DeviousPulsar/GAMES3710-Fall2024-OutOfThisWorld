using System.Collections.Generic;
using UnityEngine;

namespace OutOfThisWorld.Player
{
    public class PlayerInputHandler : MonoBehaviour
    {
    /* ----------| Component Properties |---------- */

        public string MoveXAxisName = "Strafe";
        public string MoveYAxisName = "Vertical";
        public string MoveZAxisName = "Forwards";
        public string LookXAxisName = "Mouse X";
        public string LookYAxisName = "Mouse Y";
        public string DroneShiftAction = "Drone Shift";
        public string DroneInteract = "Interact";
        public string DroneDrop = "Drop";
        public string DroneModeAction = "Mode Shift";
        public string Pause = "Pause";
        

        [Tooltip("Sensitivity multiplier for moving the camera around")]
        public float LookSensitivity = 1f;

        [Tooltip("Used to flip the horizontal input axis")]
        public bool InvertXAxis = false;
        [Tooltip("Used to flip the vertical input axis")]
        public bool InvertYAxis = false;
    /* ----------| Initalization Functions |---------- */

        void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

    /* ----------| Input Processing |---------- */

        public bool CanProcessInput()
        {
            return Cursor.lockState == CursorLockMode.Locked;
        }

        public Vector3 GetMoveForce()
        {
            if (CanProcessInput())
            {
                Vector3 move = new Vector3(
                    Input.GetAxisRaw(MoveXAxisName), 
                    Input.GetAxisRaw(MoveYAxisName),
                    Input.GetAxisRaw(MoveZAxisName)
                );

                // constrain move input to a maximum magnitude of 1, otherwise diagonal movement might exceed the max move speed defined
                move = Vector3.ClampMagnitude(move, 1);

                return move;
            }

            return Vector3.zero;
        }

        public Vector3 GetLookAngles()
        {
            if (CanProcessInput())
            {
                 Vector3 rotation = new Vector3(
                    Input.GetAxisRaw(LookXAxisName) * (InvertYAxis? -1 : 1), 
                    Input.GetAxisRaw(LookYAxisName) * (InvertXAxis? -1 : 1),
                    0f
                );

                // apply sensitivity multiplier
                rotation *= LookSensitivity;

                return rotation;
            }

            return Vector3.zero;
        }
    }
}
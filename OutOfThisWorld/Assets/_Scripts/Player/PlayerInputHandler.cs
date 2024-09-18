using UnityEngine;
using OutOfThisWorld.Debug;

namespace OutOfThisWorld.Player
{
    public class PlayerInputHandler : MonoBehaviour
    {
        public static readonly string KEYBOARD_X_AXIS = "Strafe";
        public static readonly string KEYBOARD_Y_AXIS = "Vertical";
        public static readonly string KEYBOARD_Z_AXIS = "Forwards";
        public static readonly string MOUSE_X_AXIS = "Mouse X";
        public static readonly string MOUSE_Y_AXIS = "Mouse Y";

        [Tooltip("Sensitivity multiplier for moving the camera around")]
        public float LookSensitivity = 1f;

        [Tooltip("Used to flip the vertical input axis")]
        public bool InvertYAxis = false;

        [Tooltip("Used to flip the horizontal input axis")]
        public bool InvertXAxis = false;

        void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        public bool CanProcessInput()
        {
            return Cursor.lockState == CursorLockMode.Locked;
        }

        public Vector3 GetMoveForce()
        {
            if (CanProcessInput())
            {
                Vector3 move = new Vector3(
                    Input.GetAxisRaw(KEYBOARD_X_AXIS), 
                    Input.GetAxisRaw(KEYBOARD_Y_AXIS),
                    Input.GetAxisRaw(KEYBOARD_Z_AXIS)
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
                    Input.GetAxisRaw(MOUSE_Y_AXIS) * (InvertXAxis? -1 : 1),
                    Input.GetAxisRaw(MOUSE_X_AXIS) * (InvertYAxis? -1 : 1), 
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
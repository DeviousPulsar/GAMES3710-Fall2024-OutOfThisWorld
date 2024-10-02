using UnityEngine;
using UnityEngine.UI;
using OutOfThisWorld.Debug;

namespace OutOfThisWorld.Player.HUD
{
    [RequireComponent(typeof(Image))]
    public class DroneInfoBar : MonoBehaviour
    {
        public Color ActiveColor;
        public Color InactiveColor;
        private Image _image;

        void Start()
        {
            _image = GetComponent<Image>();
            //DebugUtility.HandleErrorIfNullGetComponent<Image, DroneInfoBar>(_image, this, gameObject);
        }

        public void SetAsActive()
        {
            Vector3 scale = transform.localScale;
            scale.x = 1.25f;
            transform.localScale = scale;
            _image.color = ActiveColor;
        }

        public void SetAsInactive()
        {
            Vector3 scale = transform.localScale;
            scale.x = 1f;
            transform.localScale = scale;
            _image.color = InactiveColor;
        }
    }
}
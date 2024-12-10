using UnityEngine;
using UnityEngine.UI;

namespace OutOfThisWorld.Player.HUD {
    public class FuelGauge : MonoBehaviour {
        public float MaxWidth;
        public RectTransform Gauge;

        public void UpdateGauge(float percent) {
           Gauge.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, percent*MaxWidth);
        }
    }
}

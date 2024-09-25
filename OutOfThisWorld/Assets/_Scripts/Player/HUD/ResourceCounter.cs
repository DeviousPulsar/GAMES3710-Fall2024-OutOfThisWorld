using UnityEngine;
using OutOfThisWorld;
using TMPro;

namespace OutOfThisWorld.Player.HUD
{
    public class ResourceCounterPanel : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI _textObject;
        [SerializeField] ResourceSystem _resourceSystem;

        void Update()
        {
            _textObject.text = string.Format("{0:D3} RP", (int) _resourceSystem.GetResourceCount());
        }
    }
}
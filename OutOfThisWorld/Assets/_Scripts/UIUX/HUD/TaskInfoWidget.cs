using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace OutOfThisWorld.Player.HUD
{
    public class TaskInfoWidget : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI taskText;

        public void setTaskText(string text)
        {
            taskText.text = text;
        }
    }
}

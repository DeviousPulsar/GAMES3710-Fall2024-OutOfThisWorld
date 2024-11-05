using UnityEngine;
using UnityEngine.UI;
using OutOfThisWorld.Debug;


namespace OutOfThisWorld.Player.HUD
{
    public class TaskInfoWidget : MonoBehaviour
    {
        [SerializeField] Text taskText;


        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void setTaskText(string text)
        {
            taskText.text = text;
        }
    }
}

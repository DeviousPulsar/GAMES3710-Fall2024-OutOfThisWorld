using UnityEngine;
using TMPro;

namespace OutOfThisWorld {
    public class DisplaySubtitle : MonoBehaviour {
        [SerializeField] private string subtitleText; 
        [SerializeField] private float displayDuration = 3f; 
        [SerializeField] private TextMeshProUGUI subtitleUI; 

        private void OnMouseDown()
        {
            if (subtitleUI != null)
            {
                StopAllCoroutines(); 
                StartCoroutine(ShowSubtitle());
            }
        }

        private System.Collections.IEnumerator ShowSubtitle()
        {
            subtitleUI.text = subtitleText; 
            subtitleUI.gameObject.SetActive(true); 
            yield return new WaitForSeconds(displayDuration); 
            subtitleUI.gameObject.SetActive(false); 
        }
    }
}


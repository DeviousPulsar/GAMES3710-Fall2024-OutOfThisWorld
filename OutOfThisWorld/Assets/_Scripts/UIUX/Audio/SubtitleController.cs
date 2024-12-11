using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SubtitleController : MonoBehaviour
{
    public TextMeshProUGUI[] subtitles; 
    public float fadeDuration = 1f; 
    public float displayDuration = 2f; 
    public GameObject backgroundPanel; 

    private void Start()
    {
        StartCoroutine(PlaySubtitles());
    }

    private IEnumerator PlaySubtitles()
    {
        
        for (int i = 0; i < subtitles.Length; i++)
        {
            yield return StartCoroutine(FadeIn(subtitles[i])); 
            yield return new WaitForSeconds(displayDuration); 
            yield return StartCoroutine(FadeOut(subtitles[i])); 
        }

        
        backgroundPanel.SetActive(false);
    }

    private IEnumerator FadeIn(TextMeshProUGUI text)
    {
        Color color = text.color;
        float timer = 0f;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            color.a = Mathf.Lerp(0, 1, timer / fadeDuration); 
            text.color = color;
            yield return null;
        }

        color.a = 1; 
        text.color = color;
    }

    private IEnumerator FadeOut(TextMeshProUGUI text)
    {
        Color color = text.color;
        float timer = 0f;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            color.a = Mathf.Lerp(1, 0, timer / fadeDuration); 
            text.color = color;
            yield return null;
        }

        color.a = 0; 
        text.color = color;
    }
}

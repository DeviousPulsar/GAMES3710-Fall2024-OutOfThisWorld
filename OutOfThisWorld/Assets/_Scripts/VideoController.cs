using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;


public class VideoController : MonoBehaviour
{
    public VideoPlayer videoPlayer;

    void Start()
    {
        if (videoPlayer == null)
        {
            videoPlayer = GetComponent<VideoPlayer>();
        }

        videoPlayer.loopPointReached += OnVideoFinished;
    }
    void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(0))
        {
            LoadScene();
            
        }
    }

    void OnVideoFinished(VideoPlayer vp)
    {
        LoadScene();
    }

    void LoadScene()
    {
        videoPlayer.Stop();
        Destroy(videoPlayer);
        SceneManager.LoadScene(0);
    }

}


using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("------- Audio Source -------")]
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource SFXSource;

    [Header("------- Audio Clip -------")]
    public AudioClip background;
    public AudioClip iconClick;
    public AudioClip itemInteract;
    public AudioClip startInterface;
    public AudioClip breath;
    public AudioClip jumpScare1;
    public AudioClip jumpScare2;
    public AudioClip walk;

    public AudioClip boom;
    public AudioClip doorLocked;
    public AudioClip Switch;
    public AudioClip collection_ship;
    public AudioClip collection_drone;
    public AudioClip drone_build;
    public AudioClip drone_wreck;
    public AudioClip item_drop;
    public AudioClip endingBGM;
    public AudioClip doorOpen;
    public AudioClip doorClose;
    




    private static AudioManager instance;

    
    private void Awake()
    {
        if (instance != null)
        
            Destroy(gameObject);
        else
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }
    
    private void Start()
    {
        PlayBackgroundMusic(startInterface);
    }

    public void StopMusic()
    {
        
        musicSource.Stop();


    }

    public void PlaySFX(AudioClip clip)
    {
        SFXSource.PlayOneShot(clip);
    }

    public void PlayBackgroundMusic(AudioClip clip)
    {
        if (musicSource.clip != clip)
        {
        musicSource.clip = clip;
        musicSource.loop = true; 
        musicSource.Play();
        }
    }


}

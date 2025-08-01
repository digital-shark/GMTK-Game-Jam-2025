using UnityEngine;
using FMODUnity;
using FMOD.Studio;
public class AudioManager : MonoBehaviour
{
    public static AudioManager instance { get; private set; }

    public EventReference Music;

    private EventInstance bgMusic;


    private void Awake()
    {
        if (instance != null) 
        {
            Debug.LogError("More than 1 audiomanager not possible");
            return;
        }

        instance = this;
    }

    private void Start()
    {
        bgMusic = createInstance(Music);
        bgMusic.start();
    }

    public EventInstance createInstance(EventReference sound) 
    {
        return RuntimeManager.CreateInstance(sound);

    }


    public void PlayOneshot(EventReference sound, Vector3 worldposition) 
    {
        RuntimeManager.PlayOneShot(sound, worldposition);
    }
}

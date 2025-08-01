using UnityEngine;
using FMODUnity;
public class AudioManager : MonoBehaviour
{
    public static AudioManager instance { get; private set; }

    private void Awake()
    {
        if (instance != null) 
        {
            Debug.LogError("More than 1 audiomanager not possible");
            return;
        }

        instance = this;
    }

    public void PlayOneshot(EventReference sound, Vector3 worldposition) 
    {
        RuntimeManager.PlayOneShot(sound, worldposition);
    }
}

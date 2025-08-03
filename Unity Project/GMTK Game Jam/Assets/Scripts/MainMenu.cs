using UnityEngine;
using UnityEngine.SceneManagement;
using FMODUnity;
using FMOD.Studio;

public class MainMenu : MonoBehaviour
{
    [SerializeField]
    private GameObject cameraToRotate;
    [SerializeField]
    float rotatingSpeed = 1;

    public EventReference music;
    private EventInstance player;

    private void Start()
    {
        player = AudioManager.instance.createInstance(music);
        player.start();
    }

    private void Update()
    {
        Quaternion rot = cameraToRotate.transform.rotation;
        cameraToRotate.transform.rotation *= Quaternion.Euler(0, rotatingSpeed * Time.deltaTime, 0);
    }

    public void StartGame()
    {
        player.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        SceneManager.LoadScene(1);
    }
}

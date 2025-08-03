using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField]
    private GameObject camera;
    [SerializeField]
    float rotatingSpeed = 1;

    private void Update()
    {
        Quaternion rot = camera.transform.rotation;
        camera.transform.rotation *= Quaternion.Euler(0, rotatingSpeed * Time.deltaTime, 0);
    }

    public void StartGame()
    {
        SceneManager.LoadScene(1);
    }
}

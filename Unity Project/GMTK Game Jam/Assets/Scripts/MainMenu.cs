using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField]
    private GameObject cameraToRotate;
    [SerializeField]
    float rotatingSpeed = 1;

    private void Update()
    {
        Quaternion rot = cameraToRotate.transform.rotation;
        cameraToRotate.transform.rotation *= Quaternion.Euler(0, rotatingSpeed * Time.deltaTime, 0);
    }

    public void StartGame()
    {
        SceneManager.LoadScene(1);
    }
}

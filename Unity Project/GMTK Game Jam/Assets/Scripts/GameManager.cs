using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private ImageComparer imageComparer;
    [SerializeField]
    private GameObject finalScoreHud;
    [SerializeField]
    private TMP_Text scoreText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        finalScoreHud.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShowScoreScreen()
    {
        // Only show score screen if not already active
        if (finalScoreHud.activeSelf)
            return;

        imageComparer.Compare();
        scoreText.text = imageComparer.score.ToString();
        finalScoreHud.SetActive(true);
    }
}

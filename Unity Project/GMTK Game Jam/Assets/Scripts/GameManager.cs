using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [SerializeField]
    private ImageComparer imageComparer;
    [SerializeField]
    private GameObject finalScoreScreen;
    [SerializeField]
    private TMP_Text scoreText;
    [SerializeField]
    private TMP_Text thanksForPlayingText;

    public Texture2D[] missionTextures;
    public int activeMission;

    public bool playTutorial = true;
    [SerializeField]
    float timeBeforeSkippingScoreScreen = 3.0f;
    float timeBeforeSkippingScoreScreenTimer;

    private void Awake()
    {
        // Only make one as singleton and don't destroy it to keep values between scene changes
        if (instance != null)
        {    
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        finalScoreScreen.SetActive(false);

    }

    // Update is called once per frame
    void Update()
    {
        timeBeforeSkippingScoreScreenTimer += Time.deltaTime;
        if (finalScoreScreen.activeSelf && timeBeforeSkippingScoreScreenTimer > timeBeforeSkippingScoreScreen && Input.GetMouseButtonDown(0))
        {
            NextMission();
        }
    }

    public void ShowScoreScreen()
    {
        FindFirstObjectByType<dialogue>().gameObject.SetActive(false);
        FindFirstObjectByType<FirstPersonController>().finalScreen();
        // Only show score screen if not already active
        if (finalScoreScreen.activeSelf)
            return;

        imageComparer.Compare();
        scoreText.text = imageComparer.score.ToString("F2");
        finalScoreScreen.SetActive(true);

        if (activeMission >= missionTextures.Length - 1)
        {
            thanksForPlayingText.gameObject.SetActive(true);
        }
        timeBeforeSkippingScoreScreenTimer = 0;
    }

    public void NextMission()
    {
        if (activeMission < missionTextures.Length - 1)
        {
            activeMission++;
            finalScoreScreen.SetActive(false);
            imageComparer.referenceTexture = missionTextures[activeMission];
            
            //FindAnyObjectByType<SolutionPreview>().UpdatePreview();

            // Reload scene
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

        }
        else
        {
            // Completed all missions
        }

        
    }
}

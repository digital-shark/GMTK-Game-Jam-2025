using UnityEngine;
using UnityEngine.UI;

public class SolutionPreview : MonoBehaviour
{
    [SerializeField]
    RawImage solutionPreview;

    private void Start()
    {
        UpdatePreview();
    }

    public void UpdatePreview()
    {
        solutionPreview.texture = GameManager.instance.missionTextures[GameManager.instance.activeMission];

    }
}

using UnityEngine;
using UnityEngine.UI;

public class Fader : MonoBehaviour
{
    public float target;
    public float speed;

    private RawImage blackScreen;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        blackScreen = GetComponent<RawImage>();   
    }

    // Update is called once per frame
    void Update()
    {
        if (blackScreen.color.a != target)
        {
            Color temp = blackScreen.color;

            if (blackScreen.color.a > target)
            {
                temp.a -= Mathf.Min(speed, blackScreen.color.a - target);

            }
            else if (blackScreen.color.a < target)
            {
                temp.a += Mathf.Min(speed, target - blackScreen.color.a);
            }

            blackScreen.color = temp;
        }
    }
}

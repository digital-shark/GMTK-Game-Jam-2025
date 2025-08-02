using UnityEngine;

public class rotatingDial : MonoBehaviour
{
    public Vector3 maxPos;
    public Vector3 minPos;

    public float sliderValue;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.localRotation = Quaternion.Euler(Vector3.Lerp(minPos, maxPos, sliderValue));
    }
}

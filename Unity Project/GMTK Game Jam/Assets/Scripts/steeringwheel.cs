using UnityEngine;

public class steeringwheel : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public Vector3 currentInput;

    public float currentRotation = 0;
    public float rotationSpeed = 5;
    public float resetSpeed = 5;
    public float maxRotation = 110;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (currentInput.z != 0)
        {
            float target = currentRotation + currentInput.z * rotationSpeed;
            currentRotation = Mathf.Clamp(target, -maxRotation, maxRotation);
        }
        else
        {
            if (currentRotation > 0)
                currentRotation -= Mathf.Min(resetSpeed, Mathf.Abs(currentRotation));
            else if (currentRotation < 0)
                currentRotation += Mathf.Min(resetSpeed, Mathf.Abs(currentRotation));
        }

        transform.localRotation = Quaternion.Euler(0, 0, currentRotation);
    }
}

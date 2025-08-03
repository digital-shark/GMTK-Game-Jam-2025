using UnityEngine;

public class spinning : MonoBehaviour
{
    public bool switchToFastProp;

    public float rotateSpeedTarget;
    public float rotateSpeed;
    public float rotateSpeedStep;


    public GameObject normalProp;
    public GameObject fastProp;



    // Update is called once per frame
    void Update()
    {
        GameObject target = rotateSpeed > 60 ? fastProp : normalProp;
        normalProp.SetActive(rotateSpeed <= 60);
        fastProp.SetActive(rotateSpeed > 60);

        if (rotateSpeed < rotateSpeedTarget)
            rotateSpeed += rotateSpeedStep;

        if (rotateSpeed > rotateSpeedTarget)
            rotateSpeed -= rotateSpeedStep;


        target.transform.Rotate(0, 0, rotateSpeed > 60 ? rotateSpeed / 20 : rotateSpeed);
    }
}

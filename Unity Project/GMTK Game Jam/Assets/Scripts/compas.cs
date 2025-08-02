using UnityEngine;

public class compas : MonoBehaviour
{

    public Transform target;
    public Transform dialPivot;


    // Update is called once per frame
    void Update()
    {
        Vector3 direction = Vector3.ProjectOnPlane(target.forward, Vector3.up).normalized;

        float radians = Mathf.Atan2(-direction.x, direction.z);
        float angle = 180 * radians / Mathf.PI;

        dialPivot.localRotation = Quaternion.Euler(0, 0, angle);
    }
}

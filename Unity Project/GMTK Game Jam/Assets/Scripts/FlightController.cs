using UnityEngine;
using UnityEngine.InputSystem;

public class FlightController : MonoBehaviour
{
    simplePlane plane;
    FirstPersonController cameraController;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        plane = GetComponent<simplePlane>();
        cameraController = GetComponentInChildren<FirstPersonController>();
    }

    Vector3 controlInput;

    public void OnFlapsInput(InputAction.CallbackContext context)
    {
        if (plane == null) return;

        if (context.phase == InputActionPhase.Performed)
        {
            //plane.ToggleFlaps();
        }
    }
    public void SetThrottleInput(InputAction.CallbackContext context)
    {
        if (plane == null) return;

        plane.SetThrottleInput(context.ReadValue<float>());
    }

    public void OnRollPitchInput(InputAction.CallbackContext context)
    {
        if (plane == null) return;

        var input = context.ReadValue<Vector2>();
        controlInput = new Vector3(input.y, controlInput.y, -input.x);
    }

    public void OnYawInput(InputAction.CallbackContext context)
    {
        if (plane == null) return;

        var input = context.ReadValue<float>();
        controlInput = new Vector3(controlInput.x, input, controlInput.z);
    }

    public void OnMouseInput(InputAction.CallbackContext context) 
    {
        if (cameraController == null) return;

        var input = context.ReadValue<Vector2>();
        cameraController.mouseinput = input;
    }

    public void OnSmokeInput(InputAction.CallbackContext context)
    {
        if (plane == null) return;
        if (context.phase == InputActionPhase.Performed)
        {
            plane.SetSmokeInput(true);
        }
        else
        {
            plane.SetSmokeInput(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        plane.SetControlInput(controlInput);
    }
}

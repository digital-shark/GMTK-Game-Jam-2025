using UnityEngine;
using FMODUnity;
public class simplePlane : MonoBehaviour
{


    public float angleOfAttack;
    public float maxSpeed;
    public float currentSpeed;

    public float enginetorque;
    public AnimationCurve enginePowerCurve;

    public float targetThrust;
    public float throttlespeed;

    public float liftForce;
    public AnimationCurve AngleOfAttackCurve;

    public Transform nose;
    public Transform wings;
    public float noseForce;
    public float fakeGravity;

    public float pitchSpeed;
    public float rollSpeed;

    private Rigidbody rb;
    private Vector3 localVelocity;

    public EventReference clip;
    
    public float smokeDrainRate = 0.5f;
    public float amountOfSmoke = 100.0f;
    public ParticleSystem smokePS;

    private bool once = true;

    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (targetThrust == 0 && currentSpeed < 1) 
        {
            rb.linearVelocity = Vector3.zero;

        }

        if (once) 
        {
            AudioManager.instance.PlayOneshot(clip, transform.position);
            once = false;
        }

        updateThrust();
        //updateLift();
        updateHeavyNose();
        updatePitch();
        updateRoll();

        if (smokeTrailInput)
        {
            if (amountOfSmoke > 0)
            {
                amountOfSmoke -= smokeDrainRate * Time.deltaTime;
            }
            else
            {
                amountOfSmoke = 0;
                FindAnyObjectByType<GameManager>().ShowScoreScreen();
            }
        }
    }

    void updateHeavyNose() 
    {
        if(targetThrust == 0 && currentSpeed > 3)
            rb.AddForceAtPosition(Vector3.down * noseForce * (1 - Vector3.Dot(Vector3.down, rb.transform.forward)) * Mathf.Clamp((1 - (currentSpeed / maxSpeed)), 0, 1), nose.position);
        rb.AddRelativeForce(Vector3.down * fakeGravity * Mathf.Clamp((1 - (currentSpeed / maxSpeed)), 0, 1));

        Debug.DrawLine(nose.position, nose.position + Vector3.down * noseForce * (1 - Vector3.Dot(Vector3.down, rb.transform.forward)) * Mathf.Clamp((1 - (currentSpeed / maxSpeed)), 0 ,1), Color.green);
    }

    void updateLift() 
    {
        rb.AddForceAtPosition(controlInput.x * rb.transform.up * liftForce, nose.position);
        
    }

    void updatePitch() 
    {
        rb.AddRelativeTorque(new Vector3(controlInput.x, 0, 0) * pitchSpeed);
        rb.linearVelocity = rb.transform.forward * rb.linearVelocity.magnitude;
    }

    void updateRoll() 
    {
        rb.AddRelativeTorque(new Vector3(0, 0, controlInput.z) * rollSpeed);
        rb.angularDamping = (1 * (currentSpeed / maxSpeed) + 0.05f);
    }

    void updateThrust() 
    {
        currentSpeed = Vector3.Dot(rb.transform.forward, rb.linearVelocity);
        
        if(throttleInput != 0)
            targetThrust = Utils.MoveTo(targetThrust, throttleInput, throttlespeed, Time.deltaTime);

        float enginepower = enginePowerCurve.Evaluate(currentSpeed / maxSpeed);

        rb.AddRelativeForce(Vector3.forward * enginetorque * targetThrust * enginepower);
    }

    float throttleInput;
    Vector3 controlInput;
    bool smokeTrailInput;

    public void SetThrottleInput(float input)
    {
        //if (Dead) return;
        throttleInput = input;
    }

    public void SetControlInput(Vector3 input)
    {
        //if (Dead) return;
        controlInput = Vector3.ClampMagnitude(input, 1);
    }

    public void SetSmokeInput(bool input)
    {
        //if (Dead) return;
        smokeTrailInput = input;

        if (smokeTrailInput)
        {
            smokePS.Play();
        }
        else
        {
            smokePS.Stop();
        }
    }
}

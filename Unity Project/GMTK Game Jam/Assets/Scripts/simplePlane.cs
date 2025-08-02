using UnityEngine;
using FMODUnity;
using FMOD.Studio;
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
    private EventInstance EngineAudio;
    
    public float smokeDrainRate = 0.5f;
    public float amountOfSmoke = 100.0f;
    public ParticleSystem smokePS;

    public Interactable engineButton;
    public Interactable brakeLever;
    public Interactable smokeLever;
    public Interactable RadioTransmitter;

    public SliderDial smokeAmount;
    public rotatingDial throttle;
    public rotatingDial angleThousandHand;
    public rotatingDial angleHundredHand;

    bool once = true;

    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        EngineAudio = AudioManager.instance.createInstance(clip);
        EngineAudio.setParameterByName("RPM Pitch", 0);

        engineButton.buttonTimerFinished += startEngine;
    }

    void startEngine() 
    {
        EngineAudio.start();
    }

    // Update is called once per frame
    void Update()
    {
        if (targetThrust == 0 && currentSpeed < 1) 
        {
            rb.linearVelocity = Vector3.zero;

        }

        updateBrake();
        updateSmokeTrail();
        updateAltimeter();

        if (engineButton.buttonState == Interactable.STATE.DOWN)
        {
            once = false;


            updateThrust();
            //updateLift();
            updateHeavyNose();
            updatePitch();
            updateRoll();

        }
        else if (engineButton.buttonState == Interactable.STATE.UP && !once) 
        {
            targetThrust = 0;
            once = true;
            EngineAudio.setParameterByName("RPM Pitch", targetThrust * 100f);
            EngineAudio.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }

        if (RadioTransmitter.buttonState == Interactable.STATE.HELD) 
        {
            FindAnyObjectByType<GameManager>().ShowScoreScreen();
        }
    }

    void updateAltimeter() 
    {
        float currentAltitude = transform.position.y * 10;

        angleHundredHand.sliderValue = currentAltitude % 1000/1000;
        angleThousandHand.sliderValue = Mathf.Clamp(currentAltitude, 0, 10000) / 10000;


    }

    void updateSmokeTrail()
    {
        if (smokeLever.buttonState == Interactable.STATE.DOWN)
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
            smokePS.Play();
        }
        else 
        {
            smokePS.Stop();
        }

        smokeAmount.sliderValue = amountOfSmoke / 100.0f;

    }

    void updateBrake() 
    {
        if (brakeLever.buttonState == Interactable.STATE.DOWN)
            rb.linearDamping = 0.03f;
        else
            rb.linearDamping = 5;
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

        EngineAudio.setParameterByName("RPM Pitch", targetThrust * 100f);
        throttle.sliderValue = targetThrust;
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

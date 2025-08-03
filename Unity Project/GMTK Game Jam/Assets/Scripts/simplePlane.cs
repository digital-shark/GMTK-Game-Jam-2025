using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using System.Collections;
public class simplePlane : MonoBehaviour
{

    enum TutorialState 
    {
        START,
        CLICKEDRADIO,
        CLICKEDRADIO2,
        LOOKEDATTASK,
        STARTEDENGINE,
        RELEASEDBREAK,
        THROTTLEUP,
        FLYING,
        FLYTOBALLOON,
        RELEASESMOKE
    }

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
    public EventReference engineStart;
    public EventReference Smoke;
    private EventInstance SmokeEmitter;
    public EventReference CrashSound;
    private EventInstance EngineStartAudio; 
    private EventInstance EngineAudio;
    private EventInstance CrashAudio;

    public float smokeDrainRate = 0.5f;
    public float amountOfSmoke = 100.0f;
    public ParticleSystem smokePS;

    public Interactable engineButton;
    public Interactable brakeLever;
    public Interactable smokeLever;
    public Interactable RadioTransmitter;
    public Interactable paper;

    public SliderDial smokeAmount;
    public rotatingDial throttle;
    public rotatingDial angleThousandHand;
    public rotatingDial angleHundredHand;

    public spinning prop;

    public dialogue dialogueSystem;
    public bool finishedTutorial;

    TutorialState TUTORIAL = TutorialState.START;

    bool once = true;
    bool onceTwo = true;

    
    private void OnDestroy()
    {
        EngineAudio.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        SmokeEmitter.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        EngineStartAudio.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        CrashAudio.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        dialogueSystem = FindFirstObjectByType<dialogue>();

        if (!FindFirstObjectByType<GameManager>().playTutorial)
            finishedTutorial = true;
        else
            dialogueSystem.startDialogue();

        rb = GetComponent<Rigidbody>();

        EngineAudio = AudioManager.instance.createInstance(clip);
        EngineAudio.setParameterByName("RPM Pitch", 0);
        EngineAudio.setVolume(0.5f);

        EngineStartAudio = AudioManager.instance.createInstance(engineStart);
        EngineStartAudio.setVolume(0.5f);

        SmokeEmitter = AudioManager.instance.createInstance(Smoke);

        prop = GetComponentInChildren<spinning>();

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
        updateRadioTransmitter();
        updateTutorial();

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
            prop.rotateSpeedTarget = 0;
        }

        if (engineButton.buttonState == Interactable.STATE.HELD && onceTwo)
        {
            prop.rotateSpeedTarget = 100;
            onceTwo = false;
            EngineStartAudio.start();
        }
        else if (engineButton.buttonState == Interactable.STATE.DOWN)
        {
            onceTwo = true;
            EngineStartAudio.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }
        else if (engineButton.buttonState == Interactable.STATE.UP)
        {
            onceTwo = true;
            EngineStartAudio.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            prop.rotateSpeedTarget = 0;
        }


    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag != "Tutorial")
        {
            //We have crashed :(
            CrashAudio = AudioManager.instance.createInstance(CrashSound);
            CrashAudio.start();
            FindAnyObjectByType<Fader>().ToBlack();
            StartCoroutine(CrashCoroutine());
        }
    }

    IEnumerator CrashCoroutine()
    {
        yield return new WaitForSeconds(5);
        CrashAudio.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);

        FindFirstObjectByType<GameManager>().ShowScoreScreen();
        FindAnyObjectByType<Fader>().Fade();
    }

    void updateTutorial()
    {
        if (finishedTutorial)
            return;

        if (RadioTransmitter.buttonState == Interactable.STATE.HELD && TUTORIAL == TutorialState.START)
        {
            dialogueSystem.nextLine();
            TUTORIAL = TutorialState.CLICKEDRADIO;
        }

        if (paper.buttonState == Interactable.STATE.UP && TUTORIAL == TutorialState.CLICKEDRADIO)
        {
            dialogueSystem.nextLine();
            TUTORIAL = TutorialState.CLICKEDRADIO2;
        }

        if (engineButton.buttonState == Interactable.STATE.DOWN && TUTORIAL == TutorialState.CLICKEDRADIO2)
        {
            dialogueSystem.nextLine();
            TUTORIAL = TutorialState.STARTEDENGINE;
        }

        if (brakeLever.buttonState == Interactable.STATE.DOWN && TUTORIAL == TutorialState.STARTEDENGINE)
        {
            dialogueSystem.nextLine();
            TUTORIAL = TutorialState.RELEASEDBREAK;
        }

        if (targetThrust == 1 && TUTORIAL == TutorialState.RELEASEDBREAK)
        {
            dialogueSystem.nextLine();
            TUTORIAL = TutorialState.THROTTLEUP;
        }

        if (transform.position.y > 30 && TUTORIAL == TutorialState.THROTTLEUP)
        {
            dialogueSystem.nextLine();
            TUTORIAL = TutorialState.RELEASESMOKE;
        }

        if (smokeLever.buttonState == Interactable.STATE.DOWN && TUTORIAL == TutorialState.RELEASESMOKE)
        {
            dialogueSystem.nextLine();
            TUTORIAL = TutorialState.FLYTOBALLOON;
        }

        if (RadioTransmitter.buttonState == Interactable.STATE.HELD && TUTORIAL == TutorialState.FLYTOBALLOON) 
        {
            dialogueSystem.nextLine();
            finishedTutorial = true;
            FindAnyObjectByType<GameManager>().playTutorial = false;
        }
    }

    void updateRadioTransmitter() 
    {
        if (RadioTransmitter.buttonState == Interactable.STATE.HELD)
        {
            if(finishedTutorial)
                FindAnyObjectByType<GameManager>().ShowScoreScreen();
        }
    }

    void updateAltimeter() 
    {
        float currentAltitude = transform.position.y * 10;

        angleHundredHand.sliderValue = currentAltitude % 1000/1000;
        angleThousandHand.sliderValue = Mathf.Clamp(currentAltitude, 0, 10000) / 10000;


    }

    bool onceSmoke = true;

    void updateSmokeTrail()
    {
        if (smokeLever.buttonState == Interactable.STATE.DOWN)
        {
            if (amountOfSmoke > 0)
            {
                if (onceSmoke) 
                {
                    SmokeEmitter.start();
                    onceSmoke = false;
                }
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
            onceSmoke = true;
            SmokeEmitter.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            smokePS.Stop();
        }

        smokeAmount.sliderValue = amountOfSmoke / 100.0f;

    }

    void updateBrake() 
    {
        if (brakeLever.buttonState == Interactable.STATE.DOWN)
            rb.linearDamping = 0.05f;
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

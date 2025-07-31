using UnityEngine;

public class Plane : MonoBehaviour
{
    public Vector3 liftForceDebug;

    [SerializeField]
    float maxThrust;
    [SerializeField]
    float throttleSpeed;

    [Header("Lift")]
    [SerializeField]
    float liftPower;
    [SerializeField]
    AnimationCurve liftAOACurve;
    [SerializeField]
    float inducedDrag;
    [SerializeField]
    AnimationCurve inducedDragCurve;
    [SerializeField]
    float rudderPower;
    [SerializeField]
    AnimationCurve rudderAOACurve;
    [SerializeField]
    AnimationCurve rudderInducedDragCurve;
    [SerializeField]
    float flapsLiftPower;
    [SerializeField]
    float flapsAOABias;
    [SerializeField]
    float flapsDrag;
    [SerializeField]
    float flapsRetractSpeed;

    [Header("Steering")]
    [SerializeField]
    Vector3 turnSpeed;
    [SerializeField]
    Vector3 turnAcceleration;
    [SerializeField]
    AnimationCurve steeringCurve;


    [Header("Drag")]
    [SerializeField]
    AnimationCurve dragForward;
    [SerializeField]
    AnimationCurve dragBack;
    [SerializeField]
    AnimationCurve dragLeft;
    [SerializeField]
    AnimationCurve dragRight;
    [SerializeField]
    AnimationCurve dragTop;
    [SerializeField]
    AnimationCurve dragBottom;
    [SerializeField]
    Vector3 angularDrag;
    [SerializeField]
    float airbrakeDrag;

    [Header("Misc")]
    [SerializeField]
    bool flapsDeployed;

    public Rigidbody rb { get; private set; }
    public Vector3 velocity { get; private set; }
    public Vector3 localVelocity { get; private set; }
    public Vector3 localAngularVelocity { get; private set; }
    public Vector3 localGForce { get; private set; }
    public float angleOfAttack { get; private set; }
    public float angleOfAttackYaw { get; private set; }

    public float Throttle { get; private set; }
    public Vector3 EffectiveInput { get; private set; }

    float throttleInput;
    Vector3 controlInput;

    public bool AirbrakeDeployed { get; private set; }

    public bool FlapsDeployed
    {
        get
        {
            return flapsDeployed;
        }
        private set
        {
            flapsDeployed = value;
        }
    }


    Vector3 lastVelocity;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }
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

    private void FixedUpdate()
    {
        float dt = Time.fixedDeltaTime;

        CalculateState(dt);
        CalculateGForce(dt);

        UpdateThrottle(dt);

        UpdateThrust();
        UpdateLift();
        UpdateSteering(dt);
        UpdateDrag();

    }

    void CalculateState(float deltaTime)
    {
        Quaternion inverseRotation = Quaternion.Inverse(rb.rotation);
        velocity = rb.linearVelocity;
        localVelocity = inverseRotation * velocity;
        localAngularVelocity = inverseRotation * rb.angularVelocity;

        CalculateAngleOfAttack();
    }

    void CalculateAngleOfAttack()
    {
        if (localVelocity.sqrMagnitude < 0.1f)
        {
            angleOfAttack = 0;
            angleOfAttackYaw = 0;
            return;
        }

        angleOfAttack = Mathf.Atan2(-localVelocity.y, localVelocity.z);
        angleOfAttackYaw = Mathf.Atan2(localVelocity.x, localVelocity.z);
    }

    void CalculateGForce(float dt)
    {
        Quaternion inverseRotation = Quaternion.Inverse(rb.rotation);
        Vector3 acceleration = (velocity - lastVelocity) / dt;
        localGForce = inverseRotation * acceleration;
        lastVelocity = velocity;
    }

    void UpdateThrust()
    {
        rb.AddRelativeForce(Vector3.forward * Throttle * maxThrust);
    }

    void UpdateDrag()
    {
        Vector3 lv = localVelocity;
        float lv2 = lv.sqrMagnitude;  //velocity squared

        float airbrakeDrag = AirbrakeDeployed ? this.airbrakeDrag : 0;
        float flapsDrag = FlapsDeployed ? this.flapsDrag : 0;

        //calculate coefficient of drag depending on direction on velocity
        var coefficient = Utils.Scale6(
            lv.normalized,
            dragRight.Evaluate(Mathf.Abs(lv.x)), dragLeft.Evaluate(Mathf.Abs(lv.x)),
            dragTop.Evaluate(Mathf.Abs(lv.y)), dragBottom.Evaluate(Mathf.Abs(lv.y)),
            dragForward.Evaluate(Mathf.Abs(lv.z)) + airbrakeDrag + flapsDrag,   //include extra drag for forward coefficient
            dragBack.Evaluate(Mathf.Abs(lv.z))
        );

        var drag = coefficient.magnitude * lv2 * -lv.normalized;    //drag is opposite direction of velocity

        rb.AddRelativeForce(drag);
    }

    Vector3 CalculateLift(float angleOfAttack, Vector3 rightAxis, float liftPower, AnimationCurve aoaCurve, AnimationCurve inducedDragCurve)
    {
        Vector3 liftVelocity = Vector3.ProjectOnPlane(localVelocity, rightAxis);    //project velocity onto YZ plane
        float v2 = liftVelocity.sqrMagnitude;                                     //square of velocity

        //lift = velocity^2 * coefficient * liftPower
        //coefficient varies with AOA
        float liftCoefficient = aoaCurve.Evaluate(angleOfAttack * Mathf.Rad2Deg);
        float liftForce = v2 * liftCoefficient * liftPower;

        //lift is perpendicular to velocity
        Vector3 liftDirection = Vector3.Cross(liftVelocity.normalized, rightAxis);
        Vector3 lift = liftDirection * liftForce;

        //induced drag varies with square of lift coefficient
        float dragForce = liftCoefficient * liftCoefficient;
        Vector3 dragDirection = -liftVelocity.normalized;
        var inducedDrag = dragDirection * v2 * dragForce * this.inducedDrag * inducedDragCurve.Evaluate(Mathf.Max(0, localVelocity.z));

        return lift + inducedDrag;
    }

    void UpdateLift()
    {
        if (localVelocity.sqrMagnitude < 1f) return;

        float flapsLiftPower = FlapsDeployed ? this.flapsLiftPower : 0;
        float flapsAOABias = FlapsDeployed ? this.flapsAOABias : 0;

        var liftForce = CalculateLift(
            angleOfAttack + (flapsAOABias * Mathf.Deg2Rad), Vector3.right,
            liftPower + flapsLiftPower,
            liftAOACurve,
            inducedDragCurve
        );

        var yawForce = CalculateLift(angleOfAttackYaw, Vector3.up, rudderPower, rudderAOACurve, rudderInducedDragCurve);

        liftForceDebug = liftForce;

        rb.AddRelativeForce(liftForce);
        rb.AddRelativeForce(yawForce);
    }

    float CalculateSteering(float dt, float angularVelocity, float targetVelocity, float acceleration)
    {
        var error = targetVelocity - angularVelocity;
        var accel = acceleration * dt;
        return Mathf.Clamp(error, -accel, accel);
    }

    void UpdateSteering(float dt)
    {
        var speed = Mathf.Max(0, localVelocity.z);
        var steeringPower = steeringCurve.Evaluate(speed);

        //var gForceScaling = CalculateGLimiter(controlInput, turnSpeed * Mathf.Deg2Rad * steeringPower);

        var targetAV = Vector3.Scale(controlInput, turnSpeed * steeringPower /** gForceScaling*/);
        var av = localAngularVelocity * Mathf.Rad2Deg;

        var correction = new Vector3(
            CalculateSteering(dt, av.x, targetAV.x, turnAcceleration.x * steeringPower),
            CalculateSteering(dt, av.y, targetAV.y, turnAcceleration.y * steeringPower),
            CalculateSteering(dt, av.z, targetAV.z, turnAcceleration.z * steeringPower)
        );

        rb.AddRelativeTorque(correction * Mathf.Deg2Rad, ForceMode.VelocityChange);    //ignore rigidbody mass

        var correctionInput = new Vector3(
            Mathf.Clamp((targetAV.x - av.x) / turnAcceleration.x, -1, 1),
            Mathf.Clamp((targetAV.y - av.y) / turnAcceleration.y, -1, 1),
            Mathf.Clamp((targetAV.z - av.z) / turnAcceleration.z, -1, 1)
        );

        var effectiveInput = (correctionInput + controlInput); //* gForceScaling;

        EffectiveInput = new Vector3(
            Mathf.Clamp(effectiveInput.x, -1, 1),
            Mathf.Clamp(effectiveInput.y, -1, 1),
            Mathf.Clamp(effectiveInput.z, -1, 1)
        );
    }

    void UpdateThrottle(float dt)
    {
        float target = 0;
        if (throttleInput > 0) target = 1;

        //throttle input is [-1, 1]
        //throttle is [0, 1]
        Throttle = Utils.MoveTo(Throttle, target, throttleSpeed * Mathf.Abs(throttleInput), dt);

        AirbrakeDeployed = Throttle == 0 && throttleInput == -1;

        //if (AirbrakeDeployed)
        //{
        //    foreach (var lg in landingGear)
        //    {
        //        lg.sharedMaterial = landingGearBrakesMaterial;
        //    }
        //}
        //else
        //{
        //    foreach (var lg in landingGear)
        //    {
        //        lg.sharedMaterial = landingGearDefaultMaterial;
        //    }
        //}
    }

    public void ToggleFlaps()
    {
        if (localVelocity.z < flapsRetractSpeed)
        {
            FlapsDeployed = !FlapsDeployed;
        }
    }
}

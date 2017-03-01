using UnityEngine;
using System.Collections;
using Utils;

public class AIGoTo : AIBase {

    //public Transform Target;

    protected AITarget mTarget;

    public float RotationSpeed;
    public float RotationThreshold;

    /// <summary>
    /// How quickly the AI unit accelerates towards the target
    /// </summary>
    [Tooltip("How quickly the AI unit accelerates towards the target")]
    public float Acceleration; // Meters^-2s

    /// <summary>
    /// Speed at which the AI Unit travels
    /// </summary>
    [Tooltip("Speed at which the AI Unit travels")]
    public float Speed;

    /// <summary>
    /// Max distance the unit is happy to sit at on arrival
    /// </summary>
    [Tooltip("Max distance the unit is happy to sit at on arrival")]
    public float ArrivalMaxDistance;

    // TODO: Move the min distance code from TestAI script
    /// <summary>
    /// Minimum distance the unit is happy to sit at before moving away from the target location
    /// </summary>
    [Tooltip("Minimum distance the unit is happy to sit at before moving away from the target location")]
    public float ArrivalMinDistance;

    public MovementCurve CurveType;

    private Rigidbody rb;

    private bool mArrived = false;

    /// <summary>
    /// Returns whether the unit is in arrived state or not
    /// </summary>
    public bool Arrived
    {
        get { return mArrived; }
        private set { mArrived = value; }
    }

    protected void OnEnable()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        mTarget = gameObject.GetComponent<AITarget>();

        
    }

    void InitValues()
    {
        Vector3 difference = transform.position - mTarget.Target.transform.position;
        //Debug.Log("pos = " + transform.position + " targetpos " + mTarget.Target.transform.position + " mTarget "+ mTarget.Target.name);
        StartValue = difference.sqrMagnitude;
        //Logger.LogDebug("Start value = " + StartValue);
    }

    protected float StartValue = -1;

    public float GetPercentDistane()
    {
        Vector3 difference = transform.position - mTarget.Target.transform.position;

        float rv = difference.sqrMagnitude / StartValue;

        return rv;
    }

    // Cached vars
    Vector3 Difference;
    float speedMod = 1;
    float angle;
    float CurrentSpeed;
    float currentRotation;
    float rotation;
    Vector3 direction;
    float mag;


    public bool UseWhiskers = true;

    public float DebugSpeedView;
    public float DebugDistView;
    public EaseType MovementType;
    public float GetSpeed()
    {

        //float speed = Speed* MovementSpeed.CalculateAcceleration(CurveType, (1-GetPercentDistane()) );
        //float speed = MovementSpeed.CalculateAcceleration(CurveType, (1 - GetPercentDistane()));
        //float speed = Ease.GetValue(MovementType, (GetPercentDistane()));
        float speed = 1;// Ease.GetValue(MovementType, (GetPercentDistane()));
        DebugSpeedView = speed;
        speed = Mathf.Clamp(speed + 0.2f, 0, 1);
        //Logger.LogDebug("speed = " + speed + " ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^");
        
        DebugDistView = GetPercentDistane();
        return speed;
    }

    void GoToPoint(Vector3 Point, float _maxRange, float _minRange)
    {

        Difference = Point - transform.position;

        mag = Difference.sqrMagnitude;
        _maxRange = _maxRange * _maxRange;
        _minRange = _minRange * _minRange;
        if (mag > _maxRange)
        {
            //direction = Difference.normalized * Speed;
            direction = Difference.normalized * Speed * GetSpeed();
            
        }
        else
        {
            direction = Vector3.zero;
        }
        

        rb.velocity = direction;
    }

    protected void FixedUpdate()
    {
        if (mTarget.HasTarget() == false)
        {
            return;
        }

        if (StartValue < 0)
            InitValues();

        GoToPoint(mTarget.Target.transform.position, ArrivalMaxDistance, ArrivalMinDistance);


    }


    void OnGUI()
    {
    }

    /// <summary>
    /// Sets the AI unit to arrived state
    /// </summary>
    private void SetArrived()
    {
        Arrived = true;
        
    }
}

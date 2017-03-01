using UnityEngine;

public enum MovementCurve
{
    Linear,
    AcclerateExponential,
    DecelerateExponential,
    SCurve,
}

public class MovementSpeed : MonoBehaviour {

    public MovementCurve Type;

    public static float CalculateAcceleration(MovementCurve _type, float _percent)
    {
        float acceleration = 0;
        switch (_type)
        {
            case MovementCurve.AcclerateExponential:
                acceleration = CalculateExponentialCurveAcceleration(_percent);
                break;
            case MovementCurve.DecelerateExponential:
                acceleration = CalculateExponentialCurveDeceleration(_percent);
                break;
            case MovementCurve.SCurve:
                acceleration = CalculateSCurveAcceleration(_percent);
                break;
            case MovementCurve.Linear:
                acceleration = _percent;
                break;
        }

        return acceleration;
    }

    public float CalculateAcceleration(float _percent)
    {
        float acceleration = 0;
        switch (Type)
        {
            case MovementCurve.AcclerateExponential:
                acceleration = CalculateExponentialCurveAcceleration(_percent);
                break;
            case MovementCurve.DecelerateExponential:
                acceleration = CalculateExponentialCurveDeceleration(_percent);
                break;
            case MovementCurve.SCurve:
                acceleration = CalculateSCurveAcceleration(_percent);
                break;
            case MovementCurve.Linear:
                acceleration = _percent;
                break;
        }

        return acceleration;
    }

    void OnEnable()
    {
        CalculateSCurveAcceleration(0.0f);
        CalculateSCurveAcceleration(0.1f);
        CalculateSCurveAcceleration(0.2f);
        CalculateSCurveAcceleration(0.3f);
        CalculateSCurveAcceleration(0.4f);
        CalculateSCurveAcceleration(0.5f);
        CalculateSCurveAcceleration(0.6f);
        CalculateSCurveAcceleration(0.7f);
        CalculateSCurveAcceleration(0.8f);
        CalculateSCurveAcceleration(0.9f);
        CalculateSCurveAcceleration(1f);
    }

    public static float CalculateExponentialCurveAcceleration(float _percentComplete)
    {
        float x = _percentComplete * 10;
        x = (x * x / 100);
        // a = x ^ 2
        
        return x;
    }

    /// <summary>
    /// Returns a value between 0-1 inclusive based on a point along a curve the distance
    /// along which is the value provided (0-1)
    /// </summary>
    /// <param name="_percentComplete"></param>
    /// <returns></returns>
    public static float CalculateExponentialCurveDeceleration(float _percentComplete)
    {
        float x = (_percentComplete) * 10;
        x = ((10 - x)*(10 - x)/100);
        
        return x;
    }

    public static float CalculateSCurveAcceleration(float _percentComplete)
    {
        float x = _percentComplete * 10;
        x = (Mathf.Pow((x - 5), 3) + 125f)/250f;
        // a = (x - 5) ^ 3 + 125
        
        return _percentComplete;
    }


}

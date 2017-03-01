using UnityEngine;
using System.Collections;

public enum Axis
{
    X,
    Y,
    Z,
}

public class ZigZag : MonoBehaviour {

    [Tooltip("Distance from the parent object")]
    public float MaxOffset;
    [Tooltip("How many oscillations per second")]
    public float Rate;

    public Transform ChildObject;

    public Axis Axis;

    float position;
    Vector3 CurrentPos;
    Vector3 TargetPos;

    void OnEnable()
    {
        //Debug.Log("CAlling ease ((((((((((((((((((");
        //Ease.GetValue(EaseType.DebugFunction, 5.5f);
        CurrentPos = ChildObject.transform.localPosition;
        //Debug.Log("------- pos = " + transform.position);
        //ChildObject.transform.position = CurrentPos;


    }

    private float time = 0;
    public EaseType MovementType;
    bool down = false;

    void Update()
    {

        if (time > 1 / Rate)
            down = true;
        else if (time < 0)
            down = false;

        if (down)
            time -= Time.deltaTime;
        else
            time += Time.deltaTime;

        position = time / (1 / Rate);

        TargetPos = ChildObject.transform.localPosition;

        switch (Axis)
        {
            case Axis.X:
                //TargetPos.x += (position * 2 * MaxOffset) - MaxOffset; ;
                //TargetPos.x = easeInOutCubic(-MaxOffset, MaxOffset, position);
			TargetPos.x = Ease.GetValue(MovementType, position, -MaxOffset, MaxOffset);
                break;
            case Axis.Y:
			TargetPos.y = Ease.GetValue(MovementType, position, -MaxOffset, MaxOffset);
                break;
            case Axis.Z:
			TargetPos.z = Ease.GetValue(MovementType, position, -MaxOffset, MaxOffset);
                break;
        }

        ChildObject.transform.localPosition = TargetPos;

    }

    private float easeOutCubic(float start, float end, float value)
    {
        value--;
        end -= start;
        return end * (value * value * value + 1) + start;
    }

    private float easeInOutQuart(float start, float end, float value)
    {
        value /= .5f;
        end -= start;
        if (value < 1) return end * 0.5f * value * value * value * value + start;
        value -= 2;
        return -end * 0.5f * (value * value * value * value - 2) + start;
    }

    private float easeInOutCubic(float start, float end, float value)
    {
        value /= .5f;
        end -= start;
        if (value < 1) return end * 0.5f * value * value * value + start;
        value -= 2;
        return end * 0.5f * (value * value * value + 2) + start;
    }
}

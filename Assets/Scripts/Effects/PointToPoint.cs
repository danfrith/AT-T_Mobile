using UnityEngine;
using System.Collections;

public class PointToPoint : MonoBehaviour {

    public Transform[] Points;

    public float Speed;

    [Tooltip("Destroys the object when it completes it's path")]
    public bool DeleteOnComplete = false;

    [Tooltip("Return to the start and repeat the path")]
    public bool Looping = false;

    [Tooltip("Makes the object reverse it's path (only does this once if not looping")]
    public bool PingPong = false;
    
    public void Start()
    {
        transform.position = Points[0].position;

        StartCoroutine(Translate());
    }

    // Cached vars
    Vector3 Velocity;
    Vector3 TargetPos;
    Vector3 Pos2;

    public bool Paused;

    public void SetPosition(float _percent)
    {

        // TODO: Extend this to work out where 0-1 is in the entire path, instead of just the current
        // two traversed points

        Vector3 v = Points[1].position - Points[0].position;
        
        v *= _percent;

        transform.position = Points[0].position + v;
        
    }

    IEnumerator Translate()
    {
        NextPoint();

        while (!ReachedEnd() && !Paused)
        {
            yield return new WaitForFixedUpdate();

            TranslateToPoint();
        }

        if (DeleteOnComplete)
            Destroy(this.gameObject);
    }

    public void TranslateToPoint()
    {
        CalcPosition(TargetPos);
        if (transform.position == TargetPos)
            NextPoint();
    }

    public int pointNo = 0;

    public bool ReachedEnd()
    {
        if (pointNo > Points.Length)
        {
            //Debug.Log("Reached end ");
            if (Looping)
            {
                pointNo = 1;
                transform.position = Points[0].position;
                return false;
            }
            return true;
        }
        else
            return false;
    }

    public void NextPoint()
    {
        //Debug.Log("Getting next point " + pointNo);
        pointNo++;
        if (pointNo > Points.Length)
        {
            // We are at the end
        }
        else
        {
            
            TargetPos = Points[pointNo-1].position;
        }
        
    }

    public void CalcPosition(Vector3 TargetPos)
    {
        // Get difference vector
        // Get normalized diff vector
        // If normalized * speed magnitude is larger than diff magnitude 
        //      set position to destination (otherwise we overshoot)
        // else
        //      set position to current + normalized * speed 

        Vector3 diff = TargetPos - transform.position;
        Vector3 diffNormalized = diff.normalized;

        Velocity = diffNormalized * Speed * Time.fixedDeltaTime;

        if (diff.sqrMagnitude < Velocity.sqrMagnitude)
            transform.position = TargetPos;
        else
            transform.position = transform.position + Velocity;

    }

}

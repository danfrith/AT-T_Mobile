using UnityEngine;
using System.Collections;

/// <summary>
/// Simple script that looks for a game object of a specific tag to target. Sends an event when the target changes.
/// 
/// </summary>
public class AITarget : AIBase {

    private bool isValid = false;

    public Transform mTarget;
    //private Transform mTarget;


    /// <summary>
    /// Get set the AI unit's target. Triggers TargetChangedEvent on set
    /// </summary>
    public Transform Target
    {
        get 
        {
            if (isValid == true && mTarget == null)
            {
                isValid = false;
                Target = null;
            }
            return mTarget; 
        }
        set 
        {
            mTarget = value;
            if (mTarget != null)
                isValid = true;

            if (TargetChangedEvent != null)
                TargetChangedEvent();
        } 
    }

    /// <summary>
    /// Removes the target from the AI unit if the target dead event is recieved
    /// </summary>
    /// <param name="_damageEventArgs">type DamageEventArgs</param>
    void Target_DeadEvent(System.EventArgs _damageEventArgs)
    {
        Target = null;    
    }

    /// <summary>
    /// Tag that targeter searches for
    /// </summary>
    [Tooltip("Tag that targeter searches for")]
    public string ObjectTag;

    void OnEnable()
    {
        if (mTarget == null)
            StartCoroutine(AquireTargetWithTag(ObjectTag));

        TargetChangedEvent += new TargetChanged(AITarget_TargetChangedEvent);
    }

	void GetTarget()
	{
		StartCoroutine(AquireTargetWithTag(ObjectTag));
	}

    /// <summary>
    /// Begins aquiring a new target if the previously aquired target was null
    /// </summary>
    void AITarget_TargetChangedEvent()
    {
        //Debug.Log("My target changed -----------------");
        if (mTarget == null)
            StartCoroutine(AquireTargetWithTag(ObjectTag));
    }

    /// <summary>
    /// Find's the first target in the game object list with specified tag
    /// </summary>
    /// <param name="_tag">Object tag of target</param>
    /// <returns></returns>
    IEnumerator AquireTargetWithTag(string _tag)
    {
        while (mTarget == null)
        {
            GameObject[] g = GameObject.FindGameObjectsWithTag(_tag);
			if (g.Length > 0) {
				if (g [0] == this.gameObject && g.Length > 1) { // Don't allow the object to target itself.
					mTarget = g [1].transform;
					isValid = true;
				} else if (g [0] != this.gameObject) {
					mTarget = g [0].transform;
					isValid = true;
				}
				else
					mTarget = null;
					
			}
            
            yield return new WaitForSeconds(0.5f);
        }
    }

    public delegate void TargetChanged();
    public event TargetChanged TargetChangedEvent;

    /// <summary>
    /// Returns whether the unit currently 
    /// </summary>
    /// <returns>bool</returns>
    public bool HasTarget()
    {
        bool rv = (Target != null);
        return rv;
    }
}

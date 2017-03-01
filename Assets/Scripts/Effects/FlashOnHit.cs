using UnityEngine;
using System.Collections;

/* Causes the object to flash when hit (requires shader with "_TintColor") */
[RequireComponent(typeof(Flash))]
public class FlashOnHit : MonoBehaviour {

    private Flash mFlash; 

    void OnEnable()
    {
        mFlash = gameObject.GetComponent<Flash>();
        if (mFlash == null)
        {
            Debug.LogError("Flash component is missing on " + name);
            return;
        }
        return;

    }

    // Trigger the flash
    void OnCollisionEnter2D(Collision2D _collision)
    {
        mFlash.StartFlash();
        //StartCoroutine(Flash());
    }

    void OnCollisionEnter(Collision _collision)
    {
        mFlash.StartFlash();
        //StartCoroutine(Flash());
    }

}

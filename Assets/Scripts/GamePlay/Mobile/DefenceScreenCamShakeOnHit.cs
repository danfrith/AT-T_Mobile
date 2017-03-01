using UnityEngine;
using System.Collections;

public class DefenceScreenCamShakeOnHit : MonoBehaviour {

    public CameraFollow mCamera;
    
    public float Strength = 0.3f;
    public float Duration = 0.4f;

    void Start()
    {
        SetObjectToFrustrum();
    }

    public bool test = false;

    public Camera cam;
    public float ScreenDistance = 2.61f;
    public void SetObjectToFrustrum()
    {
        Debug.Log("Setting object to frustrum size");

        //cam = Camera.main;

        float pos = (cam.nearClipPlane + ScreenDistance);

        transform.position = cam.transform.position + cam.transform.forward * pos;

        float h = Mathf.Tan(cam.fov * Mathf.Deg2Rad * 0.5f) * pos * 2f + 0.5f;

        transform.localScale = new Vector3(h * cam.aspect + 0.4f, h, 0f);
    }

    public void Update()
    {
        if (test == true)
        {
            test = false;
            SetObjectToFrustrum();
        }
    }
    // Trigger the flash
    void OnCollisionEnter2D(Collision2D _collision)
    {
        mCamera.StartScreenShake(Strength, Duration);
        //StartCoroutine(Flash());
    }

    void OnCollisionEnter(Collision _collision)
    {
        mCamera.StartScreenShake(Strength, Duration);
        //StartCoroutine(Flash());
    }

}

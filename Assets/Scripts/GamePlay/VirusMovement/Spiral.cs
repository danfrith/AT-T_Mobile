using UnityEngine;


public class Spiral : MonoBehaviour {

    [Tooltip("Distance from the parent object")]
    public float Radius;
    [Tooltip("How fast the object spins")]
    public float RotationSpeed;

    [Tooltip("Rate at which the spiral radius decreases")]
    public float Decay;

    public Transform ChildObject;


    protected Vector3 Rotation;
    void OnEnable()
    {
        Vector3 CurrentPos = transform.position;
        CurrentPos.x += Radius;

        ChildObject.transform.position = CurrentPos;

        Rotation = new Vector3(0, 0, RotationSpeed);
    }

    void Update()
    {
        gameObject.transform.Rotate(Rotation);
    }


}

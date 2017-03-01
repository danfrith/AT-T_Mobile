using UnityEngine;
using System.Collections;

public class GlowEditScript : MonoBehaviour {

    private MeshRenderer mRenderer;

	void OnEnable()
    {
        mRenderer = GetComponent<MeshRenderer>();

		SetColour (0.0f);
    }

	void SetColour(float _colour)
	{
		if (mRenderer.material.HasProperty(PropertyName))
			Debug.LogError("GlowEditScript: Property " + PropertyName + " does not exist on object " + name);
		else
			Debug.LogError("GlowEditScript:Property exists _MKGlowPower does not exist on object " + name);
		mRenderer.material.SetFloat(PropertyName, _colour);

	}
    public bool Test;
    public float Value = 0.5f;
    public string PropertyName;
    void Update()
    {
        if (Test)
        {
            Test = false;
			SetColour (Value);
            
        }
    }

}

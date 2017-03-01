using UnityEngine;
using System.Collections;

public class ModelController : MonoBehaviour {

    public MeshRenderer TriggerRenderer;
    public Material HighlightMaterial;

    public Transform Model;
    public Transform Shield;

    private Material mOriginalMaterial;

    void OnEnable()
    {
        StartCoroutine(GetTrigger());
    }

    IEnumerator GetTrigger()
    {
        yield return new WaitForSeconds(1);

        Transform t = Model.FindChild("trigger");
        if (t == null)
            Debug.LogError("ModelController: Trigger was not found in Model " + name);

        TriggerRenderer = t.GetComponent<MeshRenderer>();

        if (TriggerRenderer == null)
            Debug.LogError("ModelController: Mesh renderer component not found on trigger for " + name);

        mOriginalMaterial = TriggerRenderer.material;

        Debug.Log("stage name " + Stage.Instance.StageName);
        if (Stage.Instance.StageName == "TutorialLevel" || Stage.Instance.StageName == "TutorialStage")
            EnableModel();
        else
            DisableModel();
    }

    public void DisableModel()
    {
        Model.gameObject.SetActive(false);
        Shield.gameObject.SetActive(true);
    }

    public void HideModel()
    {
        Model.gameObject.SetActive(false);
    }

    public void EnableModel()
    {
        Model.gameObject.SetActive(true);
        Shield.gameObject.SetActive(false);
    }

    public void SwapToGlow()
    {
        if (TriggerRenderer != null)
            TriggerRenderer.material = HighlightMaterial;
    }

    public void SwapToStandard()
    {
        if (TriggerRenderer != null)
            TriggerRenderer.material = mOriginalMaterial;
    }

	public void TriggerHitAnimation()
	{
		Flash f = Shield.GetComponent<Flash> ();
		f.StartFlash ();
	}

	private Vector3 OriginalScale;

	public float hapticDuration = 1.5f;
	public void EnableShieldAnimated()
	{
		Model.gameObject.SetActive(false);
		OriginalScale = Shield.transform.localScale;
		Shield.gameObject.SetActive(true);

		VRGripper.GetGripper (transform).HapticVibrationEaseStrength( 0.5f, MovementType, hapticDuration);
		//VRGripper.GetGrippers () [1].HapticVibration (0.5f, 2);
			
		StartCoroutine (ScaleRoutine ());
	}

	float ScaleTime = 2;
	public EaseType MovementType;

	IEnumerator ScaleRoutine()
	{
		float time = ScaleTime;

		Shield.transform.localScale = new Vector3 (0.001f, 0.001f, 0.001f);
		Vector3 newScale = Vector3.zero;

		//sffls
		while (time > 0) {
			newScale = Ease.GetValue (MovementType,1- ( time / ScaleTime) )*OriginalScale;
			time -= Time.fixedDeltaTime;

			Shield.transform.localScale = newScale;

			yield return new WaitForFixedUpdate ();
		}


	}
    public float Power = 0.01f;
    IEnumerator ShieldHitAnimation()
    {
		Debug.Log ("Starting hit animation");
        float animTime = 1;
        float time = 0;
        MeshRenderer mr = Shield.GetComponent<MeshRenderer>();

        while (time < animTime)
        {

            yield return new WaitForFixedUpdate();
            
            time += Time.fixedDeltaTime;
			string varName = "_MKGlowPower";
            //Debug.Log("Float = " + mr.material.GetFloat(varName));
            mr.material.SetFloat(varName, time);
            
        }
    }

    public bool test = false;
    public bool test2 = false;
    void Update()
    {
        if (test == true)
        {
            test = false;
            StartCoroutine(ShieldHitAnimation());
        }

        if (test2 == true)
        {
            test2 = false;
            EnableModel();
        }
    }
}

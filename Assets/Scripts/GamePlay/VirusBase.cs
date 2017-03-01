using UnityEngine;
using System.Collections;

public enum VirusType
{
	Virus1,
	Virus2,
	Virus3,
	Virus4,
	Virus5,
	Virus6,

}

public class VirusBase : MonoBehaviour {

    public float PointsValue = 20;

    public Transform OnSpawnEffectPrefab;
    public Transform DestroyedByShieldEffectPrefab;
    public Transform DestroyedEffectPrefab;

	public VirusType Type;
	public Transform AudioClipPlayerPrefab;

    public GameObject VirusBody;

	public bool DisplayVirus = false;

    public void VirusDestroyed()
    {
        Destroy(gameObject);
        StartCoroutine(EnableVirus());

        // TODO: Add particle effects
    }

    float SpawnDelay = 0.55f;
    float EffectDelay = 0.05f;

    public void SeteffectDelay(float _t)
    {
        Debug.Log("effect delay set to " + _t + " for " + name);

        EffectDelay = _t;
    }

    IEnumerator EnableVirus()
    {
        yield return new WaitForSeconds(SpawnDelay);
        GetComponent<AIGoTo>().enabled = true;
        VirusBody.SetActive(true);

        Spiral sp = GetComponent<Spiral>();
        if (sp != null)
            sp.enabled = true;

        if (Type == VirusType.Virus1)
        {
            ZigZag z = transform.GetChild(0).GetChild(0).GetComponent<ZigZag>();

            if (z != null)
                z.enabled = true;
        }
    }

    void DisableVirus()
    {
        GetComponent<AIGoTo>().enabled = false;
        VirusBody.SetActive(false);


        Spiral sp = GetComponent<Spiral>();

        if (sp != null)
            sp.enabled = false;

        

    }

    void Start()
    {
		if (DisplayVirus == true) {
			Debug.LogWarning ("Disabling display virus");
			GetComponent<AIGoTo>().enabled = false;
			Spiral sp = GetComponent<Spiral>();

			GetComponent<Rigidbody> ().constraints = RigidbodyConstraints.FreezePosition;

			if (sp != null)
				sp.enabled = false;

            if (Type == VirusType.Virus1)
            {
                //Debug.LogWarning("Attempting disable zigzag");
                ZigZag z = transform.GetChild(0).GetChild(0).GetComponent<ZigZag>();

                if (z != null)
                    z.enabled = false;
                
            }

            return;
		}
		
        if (VirusBody == null)
            VirusBody = transform.GetChild(0).gameObject;

        DisableVirus();

        StartCoroutine(SpawnEffectRoutine());

    }

    IEnumerator SpawnEffectRoutine()
    {
        //Debug.Log("Delaying by " + EffectDelay + " for virus " + name);

        yield return new WaitForSeconds(EffectDelay);

        StartCoroutine(EnableVirus());

        if (OnSpawnEffectPrefab != null)
        {
            //Debug.Log("Spawned effect");
            Instantiate(OnSpawnEffectPrefab, transform.position, Quaternion.identity);
        }

		GameObject go = Resources.Load ("PlayClipPrefab") as GameObject;
		AudioClipPlayerPrefab = go.transform;

		if (AudioClipPlayerPrefab != null) { 
			Transform t  = (Transform)Instantiate(AudioClipPlayerPrefab, transform.position, Quaternion.identity);
			AudioClipPlayer p = t.GetComponent<AudioClipPlayer> ();
			p.PlayAndDestroy ("VirusSpawn0" + ((int)Type + 1).ToString(), 0.5f); //PlayClip
        }
    }

	/// <summary>
	/// Tells the level that this type of virus was spawned / destroyed for score tracking purposes.
	/// </summary>
	public void NotifyLevel()
	{
		
	}

	public void ShieldCollision(Collision _collision)
	{
		Stage.Instance.AddScore(PointsValue);

		Stage.Instance.SpawnedType (Type);
		Stage.Instance.HitType (Type);

		VRGripper v = _collision.transform.parent.gameObject.GetComponent<VRGripper> ();
		if (v != null) {
			v.HapticVibration (0.9f, 0.09f);
			ModelController mc = v.gameObject.GetComponent<ModelController> ();
			if (mc != null)
				mc.TriggerHitAnimation ();
		}

		Destroy(this.gameObject);

		if (DestroyedByShieldEffectPrefab!=null)
		{
			Vector3 normal = transform.position - _collision.transform.position;
			normal.Normalize();

			//Quaternion dir = Quaternion.FromToRotation(Vector3.up, -normal);
			Quaternion dir1 = Quaternion.FromToRotation(Vector3.forward, -normal);

			//dir.eulerAngles = _collision.collider.transform.position - transform.position;
			//Quaternion.FromToRotation(_collision.collider.transform.position, transform.position)

			Instantiate(DestroyedByShieldEffectPrefab, _collision.contacts[0].point, dir1);
		}

		if (AudioClipPlayerPrefab != null) {
			Transform t  = (Transform)Instantiate(AudioClipPlayerPrefab, transform.position, Quaternion.identity);
			AudioClipPlayer p = t.GetComponent<AudioClipPlayer> ();
			p.PlayAndDestroy ("VirusHitShield0" + ((int)Type + 1).ToString()); // Converts "Type" into the name
		}
	}

    public void MobileShieldScreenHit(MobileStage _stage)
    {
        _stage.OnVirusEscape();
        AndroidManager.HapticFeedback();
    }

    public void OnCollisionEnter(Collision _collision)
    {
        Logger.LogDebug("Collision occured " + name + " with "+ _collision.collider.name);
        //Debug.Log("VirusBase: Collision occured");

		if (_collision.collider.tag == "Target" || _collision.collider.tag == "DefenceWall" )
        {
            Destroy(this.gameObject);

            if (DestroyedEffectPrefab != null)
            {
                Instantiate(DestroyedEffectPrefab, _collision.contacts[0].point, Quaternion.identity);
            }

			Stage.Instance.SpawnedType (Type);

            if (AudioClipPlayerPrefab != null)
            {
                //PlayClip
                Quaternion dir = Quaternion.identity;

                //dir.eulerAngles = _collision.contacts [0].normal;
                Transform t = (Transform)Instantiate(AudioClipPlayerPrefab, _collision.contacts[0].point, Quaternion.identity);
                AudioClipPlayer p = t.GetComponent<AudioClipPlayer>();
                p.PlayAndDestroy("VirusHitScreen", 0.5f);

            }

            MobileStage ms = Stage.Instance as MobileStage;
            if (ms != null) // Do mobile stuff and return.
            {
                MobileShieldScreenHit(ms);
                return;
            }

			foreach (VRGripper gripper in VRGripper.GetGrippers()) {
				gripper.HapticVibration (0.09f, 0.05f);
			}

			

        }
        else if (_collision.collider.tag == "Shield")
        {
			ShieldCollision (_collision);
        }
		else if (_collision.collider.tag == "ShieldMobile")
		{
			ShieldImpactMobile (_collision);
		}
        
    }

	public void ShieldImpactMobile(Collision _collision)
	{
		Stage.Instance.AddScore(PointsValue);

		Stage.Instance.SpawnedType (Type);
		Stage.Instance.HitType (Type);

		Destroy(this.gameObject);

		if (DestroyedByShieldEffectPrefab!=null)
		{
			Vector3 normal = transform.position - _collision.transform.position;
			normal.Normalize();

			//Quaternion dir = Quaternion.FromToRotation(Vector3.up, -normal);
			Quaternion dir1 = Quaternion.FromToRotation(Vector3.forward, -normal);

			Instantiate(DestroyedByShieldEffectPrefab, _collision.contacts[0].point, dir1);
		}

		if (AudioClipPlayerPrefab != null) {
			Transform t  = (Transform)Instantiate(AudioClipPlayerPrefab, transform.position, Quaternion.identity);
			AudioClipPlayer p = t.GetComponent<AudioClipPlayer> ();
			p.PlayAndDestroy ("VirusHitShield0" + ((int)Type + 1).ToString()); // Converts "Type" into the name
		}

        AndroidManager.HapticFeedback();
    }

}


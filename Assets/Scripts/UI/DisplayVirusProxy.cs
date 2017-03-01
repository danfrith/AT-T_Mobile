using UnityEngine;
using System.Collections;

public class DisplayVirusProxy : MonoBehaviour {

	public Transform VirusPrefab;

	void OnEnable()
	{
		if (VirusPrefab == null) {
			Logger.LogError ("DisplayVirusProxy: VirusPrefab is not assigned in editor");
			return;
		}

		Transform t = (Transform) Instantiate (VirusPrefab, transform.position, Quaternion.identity, transform.parent);
		t.localScale = transform.localScale;
		t.position = transform.position;
		t.rotation = transform.rotation;
		VirusBase b = t.GetComponent<VirusBase> ();
		if (b == null) {
			Logger.LogError ("DisplayVirusProxy: The prefab is not a virus");
			return;
		}

		b.DisplayVirus = true;

		Destroy (gameObject);
	}


}

using UnityEngine;
using System.Collections;

public class InitSpawnPrefab : MonoBehaviour {

	public bool DestroyAfterSpawn = true;

	// TODO Add "parent as parent" option

	public Transform Prefab;
	void OnEnable()
	{
		if (Prefab == null) {
			Debug.LogError ("InitSpawnPrefab: prefab not set on " + name);
			return;
		}

		Instantiate (Prefab, transform.position, Quaternion.identity, transform);

		if (DestroyAfterSpawn)
			Destroy (gameObject);
	}
}

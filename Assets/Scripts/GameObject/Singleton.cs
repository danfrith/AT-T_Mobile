using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Singleton : MonoBehaviour {

	#region Singleton
	public static List<string> Instances = new List<string>();

	void Awake()
	{

		Debug.Log ("=========== Spawned singleton " + name);
		if (Instances.Contains(name) == true)
		{
			Debug.Log ("Other singkleton found");
			Destroy(this.gameObject);
			return;
		}
		Debug.Log ("first instance");

		Instances.Add (name);
		//DontDestroyOnLoad(gameObject);

	}

	void OnDestroy()
	{
		if (Instances.Contains (name) == true)
			Instances.Remove (name);
	}

	#endregion

}

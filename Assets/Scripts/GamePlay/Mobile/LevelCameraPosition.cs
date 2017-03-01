using UnityEngine;
using System.Collections;


// Sends it's details to level camera on start,
// It's in-wrorld position (transform) is used to set the level camera position
public class LevelCameraPosition : MonoBehaviour 
{
	public LevelEnum Level;
	public World World;

		// Notify LevelCamera

	void OnEnable()
	{
		Debug.Log ("Level camera enabled");
		GameObject go = GameObject.Find ("MobilePlayArea");
		LevelCamera c = null;
		if (go == null) {
			Debug.LogError ("LevelCameraPosition: Could not find MobileTargetPrefav");
			return;
		}

		c = go.GetComponent<LevelCamera> ();

		c.AddPosition (this);

		gameObject.SetActive (false);
			
	}
}

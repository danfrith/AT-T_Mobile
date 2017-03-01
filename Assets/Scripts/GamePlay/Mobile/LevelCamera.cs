using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Fills with a list of Camera positions
// Recieves a command from the level file to move to one of the positions
// uses the cinematic cam to move to that location
/// <summary>
/// TODO Figure out how to get move the camera before the level starts
/// </summary>
public class LevelCamera : MonoBehaviour {

	public List<LevelCameraPosition> Positions = new List<LevelCameraPosition>();

	public void AddPosition(LevelCameraPosition _pos)
	{
		Positions.Add (_pos);
	}

	void Start()
	{
		int levelIndex = MobileGameManager.Instance.CurrentLevel-1;
		int worldIndex = levelIndex / 5;
		int Level = levelIndex % 5;
        

        foreach (LevelCameraPosition pos in Positions) {

            if (pos.Level == (LevelEnum)Level && (World)worldIndex == pos.World) {
                
				SetPosition (pos);
			}

			Destroy (pos.gameObject,0.1f);
		}

	}

	void SetPosition(LevelCameraPosition _position) 
	{
		Debug.Log ("Set position to " + _position.Level.ToString () + " world " + _position.World.ToString ());
		transform.position = _position.transform.position;
		transform.rotation = _position.transform.rotation;
	}
}

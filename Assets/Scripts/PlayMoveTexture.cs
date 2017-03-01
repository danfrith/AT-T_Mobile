using UnityEngine;
using System.Collections;

public class PlayMoveTexture : MonoBehaviour {

	#if  WINDOWS || UNITY_EDITOR
    public MovieTexture movTexture;
	void Start() {
		
		PlayMovieTexture ();
	}


	void PlayMovieTexture()
	{
		GetComponent<Renderer>().material.mainTexture = movTexture;
		movTexture.loop = true;
		movTexture.Play();
	}

#endif
}
using UnityEngine;
using System.Collections;

public class ResumeMusicOnSceneChange : MonoBehaviour {

    public static ResumeMusicOnSceneChange Instance;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

	void OnLevelWasLoaded()
	{
		AudioSource a = GetComponent<AudioSource> ();
		a.UnPause ();
	}
		
}

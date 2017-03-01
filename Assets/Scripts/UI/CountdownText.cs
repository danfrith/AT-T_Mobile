using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CountdownText : MonoBehaviour {

	private PulsingImage mPulseText;
	public int CountdownTime;

	private AudioClipPlayer Audio;
    public Sprite[] Images;

	void OnEnable()
	{
		mPulseText = GetComponent<PulsingImage> ();
        Audio = GetComponent<AudioClipPlayer>();
        

        if (Images.Length != CountdownTime)
        {
            Logger.LogError("CountdownText: Not enough countdown images");
            return;
        }

        StartCoroutine(Countdown(CountdownTime));

    }

	IEnumerator Countdown(int _t)
	{
		yield return new WaitForFixedUpdate (); // Delay a frame to let other components initialize.

		for (int i = 0; i < _t; i++) {
			mPulseText.SetText (Images[i]);
			Audio.PlayClip ("CountDown");
			yield return new WaitForSeconds (1);
		}

		GameObject go = Resources.Load ("PlayClipPrefab") as GameObject;
		Transform prefab = go.transform;

		if (prefab != null) {
			//PlayClip
			Transform t  = (Transform)Instantiate(prefab, transform.position, Quaternion.identity);
			AudioClipPlayer p = t.GetComponent<AudioClipPlayer> ();
			p.PlayAndDestroy ("GameStart");
		}

		Destroy (gameObject);
	}

}

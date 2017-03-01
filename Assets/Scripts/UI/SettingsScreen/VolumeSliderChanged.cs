using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class VolumeSliderChanged : MonoBehaviour {

	void OnEnable()
	{
		Slider s = GetComponent<Slider>();
		if (s == null)
		{
			Debug.LogError("VolumeSliderChanged: Slider was not found on "+ name);
			return;
		}

		AudioClipPlayer p = GetComponent<AudioClipPlayer> ();
		if (p == null) {
			Debug.Log ("VolumeSliderChanged: could not find AudioClipPlayer component on " + name);
			return;
		}

		s.value = p.GetMasterVolume ();
	}

	public void SliderChanged()
	{
		Slider s = GetComponent<Slider>();
		if (s == null)
		{
			Debug.LogError("VolumeSliderChanged: Slider was not found on "+ name);
			return;
		}

		AudioClipPlayer p = GetComponent<AudioClipPlayer> ();
		if (p == null) {
			Debug.Log ("VolumeSliderChanged: could not find AudioClipPlayer component on " + name);
			return;
		}

		if (s.value != p.GetMasterVolume ()) {
			p.SetMasterVolume (s.value);
		
			//p.PlayMain ();
			StartCoroutine(PlaySoundChangedRoutine(p));
		}
	}

	private bool isRunning = false;
	private bool newEntry = false;
	IEnumerator PlaySoundChangedRoutine(AudioClipPlayer _p)
	{
		if (isRunning == true) {
			newEntry = true;
			yield break;
		}
		isRunning = true;

		while (true) {

			yield return new WaitForSeconds (0.01f);
			if (newEntry == false) {
				if (_p.IsPlaying() == false) {
					_p.PlayMain ();
					isRunning = false;
					yield break;
				}
			} else {
				newEntry = false;
			}
		}
	}
}

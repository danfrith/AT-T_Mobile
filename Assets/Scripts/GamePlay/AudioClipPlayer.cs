using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class AudioClipPlayer : MonoBehaviour {

	public string ClipName;

	public bool PlayOnAwake = false;

	private AudioSource mPlayer;
	private float mLocalVolume;
	void OnEnable()
	{
		mPlayer = GetComponent<AudioSource> ();

		mLocalVolume = mPlayer.volume;
		mPlayer.volume = mLocalVolume * MasterVolume;

		if (PlayOnAwake == true)
			PlayMain ();
	}

	public static float MasterVolume = 1;


	public void SetMasterVolume(float _value)
	{
		if (_value < 0) {
			_value = 0;
		} else if (_value > 1) {
			_value = 1;
		}

		MasterVolume = _value;
	}

	public float GetMasterVolume()
	{
		return MasterVolume;
	}

	AudioClip GetClip()
	{
		return AudioList.GetClip (ClipName);
	}

	public AudioClip GetClip(string _clipName)
	{
		return AudioList.GetClip (_clipName);
	}

	public void SetClip(string _clip)
	{
		ClipName = _clip;
		
	}

	public void SetLooping(bool _isLooping)
	{
		mPlayer.loop = _isLooping;
	}

	public void PlayClip(string _clip)
	{
		AudioClip clip = GetClip (_clip);

		if (clip != null)
			mPlayer.PlayOneShot (clip, MasterVolume);

	}

    public void PlayClipDelayed(string _clip, float _delay)
    {
        StartCoroutine(DelayPlayClip(_clip, _delay));
    }

    IEnumerator DelayPlayClip(string _clip, float _seconds)
    {
        yield return new WaitForSeconds(_seconds);
        PlayClip(_clip);
    }

	public void Stop()
	{
		mPlayer.Stop ();
	}

	public void PlayAndDestroy(string _clipName)
	{
		PlayAndDestroy (_clipName, 1);
	}

	public void PlayAndDestroy(string _clipName, float _volume)
	{
		AudioClip clip = GetClip(_clipName);

		mPlayer.clip = clip;

		if (clip != null) {
			Destroy (this.gameObject, clip.length + 0.02f);
			mPlayer.volume = _volume * MasterVolume;
			mPlayer.Play ();
		}
	}

    public void PlayAndDestroyDelayed(string _clipName, float _volume, float _delayTime)
    {
        StartCoroutine(DelayPlayClipAndDestroy(_clipName, _volume, _delayTime));
    }

    IEnumerator DelayPlayClipAndDestroy(string _clipName, float _volume, float _seconds)
    {
        yield return new WaitForSeconds(_seconds);

        AudioClip clip = GetClip(_clipName);

        mPlayer.clip = clip;
        mPlayer.volume = _volume * MasterVolume;
        mPlayer.Play();

        Destroy(this.gameObject, clip.length + 0.02f);

    }

    public void PlayMainFadeOut(float _fadeOutTime, float _fadeLength)
	{
				AudioClip clip = GetClip(ClipName);

		mPlayer.clip = clip;

		if (clip != null) {
			mPlayer.volume = mLocalVolume * MasterVolume;
			mPlayer.Play ();
		}

		StartCoroutine(FadeOut(_fadeOutTime, _fadeLength));
	}

	IEnumerator FadeOut(float _fadeOutTime, float _fadeLength)
	{
		mPlayer.volume = mLocalVolume * MasterVolume;
		float volumeCache = mPlayer.volume;

		float fadeValue = _fadeLength / (1/Time.fixedDeltaTime);


		yield return new WaitForSeconds (_fadeOutTime);


		while (mPlayer.volume > 0.001f) {
			yield return new WaitForFixedUpdate ();
			mPlayer.volume -= (Time.fixedDeltaTime/_fadeLength) * volumeCache;

		}

		mPlayer.Pause();
		mPlayer.volume = volumeCache;
	}

	public bool IsPlaying()
	{
		return mPlayer.isPlaying;
	}

    public void PlayMain()
    {
        AudioClip clip = GetClip(ClipName);
		Debug.Log ("Playing main");
        mPlayer.clip = clip;

		mPlayer.volume = mLocalVolume * MasterVolume;

        if (clip != null)
            mPlayer.Play();

    }

	public void PlayClip()
	{
		PlayClip (ClipName);
	}
}

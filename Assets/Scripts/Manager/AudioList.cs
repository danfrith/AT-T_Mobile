using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class AudioEntry
{
	[SerializeField]
	public string Name;

	[SerializeField]
	public AudioClip AudioClip;
}

public class AudioList : MonoBehaviour {

    #region Singleton
    public static AudioList Instance;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(this.gameObject);
            return;
        }

        Instance = this;

		for (int i = 0; i < AudioClips.Length; i++) {
			ClipLookup.Add (AudioClips [i].Name, AudioClips [i].AudioClip);
		}
    }

    #endregion

    public AudioEntry[] AudioClips;

	Dictionary<string, AudioClip> ClipLookup = new Dictionary<string, AudioClip>();

	void OnEnable()
	{
		
		Instance = this;
	}

	public static AudioClip GetClip(string _name)
	{
        AudioList al = Instance;

        if (al.ClipLookup.ContainsKey(_name))
			return al.ClipLookup[_name];

		Logger.LogError ("Clip " + _name + " does not exist");

		return null;
	}

}

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ResultsScreenItem : MonoBehaviour {

	public Text PercentText;
	public ProgressBar Bar;

	public VirusType mType;

	public Color[] ForgroundColour;
	public Color[] BackgroundColour;

	void OnEnable()
	{
		Init (mType);
	}
	public int[] Indexes;
	public void Init(VirusType _type)
	{
		mType = _type;
		UpdateInterface ();

		// Index allows me to re-sort the positions arbitrarily.
		Bar.SetColours (ForgroundColour [Indexes[(int)_type]-1], BackgroundColour [Indexes[(int)_type]-1]);
	}

	public void UpdateInterface()
	{
		int spawned = Stage.Instance.GetTotalSpawned (mType);
		int hit = Stage.Instance.GetTotalHit(mType);
		PercentText.text = (hit.ToString() + "/" + spawned.ToString());
        if (spawned == 0)
        {   
            Bar.Init(1);
        }
        else
        {
            float FillPercent = (float)hit / (float)spawned;
            Bar.Init(0);
            Bar.SetValue(FillPercent);

        }
	}
}

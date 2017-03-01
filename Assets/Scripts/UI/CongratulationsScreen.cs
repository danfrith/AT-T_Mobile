using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CongratulationsScreen : MonoBehaviour {

	private Text mText;

	 public string Message = "Congratulations {0:00.0}% threats blocked";
	void OnEnable()
	{
		mText = GetComponent<Text> ();

		if (mText == null) {
			Logger.LogError ("Text component is missing from " + name);
			return;
		}

		UpdateInterface ();
	}
    public bool ShowPercent = false;
	void UpdateInterface()
	{
        float blocked = 0;

        if (ShowPercent)
            blocked = Stage.Instance.GetTotalPercentThreatsBlocked () * 100;
        else
            blocked = Stage.Instance.CurrentScore;
        
        mText.text = string.Format (Message, blocked);
	}
}

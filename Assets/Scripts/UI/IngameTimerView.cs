using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(Text))]
public class IngameTimerView : MonoBehaviour {

	private Text mText;
	private Stage stage;

	public void OnEnable()
	{
		mText = GetComponent<Text> ();
		if (mText == null)
			Logger.LogError ("mText was null in ImageScoreView " + name);

        stage = Stage.Instance;
		if (stage == null)
			Logger.LogError ("The stage object is missing in the level ");

	}

	public void Update()
	{
		if (mText == null || stage == null)
			return;
		
		if (stage.IsRunning == false)
			mText.text = "";
		else {
			if (stage.remainingTime/60 > 1)
				mText.text = string.Format ("{0}:{1:00.00}", (int)stage.remainingTime / 60, stage.remainingTime % 60);
			else
				mText.text = string.Format ("{0:00.00}", stage.remainingTime % 60);
		}

	}

}

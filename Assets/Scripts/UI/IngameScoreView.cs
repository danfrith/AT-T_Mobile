using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(Text))]
public class IngameScoreView : MonoBehaviour {

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

        //Debug.Log("stage.IsRunning " + stage.IsRunning);
        
		if (stage.IsRunning == false)
			mText.text = "";
		else
            //mText.text = "Score: " + stage.CurrentScore.ToString();

			//mText.text = string.Format("Score: {0}, {1:00}%", stage.CurrentScore, stage.GetTotalPercentThreatsBlocked()*100);
			mText.text = "Score: "+stage.CurrentScore;
			//mText.text = "Score: " + stage.GetTotalPercentThreatsBlocked()*100 + "%";
	}

}

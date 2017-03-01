using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EndGameScreen : MonoBehaviour {

	public Text TitleText;
    public Text SubTitleText;
    public Text ScoreText;
    public Text MessageText;

    public string TitleFail         = "Game Over";
    public string SubFail           = "Game Over";
    public string MessageFail       = "You failed to block enough threats, please try again.";
    
    public string TitleSuccess      = "Congratulations";
    public string SubSuccess        = "Level Complete";
    public string MessageSuccess    = "You defended the network!";

    public string Score             = "Score: {0}%";

    void OnEnable()
	{
		Debug.Log ("EndGameScreen opened");

		MobileStage m           = Stage.Instance as MobileStage;

        ScoreText.text          = string.Format(Score, m.GetScore());

        if (m.Lives <= 0) {

            TitleText.text      = TitleFail;
            SubTitleText.text   = SubFail;
            MessageText.text    = MessageFail;

		} else {

            TitleText.text      = TitleSuccess;
            SubTitleText.text   = SubSuccess;
            MessageText.text    = MessageSuccess;
            
        }

	}

	public void ContinueButtonPressed()
	{
		Debug.Log ("Continue pressed");

		MobileStage m = Stage.Instance as MobileStage;
		m.NotifyGameManagerFromUI ();
	}
}

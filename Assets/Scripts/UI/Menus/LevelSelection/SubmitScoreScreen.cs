using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Text.RegularExpressions;

public class SubmitScoreScreen : MonoBehaviour {

    public MenuController MenuController;
    public MenuBase MainMenu;
    public MenuBase LeaderBoardScreen;
    public YesNoDialogue YesNoDialogue;
    public MenuBase FullScreenDialogue;
	public InputField Input;

    public Button Skipbutton;
    public Button SubmitButton;

    public Transform Sending;
    public Text ResultText;

    public void SubmitScoreButtonPressed()
    {

		GameObject go = GameObject.Find("MobileGameManager");
		ScoreManager s = null;
		if (go != null) {
			s = go.GetComponent<ScoreManager> ();
		}
		if (s == null) {
			Debug.LogError ("Could not find score manager");
			return;
		}

        if (Input.text.Length == 0)
            return;

		// Strip bad characters out.
		Regex rgx = new Regex("[^a-zA-Z0-9 -]");
		string name = rgx.Replace(Input.text, ""); 

		ScoreData data = new ScoreData (Input.text, MobileGameManager.Instance.Score);
        Debug.Log("Got Score data " + data.ToString());
		s.SendCurrentScore (data, FailedToSendScore, ScoreSendSuccess);
        
        WaitForScore();
        //MenuController.OpenMenu(MainMenu);
    }

    public void WaitForScore()
    {
        Skipbutton.interactable = false;
        SubmitButton.interactable = false;
        Input.interactable = false;
        Sending.gameObject.SetActive(true);
    }

    public void FailedToSendScore()
    {
        Debug.Log("Score sending failed");
        Skipbutton.gameObject.SetActive(true);
        Skipbutton.interactable = true;
        SubmitButton.interactable = true;
        Input.interactable = true;
        Sending.gameObject.SetActive(false);
        ResultText.gameObject.SetActive(true);

        ResultText.text = "Failed to connect and send score ";
    }

    public void ScoreSendSuccess()
    {
        Debug.Log("Score sent Succesfully");
        //Skipbutton.interactable = true;
        SubmitButton.interactable = true;
        Input.interactable = true;
        Sending.gameObject.SetActive(false);
        ResultText.gameObject.SetActive(true);

        MenuController.OpenMenu(LeaderBoardScreen);
    }

    IEnumerator HideResultText()
    {
        yield return new WaitForSeconds(4);
        ResultText.gameObject.SetActive(false);
    }

    public void ButtonSkipPressed()
    {
        YesNoDialogue.Open("Skip score submission", "Are you sure you want to skip submitting your score?", Skip);
    }

    void Skip(YesNoDialogueResponse _response)
    {
        if (_response == YesNoDialogueResponse.Yes)
            MenuController.OpenMenu(MainMenu);

    }


}

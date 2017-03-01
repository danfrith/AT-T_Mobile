using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LevelSelectScreen : MonoBehaviour {

	public Text TitleText;
	public Text SubText;

	public Text ButtonText;
    public Button ContinueButton;
	MobileGameManager mGameManager;

    MenuBase mMenuBase;

	public delegate void UIUpdatedEvent ();
	public event UIUpdatedEvent UIUpdatedEventHandler;

	void OnEnable()
	{

        if (!mInit)
            Init();

        UpdateUI ();

    }

    public bool mInit = false;

    public void Init()
    {
        mGameManager = MobileGameManager.Instance;
        mGameManager.StateChangeEventHandler += new MobileGameManager.StateChangedEvent(StateChange);
        mMenuBase = GetComponent<MenuBase>();

        mMenuBase.MenuOpened += MMenuBase_MenuOpened;
        ContinueButton = ButtonText.transform.parent.GetComponent<Button>();
        mInit = true;
    }

    private void MMenuBase_MenuOpened(object sender, System.EventArgs e)
    {
        UpdateUI();
    }

    private void OnDisable()
    {
        mGameManager.StateChangeEventHandler -= StateChange;
    }

    void OnDestroy()
	{
		mGameManager.StateChangeEventHandler -= StateChange;
	}

	void StateChange()
	{
		UpdateUI ();
	}

	public void UpdateUI()
	{
		SetText ();
		if (mGameManager.CurrentLevel == 0) // When the game starts there is no level selected, all the levels are highlighted.
		{
			ButtonText.text = "Start";
			ContinueButton.interactable = true;
		}
		else if ((mGameManager.CurrentLevel % 5) == 0) // Continue 
		{
			ButtonText.text = "Continue";
			ContinueButton.interactable = true;
		}
        else if (mGameManager.CurrentLevelState == LevelState.Selected) // When a level is selected display the start button
        {
            ButtonText.text = "Start";
            ContinueButton.interactable = true;
        }
        else if (mGameManager.CurrentLevelState == LevelState.JustCompleted)  // Don't allow interaction while the animation is playing, the animation advances the game state
        {
            ContinueButton.interactable = false;
        }
		else if (mGameManager.CurrentLevel == 20 && mGameManager.CurrentLevelState == LevelState.Completed)
		{
			ButtonText.text = "Finish";
			ContinueButton.interactable = true;
		}
        else
        {
            ButtonText.text = "Continue";
            ContinueButton.interactable = true;
        }
        

		if (UIUpdatedEventHandler != null)
			UIUpdatedEventHandler ();
	}

	public void ContinueButtonPRessed()
	{
        if (mGameManager.CurrentLevelState == LevelState.Completed && mGameManager.CurrentLevel == 20)
        {
            // This is fairly hackey. 

            GameObject go = GameObject.Find("MenuController");
            MenuController mc = null;
            MenuBase submitScore = null;
            if (go != null)
                mc = go.GetComponent<MenuController>();

            go = GameObject.Find("SubmitScore");

            if (go != null)
                submitScore = go.GetComponent<MenuBase>();

            if (mc == null || submitScore == null)
            {
                Debug.LogError("Could not find Menu controller or Submit score pag");
                return;
            }

            mc.OpenMenu(submitScore);
        }

        if (mGameManager.CurrentLevel == 0)
        {
            mGameManager.CurrentLevel = 1;
            mGameManager.CurrentLevelState = LevelState.Selected;
            UpdateUI();
        }
        else if (mGameManager.CurrentLevelState == LevelState.Selected)
        {
            mGameManager.StartLevel();
        }
        else
        {
            mGameManager.IncrimentState();
        }

		//UpdateUI ();
	}

    void SetText()
    {
        if (mGameManager.CurrentLevel == 0 ) {//&&  mGameManager.CurrentLevelState == LevelState.Unselected) {
			TitleText.text = "New Game";
			SubText.text = "Make your way through the zones to complete your mission";
		} else if (mGameManager.CurrentLevel >= 0 && (mGameManager.CurrentLevelState != LevelState.Completed && mGameManager.CurrentLevelState != LevelState.JustCompleted)) {
            TitleText.text = "Level " + mGameManager.CurrentLevel;
            SubText.text = "";
			
		} else if (mGameManager.CurrentLevel >= 0 && (mGameManager.CurrentLevelState == LevelState.JustCompleted || mGameManager.CurrentLevelState == LevelState.Completed)) {
			TitleText.text = "Level " + mGameManager.CurrentLevel;
			SubText.text = "Completed";
		}

		if (mGameManager.CurrentLevel == 5 && (mGameManager.CurrentLevelState == LevelState.JustCompleted || mGameManager.CurrentLevelState == LevelState.Completed)){
			TitleText.text = "Congratulations";
			SubText.text = "You've completed Zone 1";
		} else if (mGameManager.CurrentLevel == 10 && (mGameManager.CurrentLevelState == LevelState.JustCompleted || mGameManager.CurrentLevelState == LevelState.Completed)){
			TitleText.text = "Congratulations";
			SubText.text = "You've completed Zone 2";
		} else if (mGameManager.CurrentLevel == 15 && (mGameManager.CurrentLevelState == LevelState.JustCompleted || mGameManager.CurrentLevelState == LevelState.Completed)){
			TitleText.text = "Congratulations";
			SubText.text = "You've completed Zone 3";
		} else if (mGameManager.CurrentLevel == 20 && (mGameManager.CurrentLevelState == LevelState.JustCompleted || mGameManager.CurrentLevelState == LevelState.Completed)){
			TitleText.text = "Congratulations Threat Defender";
			SubText.text = "You have succesfully defended the network";
		}

	}
}

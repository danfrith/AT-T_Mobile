using UnityEngine;
using System.Collections;

public class MainMenu : MonoBehaviour {

    MobileGameManager mManager;
    public MenuController MenuController;
    public MenuBase NewGameScreen;
	public MenuBase LevelSelectScreen;
    public YesNoDialogue YesNoDialogue;
    void OnEnable()
    {
        mManager = MobileGameManager.Instance;
        //mMenuBase = GetComponent<MenuBase>();
        if (mManager.LevelSelectOpen)
            StartCoroutine(OpenLevelSelectScreenDeleyed());

    }

    IEnumerator OpenLevelSelectScreenDeleyed()
    {
        yield return new WaitForEndOfFrame();
        MenuController.OpenMenu(LevelSelectScreen);
    }

    public void NewGameButtonPressed()
    {
		Debug.Log ("mManager.HasSaveGame() = " + mManager.HasSaveGame ().ToString());
        // Open yes no dialogue.
        if (mManager.HasSaveGame())
            YesNoDialogue.Open("Overwrite existing game?", "If you start a new game you will loose your current progress!\n Do you wish to continue?", NewGameResponse);
        else
			MenuController.OpenMenu(NewGameScreen);
    }
		
    public void NewGameResponse(YesNoDialogueResponse _response)
    {

        if (_response == YesNoDialogueResponse.Yes)
        {
            mManager.ResetProgress();
			MenuController.OpenMenu(NewGameScreen);
        }
        
    }

	public void OpenPrivacyPolicy()
	{
		Application.OpenURL("http://about.att.com/sites/privacy_policy");
	}

	public void OpenTestShareLink()
	{
		Application.OpenURL("https://twitter.com/share?url=http://www.simplo.me/publishers/homecraft/robot-talk/&text=Robot+Talk&hashtags=Simplo.me");

	}


	public Texture2D shareTexture;

	public void ShareIntentTwitter()
	{
		AndroidSocialGate.StartShareIntent("Hello Share Intent", "This is my text to share", "twi");
	}

    public void ComtinueGameButtonPressed()
    {
        //if ()
        // At present there's no reason to press new game as continue will start from pos 0 to what
        // ever position you have gotten to in the game. 
        MenuController.OpenMenu(LevelSelectScreen);
    }

    

}

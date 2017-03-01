using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class DebugPage : MonoBehaviour {

    public static DebugPage Instance;
	void OnEnable()
	{
        Instance = this;

        CurrentLevelText.text = "Level: " + (MobileGameManager.Instance.CurrentLevel+1).ToString ();


		if (MobileGameManager.Instance.GodMode == false) {
			TextGodModebutton.text = "Enable";
		} else {
			TextGodModebutton.text = "Disable";
		}
        AndroidSocialGate.OnShareIntentCallback += HandleOnShareIntentCallback;
    }

    public void ShareText()
    {
        
        

    }

    private void OnDisable()
    {
        AndroidSocialGate.OnShareIntentCallback -= HandleOnShareIntentCallback;
    }

    void HandleOnShareIntentCallback(bool status, string package)
    {
        

        AddDebugText("[HandleOnShareIntentCallback] " + status.ToString() + " " + package);
    }

    public Text CurrentLevelText;
	public void LevelSet(Slider _s)
	{
		MobileGameManager.Instance.CurrentLevel = (int)_s.value;
		CurrentLevelText.text = "Level: " + ((int)(_s.value+1)).ToString ();

	}

	public Text TextGodModebutton;
	public void ToggleGodMode()
	{
		if (MobileGameManager.Instance.GodMode == false) {
			MobileGameManager.Instance.GodMode = true;
			TextGodModebutton.text = "Disable";

		} else {
			MobileGameManager.Instance.GodMode = false;
			TextGodModebutton.text = "Enable";
		}

	}

    public void ResetSave()
    {
        MobileGameManager.Instance.ResetProgress();
    }

    public void DeleteSave()
    {
        // Not yet implimented
        MobileGameManager.Instance.ResetProgress();
    }

    public Texture2D shareTexture;
    public Texture2D ShareImage;


    public void TestTwitterIntent()
	{
		AddDebugText ("Testing twitter intent");

		try {
			AndroidSocialGate.StartShareIntent("Hello Share Intent", "This is my text to share", "twi");
		}
		catch (System.Exception e) {
			AddDebugText ("failed to launch intent " + e.Message + " " + e.StackTrace);
		}

			
	}

    public void TestFacebookIntent()
    {
        AddDebugText("Testing Facebook intent");

        try
        {
            AndroidSocialGate.StartShareIntent(" title ", " facebook message", ShareImage, "facebook.katana");
        }
        catch (System.Exception e)
        {
            AddDebugText("failed to launch intent " + e.Message + " " + e.StackTrace);
        }


    }

    public void TestGPlusIntent()
    {
        

        try
        {
            AddDebugText("Testing G+ intent2");
            AndroidSocialGate.StartGooglePlusShare("google plus message", ShareImage);
            //AndroidSocialGate.
        }
        catch (System.Exception e)
        {
            AddDebugText("failed to launch intent " + e.Message + " " + e.StackTrace);
        }


    }

    public void TestMultiShare()
    {
        AddDebugText("Testing Multi-share intent");

        try
        {
            AndroidSocialGate.StartShareIntent("title ", "google plus message");
        }
        catch (System.Exception e)
        {
            AddDebugText("failed to launch intent " + e.Message + " " + e.StackTrace);
        }


    }

    public Text DebugText;


	List<string> DebugTextList = new List<string>();
	public void AddDebugText(string _value)
	{
		Debug.Log(_value);
		DebugTextList.Add(_value);

		string message = "";
		foreach (string s in DebugTextList)
		{
			message = message + s + "\n";
		}

		DebugText.text = message;
	}


}

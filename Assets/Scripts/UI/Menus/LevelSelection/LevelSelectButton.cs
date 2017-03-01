using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LevelSelectButton : MonoBehaviour {

	public int LevelNo = 0;

	static Color Selected = Color.white;
	static Color UnSelected = Color.black;
	static Color ColCompleted;
	
	Image mImage;
	Text mButtonText;
	Transform mOverlay;

    public Sprite OnButton;
    public Sprite OffButton;

	bool mInit = false;
	void Validate()
	{
		bool isValid = true;

		mImage = GetComponent<Image> ();
		if (mImage == null) {
			Debug.LogError ("LevelSelectButton: Image is missing for " + name);
			isValid = false;
		}

		mButtonText = transform.GetChild(0).GetComponent<Text> ();
		if (mButtonText == null) {
			Debug.LogError ("LevelSelectButton: ButtonText is missing for " + name);
			isValid = false;
		}

		mOverlay = transform.GetChild (1);
		if (mOverlay == null) {
			Debug.LogError ("LevelSelectButton: mOverlay is missing for " + name);
			isValid = false;
		}

        if (isValid)
        {
            ColCompleted = new Color(34f / 255f, 140f / 255f, 191f / 255f, 1);

            GameObject go = GameObject.Find("LevelSelect");
            LevelSelectScreen ls = null;
            if (go != null)
                ls = go.GetComponent<LevelSelectScreen>();

            ls.UIUpdatedEventHandler += new LevelSelectScreen.UIUpdatedEvent(UpdateColour);
        }

        mInit = isValid;
		 
	}

	void OnEnable()
	{
        if (!mInit)
		    Validate ();

        //UpdateColour();
    }

	void UpdateColour()
	{
		mButtonText.text = LevelNo.ToString();
        //Debug.Log("MobileGameManager.Instance" + MobileGameManager.Instance);
        //Debug.Log("Level "+ LevelNo + " " + MobileGameManager.Instance.GetLevelState(1));
        LevelState ButtonState = MobileGameManager.Instance.GetLevelState (LevelNo);
        LevelState WorldState = MobileGameManager.Instance.GetWorldState(LevelNo);

        if (MobileGameManager.Instance.CurrentLevel == 0) {
            ButtonState = LevelState.Completed;
            WorldState = LevelState.Selected;
        }
        
		SetColour (ButtonState);

		
		if (WorldState == LevelState.Selected)
			mOverlay.gameObject.SetActive (false);
		else
			mOverlay.gameObject.SetActive (true);
	}

	void SetColour(LevelState _state)
	{
		if (_state == LevelState.Completed) {

            mImage.sprite = OffButton;
			mImage.color = Color.white;
			mButtonText.color = Color.white;

		} else if (_state == LevelState.Selected && (MobileGameManager.Instance.CurrentLevel == LevelNo || MobileGameManager.Instance.CurrentLevel == 0)) {

            mImage.sprite = OnButton;
            mImage.color = Selected;
            mButtonText.color = Color.black;

		} else if (_state == LevelState.Unselected) {

            mImage.sprite = OnButton;
            mImage.color = UnSelected;
			mButtonText.color = Color.white;

		} else if (_state == LevelState.JustCompleted && MobileGameManager.Instance.CurrentLevel == LevelNo) {
            Debug.Log("current level " + MobileGameManager.Instance.CurrentLevel + " my level = " + LevelNo + " name "+  name);
			mImage.color = Color.white;
            mImage.sprite = OnButton;
            mButtonText.color = Color.black;
			StartCoroutine (AnimateButtonChange ());
		}
	}

	IEnumerator AnimateButtonChange()
	{
		yield return new WaitForSeconds (1f);

		Debug.Log ("Item " + name + " just completed " );
		Flash f = GetComponent<Flash> ();

		f.ReCacheColour ();
		f.StartFlash ();
		f.FlashCompleteEvent += new System.EventHandler (FlashComplete);
	}

	public void FlashComplete(object sender, System.EventArgs _args)
	{
		mImage.color = ColCompleted;
		mButtonText.color = Color.white;
        Debug.Log("Flash event for " + name);
		MobileGameManager.Instance.IncrimentState ();

		// TODO: add sounds here.

	}

}

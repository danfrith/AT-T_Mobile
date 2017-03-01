using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

public class LevelStage : Stage {

    void OnEnable()
	{
		//BeginStage();
		GameObject go =  GameObject.Find("PressToContinue");
		if (go != null) {
			PressToContinue = go.GetComponent<Image> ();
		}

		if (PressToContinue == null)
			Logger.LogError ("PressToContinue object is missing for LevelStage");
		
	}

	public PageTurner Outro;
	public MobSpawner Spawner;

	public string LevelFile = "Level2";
    public bool Completed = false;

	private Image PressToContinue;

	override public void BeginStage()
	{
		Debug.Log ("Starting stage");

		Outro.Hide ();
		
        SpawnCountdown ();

		GameObject go = GameObject.Find ("Music");
		AudioClipPlayer ac = null;
		if (go != null) {
			ac = GetComponent<AudioClipPlayer> ();
		}

		if (ac != null) {
			Debug.Log ("Got AC " + ac.ToString () + " &&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&");
			ac.Stop ();
			ac.SetClip ("BGM2A");
			ac.SetLooping (false);
			ac.PlayMain ();


		}

        StartCoroutine(StartStageCoroutine(3));
    }

    IEnumerator StartStageCoroutine(float _delay)
    {
        yield return new WaitForSeconds(1.5f);
        
        AudioClipPlayer ac = GetComponent<AudioClipPlayer>();

        if (AudioClipPlayerPrefab != null)
        {
            Transform t = (Transform)Instantiate(AudioClipPlayerPrefab, transform.position, Quaternion.identity);
            AudioClipPlayer p = t.GetComponent<AudioClipPlayer>();
            p.PlayAndDestroy("VO_GoodLuck", 1);
        }

        yield return new WaitForSeconds(_delay-1.5f);

        StartCoroutine(stageTimer());

        Spawner = MobSpawner.Instance;

        int selectedLevel = 1;

        List<string> keys = new List<string>();

        foreach (string key in Spawner.LevelsList.Keys)
        {
            keys.Add(key);
        }

		Spawner.SetInstructionList(LevelFile);

		GameObject go = GameObject.Find ("Music");
		ac = null;
		if (go != null) {
			ac = GetComponent<AudioClipPlayer> ();
		}
		if (ac != null) {
			ac.Stop ();
			ac.SetClip ("BGM2B");
			ac.SetLooping (true);
			ac.PlayMainFadeOut(StageDuration, 2);
		}

    }

    public Transform AudioClipPlayerPrefab;
    override public void EndStage()
	{
		Debug.Log ("End level 1 ");
        if (Completed == true)
            return;

		GameObject go = Resources.Load ("PlayClipPrefab") as GameObject;
		Transform prefab = go.transform;

		if (prefab != null) {
			Transform t  = (Transform)Instantiate(prefab, transform.position, Quaternion.identity);
			AudioClipPlayer p = t.GetComponent<AudioClipPlayer> ();
			p.PlayAndDestroy ("GameFinished");
		}

		go = GameObject.Find ("Music");
		if (go != null)
		{
			go.GetComponent<AudioSource> ().Pause();

		}

        Completed = true;

        mRemainingTime = -1;

        Spawner.EndLevel();
        Spawner.DestroyAllEnemies();

        Outro.Show ();

        AudioClipPlayer ac = GetComponent<AudioClipPlayer>();

        if (ac != null)
        {
            Debug.Log("Queueing audio");

            var clip = ac.GetClip("GameFinished");

            if (AudioClipPlayerPrefab != null)
            {
                Transform t = (Transform)Instantiate(AudioClipPlayerPrefab, transform.position, Quaternion.identity);
                AudioClipPlayer p = t.GetComponent<AudioClipPlayer>();
                
                if (this.GetTotalPercentThreatsBlocked() >= 0.99999f)
                    //ac.PlayClipDelayed("VO_Complete", clip.length + 1.5f);
                    p.PlayAndDestroyDelayed("VO_Complete", 1, clip.length + 1.5f);
                else
                    //ac.PlayClipDelayed("VO_EndIncomplete", clip.length + 1.5f);
                    p.PlayAndDestroyDelayed("VO_EndIncomplete", 1, clip.length + 1.5f);

            }

            

        }

        //DebugPrintScoreValues();

        Outro.SetPage (0);

	}

	public void Continue()
	{
        Debug.Log("" + PressToContinue.enabled + " " + Completed + " " + IsRunning);
		if (PressToContinue.enabled == true && Completed == false && IsRunning == false) {
			PressToContinue.enabled = false;
			Logger.Log ("Beggining stage from \"Continue\" pressed");
			BeginStage ();
		}
        else
        {
            Logger.Log("Continue screen is disabled");
        }

		if (IsRunning == true || Completed == false) {
			Logger.Log("Returning (levle still running)");
			return;
		}

		Logger.LogDebug("LevelStage:Continue: called");

		if (Outro.NextPage() == false)
		{
			Logger.Log ("Last page reached load staging area");

			Outro.CloseCurrentPage();
			NotifyGameManager ();
		}
	}

	void Update()
	{
		remainingTime = mRemainingTime;

		ReadControllers ();
	}

	void DebugEndGame()
	{
        if (GameManager.Instance.DebugEnabled == true)
		    EndStage ();
	}

	private int MainController = -1;
	void ReadControllers()
	{

		List<SteamVR_TrackedObject> Controllers = VRGripper.GetControllers ();

		if (Controllers == null)
			return;
		
		foreach (SteamVR_TrackedObject controller in Controllers) {
			
			if ((int)controller.index == -1)
				continue;

			try {
				var device = SteamVR_Controller.Input ((int)controller.index); // Get the device 
				if (device.GetPressUp (Valve.VR.EVRButtonId.k_EButton_ApplicationMenu)) {
					Debug.Log ("TutorialStage: Menu pressed");
				}

				if (device.GetPressUp (Valve.VR.EVRButtonId.k_EButton_Grip)) {
					Debug.Log ("LevelStage: Grip pressed");
					DebugEndGame ();
				}
					
				if (device.GetPressUp (Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger)) {
					Debug.Log ("TutorialStage: Trigger pressed " + controller.index.ToString () + " Main controller = " + MainController);
					if (MainController == -1)
						MainController = (int)controller.index;

					if ((int)controller.index == MainController) {
						Continue();
						VRGripper.GetGripper (controller).HapticVibration (0.1f, 0.05f);
					} else {
						//Back();
						VRGripper.GetGripper (controller).HapticVibration (0.1f, 0.05f);
					}
				}
			} catch (System.Exception e) {
				Debug.LogError ("Controller missing ID " + controller.index + " name " + controller.name + " Exception " + e.Message + "\n" + e.StackTrace);
			}
		}

        if (Input.GetKeyDown(KeyCode.C))
        {
            Continue();
        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            //Back();
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            DebugEndGame();
        }
        
    }

}

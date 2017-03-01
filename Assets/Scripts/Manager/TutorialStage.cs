using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Stage : MonoBehaviour {

	#region Singleton
	public static Stage Instance;


	public Transform CountdownTextPrefab;

	public Dictionary<VirusType, int> TotalSpawned;
	public Dictionary<VirusType, int> TotalHit;

	public void HitType(VirusType _type)
	{
		TotalHit [_type]++;
	}

	public virtual void SpawnedType(VirusType _type)
	{

        TotalSpawned [_type]++;
        //Debug.Log("TotalSpawned "+ _type + " " + TotalSpawned[_type]);
	}

	public int GetTotalSpawned(VirusType _type)
	{
		return TotalSpawned [_type];	
	}

	public int GetTotalHit(VirusType _type)
	{
		return TotalHit [_type];
	}

    public void DebugPrintScoreValues()
    {
        foreach (VirusType type in TotalSpawned.Keys)
        {
            Debug.Log(" Virus " + type + " " + TotalHit[type] + "/" + TotalSpawned[type]);
        }

    }

	public float GetTotalPercentThreatsBlocked()
	{
		float spawned = 0;
		float hit = 0;
		foreach (VirusType type in TotalSpawned.Keys)
		{
			spawned += TotalSpawned [type];
			hit += TotalHit [type];
		}
		if (spawned == 0)
			spawned = 1;
		
        //Debug.Log("Spawned percent = " + ((float)hit / (float)spawned));

		return (float)hit / (float)spawned;
	}

	protected void SpawnCountdown ()
	{
		Debug.Log ("Spawning countdown");
		GameObject parent = GameObject.Find ("InWorldHud");
		RectTransform containerRect = parent.GetComponent<RectTransform> ();
		GameObject Item = ((Transform)Instantiate (CountdownTextPrefab, parent.transform.position, Quaternion.identity, parent.transform)).gameObject;

		RectTransform ItemRect  = Item.GetComponent<RectTransform> ();
		Rect newRect = containerRect.rect;

		ItemRect.sizeDelta = new Vector2(400, 400);
		ItemRect.localScale = Vector3.one;

		ItemRect.localRotation = Quaternion.identity;
        

        Item.transform.position = parent.transform.position;
		Debug.Log ("countdown completed");
	}

	void Awake()
	{

		if (Instance != null)
		{
			Debug.Log ("Destroying self " + name);
			Destroy(Instance.gameObject);
			return;
		}

		Instance = this;
		Debug.Log ("Instance = " + Instance.name);

        if (CountdownTextPrefab == null)
            Logger.LogError("Stage: countdown text missing for " + name);

        InitScoreTracking ();
	}


	void InitScoreTracking()
	{
		TotalHit = new Dictionary<VirusType, int> ();
		TotalSpawned = new Dictionary<VirusType, int> ();

		for (int i = 0; i < 6; i++) {
			TotalHit.Add ((VirusType)(i), 0);
			TotalSpawned.Add((VirusType)(i), 0);
            //Debug.Log("Added virus tyoe " + (VirusType)(i + 1));
		}

        //TotalSpawned[VirusType.Virus1]++;
        //Debug.Log("^^^^^^^^^^^^^^^^ virus init: " +TotalSpawned[VirusType.Virus1]);
        //DebugTestScoreData ();
    }

	void DebugTestScoreData()
	{
		TotalHit [VirusType.Virus1] = 3;
		TotalHit [VirusType.Virus2] = 4;
		TotalHit [VirusType.Virus3] = 6;
		TotalHit [VirusType.Virus4] = 1;
		TotalHit [VirusType.Virus5] = 9;
		TotalHit [VirusType.Virus6] = 10;

		TotalSpawned[VirusType.Virus1] = 11;
		TotalSpawned[VirusType.Virus2] = 10;
		TotalSpawned[VirusType.Virus3] = 6;
		TotalSpawned[VirusType.Virus4] = 9;
		TotalSpawned[VirusType.Virus5] = 10;
		TotalSpawned[VirusType.Virus6] = 15;

    }

	void OnDestroy()
	{
		if (Instance == this)
		{
			Instance = null;
		}

		CleanupStage ();
	}

    #endregion

    public bool IsRunning;

    public string StageName;

	public float StageDuration;

	protected float mRemainingTime = 0;

    public float remainingTime;

    public float GetRemainingTime()
	{
		return mRemainingTime;
	}

    public float CurrentScore;

    virtual public void AddScore(float _value)
    {
        if (_value < 0)
        {
            Logger.LogDebug("Score value is invalid " + _value);
            return;
        }

        CurrentScore += _value;
    }

    public float GetScore()
    {
        return CurrentScore;
    }

    virtual public void BeginStage()
	{
		
		Debug.Log ("default begin stage");
	}
		
	virtual public void EndStage()
	{
		Debug.Log ("default end stage");
	}

	/* Disposes of any objects that this stage creates when it starts */
	virtual public void CleanupStage()
	{
		Debug.Log ("Default clean up stage");

	}

    private bool timerRunning = false;
    public float FixedDeltaTime;
	protected IEnumerator stageTimer()
	{
        if (timerRunning)
            yield break;

        timerRunning = true;

		Debug.Log ("Stage timer starting");
		mRemainingTime = StageDuration;
		IsRunning = true;
		while (mRemainingTime > 0 ) {
            FixedDeltaTime = Time.fixedDeltaTime;
            mRemainingTime -= Time.fixedDeltaTime;
			yield return new WaitForFixedUpdate ();
			remainingTime = mRemainingTime;

		}
		Debug.Log ("Staging ending");
		EndStage ();
		IsRunning = false;
	}

	protected void NotifyGameManager()
	{
		GameManager.Instance.StageEnded (StageName);
	}
}

public class TutorialStage : Stage {

    public PageTurner IntroPages;

    public Image ReadyPanel;

    List<SteamVR_TrackedObject> Controllers;

    public MobSpawner Spawner;

    int MainController = -1;

    public int mHits = 0; // count up for number of hits in the tutorial 

    void OnEnable()
    {
        GameObject go = GameObject.Find("IntroPages");
        if (go != null)
            IntroPages = go.GetComponent<PageTurner>();

        if (IntroPages == null)
            Logger.LogError("InstroPages missing");

        go = GameObject.Find("ReadyPanel");
        if (go != null)
            ReadyPanel = go.GetComponent<Image>();

        if (ReadyPanel == null)
            Logger.LogError("ReadyPanel missing");
        else
            ReadyPanel.enabled = false;


        Spawner = MobSpawner.Instance;

        Debug.Log("Getting controllers");
        Controllers = VRGripper.GetControllers();
        Debug.Log("Controllers gotten" + Controllers.ToString());

        

        StartCoroutine(ControllerSetup());

        IntroPages.Show();
        IntroPages.SetPage(0);
    }

    IEnumerator ControllerSetup()
    {
        yield return new WaitForSeconds(2);
        foreach (SteamVR_TrackedObject c in Controllers)
        {
            ModelController mc = c.transform.GetComponent<ModelController>();
            //mc.EnableModel();
            //mc.SwapToGlow();
        }
    }


	IEnumerator EnableShields()
    {
		yield return new WaitForSeconds (0.5f);
		VRGripper a = null;
		VRGripper b = null;
		foreach (SteamVR_TrackedObject c in Controllers) {
            ModelController mc = c.transform.GetComponent<ModelController>();
            mc.HideModel();

            if (a == null)
				a = VRGripper.GetGripper (c);
			else
				b = VRGripper.GetGripper (c);
		}

        for (int i = 0; i < 7; i++) {
			if (a != null)
				a.HapticPulse (i * 0.1f);
			if (b != null)
				b.HapticPulse (i * 0.1f);
			
			yield return new WaitForSeconds (0.9f - i*0.15f);
		}

        foreach (SteamVR_TrackedObject c in Controllers)
        {
            ModelController mc = c.transform.GetComponent<ModelController>();
            //mc.DisableModel();
			mc.EnableShieldAnimated();
            mc.SwapToStandard();
        }
    }

    override public void AddScore(float _value)
    {
        base.AddScore(_value);
        mHits++;

        if (mHits >= 5)
            EndStage();
    }
    
    void Continue(SteamVR_TrackedObject _controller)
    {
        if (IsRunning == true)
            return;

        if (GameManager.Instance.StartWithoutCurrentPlayer)
        {   // If the game skips starting without a current player and the current player is null, return.
            if (string.IsNullOrEmpty(GameManager.Instance.GetPlayerName()))
            {
                Debug.Log("No player found, no continue");
                return;
            }
            else
            {
                //Debug.Log("Tutorial found player " + GameManager.Instance.GetPlayerName());
            }
        }
        //SpawnCountdown ();

        try
        {
            VRGripper.GetGripper(_controller).HapticVibration(0.1f, 0.05f);
            _controller.GetComponent<AudioClipPlayer>().PlayClip("Click");

        }
        catch (System.Exception e)
        {
            Logger.LogError("TutorialStage:Continue: could not trigger hapti feedback Error: " + e.Message + " " + e.StackTrace);
        }

        if (IntroPages.NextPage() == false)
        {
            Debug.Log("Closing page");
            IntroPages.CloseCurrentPage();
            BeginStage();
            IsRunning = true;
            return;
        }

        if (IntroPages.GetCurrentPage() == 5)
        {
            Debug.Log("GotPage 5 triggering controllers");
            StartCoroutine(EnableShields());
        }

        
        
    }

    void Back(SteamVR_TrackedObject _controller)
    {
        Debug.Log("Back button pressed");
        if (IsRunning == true)
            return;

		if (IntroPages.GetCurrentPage () == 2) {
			NotifyGameManager ();
			return;
		}

		if (IsRunning == false)
			IntroPages.PreviousPage ();
		
        if (_controller == null)
            return;

        VRGripper.GetGripper(_controller).HapticVibration(0.1f, 0.05f);
        _controller.GetComponent<AudioClipPlayer>().PlayClip("Click");

    }

    override public void BeginStage()
	{
		
		SpawnCountdown ();

        Debug.Log("Begin tutorial stage");
        StartCoroutine(StartStageCoroutine(3));
    }

    IEnumerator StartStageCoroutine(float _delay)
    {
        yield return new WaitForSeconds(_delay);

        StartCoroutine(stageTimer());

        Spawner = MobSpawner.Instance;

        Spawner.SetInstructionList("Tutorial");

    }

	override public void EndStage()
	{
		Debug.Log ("End tutorial stage");

        Spawner.EndLevel();
        Spawner.DestroyAllEnemies();

        ReadyPanel.enabled = true;

        Debug.Log("enemies destroyed");

        StartCoroutine(EndStageRoutine());
	}

    IEnumerator EndStageRoutine()
    {
        yield return new WaitForSeconds(3);
        NotifyGameManager();
    }

	void Update()
	{
		remainingTime = mRemainingTime;

        ReadControllers();

    }

    void ReadControllers()
    {
        
        Controllers = VRGripper.GetControllers();
        if (Controllers == null)
            return;
        //Debug.Log("controller found count = "+ Controllers.Count);
        foreach (SteamVR_TrackedObject controller in Controllers)
        {
            //Debug.Log("controller index = " + controller.index);
            if ((int)controller.index == -1)
                continue;

            try
            {
                var device = SteamVR_Controller.Input((int)controller.index); // Get the device 
                if (device.GetPressUp(Valve.VR.EVRButtonId.k_EButton_ApplicationMenu))
                {
                    Debug.Log("TutorialStage: Menu pressed");
                }
                if (device.GetPressUp(Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger))
                {
                    Debug.Log("TutorialStage: Trigger pressed "+ controller.index.ToString());
                    if (MainController == -1)
                        MainController = (int)controller.index;

                    int rightIndex = SteamVR_Controller.GetDeviceIndex(SteamVR_Controller.DeviceRelation.Rightmost);

                    if ((int)controller.index == rightIndex)
					{
                        Continue(controller);
						
                    }
                    else
					{
                        Back(controller);
                    }
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError("Controller missing ID " + controller.index + " name " + controller.name + " Exception "+ e.Message + "\n" + e.StackTrace);
            }
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            Continue(null);
        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            Back(null);
        }

    }



}

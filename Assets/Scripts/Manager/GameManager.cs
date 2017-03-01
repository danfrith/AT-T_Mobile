using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System;

public class GameManager : MonoBehaviour {


	// Responsibility is to load levels and trigger game start and pause
	// Contains the main functions to move between game modes and manipulate them. (Admin overlay functions?)
	#region Singleton
	public static GameManager Instance;

	void Awake()
	{
		if (Instance != null)
		{
			Destroy(this.gameObject);
			return;
		}

		Instance = this;
		DontDestroyOnLoad(gameObject);

	}

	void OnDestroy()
	{
        Debug.Log("Gamemanager destroyed" + name);
		if (Instance == this)
			Instance = null;

	}

    #endregion

    public bool DebugEnabled = true;

	void OnEnable()
	{
		GetControllers ();

		CurrentPlayerData = new ScoreData("", "emanNo2", ""); // Clear the player (not nullable type)

        Logger.Log("Test file loading");
        OnLevelLoaded();

    }

	public string StagingScene 	= "StagingArea";
	public string Level01 		= "Level01";

	public void LoadStagingArea()
	{
		SteamVR_LoadLevel ll = GetComponent<SteamVR_LoadLevel> ();
		ll.levelName = StagingScene;
		Logger.Log("Attempting load scene " + StagingScene);
		ll.Trigger ();
		//LoadLevel (StagingScene);
	}

	public void LoadLevel01()
	{
		SteamVR_LoadLevel ll = GetComponent<SteamVR_LoadLevel> ();
		ll.levelName = Level01;
		Logger.Log("Attempting load scene " + Level01);
		ll.Trigger ();
		//LoadLevel (Level01);
	}

	private void LoadLevel(string _levelName)
	{
		Debug.Log ("Attempting to load scene" + _levelName);
		try 
		{
			SceneManager.LoadScene (_levelName);
		}
		catch (Exception e) {
			Logger.LogError ("Failed to load scene" + _levelName + " Reason: " + e.Message);
			return;
		}

		Logger.Log("Scene loaded succesfully");

	}

    public delegate void StageEndedEvent(ScoreData _data);
    public event StageEndedEvent StageEndedEventHandler;

	public void StageEnded(string _stageName)
	{
		Logger.Log ("GameManager Stage ended " + _stageName + " : " + StagingScene);

        if (_stageName == StagingScene || _stageName == "TutorialStage")
            LoadLevel01();
        else
        {
			try{
				
            TransmitScoreClearActivePlayer();


			}
			catch(Exception e) 
			{
				Logger.Log ("Failed to transmit score Error: " + e.Message + " Callstack: " + e.StackTrace);
			}

			Debug.Log ("No info");

			try
			{
				
            	if (StageEndedEventHandler != null)
                	StageEndedEventHandler(CurrentPlayerData);

			
			}
			catch(Exception e) 
			{
				Logger.Log ("Failed to run stage ended handler Error: " + e.Message + " Callstack: " + e.StackTrace);
			}

        }
    }

    public void TestTransmitScoreClearActivePlayer()
    {
        Debug.Log("Clearing active player and file");
        Debug.Log(CurrentPlayerData.ToString());
        TransmitScoreClearActivePlayer();
    }

    private void TransmitScoreClearActivePlayer()
    {
        CurrentPlayerData.Score = (int)Stage.Instance.GetScore();

		Logger.Log ("GameManager:TransmitScoreClearActivePlayer: CurrentPlayerData " + CurrentPlayerData.ToCSV());
        SendScore(CurrentPlayerData);

        CurrentPlayerData = new ScoreData("", "emanNo", ""); // Clear the player (not nullable type)

        // Remove the old current player file
        if (RegistrationFileManager.RemoveFile("CurrentPlayer") == false)
        {
            Logger.LogError("Failed to delete player file");
        }

        Logger.Log("Deleted current player, Loading staging area");

		StartCoroutine(CheckLoadPlayerRoutine());

        LoadStagingArea();
    }

    private void SendScore( ScoreData _data)
    {
        GameObject go = GameObject.Find("ScoreSender");

        if (go == null)
        {
            go = GameObject.Find("ScoreSender(Clone)");
        }
        if (go == null)
        {

            Logger.LogError("GameManager: ScoreSender is missing in the scene. Has a network connection been established?");
            return;
        }

        GameObject[] csList = GameObject.FindGameObjectsWithTag("ScoreSender");

        ClientScoreSender css = null;
        for (int i = 0; i < csList.Length; i++)
        {
            css = csList[i].GetComponent<ClientScoreSender>();
            if (css.isLocalPlayer == true)
                break;
        }

        Logger.Log("Sending score" + _data.ToCSV());
        css.SendScore(_data);
    }

    public string PlayerName;
    public ScoreData CurrentPlayerData;

    public string GetPlayerName()
    {
        //LoadPlayerFile();
        PlayerName = CurrentPlayerData.FirstName;
        return CurrentPlayerData.FirstName;
    }

    public bool StartWithoutCurrentPlayer = true;

    public void OnDisable()
    {
        Debug.Log("Game manager disabled");
    }

	public void StartStage()
	{
		Debug.Log ("Attempting to start stage");
        Stage.Instance.BeginStage ();


        Debug.Log ("Started stage");


	}

	List<SteamVR_TrackedObject> Controllers;

    // Input monitoring
    public void GetControllers()
	{
		Controllers = VRGripper.GetControllers (); 
	}

    void OnSceneLoad()
    {
        GetControllers();
    }

    public bool Test = false;
	void Update()
	{

        if (Test == true)
        {
            Test = false;
            TestTransmitScoreClearActivePlayer();
        }

        if (DebugEnabled == true)
        {
            ReadControllers();
        }
        

    }

    public void ReadControllers()
    {
		
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
                    BeginLevel();
                }

				if (device.GetPressUp(Valve.VR.EVRButtonId.k_EButton_Grip))
				{
					//BeginLevel();
                    if (Stage.Instance.IsRunning == false)
					    LoadLevel01();
				}
            }
            catch(System.Exception e)
            {
                Debug.LogError("Controller missing ID " + controller.index + " name " + controller.name + " Error " + e.Message + "\n"+ e.StackTrace);
            }
        }

        //if (Input.GetKeyDown(KeyCode.C))
        //{

        //}
    }

	void BeginLevel()
	{
		Debug.Log ("Begin level pressed");
		Stage currentStage = Stage.Instance;
		if (currentStage.IsRunning == false)
			currentStage.BeginStage ();
	}

//	// For debug purposes
//	void Start()
//	{
//		StartStage ();
//	}
//
	void OnLevelWasLoaded()
	{
		Debug.LogError ("GameManager: OnLevelWasLoaded");
		//		if (Stage.Instance.StageName != "StagingArea" && Stage.Instance.StageName != "TutorialStage") {
//            Debug.Log("Stage " + Stage.Instance.StageName + " starting game");
//			StartStage();
//		}

	}

	void OnLevelLoaded()
	{
		Logger.Log ("OnLevelLoaded2");

        StartCoroutine(CheckLoadPlayerRoutine());

        /*
         * Poll for file
         * delete file on game complete
         * blank player name
         */
	}

	bool isSearching = false;

    IEnumerator CheckLoadPlayerRoutine()
    {

        yield return new WaitForSeconds(0.01f);
        Logger.Log("GameManager: Checking level name" + Stage.Instance.StageName);
        //if (Stage.Instance.StageName == "StagingArea" || Stage.Instance.StageName == "TutorialStage")
		if (CurrentPlayerData.FirstName == "")
        {
            Logger.Log("Beggining current player file search");
            StartCoroutine(LoadPlayerNameFromFileRoutine());
        }
    }

    IEnumerator LoadPlayerNameFromFileRoutine()
    {

		if (isSearching == true)
			yield break;

		isSearching = true;

        bool end = false;
        
        if (Stage.Instance.StageName != "StagingArea" && Stage.Instance.StageName == "TutorialLevel")
        {
            end = true;
        }

        while (end == false)
        {
            yield return new WaitForSeconds(0.6f);

            end = LoadPlayerFile();

        } // While

		isSearching = false;

    } // LoadPlayerNameFromFileRoutine

    bool LoadPlayerFile()
    {
        Logger.Log("Attempting to open file CurrentPlayer -+-+ ");
        List<ScoreData> list = RegistrationFileManager.LoadRegistrants("CurrentPlayer", false);
        if (list.Count != 0)
        {
            CurrentPlayerData = list[0];
            if (string.IsNullOrEmpty(CurrentPlayerData.FirstName))
            {
                Debug.LogError("GameManager: Player name is empty");
                return false;
            }

        }
		if (string.IsNullOrEmpty (CurrentPlayerData.FirstName)) {
			Logger.LogError("GameManager: Player name is empty");
			return false;
		}

		Logger.Log("GameManager: Found current player " + CurrentPlayerData.ToString());
        return true;
    }

}

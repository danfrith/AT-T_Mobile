using UnityEngine;
using System.Collections;
using System.IO;
using System.Text;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Net;

public enum LevelState
{
	Unselected,
	Selected,
	JustCompleted,
	Completed,
}

public struct GameState
{
	public LevelState State;
	public int CurrentLevel;
}

public class MobileGameManager : MonoBehaviour {

	#region Singleton
	public static MobileGameManager Instance;

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
	public bool UseSaveFile = true;
    public bool SkipLevels = false;
	public int CurrentLevel = 0;
	public LevelState CurrentLevelState;
    public int Score = 0;
	public bool GodMode = false;

	/// <summary>
	/// Used by level buttons to get what state they should currently display.
	/// </summary>
	/// <returns>The level state.</returns>
	/// <param name="_level">Level.</param>
	public LevelState GetLevelState(int _level)
	{
		if (CurrentLevel > _level) {
			return LevelState.Completed;
		} else if (CurrentLevel < _level) {
			return LevelState.Unselected;
		} else {
			return CurrentLevelState;
		}
	}

    void debugPOST()
    {
        //var httpWebRequest = (HttpWebRequest)WebRequest.Create("https://52abm5d0id.execute-api.us-west-2.amazonaws.com/prod/TestFunction");
        //httpWebRequest.ContentType = "application/json";
        //httpWebRequest.Method = "POST";

        //using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
        //{
        //    string json = "{\"user\":\"test\"," +
        //                  "\"password\":\"bla\"}";

        //    streamWriter.Write(json);
        //    streamWriter.Flush();
        //    streamWriter.Close();
        //}

        //var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
        //using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
        //{
        //    var result = streamReader.ReadToEnd();
        //}

        //Debug.Log("resp " + httpResponse);
    }

    private float TimeScale;

    /// <summary>
    /// Pauses the game using timescale
    /// </summary>
    public void Pause()
    {
        Time.timeScale = 0;
    }

    /// <summary>
    /// Resumes the game
    /// </summary>
	/// 
    public void Resume()
    {
        Time.timeScale = TimeScale;
        Debug.Log("Resumeing " + Time.timeScale.ToString());
    }

    void OnEnable()
    {
        TimeScale = Time.timeScale;
        LoadProgress();
        //debugPOST();
    }

    public LevelState GetWorldState(int _level)
	{
		if (CurrentLevel == 0)
			return LevelState.Unselected;
		
		if (CurrentLevel > 15) {
			if (_level > 15)
				return LevelState.Selected;
			else
				return LevelState.Unselected;
			
		} else if (CurrentLevel > 10) {
			if (_level > 10 && _level < 16)
				return LevelState.Selected;
			else
				return LevelState.Unselected;
			
		} else if (CurrentLevel > 5) {
			if (_level > 5 && _level < 11)
				return LevelState.Selected;
			else
				return LevelState.Unselected;
		} else {
			if (_level < 6)
				return LevelState.Selected;
			else
				return LevelState.Unselected;
		}
	}

    public bool HasSaveGame()
    {
        return (CurrentLevel > 1);
    }

    public void ResetProgress()
    {
        // Changed from 0 and completed, we want to skip having to keep pressing continue (The original intention was to animate the selections at the start)
        CurrentLevel = 0;
		CurrentLevelState = LevelState.Completed;
        Score = 0;

        SaveProgress();
    }

    public void RestartLevel()
    {
        // Don't call this while not in a level
        StartLevel();
    }

    public void StageEnded(bool _completed, float _score)
    {
        Logger.Log("Stage ended");
        if (_completed)
        {
            IncrimentState();
            Score += (int)_score;
        }

        LevelSelectOpen = true;

        LoadLevel("Menus");
    }

    public bool LevelSelectOpen = false;

	public delegate void StateChangedEvent ();
	public StateChangedEvent StateChangeEventHandler;
    public void IncrimentState()
    {
        if (CurrentLevelState == LevelState.Unselected)
            CurrentLevelState = LevelState.Selected;
        else if (CurrentLevelState == LevelState.Selected)
            CurrentLevelState = LevelState.JustCompleted;
        else if (CurrentLevelState == LevelState.JustCompleted)
        { // This was changed from just incrimenting to completed. We don't need this step for now.
            CurrentLevel++;
            CurrentLevelState = LevelState.Selected;
        }
        else if (CurrentLevelState == LevelState.Completed)
        {
            CurrentLevel++;
            CurrentLevelState = LevelState.Unselected;
        }

        if (CurrentLevel > 20)
        {
            CurrentLevel = 20;
            CurrentLevelState = LevelState.Completed;
        }

        Debug.Log("State incremented " + CurrentLevel +  " state "+ LevelState.Completed);
        SaveProgress();

		try
		{
		if (StateChangeEventHandler != null)
			StateChangeEventHandler ();
		}
		catch {
			Logger.LogError ("StateChangeEventHandler failed to execute");
		}
        

    }
	public string SelectedLevel;

    public void StartLevel()
    {
        int world = (CurrentLevel-1) / 5;
        switch (world)
        {
            case 0:
                SelectedLevel = "World1";
                break;
            case 1:
                SelectedLevel = "World2";
                break;
            case 2:
                SelectedLevel = "World3";
                break;
            case 3:
                SelectedLevel = "World4";
                break;
            case 4:
                SelectedLevel = "World4";
                break;
        }


        if (SkipLevels == true) // Debug option for testing
        {
            IncrimentState();
        }
        else
		    LoadLevel("Loading");
    }

    private void LoadLevel(string _levelName)
    {
        Debug.Log("Attempting to load scene" + _levelName);
        try
        {
            SceneManager.LoadScene(_levelName);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Failed to load scene" + _levelName + " Reason: " + e.Message);
            return;
        }

        Debug.Log("Scene loaded succesfully");

    }

    #region SaveLoad
    private string SaveFileName = "Save";
    bool useEncryption = false;

    static int key = 129;

    /// <summary>
    /// From http://www.nullskull.com/a/780/simple-xor-encryption.aspx
    /// 
    /// </summary>
    /// <param name="textToEncrypt"></param>
    /// <returns></returns>
    public static string EncryptDecrypt(string textToEncrypt)
    {
        StringBuilder inSb = new StringBuilder(textToEncrypt);
        StringBuilder outSb = new StringBuilder(textToEncrypt.Length);
        char c;
        for (int i = 0; i < textToEncrypt.Length; i++)
        {
            c = inSb[i];
            c = (char)(c ^ key);
            outSb.Append(c);
        }
        return outSb.ToString();
    }

    void SaveProgress()
    {
        try
        {
            
            FileStream fs = File.Open(Application.persistentDataPath + "\\" + SaveFileName, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);


            string s = CurrentLevel.ToString() + "#" + ((int)CurrentLevelState).ToString() + "#" + Score.ToString();
            
            if (useEncryption)
                s = EncryptDecrypt(s);

            sw.Write(s);

            sw.Close();
            fs.Dispose();
        }
        catch (System.Exception e)
        {
            Logger.LogError("Failed to save file: " + SaveFileName + ".txt" + " Exception: " + e.Message);
            //success = false;
            //SaveError = SaveError + "Failed to save file: " + SaveFileName + ".txt" + " Exception: " + e.Message + "\n";
        }
    }

    void LoadProgress()
    {
		if (UseSaveFile == false)
			return;
		
        try
        {
            Stream stream = File.Open(Application.persistentDataPath + "\\" + SaveFileName, FileMode.OpenOrCreate);
            using (var reader = new StreamReader(stream, System.Text.Encoding.UTF8))
            {
                string s = "";

                if (useEncryption)
                    s = EncryptDecrypt(reader.ReadToEnd());
                else
                    s = reader.ReadToEnd();

                List<string> lines = new List<string>(s.Split(new[] { '\r', '\n', '#' }));
                if (lines.Count == 0)
                {
                    CurrentLevel = 0;
                    CurrentLevelState = LevelState.Unselected;
                } else {

                    bool success = int.TryParse(lines[0], out CurrentLevel);

                    if (!success)
                        Logger.LogError("LoadPgrogress: Failed to parse item 0 ");

                    int value;

                    success = int.TryParse(lines[1], out value);

                    Logger.Log("value " + value);

                    if (success)
                    {
                        CurrentLevelState = (LevelState)value;
                        Debug.Log(" current level state  = " + CurrentLevelState.ToString());
                    }
                    else
                        Logger.LogError("LoadPgrogress: Failed to parse item 1 ");

                    success = int.TryParse(lines[2], out Score);

                }

                Logger.Log(" Loaded file " + s);
            }
            stream.Close();

        }
        catch (System.Exception e)
        {
            Logger.LogError("Failed to load file " + SaveFileName + " Exception: " + e.Message);
            //SaveError = SaveError + "Failed to load file " + SaveFileName + " Exception: " + e.Message + "\n";
        }
    }

    string SaveError = "";

    //void OnGUI()
    //{
    //    GUI.TextArea(new Rect(0, 0, 280, 60), SaveError);
    //}
    #endregion
}

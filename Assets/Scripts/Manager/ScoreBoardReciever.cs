using UnityEngine;
//using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

public class RegistrationFileManager
{
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

    public static bool SaveRegistrants(string _fileName, List<ScoreData> _registrants, bool _useEncryption)
    {
        Logger.Log("Saving registrants to " + _fileName + ".txt");
        bool success = true;
        try
        {
            FileStream fs = File.Open(_fileName + ".txt", FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);


            string s = "";

            for (int i = 0; i < _registrants.Count; i++)
                s = s + _registrants[i].ToCSV() + "#";

            if (_useEncryption)
                s = EncryptDecrypt(s);

            sw.Write(s);

            sw.Close();
            fs.Dispose();
        }
        catch (System.Exception e)
        {
            Logger.LogError("Failed to save file: " + _fileName + ".txt" + " Exception: " + e.Message);
            success = false;
        }

        return success;
    }

    public static bool ExportRegistrants(string _fileName, List<ScoreData> _registrants)
    {
        Logger.Log("Saving registrants to " + _fileName);
        bool success = true;
        try
        {
            FileStream fs = File.Open(_fileName + ".csv", FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);

            sw.WriteLine("FirstName,LastName,Email,Score");


            for (int i = 0; i < _registrants.Count; i++)
                sw.WriteLine(_registrants[i].ToCSV());

            sw.Close();
            fs.Dispose();
        }
        catch (System.Exception e)
        {
            Logger.LogError("Failed to save file: " + _fileName + " Exception: " + e.Message);
            success = false;
        }

        return success;
    }

    public static bool FileExists(string _fullFileName)
    {
        return File.Exists(_fullFileName);
    }

    public static List<ScoreData> LoadRegistrants(string _fullFileName, bool _useEncryption)
    {
        Logger.Log("Loading entries from " + _fullFileName + " Encryption: " + _useEncryption);

        List<ScoreData> loadedRegistrants = new List<ScoreData>();

        try
        {
			Stream stream = File.Open(_fullFileName + ".txt", FileMode.Open);
            using (var reader = new StreamReader(stream, System.Text.Encoding.UTF8))
            {
                string s = "";

                if (_useEncryption)
                    s = EncryptDecrypt(reader.ReadToEnd());
                else
                    s = reader.ReadToEnd();

                Logger.Log("decrpted = " + s);

                loadedRegistrants = ParseFile(s);

                Logger.Log(loadedRegistrants.Count + " Registrants loaded ");
            }
            stream.Close();

        }
        catch (System.Exception e)
        {
            Logger.LogError("Failed to load file " + _fullFileName + " Exception: " + e.Message);
        }

        return loadedRegistrants;
    }

    public static List<ScoreData> ParseFile(string _file)
    {
		Logger.Log ("Opened file " + _file);
        _file = System.Text.RegularExpressions.Regex.Replace(_file, "\n", string.Empty);

        List<string> lines = new List<string>(_file.Split(new[] { '\r', '\n', '#' }));
        //Logger.Log("Lines " + lines.Count);

        List<ScoreData> loadedRegistrants = new List<ScoreData>();

        for (int i = 0; i < lines.Count; i++)
        {
            //Logger.Log("lines[i]" + lines[i]);

            if (lines[i].Length > 2)
            {
                ScoreData si = new ScoreData(lines[i]);
                loadedRegistrants.Add(si);
            }
        }

        return loadedRegistrants;
    }

    public static bool RemoveFile(string _file)
    {
        bool success = true;
        try
        {
            File.Delete(_file + ".txt");
        }
        catch(System.Exception e)
        {
            Logger.LogError("Failed to remove file " + _file + " Error " + e.Message);
            success = false;
        }

        return success;
    }

}

public struct ScoreData
{
    public ScoreData(string _firstName, string _lastName, string _email, int _score, string _date, int _hour)
    {
        FirstName = _firstName;
        LastName = _lastName;
        Email = _email;
        Score = _score;
        Date = _date;
        Hour = _hour;
    }

    public ScoreData(string _firstName, string _lastName, string _email, int _score)
    {
        FirstName = _firstName;
        LastName = _lastName;
        Email = _email;
        Score = _score;
        Date = "";
        Hour = -1;
    }

    public ScoreData(string _firstName, string _lastName, string _email)
    {
        FirstName = _firstName;
        LastName = _lastName;
        Email = _email;
        Score = -1;
        Date = "";
        Hour = -1;
    }

    public ScoreData(string _firstName, int _score)
    {
        FirstName = _firstName;
        LastName = "";
        Email = "";
        Score = _score;
        Date = "";
        Hour = -1;
    }

    public string FirstName;
    public string LastName;
    public string Email;
    public int Score;
    public string Date;
    public int Hour; 

    public string ToCSV()
    {
        return string.Format("{0},{1},{2},{3},{4},{5},", FirstName, LastName, Email, Score, Date, Hour);
    }

    public override string ToString()
    {
        return string.Format("{0},{1},{2},{3},{4},{5},", FirstName, LastName, Email, Score, Date, Hour);
    }

    public bool SameAs(ScoreData _other)
    {
        if (this.Date == _other.Date &&
            this.FirstName == _other.FirstName &&
            this.LastName == _other.LastName &&
            this.Score == _other.Score &&
            this.Hour == _other.Hour)
        {
            return true;
        }

        return false;
    }
    public ScoreData (string _csv)
    {
        List<string> items = Utils.Utilities.CSVLineToList(_csv);

        try
        {
            if (items.Count < 5)
                throw new System.Exception("Item not correct size only " + items.Count + " from " + _csv);

            FirstName = items[0];
            LastName = items[1];
            Email = items[2];
            
            if (items.Count < 5)
                Debug.LogError("Not enough items");
            
            if (int.TryParse(items[3], out Score) == false)
                throw new System.Exception("Failed to parse StartLocked from " + items[3]);
            
            Date = items[4];
            
            if (int.TryParse(items[5], out Hour) == false)
                throw new System.Exception("Failed to parse Hour from " + items[5]);

            
        }
        catch (System.Exception e)
        {
            Logger.LogError("Failed to create SpawnInstruction from CSV Error: " + e.Message);
            FirstName = "";
            LastName = "";
            Email = "";
            Score = 0;
            Date = "";
            Hour = 0;
        }

    }

}

/// <summary>
/// Responsible for recieving score data over the network from the clients and saving that data to the file.
/// There are events for when the score data list has been updated
/// </summary>
public class ScoreBoardReciever : MonoBehaviour {
    //public class ScoreBoardReciever : NetworkBehaviour {

    public List<ScoreData> Registrants;
    public string DefaultFileName = "Registrants";
    public string DefaultDirectory = "";

    public delegate void RegistrantsUpdated();
    public RegistrantsUpdated RegistrantsUpdatedHandler; 

    // Use this for initialization
    void Start () {

        Logger.Log("Attempting to create server");
        NetworkConnectionError nce = Network.InitializeServer(10, 25003, false);
        Logger.Log(nce.ToString());


        LoadRegistrants();
    }

    #region FileManagement

    public bool UseEncryption = true;

    public void LoadRegistrants()
    {
        Registrants = RegistrationFileManager.LoadRegistrants(GetActiveFileName(), UseEncryption);

        Registrants.Sort(SortScores);

        if (RegistrantsUpdatedHandler != null)
            RegistrantsUpdatedHandler();
    }

    /// <summary>
    /// Gets the file for the archived registration file. 
    /// The name is registrants + todays date + file number
    /// </summary>
    /// <returns>string name of file including directory</returns>
    string GetBackupFileName()
    {
        string fileName = DefaultDirectory + DefaultFileName + string.Format("_{0:yyyy-MM-dd_hh-mm-ss-tt}", System.DateTime.Now);

        Logger.LogError("File number check needs to be implimented");

        return fileName;
    }

    /// <summary>
    /// Gets the file for the active registration file. 
    /// </summary>
    /// <returns>string name of file including directory</returns>
    string GetActiveFileName()
    {
        string fileName = DefaultDirectory + DefaultFileName;

        return fileName;
    }

    #endregion

    /// <summary>
    /// Adds a new score data entry - this will save the entry to the active file and trigger the scoreboard update
    /// </summary>
    /// <param name="_data"></param>
    void AddRegistrant(ScoreData _data)
    {
        _data.Date = string.Format("{0:yy.MM.dd}", System.DateTime.Now);
        _data.Hour = System.DateTime.Now.Hour;
        // TODO: ScoreDataDate;
        Registrants.Add(_data);

        Registrants.Sort(SortScores);

        mLastScore = _data;

        RegistrationFileManager.SaveRegistrants(GetActiveFileName(), Registrants, UseEncryption);

        if (RegistrantsUpdatedHandler != null)
            RegistrantsUpdatedHandler();
    }

    /// <summary>
    /// Saves the old score list to a new file, then wipes the current file
    /// </summary>
    public void ResetScoreBoard()
    {
        if (RegistrationFileManager.ExportRegistrants(GetBackupFileName(), Registrants) == false)
        {
            Logger.LogError("Failed to backup file: " + GetBackupFileName());
            return;
        }
            
        Registrants = new List<ScoreData>();

        RegistrationFileManager.SaveRegistrants(GetActiveFileName(), Registrants, UseEncryption);

        if (RegistrantsUpdatedHandler != null)
            RegistrantsUpdatedHandler();
    }

    /// <summary>
    /// Returns the current score data list
    /// </summary>
    /// <returns></returns>
    public List<ScoreData> GetScores()
    {
        return Registrants;
    }

    ScoreData mLastScore;
    public ScoreData GetLastScore()
    {
        return mLastScore;
    }

    public int SortScores(ScoreData _first, ScoreData _second)
    {
        if (_first.Score > _second.Score)
            return -1;
        else if (_first.Score < _second.Score)
            return 1;
        else
            return 0;
    }

    #region Network

    /* Use http://stackoverflow.com/questions/16110014/unity3d-server-auto-discover-lan-only#16110015
     * for automatic server discovery for the clients
     */
    
    /// <summary>
    /// 
    /// </summary>
    void OnServerInitialized()
    {
        Logger.Log("Server running");
    }

    void OnApplicationQuit()
    {
        if (Network.isServer)
        {
            //if (Network.isServer) MasterServer.UnregisterHost();
            Network.Disconnect();
        }
    }

    [RPC]
    void LogMessage(string msg)
    {
        Logger.Log(msg);

        //RpcDoPrint();
    }

    [RPC]
    public void RecieveScoreData(string _firstName, string _lastName, string _email, int _score)
    {
        Logger.Log("Recieved data" + _firstName + " " + _lastName + " " + _email + " " + _score);

        AddRegistrant(new ScoreData(_firstName, _lastName, _email, _score));
    }

    public void ServerRecieveScoreData(string _firstName, string _lastName, string _email, int _score)
    {
        Logger.Log("Recieved data" + _firstName + " " + _lastName + " " + _email + " " + _score);

        AddRegistrant(new ScoreData(_firstName, _lastName, _email, _score));
    }

    #endregion

    //[Command]
    //public void CmdDoPrint()
    //{
    //    Logger.Log("server print");
    //}

    //[ClientRpc]
    //public void RpcDoPrint()
    //{
    //    Logger.Log("server rpc print");
    //}

}

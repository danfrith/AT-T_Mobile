using UnityEngine;
using System.Collections;
using System.IO;

public enum LogLevel
{
    DEBUG,      // Only displayed when "debug" is checked in the logger
    LOG,        // Basic log message
    WARNING,    // Something is potentially wrong
    ERROR,      // Something is wrong, the application may not work properly.
}

public class Logger : MonoBehaviour {

    #region Singleton
    public static Logger Instance;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(this.gameObject);
            return;

        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        OpenLogFile();
    }

    void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }

        CloseLogFile();
    }


    #endregion

    void OnApplicationQuit()
    {
        CloseLogFile();
    }

    public string DefaultLogFileName;
    public string DefaultFileLocation;
    public bool DebugLogging = true;
    public bool LoggingEnabled = true;

    protected StreamWriter FileStream;
    protected bool FileActive = false;

    void OnEnable()
    {
        Logger.LogWarn("Logging enabled");
    }

    public static void Log(LogLevel _logLevel, string _message, params object[] _formattedString)
    {
        LogMessageStatic(_logLevel, _message, _formattedString);
    }

    public static void Log(string _message, params object[] _formattedString)
    {
        LogMessageStatic(LogLevel.LOG, _message, _formattedString);
    }

    public static void LogDebug(string _message, params object[] _formattedString)
    {
        LogMessageStatic(LogLevel.DEBUG, _message, _formattedString);
    }

    public static void LogError(string _message, params object[] _formattedString)
    {
        LogMessageStatic(LogLevel.ERROR, _message, _formattedString);
    }

    public static void LogWarn(string _message, params object[] _formattedString)
    {
        LogMessageStatic(LogLevel.WARNING, _message, _formattedString);
    }

    /// <summary>
    /// Handles the fact that a scene might not have a logger. Logger.Log will behave just like Debug.Log if Logger instance is not available
    /// Consider updating Instance property to auto instantiate the logger prefab from the resource folder.
    /// </summary>
    /// <param name="_logLevel"></param>
    /// <param name="_message"></param>
    /// <param name="_formattedString"></param>
    public static void LogMessageStatic(LogLevel _logLevel, string _message, params object[] _formattedString)
    {
        if (Instance == null)
        {
            string msg = BuildMesageString(_message, _formattedString);
			if (_logLevel == LogLevel.ERROR)
				Debug.LogError (msg);
			else
				Debug.Log(msg);
        }
        else
            Instance.LogMessage(_logLevel, _message, _formattedString);

    }

    /// <summary>
    /// Main logging function, called by the other logging functions. 
    /// </summary>
    /// <param name="_logLevel">Message log level</param>
    /// <param name="_message">Messaege</param>
    /// <param name="formattedString">Optional formatted parameters</param>
    public void LogMessage(LogLevel _logLevel, string _message, params object[] _formattedStrings)
    {

        string msg = BuildMesageString(_message, _formattedStrings);

		if (_logLevel == LogLevel.ERROR)
			Debug.LogError (msg);
		else
			Debug.Log(msg);


		//Debug.Log ("log level = " + _logLevel);

        // If it's a debug message and debugging is disabled don't log, if logging is disabled don't log
        if (_logLevel == LogLevel.DEBUG && !DebugLogging || !LoggingEnabled) 
            return;

        string logText = BuildLogString(_logLevel, msg);

        WriteMessage(logText);
    }

    protected static string BuildMesageString(string _message, params object[] _formattedString)
    {
        string msg = "";

        try
        {
            msg = string.Format(_message, _formattedString);
        }
        catch(System.Exception e)
        {
            msg = _message + " Failed to build log string : " + e.Message;
        }

        return msg;
    }

    protected string BuildLogString(LogLevel _logLevel, string _message)
    {
        return _logLevel.ToString() + string.Format(":{0:dd.hh:mm:ss}: ", System.DateTime.Now) + _message;
    }

    protected void WriteMessage(string _logText)
    {
        //Debug.Log(_logText); // Just for now.

        if (!FileActive) // No log file to write to
            return;

        FileStream.WriteLine(_logText);
    }

    /// <summary>
    /// Opens the active log file for writing
    /// </summary>
    protected void OpenLogFile()
    {
        if (!LoggingEnabled)
            return;

        string FileName = GetFileName();

        try
        {
            FileStream fs = File.Open(FileName + ".txt", FileMode.Create);
            FileStream = new StreamWriter(fs);

            FileActive = true;
            Debug.Log("Log file opened succesfully " + FileName);

        }
        catch (System.Exception e)
        {
            LogError("Failed to open file " + FileName + " Exception: " + e.Message);
        }

    }


    protected string GetFileName()
    {
        if (string.IsNullOrEmpty(DefaultFileLocation) == false)
        {
            if (Directory.Exists(DefaultFileLocation) == false)
            {
                Directory.CreateDirectory(DefaultFileLocation);
                Debug.Log("Created directory " + DefaultFileLocation);
            }
        }
        string fileName = DefaultFileLocation + "\\"+ DefaultLogFileName + string.Format("_{0:yy.MM.dd_hh-mm-ss}", System.DateTime.Now);

        return fileName;
    }

    
    protected void CloseLogFile()
    {
        if (FileActive)
            FileStream.Close();

        FileActive = false;
    }

}

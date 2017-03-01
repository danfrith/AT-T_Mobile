using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
//using System.ComponentModel.DataAnnotations;

public class RegistrationForm : MonoBehaviour
{
    #region Singleton
    public static RegistrationForm Instance;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(this.gameObject);
            return;
        }

        Instance = this;
        //DontDestroyOnLoad(gameObject);

    }

    void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    #endregion

    public InputField FirstNameInput;
    public InputField LastNameInput;
    public InputField EmailInput;

    public Button SubmitButton;

    public string RegistrationFileName;
    public string CurrentPlayerFileName; // Ideal location: in shared class for this and the game manager.
    public string RegistrationFileDirectory;

    private EventSystem system;
    
    List<ScoreData> Registrants = new List<ScoreData>();

    public delegate void RegistrantsUpdated();
    public RegistrantsUpdated RegistrantsUpdatedHandler;

    public delegate void CurrentPlayerUpdated();
    public CurrentPlayerUpdated CurrentPlayerUpdatedHandler;

	public MenuBase DialogueWindow;
	public Text DialogueText;

    public bool Validate()
    {
        bool valid = true;
        if (FirstNameInput == null)
        {
            Debug.LogError("FirstNameInput is missing for Registration form " + name);
            valid = false;
        }

        if (LastNameInput == null)
        {
            Debug.LogError("LastNameInput is missing for Registration form " + name);
            valid = false;
        }
        if (EmailInput == null)
        {
            Debug.LogError("EmailInput is missing for Registration form " + name);
            valid = false;
        }
        

        return valid;
    }

    bool IsValidEmail(string email)
    {
        try {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch {
            return false;
        }
        
    }
    static Regex ValidEmailRegex = CreateValidEmailRegex();

    /// <summary>
    /// Taken from http://haacked.com/archive/2007/08/21/i-knew-how-to-validate-an-email-address-until-i.aspx
    /// </summary>
    /// <returns></returns>
    private static Regex CreateValidEmailRegex()
    {
        string validEmailPattern = @"^(?!\.)(""([^""\r\\]|\\[""\r\\])*""|"
            + @"([-a-z0-9!#$%&'*+/=?^_`{|}~]|(?<!\.)\.)*)(?<!\.)"
            + @"@[a-z0-9][\w\.-]*[a-z0-9]\.[a-z][a-z\.]*[a-z]$";

        return new Regex(validEmailPattern, RegexOptions.IgnoreCase);
    }

    internal static bool EmailIsValid(string emailAddress)
    {
        bool isValid = ValidEmailRegex.IsMatch(emailAddress);

        return isValid;
    }

    public void Testemailstring(string _string)
    {
        bool value = EmailIsValid(_string);
        Debug.LogError(_string + " valid = " + value);

    }
    public void OnEnable()
    {

        //Testemailstring("someone@somewhere.com");         //true
        //Testemailstring("someone@somewhere.co.uk");       //true
        //Testemailstring("someone+tag@somewhere.net");     //true
        //Testemailstring("futureTLD@somewhere.fooo");      //true

        //Testemailstring("fdsa");                          //false
        //Testemailstring("fdsa@");                         //false
        //Testemailstring("fdsa@fdsa");                     //false
        //Testemailstring("fdsa@fdsa.");                    //false

        StartCoroutine(LoadDelayed());
        
    }

    IEnumerator LoadDelayed()
    {
        yield return new WaitForSeconds(0.01f);

        if (Validate())
        {
            FirstNameInput.onEndEdit.AddListener(delegate { InputSubmittedEvent(FirstNameInput); });
            LastNameInput.onEndEdit.AddListener(delegate { InputSubmittedEvent(FirstNameInput); });
            EmailInput.onEndEdit.AddListener(delegate { InputSubmittedEvent(FirstNameInput); });
        }

        LoadQueue();
        LoadCurrentPlayer();

        StartCoroutine(CheckCurrentPlayerFile());
    }

    ScoreData? CurrentPlayer;
    ScoreData? PreviousPlayer;

    public void LoadCurrentPlayer()
    {
        List<ScoreData> s = RegistrationFileManager.LoadRegistrants(GetCurrentPlayerFileName(), UseEncryption);
        
        if (s.Count > 0)
            SetCurrentPlayer(s[0]);
    }

    public void SetCurrentPlayer(ScoreData _sd)
    {
        PreviousPlayer = CurrentPlayer;

        RemoveRegistrant(_sd);

        SetCurrentPlayerValue(_sd);

        Debug.Log("RegistrationForm:SetCurrentPlayer");
        if (CurrentPlayerUpdatedHandler != null)
            CurrentPlayerUpdatedHandler();
    }

    private void SetCurrentPlayerValue(ScoreData _cur)
    {
        CurrentPlayer = _cur;
        List<ScoreData> currentPlayerList = new List<ScoreData>();
        currentPlayerList.Add((ScoreData)CurrentPlayer);

		if (currentPlayerList.Count != 0)
        	RegistrationFileManager.SaveRegistrants(GetCurrentPlayerFileName(), currentPlayerList, UseEncryption);
    }

    public bool HasCurrentPlayer()
    {
        if (CurrentPlayer == null)
            return false;
        else
            return true;
    }

    public ScoreData GetCurrentPlayer()
    {
        return (ScoreData)CurrentPlayer;
    }

    /// <summary>
    /// Removes the current player if the file has been removed by the VR game. UI update will reflect this change.
    /// </summary>
    public void CurrentPlayerRemovedByGame()
    {
        CurrentPlayer = null;

        if (CurrentPlayerUpdatedHandler != null)
            CurrentPlayerUpdatedHandler();
    }

    const float CheckCurrentPlayerFileDelayTime = 0.5f;

    IEnumerator CheckCurrentPlayerFile()
    {
        while (true)
        {
            yield return new WaitForSeconds(CheckCurrentPlayerFileDelayTime);
            if (CurrentPlayer != null)
            {
                List<ScoreData> s = RegistrationFileManager.LoadRegistrants(GetCurrentPlayerFileName(), UseEncryption);
                if (s.Count <= 0)
                {

                    CurrentPlayerRemovedByGame();
                    Debug.Log("removed by game " + RegistrationFileManager.FileExists(GetCurrentPlayerFileName()));
                    
                }
                else
                {
                    Debug.Log("Still have current player " + RegistrationFileManager.FileExists(GetCurrentPlayerFileName()));
                }
            }
            else
            {
                Debug.Log("Current player null");
            }
        }
    }

    public void RevertCurrentPlayer()
    {
        AddRegistrant((ScoreData)CurrentPlayer);

        if (PreviousPlayer == null)
        {
            CurrentPlayer = null;

            RegistrationFileManager.SaveRegistrants(GetCurrentPlayerFileName(), new List<ScoreData>(), UseEncryption);

            if (CurrentPlayerUpdatedHandler != null)
                CurrentPlayerUpdatedHandler();

            return;
        }

        SetCurrentPlayerValue((ScoreData)PreviousPlayer);
         
        PreviousPlayer = null;

        if (CurrentPlayerUpdatedHandler != null)
            CurrentPlayerUpdatedHandler();

    }

	public YesNoDialogue YesNoDialogue;
    public void SendDetailsButtonPressed()
    {
		YesNoDialogue.Open ("Send details to server", "This will delete the current queue and send the details to the server, are you sure you want to do this?", SendDetails);
        
    }

	public void SendDetails(YesNoDialogueResponse _response) 
	{
		if (_response == YesNoDialogueResponse.No)
			return;
		
		for (int i = 0; i < Registrants.Count; i++)
		{
			SendScore(Registrants[i]);
		}

		Registrants.Clear();
		UpdateRegistrants();

	}

    private void SendScore(ScoreData _data)
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
        
        Debug.Log("Sending score" + _data.ToCSV());
        css.SendScore(_data);
    }

    public bool UseEncryption = false;

    public void LoadQueue()
    {
        Registrants = RegistrationFileManager.LoadRegistrants(GetDefaultFileName(), UseEncryption);

        if (RegistrantsUpdatedHandler != null)
            RegistrantsUpdatedHandler();
    }

    public string GetDefaultFileName()
    {
        return RegistrationFileDirectory + RegistrationFileName;
    }
    
    public string GetCurrentPlayerFileName()
    {
        return RegistrationFileDirectory + CurrentPlayerFileName;
    }
    public List<ScoreData> GetRegistrants()
    {
        return Registrants;
    }

    public void InputSubmittedEvent(InputField _input)
    {
        Debug.Log("recieved event with text " + _input.text);
    }

    public void SubmitButtonPressed()
    {
        Debug.Log("Submit button pressed");

        // TODO: Add more validation
        // TODO: Get validation criteria (is email mandatory?)
        if (!ValidateForm())
            return;

        ScoreData rd = new ScoreData(FirstNameInput.text, LastNameInput.text, EmailInput.text);

        ClearForm();

        /// This is only here for debug purposes, we don't need to store when the player queued up.
        rd.Date = string.Format("{0:yy.MM.dd}", System.DateTime.Now);
        rd.Hour = System.DateTime.Now.Hour;

        
        Registrants.Add(rd);

        RegistrationFileManager.SaveRegistrants(GetDefaultFileName(), Registrants, UseEncryption);

        if (RegistrantsUpdatedHandler != null)
            RegistrantsUpdatedHandler();

        Debug.Log("Added entry " + rd.ToString());
    }

    public bool ValidateForm()
    {
        if (FirstNameInput.text == "" && LastNameInput.text == "" && EmailInput.text == "")
        {
			DialogueText.text = "No details input";
			DialogueWindow.Show ();
            Logger.LogWarn("No details input");
            return false;
        }

		if (FirstNameInput.text.Length < 2) {
			DialogueText.text = "Name must be at least two characters long";
			DialogueWindow.Show ();
			Logger.LogWarn("Name must be at least two characters long");
			return false;
		}

        if (!string.IsNullOrEmpty(EmailInput.text))
        {
            if (!EmailIsValid(EmailInput.text))
            {
                DialogueText.text = "Email is invalid";
                DialogueWindow.Show();

                Logger.LogWarn("Email is invalid");
                return false;
            }
        }

        return true;
    }

    public void UpdateRegistrants()
    {
        RegistrationFileManager.SaveRegistrants(GetDefaultFileName(), Registrants, UseEncryption);

        if (RegistrantsUpdatedHandler != null)
            RegistrantsUpdatedHandler();
    }

    public void AddRegistrant(ScoreData _sd)
    {
        Registrants.Add(_sd);
        
        UpdateRegistrants();
    }

    public void RemoveRegistrant(ScoreData _sd)
    {
        Registrants.Remove(_sd);

        UpdateRegistrants();
    }

    public void ClearForm()
    {
        FirstNameInput.text = "";
        LastNameInput.text = "";
        EmailInput.text = "";
    }

    public void SelectNextItem()
    {
        if (FirstNameInput.isFocused)
            LastNameInput.Select();
        if (LastNameInput.isFocused)
            EmailInput.Select();
        else if (EmailInput.isFocused)
            SubmitButton.Select();
        else
            FirstNameInput.Select();
    }
    public void Update()
    {

        // Changes the focus on tab key press, makes it feel more like a browser / windows form.
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            SelectNextItem();
        }
    }

}


using UnityEngine;
using UnityEngine.UI;
using System.Collections;


public enum YesNoDialogueResponse
{
    Yes,
    No,
}

[RequireComponent(typeof(MenuBase))]
public class YesNoDialogue : MonoBehaviour
{

    public Text TitleText;
    public Text MessageText;
    public Text ButtonYesText;
    public Text ButtonNoText;

    private MenuBase mMenuBase;

    static YesNoDialogue Instance;

    public YesNoDialogueResponse CurrentDialogueResponse;
    private void Awake()
    {
        if (Instance != null)
        {
            if (Instance != this)
                Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }

    }

    private void OnEnable()
    {
        mMenuBase = gameObject.GetComponent<MenuBase>();
    }

    public delegate void Callback(YesNoDialogueResponse _response);
    public Callback MyCallback;

    public static void Open(string _title, string _message, Callback _callback)
    {
        Instance.OpenInstance(_title, _message, _callback);
    }

    public static void Open(string _title, string _message, Callback _callback, string _yesText, string _noText)
    {
        Instance.OpenInstance(_title, _message, _callback, _yesText, _noText);
    }


    public void OpenInstance(string _title, string _message, Callback _callback)
    {
        TitleText.text = _title;
        MessageText.text = _message;
        MyCallback = _callback;

        mMenuBase.Show();
    }

    public void OpenInstance(string _title, string _message, Callback _callback, string _yesText, string _noText)
    {
        ButtonNoText.text       = _noText;
        ButtonYesText.text      = _yesText;

        OpenInstance(_title, _message, _callback);
    }

    public void YesButtonPressed()
    {
        CurrentDialogueResponse = YesNoDialogueResponse.Yes;

        mMenuBase.Hide();

        TriggerCallback();
    }

    public void NoButtonPressed()
    {
        CurrentDialogueResponse = YesNoDialogueResponse.No;

        mMenuBase.Hide();

        TriggerCallback();
    }

    private void TriggerCallback()
    {
        if (MyCallback != null)
            StartCoroutine(CallbackCoroutine(MyCallback));

        // Make super sure we don't call this later.
        MyCallback = null;
    }

    // The users function is called seperatly to the notification Button pressed, this is in case the button pressed event calls
    // the notification screen code
    IEnumerator CallbackCoroutine(Callback _func)
    {
        yield return new WaitForEndOfFrame();
        _func(CurrentDialogueResponse);
    }
}

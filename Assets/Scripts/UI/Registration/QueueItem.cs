using UnityEngine;
using UnityEngine.UI;

public class NameItem : MonoBehaviour
{
    protected ScoreData Data;

    public Text FirstName;
    public Text LastName;

    protected bool mInitialized = false;

    public bool ValidateNameItem()
    {
        bool bValid = true;
        if (FirstName == null)
        {
            Logger.LogDebug("ValidateNameItem: FirstName text is null");
            bValid = false;
        }

        if (LastName == null)
        {
            Logger.LogDebug("ValidateNameItem: LastName text is null");
            bValid = false;
        }

        return bValid;
    }

    public void InitName(ScoreData _data)
    {
        //Debug.Log("Initing name  " + _data.ToCSV());
        FirstName.text = _data.FirstName;
        LastName.text = _data.LastName;

        Data = _data;
    }

}

public class QueueItem : NameItem {

    public Button NextButton;
    public Button RemoveButton;

    
    void OnEnable()
    {
        mInitialized = this.ValidateNameItem();
    }

    public void Init(ScoreData _data)
    {
        if (mInitialized)
            InitName(_data);
        else
            Logger.LogError("QueueItem is not initialized");

    }

    public void NextButtonPressed()
    {
        RegistrationForm.Instance.SetCurrentPlayer(Data);
    }

    public void RemoveButtonPressed()
    {
        Logger.LogDebug("RemoveButtonPressed");

        RegistrationForm.Instance.RemoveRegistrant(Data);
    }
}

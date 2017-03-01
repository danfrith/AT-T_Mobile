using UnityEngine;
using UnityEngine.UI;


public class CurrentPlayer : NameItem {

    public Button ReturnButton;

    void OnEnable()
    {
        mInitialized = this.ValidateNameItem();

        if (ReturnButton == null)
        {
            Logger.Log("CurrentPlayer: Missing return button ");
            mInitialized = false;
        }
    }

    public void Init(ScoreData _data)
    {
        if (mInitialized)
            InitName(_data);


    }

    public void ReturnToQueue()
    {
        RegistrationForm.Instance.RevertCurrentPlayer();
    }

}

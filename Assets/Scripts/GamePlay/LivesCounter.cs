using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LivesCounter : MonoBehaviour {

    public Image[] Lives;

    private MobileStage CurrentStage;

    void OnEnable()
    {
        CurrentStage = Stage.Instance as MobileStage;
        if (CurrentStage == null)
        {
            Debug.LogError("LivesCounter: Mobile Stage is missing");
            return;
        }

        CurrentStage.LivesChangedEventHandler += new MobileStage.LivesChangedEvent(LivesChanged);
    }

    void OnDisable()
    {
        if (CurrentStage != null)
            CurrentStage.LivesChangedEventHandler -= LivesChanged;
    }

    void LivesChanged(int _newValue)
    {
        Debug.Log("Lives changed---------- " + _newValue);
        for (int i =0; i < Lives.Length; i++)
        {
            if (_newValue < (i + 1))
                TriggerFlash(Lives[i]);
            else
                Lives[i].enabled = true;
        }

    }

    void TriggerFlash(Image _object)
    {
        Debug.Log("Triggering flash on " + _object.name);
        Flash f = _object.GetComponent<Flash>();
        f.FlashCompleteEvent += F_FlashCompleteEvent;
        f.StartFlash();
    }

    private void F_FlashCompleteEvent(object sender, System.EventArgs e)
    {
        Flash f = sender as Flash;
        Image i = f.GetComponent<Image>();
        i.enabled = false;
    }
}

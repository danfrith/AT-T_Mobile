using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class NoPlayerSetText : MonoBehaviour {

    private Text mText;
	void OnEnable()
    {
        mText = GetComponent<Text>();

        if (GameManager.Instance.StartWithoutCurrentPlayer)
            StartCoroutine(CheckPlayerRoutine());
    }

    IEnumerator CheckPlayerRoutine()
    {
        while(true)
        {
            yield return new WaitForFixedUpdate();

            if (string.IsNullOrEmpty(GameManager.Instance.PlayerName))
            {
                TutorialStage ts = Stage.Instance.GetComponent<TutorialStage>();
                if (ts != null)
                    mText.enabled = true;
            }
            else
            {
                mText.enabled = false;
            }
        }

    }

}

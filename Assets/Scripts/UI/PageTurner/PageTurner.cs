using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

[System.Serializable]
public class Page
{

    public string Name;
    public Transform WorldObjects;
	public Transform CanvasObjects;
    public Sprite Image;
    public IntroOutro Intro;
    public IntroOutro Outro;
	public string AudioClipName;

}

public enum IntroOutro
{
    Fade,
    SwipeLeft,
    SwipeRight,
    SwipeUp,
    SwipeDown,
}

/// <summary>
/// Sets a texture on a target and allows you to cycle between "pages" with optional animations in the transition.
/// </summary>
public class PageTurner : MonoBehaviour {

    [SerializeField]
    public Page[] Pages;

    public Image[] Panels;

    private int mCurrentPage = -1;

    private int mCurrentPanel = 0;

	private AudioSource mAudioPlayer;
    void OnEnable()
    {
        DisableAllWorldObjects();
		DisableAllCanvasObjects();

        //SetPage(0);

		mAudioPlayer = GetComponent<AudioSource> ();
		if (mAudioPlayer == null)
			mAudioPlayer = new AudioSource ();

		

        PageClosedEventHandler += PageTurner_PageClosedEventHandler;
    }

    private void PageTurner_PageClosedEventHandler(int _pageNo)
    {
    	Debug.Log("old page closed" + _pageNo + " " + Pages[_pageNo].Name);

       	if (mCurrentPage != _pageNo) // If the page has changed, we need to open the next page
    	    OpenPage(Panels[mCurrentPanel], mCurrentPage);

       // Otherwise we have closed a page without opening a new one.

    }

    public delegate void PageclosedEvent(int _pageNo);
    public event PageclosedEvent PageClosedEventHandler;

    private bool bTransitionInProgress;

	public void Hide()
	{
		Panels [0].gameObject.SetActive (false);
		Panels [1].gameObject.SetActive (false);

		DisableAllWorldObjects();
		DisableAllCanvasObjects();

	}

	public int GetCurrentPage()
	{
		return mCurrentPage;
	}
	
	public void Show()
	{
		bTransitionInProgress = false;

		Panels [0].gameObject.SetActive (true);
		Panels [1].gameObject.SetActive (true);
	}

    public void SetPage(int _pageNo)
    {
        if (bTransitionInProgress == true)
            return;

		Show ();

        bTransitionInProgress = true;

        if (mCurrentPage != -1)
        {
            Debug.Log("Setting Page to " + _pageNo + " " + Pages[_pageNo].Name + " Closing page " + mCurrentPage);
            ClosePage(Panels[mCurrentPanel], mCurrentPage);

            mCurrentPanel = 1 - mCurrentPanel;
            mCurrentPage = _pageNo;
        }
        else
        {
            Debug.Log("Setting Page to " + _pageNo + " " + Pages[_pageNo].Name);
            mCurrentPanel = 1 - mCurrentPanel;
            mCurrentPage = _pageNo;

            OpenPage(Panels[mCurrentPanel], mCurrentPage);
        }
    }

    public void DisableAllWorldObjects()
    {
        for (int i = 0; i < Pages.Length; i++)
        {
            if (Pages[i].WorldObjects != null)
                Pages[i].WorldObjects.gameObject.SetActive(false);
        }
    }
	public void DisableAllCanvasObjects()
	{
		for (int i = 0; i < Pages.Length; i++)
		{
			if (Pages[i].CanvasObjects != null)
				Pages[i].CanvasObjects.gameObject.SetActive(false);
		}
	}
		
	public bool NextPage()
    {
        if ((mCurrentPage + 1) >= Pages.Length)
            return false;

		Debug.Log ("Next page called " + (mCurrentPage + 1).ToString() +  " in progress = " + bTransitionInProgress);
        SetPage(mCurrentPage + 1);

        return true;
    }

    public bool PreviousPage()
    {
        if ((mCurrentPage - 1 ) < 0)
            return false;

        SetPage(mCurrentPage - 1);

        return true;
    }

    void OpenPage(Image _panel, int _pageNo)
    {
        StartCoroutine(OpenPageRoutine(_panel, _pageNo));
    }

    public void CloseCurrentPage()
    {
        ClosePage(Panels[mCurrentPanel], mCurrentPage);
    }

    void ClosePage(Image _panel, int _pageNo)
    {
        StartCoroutine(ClosePageRoutine(_panel, _pageNo));
    }

    IEnumerator OpenPageRoutine(Image _panel, int _pageNo)
    {
        float t = 1;
		Debug.Log ("Opening page " + _pageNo);

        while (t > 0)
        {
            t -= Time.fixedDeltaTime;

            _panel.sprite = Pages[_pageNo].Image;

            yield return new WaitForFixedUpdate();

            Color newCol = _panel.color;

            newCol.a = Ease.GetValue(EaseType.easeInOutSine, 1-t);
            //Debug.Log("closing Colour = " + newCol.a + " panel " + _panel.name);
            _panel.color = newCol;
        }

		Debug.Log ("Page fade up finished");

        if (Pages[_pageNo].WorldObjects!= null)
            Pages[_pageNo].WorldObjects.gameObject.SetActive(true);
		
		if (Pages[_pageNo].CanvasObjects!= null)
			Pages[_pageNo].CanvasObjects.gameObject.SetActive(true);

		if (string.IsNullOrEmpty (Pages [_pageNo].AudioClipName) == false) {
			if (AudioList.GetClip (Pages [_pageNo].AudioClipName) != null) {
				mAudioPlayer.clip = AudioList.GetClip (Pages [_pageNo].AudioClipName);
                Debug.Log("Playing audio for page " + _pageNo);
                mAudioPlayer.Play ();
			}
		}

		Debug.Log ("Ending transition");
        bTransitionInProgress = false;
    }

    IEnumerator ClosePageRoutine(Image _panel, int _pageNo)
    {
        float t = 1;

        if (Pages[_pageNo].WorldObjects != null)
            Pages[_pageNo].WorldObjects.gameObject.SetActive(false);

		if (Pages[_pageNo].CanvasObjects!= null)
			Pages[_pageNo].CanvasObjects.gameObject.SetActive(false);
		

        while(t > 0)
        {
            t -= Time.fixedDeltaTime;

            yield return new WaitForFixedUpdate();

            Color newCol = _panel.color;

            newCol.a = Ease.GetValue(EaseType.easeInExpo, t);
            //Debug.Log("closing Colour = " + newCol.a + " panel " + _panel.name);
            _panel.color = newCol;
        }

        if (mAudioPlayer.isPlaying) {
            Debug.Log("Stopping audio for page " + _pageNo);
            mAudioPlayer.Stop();
		}

        if (PageClosedEventHandler != null)
            PageClosedEventHandler(_pageNo);
    }

    public void PlayIntroOutro(Image _panel)
    {

    }
}

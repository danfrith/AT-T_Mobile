using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class ScoreBoard : MonoBehaviour {

    public Transform ScoreItemContainer;
    public Transform ScoreViewItemPrefab;
    public ScoreBoardReciever ScoreBoardManager;

    public ScoreManager ScoreBoardManagerMobile;

	public Text LoadingText;
    public bool MobileScoreDisplay = false;

    public bool ScoresLoaded = false;
    void OnEnable()
    {
        if (ScoreItemContainer == null)
            Logger.LogError("ScoreItemContainer is missing in Scoreboard " + name);

        if (ScoreViewItemPrefab == null)
            Logger.LogError("ScoreViewItemPrefab is missing in Scoreboard " + name);

        if (ScoreBoardManager == null)
            Logger.LogError("ScoreBoardManager is missing in Scoreboard " + name);
        else
            ScoreBoardManager.RegistrantsUpdatedHandler += new ScoreBoardReciever.RegistrantsUpdated(UpdateScores);

        if (MessageText == null)
            Logger.LogError("MessageText is missing in Scoreboard " + name);
        //StartCoroutine(RepeatTriggerUpdateScores());

        MessageText.gameObject.SetActive(false);

        if (MobileScoreDisplay)
        {
            if (ScoresLoaded == false)
            {
                // Try to load the scores from the score manager
				GameObject go = GameObject.Find("MobileGameManager");
				ScoreManager s = null;
				if (go != null) {
					s = go.GetComponent<ScoreManager> ();
				}
				if (s == null) {
					Debug.LogError ("Could not find score manager");
					return;
				}

                ScoreBoardManagerMobile = s;

                if (LoadingText != null)
					LoadingText.gameObject.SetActive (true);
				
				s.FetchScores (null, OnFailLoadScores);
				s.ScoresUpdatedHandler += MobileScoresUpdated;
            }

            //DebugInitScores();
            CacheViewPosition();
        }
        
    }

    public Transform MessageText;
    
    public void OnFailLoadScores()
    {
        LoadingText.gameObject.SetActive(false);
        MessageText.gameObject.SetActive(true);
    }

	void OnDisable()
	{
		GameObject go = GameObject.Find("MobileGameManager");
		ScoreManager s = null;
		if (go != null) {
			s = go.GetComponent<ScoreManager> ();
		}
		if (s == null) {
			Debug.LogError ("Could not find score manager");
			return;
		}
			
		s.ScoresUpdatedHandler -= MobileScoresUpdated;
	}

	void MobileScoresUpdated(List<ScoreData> _data)
	{
		Scores = _data;
		if (LoadingText != null)
			LoadingText.gameObject.SetActive (false);
		ClearItems ();
		PopulateList ();
	}

    float viewPosCache = 0;

    void CacheViewPosition()
    {
        RectTransform rt = ScoreItemContainer.GetComponent<RectTransform>();
        viewPosCache = rt.localPosition.y;
    }

    public float ScrollDistance = 50;

    public void ScrollUpPressed()
    {
        Debug.Log("SCrolling up");
        Scroll(-ScrollDistance);
    }

    public void ScrollDownPressed()
    {
        Scroll(ScrollDistance);
    }

    void Scroll(float _scrollDistance)
    {

        RectTransform rt = ScoreItemContainer.GetComponent<RectTransform>();
        Image img = ScoreItemContainer.GetComponent<Image>();

        float newY = Mathf.Clamp(rt.localPosition.y + _scrollDistance, viewPosCache, 99999);
        //Debug.Log("Scrolling by " + _scrollDistance + " from " + rt.localPosition.y + " to " + newY + " in " + ScoreItemContainer.name);

        //Debug.Log("Scrolling to " + newY);
        //rt.rect.Set(rt.rect.x, newY, rt.rect.height, rt.rect.width);

        img.rectTransform.rect.Set(rt.rect.x, newY, rt.rect.height, rt.rect.width);
        img.rectTransform.localPosition = new Vector3(img.rectTransform.localPosition.x, newY, img.rectTransform.localPosition.z);

    }

    void ClearItems()
    {

        Logger.Log("Clearing");

        // TODO possibly breaks if the indexes are updated during the loop
        for (int i = 0; i < ScoreItemContainer.childCount; i++)
        {
            GameObject.Destroy(ScoreItemContainer.GetChild(i).gameObject);
        }

    }

    private bool bUpdateScores = true;

    IEnumerator RepeatTriggerUpdateScores()
    {
        while (bUpdateScores)
        {
            yield return new WaitForSeconds(1);
            UpdateScores();
        }

    }

    #region TempScoreData
    public struct ScorePair
    {
        public string Name;
        public int Score;

        public ScorePair(string _name, int _score)
        {
            Name = _name;
            Score = _score;
        }

    }

    public List<ScoreData> Scores;

    public void DebugInitScores()
    {
        Logger.Log("Setting up debug scores");
        Scores = new List<ScoreData>();

        Scores.Add(new ScoreData("Shia", 347));
        Scores.Add(new ScoreData("Dina", 35405));
        Scores.Add(new ScoreData("Tobjorn", 549845));
        Scores.Add(new ScoreData("Simon", 9945));
        Scores.Add(new ScoreData("Ilyah", 48393));
        Scores.Add(new ScoreData("Clair", 2060129));
        Scores.Add(new ScoreData("Rei", 1594958));

        Scores.Sort(SortScores);

        ClearItems();
        PopulateList();

    }

    public List<ScoreData> GetScores()
    {
        if (Scores == null)
            DebugInitScores();

        return Scores;
    }

    public void ResetScores()
    {
        Scores = new List<ScoreData>();
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

    // This needs to be moved to scoreboard manager
    public void ExitApplication()
    {
        Application.Quit();
    }

    #endregion

    private void PopulateList()
    {
        List<ScoreData> currentScores = null;
        ScoreData LastScore;
        if (!MobileScoreDisplay)
        {
            currentScores = ScoreBoardManager.GetScores();
            LastScore = ScoreBoardManager.GetLastScore();
        }
        else
        {
            currentScores = Scores;
            LastScore = ScoreBoardManagerMobile.GetLastScore();
        }

        int count = 0;
        foreach (ScoreData a in currentScores)
        {
            count++;
            //Logger.Log("Adding item" + a.FirstName);

            if (a.Score == -1)
                continue;

            InstantiateScoreItem(count, a, LastScore.SameAs(a));
            
        }

        //AchievementGrid.repositionNow = true;
    }

    private Transform InstantiateScoreItem(int _index, ScoreData _item, bool _isSelected = false)
    {
        Transform t = (Transform)Instantiate(ScoreViewItemPrefab);
        t.transform.SetParent(ScoreItemContainer, false);

        ScoreBoardItem sb = t.gameObject.GetComponent<ScoreBoardItem>();
        if (sb == null)
        {
            Logger.LogError("Scoreboard prefab item spawned without a ScoreBoardItem component " + _item.FirstName);
            return t;
        }
        else
            sb.SetItem(_index, _item.Score, _item.FirstName, _item.LastName, _isSelected);

        return t;

    }

    public void UpdateScores()
    {
        ClearItems();
        PopulateList();
    }
}

using UnityEngine;
using System.Collections;

public class MobileStage : Stage {


    public bool CompletedStage = false;

	public int Lives = 3;
	public void DebugFailLevelPressed()
    {
        EndStage();
    }

    public void DebugCompleteLevelPressed()
    {
        CompletedStage = true;
        EndStage();
    }

    void OnEnable()
    {
        Spawner = MobSpawner.Instance;
        // We need to convert this to the format for the level files. 
        int level = MobileGameManager.Instance.CurrentLevel-1;
        int world = level / 5;
        Logger.Log("MobileStage: Current level " + level + " world " + world);
		world = world < 0 ? 0 : world;
		level = level < 0 ? 0 : level;
		LevelFile = string.Format("MobileLevel{0}World{1}", level, world);
        BeginStage();


    }

	public delegate void LivesChangedEvent(int _newLives);
	public event LivesChangedEvent LivesChangedEventHandler;

    //public void SpawnedType(VirusType _type)
    //{
    //    base.SpawnedType(_type);

    //    OnVirusEscape();
    //}

	public void OnVirusEscape()
	{
		if (MobileGameManager.Instance.GodMode)
			return;

		Lives--;

        Debug.Log("Virus escaped " + Lives);

        if (Lives < 0) // Stage failed
			EndStage (); 
		
		if (LivesChangedEventHandler != null)
			LivesChangedEventHandler (Lives);
		
	}

    override public void BeginStage()
    {
        Debug.Log("Starting stage");

        this.StageDuration = MobSpawner.Instance.GetStageDuration(LevelFile);

        SpawnCountdownMobile();

        GameObject go = GameObject.Find("Music");
        AudioClipPlayer ac = null;
        if (go != null)
        {
            ac = GetComponent<AudioClipPlayer>();
        }

        if (ac != null)
        {
            //Debug.Log("Got AC " + ac.ToString() + " &&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&");
            ac.Stop();
            ac.SetClip("BGM2A");
            ac.SetLooping(false);
            ac.PlayMain();

        }

        StartCoroutine(StartStageCoroutine(3));
    }
    public MobSpawner Spawner;
    public string LevelFile;

    IEnumerator StartStageCoroutine(float _delay)
    {
        yield return new WaitForSeconds(_delay);

        StartCoroutine(stageTimer());

        Spawner.SetInstructionList(LevelFile);

        GameObject go = GameObject.Find("Music");
        AudioClipPlayer ac = null;
        if (go != null)
        {
            ac = GetComponent<AudioClipPlayer>();
        }
        if (ac != null)
        {
            ac.Stop();
            ac.SetClip("BGM2B");
            ac.SetLooping(true);
            ac.PlayMainFadeOut(StageDuration, 2);
        }

    }

    protected void SpawnCountdownMobile()
    {
        Debug.Log("Spawning countdown");

        GameObject parent = GameObject.Find("CountdownContainer");

        RectTransform containerRect = parent.GetComponent<RectTransform>();

        GameObject Item = ((Transform)Instantiate(CountdownTextPrefab, parent.transform.position, Quaternion.identity, parent.transform)).gameObject;

        RectTransform ItemRect = Item.GetComponent<RectTransform>();
        Rect newRect = containerRect.rect;

        ItemRect.sizeDelta = new Vector2(400, 400);
        ItemRect.localScale = Vector3.one;

        ItemRect.localRotation = Quaternion.identity;


        Item.transform.position = parent.transform.position;
        Debug.Log("countdown completed");
    }

    bool Completed = false;

    override public void EndStage()
    {
        Debug.Log("---- Staging ending -------");
        if (Completed == true)
            return;

        if (Lives > 0)
            CompletedStage = true;

        GameObject go = Resources.Load("PlayClipPrefab") as GameObject;
        Transform prefab = go.transform;

        if (prefab != null)
        {
            Transform t = (Transform)Instantiate(prefab, transform.position, Quaternion.identity);
            AudioClipPlayer p = t.GetComponent<AudioClipPlayer>();
            p.PlayAndDestroy("GameFinished");
        }

        go = GameObject.Find("Music");
        if (go != null)
        {
            go.GetComponent<AudioSource>().Pause();

        }

        Completed = true;

        mRemainingTime = -1;

        Spawner.EndLevel();
        Spawner.DestroyAllEnemies();
        //DebugPrintScoreValues();

        Destroy(Spawner.gameObject);

        //StartCoroutine(NotifyGameManagerDelayed());
		StartCoroutine(OpenEndGameScreen());
    }

	public void NotifyGameManagerFromUI()
	{
		MobileGameManager.Instance.StageEnded(CompletedStage, GetScore());
	}

    IEnumerator NotifyGameManagerDelayed()
    {
        yield return new WaitForSeconds(5);
        MobileGameManager.Instance.StageEnded(CompletedStage, GetScore());
    }

	IEnumerator OpenEndGameScreen()
	{
		yield return new WaitForSeconds(1);
		GameObject go = GameObject.Find ("EndGameScreen");
		if (go == null) {
			Debug.LogError ("MobileStage: Could not find EndGameScreen");
			yield break;
		}

		MenuBase e = go.GetComponent<MenuBase> ();
		e.Show ();
	}
    
}




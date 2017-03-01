using UnityEngine;
using System.Collections;
using System.Collections.Generic;


class QueuedSpawn
{
	public Transform Prefab;
	public string Type;
	public Vector3 Position;
}

public enum SpawnInstructionType
{
	SPAWN_UNIT,
	CHANGE_DIRECTION,
	DELAY,
	ROTATE_180,
	ROTATE_90,
	STOP_ROTATION,
	START_ROTATION,
	END_LEVEL,
	DELAY_DEAD, // Delays until all the units are dead
    SET_POSITION, // Sets the specified spawn to a position 0-1 along it's path points (the float is using Delay time


}

[System.Serializable]
public class SpawnInstruction
{
	[SerializeField]
	public SpawnInstructionType Type;
	[SerializeField]
	public string UnitType;
	[SerializeField]
	public float DelayTime; 
	[SerializeField]
	public int Quantity;
    [SerializeField]
    public string Spawner;

    public string ToCSVString()
	{
//        string s = Type.ToString();
//        s = UnitType.ToString();
//        s = DelayTime.ToString();
//        s = Quantity.ToString();
		 
        return Type.ToString () + "," + UnitType.ToString () + "," + DelayTime.ToString () + "," + Quantity.ToString() + "," + Spawner;
        //return Type.ToString() + "," + UnitType.ToString() + "," + DelayTime.ToString() + "," + Quantity.ToString();
    }

	public SpawnInstruction ()
	{
		
	}

	public SpawnInstruction (string _csv)
	{
		//try{
		//SpawnInstruction si = new SpawnInstruction ();
		List<string> items = Utils.Utilities.CSVLineToList(_csv);

		if (items.Count < 3) {
				Debug.LogError ("Item not correct size only " + items.Count + " from " + _csv);
			return;
		}

		if (float.TryParse (items [2], out DelayTime) == false) {
			Debug.LogError ("Failed to load float value " + items [2]);
		}

        UnitType = items [1];

        Type = (SpawnInstructionType) System.Enum.Parse(typeof(SpawnInstructionType), items[0], true);

			if (int.TryParse (items [3], out Quantity) == false) {
				Debug.LogError ("Failed to load int value[3] " + items [3].ToString() + " from " + _csv);
				return;
			}

            if (items.Count > 3)
            {
                Spawner = items[4];
                //if (Spawner.Length > 5)
                //   Debug.Log("********************* loaded " + this.ToCSVString());

                //Debug.Log("********************* loaded " + si.Spawner);
            }

  //      }
		//catch (System.Exception e) {
		//	Debug.LogError ("Failed to create SpawnInstruction from CSV Error: " + e.Message + "\nFrom string: " + _csv);
		//}

    }

	public static SpawnInstruction FromCSV(string _csv)
	{
		
		SpawnInstruction si = new SpawnInstruction ();
		List<string> items = Utils.Utilities.CSVLineToList(_csv);
		if (items.Count != 3) {
			Debug.LogError ("Item not correct size"  + " from " + _csv);
			return null;
		}
		
		if (float.TryParse (items [2], out si.DelayTime) == false) {
			Debug.LogError ("Failed to load float value " + items [2] + " from " + _csv);
			return null;
		}

        

		si.UnitType = items [1];
		si.Type = (SpawnInstructionType) System.Enum.Parse(typeof(SpawnInstructionType), items[0], true);

		return si;
	}
}

class SpawnInstructionData
{
	void Save()
	{
		//data = FileHandler.LoadFile ("player.sav");

	}

	public List<SpawnInstruction> ParseFile(string _file)
	{
		//_file = System.Text.RegularExpressions.Regex.Replace (_file, "\n", string.Empty);
		List<string> lines = new List<string>( _file.Split (new [] { '\r', '\n' }));

		List<SpawnInstruction> instructions = new List<SpawnInstruction> ();

		for (int i = 0; i < lines.Count; i++) {
			if (lines [i].Length > 2) {
				SpawnInstruction si = new SpawnInstruction (lines [i]);	
				instructions.Add (si);
			}
		}

		return instructions;
	}

	[SerializeField]
	public Dictionary<string, List<SpawnInstruction>> LevelsList = new Dictionary<string, List<SpawnInstruction>>();

	public void Load()
	{
		//data = FileHandler.LoadFile("player.sav") as TextAsset;

		var files = Resources.LoadAll ("SpawnData");


		for (int i = 0; i < files.Length; i++) {
            TextAsset file = files [i] as TextAsset;
            Debug.Log("Found file " + file.name);
            List<SpawnInstruction> list = ParseFile (file.text);
			LevelsList.Add (file.name, list);
            Debug.Log("finished loading file " + file.name);
        }

	}

}

[System.Serializable]
public class SpawnArea
{
	[SerializeField]
	public LevelEnum Level;

	[SerializeField]
	public Transform Transforms;
}

public class MobSpawner : MonoBehaviour 
{

	#region Singleton
	public static MobSpawner Instance;

	void Awake()
	{
		if (Instance != null)
		{
			Destroy(this.gameObject);
			return;

		}

		Instance = this;
        // TODO Check if this affects the VR build
		//DontDestroyOnLoad(gameObject);


	}

	void OnDestroy()
	{
		if (Instance == this)
		{
			Instance = null;
		}
	}

	#endregion

	public Transform[] Prefabs = new Transform[0];
	private Dictionary<string, Transform> PrefabList = new Dictionary<string, Transform>();

	public bool isMobile = false;
	public SpawnArea[] SpawnAreas;
    public Transform[] SpawnPoints;

    protected Dictionary<string, Transform> SpawnPointLookup = new Dictionary<string, Transform>();
    
    //public StageShooter.LevelController CurrentLevelController;

    [SerializeField]
	public Dictionary<string, List<SpawnInstruction>> LevelsList = new Dictionary<string, List<SpawnInstruction>>();

	public string SelectedLevel = "level2";
	void OnEnable()
	{

		SpawnInstructionData sid = new SpawnInstructionData ();

		sid.Load ();

		LevelsList = sid.LevelsList;
        Debug.Log("Found " + LevelsList.Count + " levels");

		foreach (Transform prefab in Prefabs) {

			PrefabList.Add (prefab.name, prefab);
            //Logger.LogDebug("Prefab added" + prefab.name);
		}

        //SetInstructionList("level6");
		//SetInstructionList(SelectedLevel);

		if (isMobile == true)
			SetSpawnLocations ();

        LoadSpawnPointNames();

    }
	
	public void SetSpawnLocations()
	{
		int level = (MobileGameManager.Instance.CurrentLevel-1) % 5;

		if (SpawnAreas == null || SpawnAreas.Length == 0) {
			Debug.LogError ("MobSpawner: No spawn areas found");
		}

		foreach (SpawnArea s in SpawnAreas)
		{
			if (s.Level == (LevelEnum)level)
			{
				List<Transform> t = new List<Transform> ();
				Debug.Log ("Setting spawn points from area " + s.Transforms.name);

                s.Transforms.gameObject.SetActive(true);

                for (int i = 0; i <s.Transforms.childCount; i++)
					t.Add(s.Transforms.GetChild(i));
				
				SpawnPoints = t.ToArray ();
				break;
			}
		}
	}

    public void LoadSpawnPointNames()
    {
        SpawnPointLookup = new Dictionary<string, Transform>();
        for (int i = 0; i < SpawnPoints.Length; i++)
        {
            SpawnPointLookup.Add(SpawnPoints[i].name, SpawnPoints[i]);

            // Debug here -- Hide the cubes when the game runs
            if (isMobile == false)
            {
                SpawnerAnimator s = SpawnPoints[i].GetComponent<SpawnerAnimator>();
                if (s == null)
                    SpawnPoints[i].GetComponent<MeshRenderer>().enabled = false;
            }
        }
    }

    public float LevelTime = 0;
    float EndLevelFudge = 6.5f; // How much time to add to the end of a level file. The file "ends" when the last item is spawned, but we want it to finish traveling toward the player
    
    public float GetStageDuration(string _levelName)
    {
        float totalTime = 0;
        if (isMobile) // This needs changes to work with the VR version.
        {
            if (LevelsList.ContainsKey(_levelName) == false)
            {
                Debug.LogError("Key " + _levelName + " is not in levels list");
                return 30f;
            }

            foreach (SpawnInstruction s in LevelsList[_levelName])
            {
                totalTime += s.DelayTime;
            }

            totalTime += EndLevelFudge;
        }
        return totalTime;
    }

	public bool SetInstructionList(string _levelName)
	{
		if (LevelsList.ContainsKey (_levelName) == false) {
			Debug.LogError ("Level not found " + _levelName);
			return false;
		}
		SelectedLevel = _levelName;
		InstructionsList = LevelsList [_levelName];
		InstructionCount = 0;
		ActionDelay = 0;
		bWaitForEnemyElimination = false;

        

		mEnemies = new List<VirusBase> ();
		return true;
	}
	public Rotate SpawnerArm;
	public Transform SpawnPosition;


	Transform Player;

	public List<SpawnInstruction> InstructionsList;

	List<VirusBase> EnemyList = new List<VirusBase> ();

	bool bFindingPlayer = false;


	int InstructionCount = 0;
	public float ActionDelay = 0;

	void ChangeDirection()
	{
		Debug.Log ("Change direction");
		SpawnerArm.rotation.z = -SpawnerArm.rotation.z;
	}

	float debugVar = 0;

	void Rotate(float _angle)
	{
		float rotation = -1*_angle*Mathf.Sign(SpawnerArm.rotation.z);
		//float r = SpawnerArm.transform.rotation.eulerAngles.z + rotation;
		//Debug.Log ("Rotate " + rotation.ToString() + " from angle " + SpawnerArm.transform.rotation.eulerAngles.z + " to " + r);
		//SpawnerArm.transform.rotation = Quaternion.Euler (SpawnerArm.transform.rotation.x, SpawnerArm.transform.rotation.y, SpawnerArm.transform.rotation.z + rotation);
		Quaternion rot = SpawnerArm.transform.rotation;
		rot.eulerAngles = new Vector3(SpawnerArm.transform.rotation.x, SpawnerArm.transform.rotation.y, SpawnerArm.transform.eulerAngles.z + rotation);
		SpawnerArm.transform.rotation = rot;
	}

	void SpawnUnit(SpawnInstruction _instruction)
	{
		//Debug.Log ("Spawn" + _instruction.UnitType + " Quantity " + _instruction.Quantity);
		ActionDelay = _instruction.DelayTime * _instruction.Quantity;

		for (int i = 0; i < _instruction.Quantity; i++)
		{
            //Logger.LogDebug("Spawning {0} at {1}", _instruction.UnitType, _instruction.Spawner);
			StartCoroutine(SpawnUnityDelayed (_instruction.UnitType,_instruction.DelayTime*i, _instruction.Spawner));
		}
	}

	public void ExecuteInstruction(SpawnInstruction _instruction)
	{
		InstructionCount++;

		switch (_instruction.Type) {

			case SpawnInstructionType.CHANGE_DIRECTION:
				ChangeDirection ();
				ActionDelay = _instruction.DelayTime;	
			break;
		case SpawnInstructionType.DELAY:
			    ///Debug.Log ("Delay action" + _instruction.DelayTime);
				ActionDelay = _instruction.DelayTime;	
				break;
			case SpawnInstructionType.ROTATE_180:
				Rotate (180);
				ActionDelay = _instruction.DelayTime;	
				break;
			case SpawnInstructionType.ROTATE_90:
				Rotate (90);
				ActionDelay = _instruction.DelayTime;	
				break;
			case SpawnInstructionType.STOP_ROTATION:
				SetRotationSpeed (0);
				ActionDelay = _instruction.DelayTime;	
			break;
			case SpawnInstructionType.START_ROTATION: // TODO Add var for speed
				SetRotationSpeed (1);
				ActionDelay = _instruction.DelayTime;	
			break;
			case SpawnInstructionType.SPAWN_UNIT:
				SpawnUnit (_instruction);
				break;
			case SpawnInstructionType.DELAY_DEAD:
				DelayDead ();
				ActionDelay = _instruction.DelayTime; // Creates a "minimum" time between the next spawn
				break;
            case SpawnInstructionType.SET_POSITION:
                //Debug.Log("----------- Setting position for " + _instruction.ToCSVString());
                SetSpawnerPosition(_instruction);
                ActionDelay = _instruction.DelayTime; // Creates a "minimum" time between the next spawn
                break;
            case SpawnInstructionType.END_LEVEL:
			Debug.Log ("Reached end level instruction");
				EndLevel(_instruction);
				break;
		default:
			Debug.LogError ("Could not find instruction " + _instruction.Type);
			ActionDelay = _instruction.DelayTime;	
		break;
		}

	}

	private List<string> IndexedSpawnList;
	public Transform GetSpawnPoint(string _name)
	{
		if (IndexedSpawnList == null) {
			IndexedSpawnList = new List<string> ();
			foreach (string key in SpawnPointLookup.Keys) {
				IndexedSpawnList.Add (key);
			}
		}

		if (SpawnPointLookup.ContainsKey (_name) == false) {
			Logger.LogError ("Spawner " + _name + " does not exist");
			int item = Random.Range (0, IndexedSpawnList.Count);
			Transform transform = SpawnPointLookup [IndexedSpawnList [item]];
			if (transform == null)
				return SpawnPosition;
			else
				return transform;
					
		}

		return SpawnPointLookup[_name];
	}

    private void SetSpawnerPosition(SpawnInstruction _instruction)
    {
		Debug.Log (_instruction.ToCSVString ());
        if (SpawnPointLookup.ContainsKey(_instruction.Spawner) == false)
        {
            Logger.LogError("Spawner: Spawner id " + _instruction.Spawner + " was not found");
            return;
        }

		Transform Spawner = GetSpawnPoint(_instruction.Spawner);

        PointToPoint p = Spawner.GetComponent<PointToPoint>();
		if(p!=null){p.SetPosition(_instruction.DelayTime);}
        
    }

    /// <summary>
    /// 
    /// Todo:
    /// Add spawner list
    ///     Add a temporary random spawn selector
    ///     Add a spawn selector that uses the object name/
    ///     add a line in the CSV for spawn name
    /// 
    /// Update positional controls to specify spawner
    /// Add PointToPoint type instructions to the spawner
    ///     Get the p2p component, 
    ///     or get the rotation component,
    ///     apply the instruction relative to the type of component
    /// 
    /// 
    /// </summary>

	private bool bWaitForEnemyElimination = false;

	private void DelayDead()
	{
		//bWaitForEnemyElimination = true;
	}

	public void EnemiesEliminated()
	{
		Debug.Log ("Resume instructions");
		bWaitForEnemyElimination = false;
	}

    public void DestroyAllEnemies()
    {
        foreach (VirusBase virus in mEnemies)
        {
            if (virus != null)
                virus.VirusDestroyed();
        }
    }

	private void EndLevel(SpawnInstruction _instruction)
	{
		// Ending level
		Debug.Log("Calling end level");
		if (_instruction.DelayTime == 0) {
			// End level when there are no enemies left
			Debug.Log ("Ending level with 0 time");
			//CurrentLevelController.EndLevelAfterEnemiesDestroyed ();
		} else {
			//CurrentLevelController.EndLevel (_instruction.DelayTime);
			Debug.Log("Ending level with "+ _instruction.DelayTime + " time");
		}
		
	}
	void SetRotationSpeed(float _value)
	{
		SpawnerArm.rotation.z = _value;
	}

    public void EndLevel()
    {
        InstructionsList = null;
    }

	private bool debugValue = false;
	void Update()
	{
		if (InstructionsList == null)
			return;

		if (ActionDelay <= 0 && InstructionCount < InstructionsList.Count && bWaitForEnemyElimination == false) {
			
			ExecuteInstruction (InstructionsList [InstructionCount]);
		} else if (ActionDelay > 0) {
			ActionDelay -= Time.deltaTime;
		} else
		{
			if (debugValue == false) {
				Debug.Log ("End reached --------------------");
				debugValue = true;
			}
		}

		if (SpawnQueue.Count > 0) {
			QueuedSpawn sq = SpawnQueue.Dequeue();
			SpawnEnemy (sq.Type, sq.Position, transform);
		}
			
	}

	void FindPlayer()
	{
		if (bFindingPlayer == true)
			return;
		else
			bFindingPlayer = true;

		StartCoroutine (FindPlayerPing ());

	}

	IEnumerator FindPlayerPing()
	{
		while (Player == null) {
			GameObject go = GameObject.FindGameObjectWithTag ("Player");
			if (go != null)
				Player = go.transform;
			
			yield return new WaitForSeconds (0.5f);
		}
		bFindingPlayer = false;
	}

	void SpawnEnemyAtRelativePoint(string _type, Vector2 _position)
	{
		Vector3 spawnPos = Player.position;
		//Vector3 spawnPos = transform.position;

		spawnPos.x += _position.x;
		spawnPos.y += _position.y;
		//Debug.Log ("Spawn position = " + spawnPos);
		SpawnEnemy (_type, spawnPos, transform);
	}

	Queue<QueuedSpawn> SpawnQueue = new Queue<QueuedSpawn> ();

	void SpawnEnemyQueued(string _type, Vector2 spawnPos)
	{
		QueuedSpawn qs = new QueuedSpawn();

		qs.Position = spawnPos;
		qs.Prefab = GetEnemyPrefab(_type);
		qs.Type = _type;
		SpawnQueue.Enqueue (qs);
	}

	public Transform DebugPrototype; 

	Transform GetEnemyPrefab(string _type)
	{
		if (PrefabList.ContainsKey (_type) == false) {
			Logger.LogDebug("Cound not find unit type " + _type);

            return DebugPrototype;
		}
			
		return PrefabList[_type];
	}

	//public Transform MainCanvas;

	List <VirusBase> mEnemies = new List<VirusBase> ();

	IEnumerator SpawnUnityDelayed (string _type, float _delay)
	{
		yield return new WaitForSeconds (_delay);

        //SpawnEnemyAtRelativePoint(_type, SpawnPosition.position);

		SpawnEnemy(_type, SpawnPosition.position, SpawnPosition);
	}

	IEnumerator SpawnUnityDelayed (string _type, float _delay, string _spawnPoint)
	{
		yield return new WaitForSeconds (_delay);

		Transform spawnPoint = GetSpawnPoint (_spawnPoint);
		SpawnEnemy(_type, spawnPoint.position, spawnPoint);

	}

    IEnumerator SpawnUnityDelayed(string _type, float _delay, Transform _spawner)
    {
        yield return new WaitForSeconds(_delay);
        
		SpawnEnemy(_type, _spawner.position, _spawner);
    }

    VirusBase LocalSpawn(Transform _prefab, Vector3 _location, Transform _spawner)
    {

        Transform t = (Transform)Instantiate(_prefab, _location, Quaternion.identity);
        t.name = t.name + InstructionCount.ToString();
        t.parent = this.transform;

        //Debug.Log("Spawned " + _prefab.name + " at " + _spawner.name + " pos " + _location);
        InitializePerson(t, _spawner);

        VirusBase en = (VirusBase)t.gameObject.GetComponent<VirusBase>();

        return en;
    }

	void SpawnEnemy(string _type, Vector3 _location, Transform _spawner)
	{
        if (InstructionsList == null) // The list has been cleared.
            return; 

		Transform EnemyPrefab = GetEnemyPrefab (_type);

        ProxySpawner ps = EnemyPrefab.GetComponent<ProxySpawner>();
        SpawnerAnimator sa = _spawner.GetComponent<SpawnerAnimator>();
        VirusBase en;

        if (ps != null)
            en = LocalSpawn(EnemyPrefab, _location, _spawner);
        else if (sa != null)
            en = sa.DelegatedSpawn(EnemyPrefab, InstructionCount, _spawner);
        else
            en = LocalSpawn(EnemyPrefab, _location, _spawner);

        if (en != null)
		    mEnemies.Add(en);

        //AnimateSpawner(en, _spawner);
    }

    //public void AnimateSpawner(VirusBase _virus, Transform _spawner)
    //{
    //    SpawnerAnimator s = _spawner.GetComponent<SpawnerAnimator>();

    //    if (s != null)
    //        s.StartAnimator(_virus);
    //}

	public void InitializePerson(Transform _person, Transform _spawner)
	{
        Person p = _person.gameObject.GetComponent<Person>();

        if (p == null)
            return;
        
        PathList pl = _spawner.GetComponent<PathList>();

		if (pl == null) {
			Logger.LogError ("Pathlist not available on " + _spawner.name);
			return;
		}

        p.SetPath(pl.Paths[Random.Range(0, pl.Paths.Length)], pl.SelectedPath);
        p.transform.position = _person.transform.position + pl.SpawnOffset;

	}

    public void AddEnemy(VirusBase _enemy)
    {
        mEnemies.Add(_enemy);
    }

		
}
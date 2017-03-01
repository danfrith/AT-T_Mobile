using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public enum LevelEnum
{
    One,
    Two,
    Three,
    Four,
    Five,
}

public enum World
{
    One,
    Two,
    Three,
    Four,
}

/// TODO:
/// Write out the camera position headers 
/// 
/// 
/// 
public class MobileStageGenerator : MonoBehaviour {


    public VirusBase[] VirusTypes;

	void OnEnable()
	{
		List<string> ls = new List<string> ();
		ls.Add ("string");
		ls.Add ("string2");
		ls.Add ("string3");
		SaveFile ("mobileTestLevel", ls);
	}

	public void GenerateStages()
	{
		
	}

	string[] SpawnPoints;

	public void SpawnPointList()
	{
		SpawnPoints = new string[6];

		SpawnPoints [0] = "BottomLeft";
		SpawnPoints [1] = "BottomRight";
		SpawnPoints [2] = "MiddleLeft";
		SpawnPoints [3] = "MiddleRight";
		SpawnPoints [4] = "TopLeft";
		SpawnPoints [5] = "TopRight";
	}

	public bool TestSave = false;

    public void OnValidate()
    {

        Dictionary<int, List<string>> InstructionLists = new Dictionary<int, List<string>>();
        if (TestSave == true)
        {
            SpawnPointList();
            Debug.Log("Debug test");
            TestSave = false;

            int levelCount = 0;

            //GenerateStage(1, 1);

            for (int i = 0; i < 20; i++)
            {
                int world = i / 5;
                InstructionLists.Add(i, GenerateStage(i, world));

                Debug.Log("Generated instruction list " + i + " world " + world);

                SaveFile(string.Format("MobileLevel{0}World{1}.csv", i, world), InstructionLists[i]);
            }


            //for (int i = 0; i < 20; i++)
            //{
            //    int world = i / 5;
            //    int[] viruses = GetAllowedViruses(i, world);
            //    string virus = "";

            //    for (int j = 0; j < viruses.Length; j++)
            //        virus = virus + ", " + viruses[j];

            //    Debug.Log("Viruses for level " + i + " world " + world + " Allowed viruses" + virus);


            //}
            //GenerateStage(5, 1);
        }
    }

    public float AverageSpawnDelay = 1.80f;
    public float DifficultyIncreaseFactor = 0.5f;
    public int StageLength = 45;

    List<string> GenerateStage(int _level, int _world)
	{
		List<string> instructions = new List<string>();

		int[] AllowedViruses = GetAllowedViruses (_level, _world);

		int[] Weights = new int[AllowedViruses.Length];

		int totalWeight = 0;
		for (int i = 0; i < Weights.Length; i++) {
			Weights [i] = (((Weights.Length) - i) * ((Weights.Length - i)/2)) + 8;
            Debug.Log("Adding weight " + Weights[i]);
			totalWeight += Weights [i];
			Weights [i] = totalWeight;
		}

        System.Random VirusTypeRandom = new System.Random(_level + BaseSeed);
        System.Random DelayRandom = new System.Random(_level + BaseSeed);
        System.Random PositionRandom = new System.Random(_level + BaseSeed);
        
        float averageSpawnTime = AverageSpawnDelay * (1-(DifficultyIncreaseFactor * (_level / 20))); // The higher the level the faster the virii spawn on average.
        int TotalVirii = (int)(StageLength / AverageSpawnDelay);

        float bonusTime = 0;
        string s = "";

        AddHeaders(instructions);
        string spawnerSelection = "";
        for (int i = 0; i < TotalVirii; i++)
        {
            s = "SPAWN_UNIT";
            s = s + "," + GetVirusType(VirusTypeRandom, totalWeight, Weights, AllowedViruses, _level);
            s = s + "," + GetSpawnDelay(DelayRandom, averageSpawnTime, bonusTime, out bonusTime);
            s = s + ",1";
            spawnerSelection = GetSpawnLoactionExcludingSelected(PositionRandom, spawnerSelection); // Don't pick the some one twice in a row
            s = s + "," + spawnerSelection;
            instructions.Add(s);
            //Debug.Log("Generated instruction: " + s);
            s = "";
        }
        return instructions;
	}

    public void AddHeaders(List<string> _instructionList)
    {

    }

    public float GetSpawnDelay(System.Random _r, float _averageDelay, float _bonusTime, out float _newBonus)
    {
        _newBonus = 0f;
        int max = (int)( ( (_averageDelay * 2) + _bonusTime) * 1000);
        float t = (float)_r.Next(0, max) / 1000f;
        float remaining = _averageDelay - t;
        
        _newBonus = _bonusTime + remaining;

        //Debug.Log(string.Format("t: {0}, remaining: {1}, bonus: {2}, new: {3}, max {4}, avg: {5}", t, remaining, _bonusTime, _newBonus, max, _averageDelay));

        return t;
    }

	public int BaseSeed = 6598648;

    public string GetSpawnLoaction(System.Random _r)
    {
        return SpawnPoints[_r.Next(SpawnPoints.Length)];
    }
    
    public string GetSpawnLoactionExcludingSelected(System.Random _r, string _selection)
    {
        string newSelection = "";
        for (int i = 0; i < 20; i++)
        {
            newSelection = SpawnPoints[_r.Next(SpawnPoints.Length)];
            if (newSelection != _selection)
                return newSelection;
        }

        return newSelection;
    }

    public string GetVirusType(System.Random _r, int _totalWeight, int[] _weights, int[] _allowedViruses, int _level)
	{
		
		int value = _r.Next (_totalWeight+1);

		int virus = 0;

		for (int i = 0; i < _weights.Length; i++) {
            if (value <= _weights[i])
            {
                virus = _allowedViruses[i];
                break;
            }
		}

        return VirusTypes[virus].name;

	}

	public int[] GetAllowedViruses(int _level, int _world)
	{
		int[] AllowedViruses = new int[(_level % 5) + 2];

		for (int i = 0; i < AllowedViruses.Length; i++) {
			int virus = i + _world;
			virus = virus % 6;
			AllowedViruses [i] = virus;
		}

		return AllowedViruses;
	}

	public void SaveFile(string _fileName, List<string> _instruction)
	{
		try
		{
			FileStream fs = File.Open (_fileName, FileMode.OpenOrCreate);
			StreamWriter sw = new StreamWriter (fs);

			foreach (string s in _instruction) {
				sw.WriteLine (s);
			}

			sw.Close ();
			fs.Dispose ();
			Debug.Log("Succesfully wrote data");
		}
		catch(System.Exception e) {
			Debug.Log ("Failed top open file. Error " + e.Message + " Stack: " + e.StackTrace);
		}
	}



}

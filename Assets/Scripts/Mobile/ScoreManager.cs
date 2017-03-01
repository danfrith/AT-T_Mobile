using UnityEngine;
using UnityEngine.UI;
using System.Collections;

using System.Collections.Generic;
using System;
//using SimpleJSON;

using System.Net;
using System.Text;
using System.IO;

using Amazon;
using Amazon.CognitoSync;
using Amazon.Runtime;
using Amazon.CognitoIdentity;
using Amazon.CognitoIdentity.Model;
using Amazon.CognitoSync.SyncManager;

using FullSerializer;

public class DeveloperAuthenticatedCredentials : CognitoAWSCredentials
{
    const string PROVIDER_NAME = "example.com";
    const string IDENTITY_POOL = "IDENTITY_POOL_ID";
    static readonly RegionEndpoint REGION = RegionEndpoint.USEast1;

    private string login = null;

    public DeveloperAuthenticatedCredentials(string loginAlias)
        : base(IDENTITY_POOL, REGION)
    {
        login = loginAlias;
    }

    protected override IdentityState RefreshIdentity()
    {
        
        IdentityState state = new IdentityState(
        "4d0a83ad-12f7-400b-b605-03cf0356d9bd",
        false);
        //ManualResetEvent waitLock = new ManualResetEvent(false);
        //MainThreadDispatcher.ExecuteCoroutineOnMainThread(ContactProvider((s) =>
        //{
        //    state = s;
        //    waitLock.Set();
        //}));
        //waitLock.WaitOne();
        return state;
    }

}

public class ScoreManager : MonoBehaviour {

	public void ScoreAuthentication()
	{
		// 
	}

	private string playerName, alias;
	public string IdentityPoolId = "";

	public delegate void ScoresUpdated(List<ScoreData> _scores);
	public event ScoresUpdated ScoresUpdatedHandler;

	private CognitoAWSCredentials _credentials;

	private CognitoAWSCredentials Credentials
	{
		get
		{
			//CognitoAWSCredentials c = new CognitoAWSCredentials (acountid, identityPoolId, unAuthRoleArn, authRoleArn);
			if (_credentials == null)
				_credentials = new CognitoAWSCredentials(IdentityPoolId, _Region);
			return _credentials;
		}
	}

	public string Region = RegionEndpoint.USEast1.SystemName;

	private RegionEndpoint _Region
	{
		get { return RegionEndpoint.GetBySystemName(Region); }
	}

    void Start()
    {
        UnityInitializer.AttachToGameObject(this.gameObject);
    }

    // For debug purposes
    public void ConnectButtonPressed()
    {
        try
        {
            SyncManager.OpenOrCreateDataset("TestDataset");

            List<DatasetMetadata> sets = SyncManager.ListDatasets();
            foreach (DatasetMetadata set in sets)
            {

                Debug.Log("set.DatasetName = " + set.DatasetName);

            }
        }
        catch (System.Exception e)
        {
            Debug.Log("Cound not connect. Reason : " + e.Message + " Stack " + e.StackTrace);
        }
    }

    CognitoSyncManager syncManager;
    public Text DebugText;
    public void Test2()
    {
        
        DebugText.text = "";
        //CognitoAWSCredentials c = new CognitoAWSCredentials(IdentityPoolId, _Region);

        CognitoAWSCredentials credentials = new CognitoAWSCredentials(
        "us-west-2:94fe1193-1b5f-4eb5-a34e-c1adfa6f7c9c", // Identity Pool ID
        //"us-west-2:8435d39c-9b85-4404-ba67-3ab4c181b64a",
        RegionEndpoint.USWest2 // Region
        );

        
        //credentials.refre
        syncManager = new CognitoSyncManager(
            credentials,
            new AmazonCognitoSyncConfig
            {
                RegionEndpoint = RegionEndpoint.USWest2 // Region
            }
        );
        
        // Create a record in a dataset and synchronize with the server
        Dataset dataset = syncManager.OpenOrCreateDataset("myDataset");
        //dataset.OnSyncSuccess += SuccessCallback;
        dataset.OnSyncFailure += Dataset_OnSyncFailure;

        AddDebugText("Requesting sync");
        //dataset.Put("myKey", "three");

        dataset.SynchronizeAsync();

        List<DatasetMetadata> sets = syncManager.ListDatasets();
        foreach (DatasetMetadata set in sets)
        {

            Debug.Log("set.DatasetName = " + set.DatasetName);

        }
        
        credentials.GetIdentityIdAsync(delegate (AmazonCognitoIdentityResult<string> result) {
            if (result.Exception != null)
            {
                //Exception!
            }
            string identityId = result.Response;
            AddDebugText("identity ID");
        });
    }

	void OnEnable()
	{
		//StartCoroutine(GetScores());
	}

	IEnumerator Test() {

		yield return GetIndex ();

		Debug.Log ("Send first");
		yield return SendScore(new ScoreData("testName1", 454));
		Debug.Log ("Send second");
		yield return SendScore(new ScoreData("testName1", 2423));
		Debug.Log ("Send third");
		yield return SendScore(new ScoreData("testName1", 454));
		Debug.Log ("Send third");
		yield return SendScore(new ScoreData("testName2", 523424));
		Debug.Log ("Send third");
		yield return SendScore(new ScoreData("testName3", 54));
		Debug.Log ("Send third");
		yield return SendScore(new ScoreData("testName5", 3424));
	}

	private WWW mRequest;
	private int ScoreIndex = -1;
	IEnumerator GetIndex()
	{
		yield return UnityFectchRequestRoutine("https://52abm5d0id.execute-api.us-west-2.amazonaws.com/prod/GetIndex");
		

		 							/// This is what the return value looks like
		string json = mRequest.text; //"{\"statusCode\": \"200\", \"body\": \"{\"Items\":[{\"CurrentIndex\":0,\"Value\":5}],\"Count\":1,\"ScannedCount\":1}\",\"headers\": {\"Content-Type\": \"application/json\"}}";

		FullSerializer.fsData data = FullSerializer.fsJsonParser.Parse (json);

		int index = -1; 
		try 
		{
			index = (int)data.AsDictionary ["Items"].AsList [0].AsDictionary ["Value"].AsInt64;
		}
		catch (Exception e) {
			Debug.Log ("Could get index " + e.Message + " Stack : " + e.StackTrace);
            
			yield break;	
		}
		ScoreIndex = index;

		yield break;
	}

	string ExploreValues(fsData _data, string _tab)
	{
		if (_data.IsDictionary) {
			foreach (string key in _data.AsDictionary.Keys) {
				if (_data.AsDictionary [key].IsDictionary)
					Debug.Log ("key = " + key + " value = dic " + ExploreValues (_data.AsDictionary [key], _tab + "\t"));
				else if (_data.AsDictionary [key].IsBool)
					Debug.Log ("key = " + key + " value = bool " + _data.AsDictionary [key].AsBool.ToString ());
				else if (_data.AsDictionary [key].IsString)
					Debug.Log ("key = " + key + " value = string " + _data.AsDictionary [key].AsString.ToString ());
				else if (_data.AsDictionary [key].IsInt64)
					Debug.Log ("key = " + key + " value = int " + _data.AsDictionary [key].AsInt64.ToString ());
				else if (_data.AsDictionary [key].IsList) {
					Debug.Log ("key = " + key + " value = list, exploring");
					ExploreValues (_data.AsDictionary [key], _tab + "\t");
				}

			}
		} else if (_data.IsList)
			foreach (fsData item in _data.AsList) {
				if (item.IsDictionary)
					Debug.Log ("value = dic " + ExploreValues (item, _tab + "\t"));
				else if (item .IsBool)
					Debug.Log ("value = bool " + item .AsBool.ToString ());
				else if (item .IsString)
					Debug.Log ("value = string " + item .AsString.ToString ());
				else if (item .IsInt64)
					Debug.Log ("value = int " + item .AsInt64.ToString ());
				else if (item .IsList) {
					Debug.Log ("value = list, exploring");
					ExploreValues (item , _tab + "\t");
				}				
		}

		return "";
	}

	private List<ScoreData> mScores = null;

	public void FetchScores(Callback _success, Callback _fail)
	{
        SuccessCallback     = _success;
        FailCallback        = _fail;

		StartCoroutine (GetScores ());
	}

	IEnumerator GetScores()
	{
		yield return UnityFectchRequestRoutine("https://52abm5d0id.execute-api.us-west-2.amazonaws.com/prod/TestFunction");
		Debug.Log ("Text = " + mRequest.text);

		/// This is what the return value looks like
		string json = mRequest.text; //"{\"statusCode\": \"200\", \"body\": \"{\"Items\":[{\"CurrentIndex\":0,\"Value\":5}],\"Count\":1,\"ScannedCount\":1}\",\"headers\": {\"Content-Type\": \"application/json\"}}";

		fsData data = fsJsonParser.Parse (json);

		List<fsData> Items;
		List<ScoreData> Scores = new List<ScoreData> ();

        if (data.IsDictionary == false)
        {
            Debug.Log("Failed to load data");

            if (FailCallback != null)
                FailCallback();

            yield break;

        }
		
		try 
		{
            data = fsJsonParser.Parse(data.AsDictionary["body"].AsString);
            //ExploreValues(data, "");

            Items = data.AsDictionary ["Items"].AsList;
			foreach(fsData item in Items)
			{
				try{
					string name = item.AsDictionary["Name"].AsString;
					string score = (string)item.AsDictionary["Score"].AsString;
					Debug.Log("Name = " + name + " Score: " + score);
					Scores.Add(new ScoreData(name,int.Parse(score) ));
				}
				catch {}
			}
		}
		catch (Exception e) {
		    Debug.Log ("Could get index " + e.Message + " Stack : " + e.StackTrace);

            if (FailCallback != null)
                FailCallback();

            yield break;	
		}

		if (Scores.Count == 0) {
			Debug.LogError ("Failed to get scores");
			mScores = null;
            if (FailCallback != null)
                FailCallback();

		} else {
			mScores = Scores;
			mScores.Sort (SortScores);
			if (ScoresUpdatedHandler != null)
				ScoresUpdatedHandler (mScores);
		}
			
		//foreach (ScoreData score in Scores) {
		//	Debug.Log (score);
		//}

		yield break;
	}

	int SortScores(ScoreData _a, ScoreData _b)
	{
		if (_a.Score > _b.Score) {
			return -1;
		} else if (_a.Score < _b.Score) {
			return 1;
		} else
			return 0;
	}

	public string ErrorMessage = "";

    public Callback SuccessCallback;
    public Callback FailCallback;
    public void SendCurrentScore(ScoreData _data, Callback _fail, Callback _success)
	{
        Debug.Log("Sending score");

        FailCallback = _fail;
        SuccessCallback = _success;

        StartCoroutine(SendScore(_data));
	}

    public ScoreData GetLastScore()
    {
        return LastScore;
    }
    ScoreData LastScore;

	IEnumerator SendScore(ScoreData _data)
	{
        // First part fetches the current score board index
		yield return GetIndex ();

		if (ScoreIndex == -1) {
			ErrorMessage = "Could not get current score index";

            Logger.LogError("Fetch request failed: Could not get current score index");

            if (FailCallback != null)
                FailCallback();
            yield break;
		}

        
        //Debug.Log("Index = " + index);
        int index = ScoreIndex + 1;
        Debug.Log("Index = " + index);
		string body = "{\"httpMethod\": \"POST\", \"currentIndex\" : " + index + ",\"body\" : \"{\\\"TableName\\\" : \\\"Scores\\\",\\\"Item\\\" : { \\\"Index\\\" : "+ ScoreIndex + ",\\\"Name\\\" : \\\"" + _data.FirstName + "\\\",\\\"Score\\\" : \\\""+ _data.Score + "\\\" }}\"}";

		mRequest = null;

        // This part writes the score to the current index. The index is incrimented automatically.
		yield return UnityMakeRequestRoutine ("https://52abm5d0id.execute-api.us-west-2.amazonaws.com/prod/TestFunction", body);

        if (mRequest != null)
        {
            fsData d = fsJsonParser.Parse(mRequest.text);
            if (d.AsDictionary.ContainsKey("statusCode") == false)
            {
                Debug.Log("No status code"); // Some strange erorr has happened
                if (FailCallback != null)
                    FailCallback();
                yield break;
            }
            else
            {
                string status = d.AsDictionary["statusCode"].AsString;
                Debug.Log("Message status = " + status);

                if (status != "200")
                {
                    if (FailCallback != null)
                        FailCallback();
                    yield break;
                }

            }

            LastScore = _data; // Set the last score for the scoreboard

            if (SuccessCallback != null)
                SuccessCallback();
            yield break;
        }
        else
        {
            if (FailCallback != null)
                FailCallback();

            Logger.LogError("Send request failed");
            yield break;
        }
	}
		
	IEnumerator UnityMakeRequestRoutine(string _url, string _requestBody)
	{
		byte[] postBytes 				= Encoding.ASCII.GetBytes(_requestBody);
		mRequest = new WWW(_url, postBytes);
		yield return mRequest;
		Logger.Log ("Recieved " + mRequest.text);
	}

	IEnumerator UnityFectchRequestRoutine(string _url)
	{
		mRequest = new WWW(_url);
		yield return mRequest;
		Logger.Log ("Recieved " + mRequest.text);
	}

	public string UnityMakeRequest(string _url, string _requestBody, WWWCallback _callback = null)
	{
		string responseBody = "error";
		//Debug.Log ("Unity Request : " + _url + "\n with " + _requestBody);

		try {
			byte[] postBytes 				= Encoding.ASCII.GetBytes(_requestBody);

			WWW www = new WWW(_url, postBytes);
			StartCoroutine(DelayedMakeRequest(www));
			if (_callback != null)
				_callback(www);
			return "";
		}
		catch (System.Exception e) {
			// Server failed somehow
			Debug.Log("Exception occured " + e.Message + " \n Trace " + e.StackTrace);
			if (_callback != null)
				_callback(null);
			return null;
		}
		if (_callback != null)
			_callback(null);
		return "";
	}

	public void Test3()
	{
		Debug.Log ("Attempting send");
		string headers = AddApiAuthentication ();

		ScoreData _data = new ScoreData ("orange", 4304);
	}


	public delegate void WWWCallback(WWW _w);
    public delegate void Callback();

    IEnumerator DelayedMakeRequest(WWW _w)
	{
		yield return _w;
	}

	public string FectchRequest(string url)
	{
		Debug.Log ("Get request " + url);

		string html = "";
		try{
			
		HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
		//request.AutomaticDecompression = DecompressionMethods.GZip;

		

		using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
		using (Stream stream = response.GetResponseStream())
		using (StreamReader reader = new StreamReader(stream))
		{
			html = reader.ReadToEnd();
		}

		Console.WriteLine(html);
		}
		catch (Exception e) {
			Debug.Log("Failed fetch " + e.Message + " at " + e.StackTrace);
		}
		WebClient wc = new WebClient ();

		return html;

	}

	public string FectchRequest2(string url)
	{
		Debug.Log ("Get request " + url);

		string html = "";
		try{
			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
			HttpWebResponse response = (HttpWebResponse)request.GetResponse();
			StreamReader resStream = new StreamReader(response.GetResponseStream());
			request.Method = "GET";
			string s = resStream.ReadToEnd();
			Console.WriteLine(html);
		}
		catch (Exception e) {
			Debug.Log("Failed fetch " + e.Message + " at " + e.StackTrace);
		}

		return html;



	}

	public string SimpleFetch(string _url)
	{
		StartCoroutine (DelayedFetch (_url));
		return "";
	}

	IEnumerator DelayedFetch(string _url)
	{
		WWW www = new WWW(_url);
		yield return www;

		Debug.Log("resp header "+  www.responseHeaders);
		Debug.Log ("string =  " + www.text);


	}
	public string ComplexFetch(string url) 
	{
		string responseBody = "error";
		Debug.Log ("ComplexFetch : " + url);

		try {
			//byte[] postBytes 				= Encoding.ASCII.GetBytes(requestBody);

			HttpWebRequest request 			= (HttpWebRequest)WebRequest.Create(url); 
			request.KeepAlive 				= false;
			request.ProtocolVersion 		= HttpVersion.Version10;
			request.Method 					= "GET";
			request.ContentType 			= "text/html";
			//request.ContentType 			= "application/json";
			request.Accept 					= "text/plain";
			request.AutomaticDecompression 	= DecompressionMethods.GZip;
			//request.ContentLength 			= postBytes.Length;

			//Stream requestStream = request.GetRequestStream();

//			requestStream.Write(postBytes, 0, postBytes.Length);
//			requestStream.Close();

			HttpWebResponse response = (HttpWebResponse)request.GetResponse();

			responseBody = (new StreamReader(response.GetResponseStream()).ReadToEnd());

			Debug.Log("Recieved response: " + response.StatusCode);
		}
		catch (System.Exception e) {
			// Server failed somehow
			Debug.Log("Exception occured " + e.Message + " \n Trace " + e.StackTrace);
			return null;
		}

		return responseBody;
	}

	public string MakeRequest(string url, string requestBody) 
	{
		string responseBody = "error";
		Debug.Log ("Request : " + url + "\n with " + requestBody);

		try {
			byte[] postBytes 				= Encoding.ASCII.GetBytes(requestBody);

			HttpWebRequest request 			= (HttpWebRequest)WebRequest.Create(url); 
			request.KeepAlive 				= false;
			request.ProtocolVersion 		= HttpVersion.Version10;
			request.Method 					= "POST";
			//request.ContentType 			= "text/html";
			request.ContentType 			= "application/json";
			//request.Accept 					= "text/plain";
			//request.AutomaticDecompression 	= DecompressionMethods.GZip;
			request.ContentLength 			= postBytes.Length;

			Stream requestStream = request.GetRequestStream();

			requestStream.Write(postBytes, 0, postBytes.Length);
			requestStream.Close();

			HttpWebResponse response = (HttpWebResponse)request.GetResponse();

			responseBody = (new StreamReader(response.GetResponseStream()).ReadToEnd());

			Debug.Log("Recieved response: " + response.StatusCode);
		}
		catch (System.Exception e) {
			// Server failed somehow
			Debug.Log("Exception occured " + e.Message + " \n Trace " + e.StackTrace);
			return null;
		}

		return responseBody;
	}

	private string CreateToken(string message, string secret)
	{
		secret = secret ?? "";
		var encoding = new System.Text.ASCIIEncoding();
		byte[] keyByte = encoding.GetBytes(secret);
		byte[] messageBytes = encoding.GetBytes(message);
		using (var hmacsha256 = new System.Security.Cryptography.HMACSHA256(keyByte))
		{
			byte[] hashmessage = hmacsha256.ComputeHash(messageBytes);
			return Convert.ToBase64String(hashmessage).ToUpper();
		}
	}

	public string apiKey = "AKIAJCJ2PXDTUIQW33XA";
	public string apiSecret = "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";
	public string userID = "ThreatIntellect";

	public string AddApiAuthentication()
	{
		var nonce = DateTime.Now.Ticks;
		var signature = GetSignature(nonce, apiKey, apiSecret, userID);
		string headers = "{\"key\": \"" + apiKey + "\",\"signature\": \"" + signature + "\",\"nonce\": \"" + nonce + "\"";
		return headers;
	}

	private string GetSignature(long nonce, string key, string secret, string clientId)
	{
		string msg = string.Format("{0}{1}{2}", nonce,
			clientId,
			key);

		return ByteArrayToString(SignHMACSHA256(secret, StrinToByteArray(msg))).ToUpper();
	}

	public static byte[] SignHMACSHA256(String key, byte[] data)
	{
		System.Security.Cryptography.HMACSHA256 hashMaker = new System.Security.Cryptography.HMACSHA256(Encoding.ASCII.GetBytes(key));
		return hashMaker.ComputeHash(data);
	}

	public static byte[] StrinToByteArray(string str)
	{
		return System.Text.Encoding.ASCII.GetBytes(str);
	}

	public static string ByteArrayToString(byte[] hash)
	{
		return BitConverter.ToString(hash).Replace("-", "").ToLower();
	}
    private void Dataset_OnSyncFailure(object sender, SyncFailureEventArgs e)
    {
        AddDebugText("Sync fail");
        
    }

    List<string> DebugTextList = new List<string>();
    public void AddDebugText(string _value)
    {
        Debug.Log(_value);
        DebugTextList.Add(_value);

        string message = "";
        foreach (string s in DebugTextList)
        {
            message = message + s + "\n";
        }

        DebugText.text = message;
    }

    //public void SuccessCallback(object sender, SyncSuccessEventArgs e)
    //{
    //    AddDebugText("Sync success");
        
        
    //    Debug.Log("sets " + syncManager.ListDatasets().Count);
    //    List<DatasetMetadata> sets = syncManager.ListDatasets();

    //    AddDebugText("Sets count " + sets.Count);
        

    //    foreach (DatasetMetadata set in sets)
    //    {
    //        AddDebugText("set.DatasetName = " + set.DatasetName);
    //    }
    //    Dataset dataset = syncManager.OpenOrCreateDataset("myDataset");
    //    string value = dataset.Get("myKey");

    //    AddDebugText("Value = " + value);
        
    //}

    //public void FailCallback(object sender, SyncSuccessEventArgs e)
    //{

    //}

    private CognitoSyncManager _syncManager;

	private CognitoSyncManager SyncManager
	{
		get
		{
			if (_syncManager == null)
			{
				_syncManager = new CognitoSyncManager(Credentials, new AmazonCognitoSyncConfig { RegionEndpoint = _Region });
				//_syncManager = new CognitoSyncManager(Credentials, new AmazonCognitoSyncConfig { RegionEndpoint = _Region });
			}
			return _syncManager;
		}
	}

	Dataset Scores;
	public void PullScoreData()
	{
		//Amazon.CognitoSync.SyncManager.Internal.DatasetUtils.
		CognitoSyncManager s = SyncManager;
		//s.ListDatasets

		Scores = s.OpenOrCreateDataset("Scores");
		Scores.OnSyncSuccess +=  ScoresSynced;
		//AsyncOptions
		//Scores.Sync()
//		Dataset Names = s.OpenOrCreateDataset("Names");
//		Names.OnSyncSuccess +=  NamesSynced;

		// TODO: Sync the datasets and move the rest of this function 
		// to the "on complete" function

		List<ScoreData> ScoreDataList = new List<ScoreData> ();
		foreach (Record rec in Scores.Records) {
			// This is a list of 
			// 1name,name,
			// 2name,name,
			// 1score,44
			// 2score,55
			string id = rec.Key;
			string name = rec.Value;
			string scoreIndex = "";
			if (id.Contains ("score"))
				continue;
			else { // Entry is a name
				string index = id.Replace("name",""); // TODO: test this works as intended
				scoreIndex = index+"score";

			}

			int score = int.Parse(Scores.Get(scoreIndex));

			ScoreDataList.Add(new ScoreData(name, score));

		}

		// TODO:
		// UpdateUIHere
	}

	public void NamesSynced(object sender, SyncSuccessEventArgs e)
	{
		
	}

	public void ScoresSynced(object sender, SyncSuccessEventArgs e)
	{

	}

	Dataset Index;
	ScoreData ScoreToSync;

	public void PushScore(ScoreData _data)
	{
		Index = SyncManager.OpenOrCreateDataset ("Index");
		Index.OnSyncSuccess +=  PushScoreFirstStepSuccess;
		Index.SynchronizeAsync ();
		ScoreToSync = _data;
	}

    public void PushScoreFirstStepSuccess(object sender, SyncSuccessEventArgs e)
    {

        int indexes = int.Parse(Index.Get("indexCount"));
		indexes++;

		string NameKey = string.Format ("{0}name", indexes);
		string ScoreKey = string.Format ("{0}score", indexes);

		Dataset Scores = SyncManager.OpenOrCreateDataset("Scores");

		Scores.Put(NameKey, ScoreToSync.Score.ToString());
		Scores.Put (ScoreKey, ScoreToSync.FirstName);
		Scores.SynchronizeAsync ();

	}

	public void PushScoreSuccess()
	{
		Debug.Log ("PushScoreSuccess");
	}

	public void PushScoreFail()
	{
		Debug.Log ("PushScoreFail");
	}
}

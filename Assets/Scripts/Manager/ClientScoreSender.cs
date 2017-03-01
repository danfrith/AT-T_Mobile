using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class ClientScoreSender : NetworkBehaviour {

	void Start()
    {
        if (!isClient)
        {
            NetworkInstanceId id = this.netId;
            
            Debug.Log("ClientScoreSender: Client connected: " + id.Value);
            //Debug.Log("Network view" + this.)

            
        }

        //name = System.Environment.MachineName + isClient;
    }

    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.G))
        //{
        //    //CmdSendScore("sent from " + name);
        //    DebugSendScore();
        //}
    }

    public int count = 0;
    public void DebugSendScore()
    {
        ScoreData _scoreData = new ScoreData();
        switch (count)
        {
            case 0:
                _scoreData.FirstName = "Tia";
                _scoreData.LastName = "Osmand";
                _scoreData.Email = "tos@fgkf.com";
                _scoreData.Score = 45348;
                break;

            case 1:
                _scoreData.FirstName = "Dara";
                _scoreData.LastName = "Kinley";
                _scoreData.Email = "Dkin@ffg.com";
                _scoreData.Score = 398368;
                break;
            case 2:
                _scoreData.FirstName = "Jeff";
                _scoreData.LastName = "McLeod";
                _scoreData.Email = "Dkin@ffg.com";
                _scoreData.Score = 398368;
                break;
        }

        count++;

        CmdRecieveScoreData(_scoreData.FirstName, _scoreData.LastName, _scoreData.Email, _scoreData.Score);
    }

    public void SendScore(ScoreData _scoreData)
    {
        Debug.Log("Sending score, but is this local player? " + this.isLocalPlayer);
        CmdRecieveScoreData(_scoreData.FirstName, _scoreData.LastName, _scoreData.Email, _scoreData.Score);
		Logger.Log ("Score send success");
    }

    [Command]
    void CmdSendScore(string _msg)
    {
        Debug.Log(_msg + ":: recieved Server" + name);
    }

    [Command]
    public void CmdRecieveScoreData(string _firstName, string _lastName, string _email, int _score)
    {
        Logger.Log("CmdRecieveScoreData: Recieved data " + _firstName + " " + _lastName + " " + _email + " " + _score);
        if (isClient)
        {
            Debug.LogError("Should not be recieving this message on the client");
            return;
        }

        GameObject go = GameObject.Find("ScoreBoardReciever");
        ScoreBoardReciever sbr = go.GetComponent<ScoreBoardReciever>();

        sbr.ServerRecieveScoreData(_firstName, _lastName, _email, _score);
        
    }

    public override void OnStartClient()
    {
        Debug.Log("Client has connected");
        
    }

    void OnGUI()
    {
        if (isClient == false)
        {
            
        }

    }

}

using UnityEngine;
using UnityEngine.Networking;

//public class ScoreSender : MonoBehaviour {
public class ScoreSenderPrototype : NetworkBehaviour
{

    public void ConnectButtonPressed()
    {
        NetworkClient client = new NetworkClient();

        Debug.Log("Attempting client connection");
        
        Network.Connect("127.0.0.1", 25003);
        
        if (Network.isClient)
        {
            Debug.Log("Connected");
        }
        else
        {
            Debug.Log("Not connected");
        }
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
        SendScore(_scoreData);
    }

    void SendScore(ScoreData _scoreData)
    {
        GetComponent<NetworkView>().RPC("RecieveScoreData", RPCMode.Server, _scoreData.FirstName, _scoreData.LastName, _scoreData.Email, _scoreData.Score);
    }

    [RPC]
    public void RecieveScoreData(string _firstName, string _lastName, string _email, int _score)
    {
        Debug.Log("Recieved data2" + _firstName + " " + _lastName + " " + _email + " " + _score);
    }

    void OnConnectedToServer()
    {
        Debug.Log("Connected to server");

        GetComponent<NetworkView>().RPC("LogMessage", RPCMode.Server, "Hello World!");
        //NetworkView nv = GetComponent<NetworkView>();
        Debug.Log("Message sent");

        //CmdDoPrint();
        //RpcDoPrint();
        //Network.RPC("LogMessage", RPCMode.All, "Hello World!");
    }

    [Command]
    public void CmdDoPrint()
    {
        Debug.Log("Client print");
    }
    [ClientRpc]
    public void RpcDoPrint()
    {
        Debug.Log("Client rpc print");
    }

    void OnDisconnectedFromServer()
    {
        Debug.Log("disconnected from server");
    }

    [RPC]
    void LogMessage(string msg)
    {
        Debug.Log(msg);
    }
}

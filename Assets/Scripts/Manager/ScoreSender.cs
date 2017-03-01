using UnityEngine;
using UnityEngine.Networking;

//public class ScoreSender : MonoBehaviour //{
public class ScoreSender : NetworkBehaviour
{

    void OnEnable()
    {
        Debug.Log("Enabled");
        GameManager.Instance.StageEndedEventHandler += new GameManager.StageEndedEvent(SendScore);

        ConnectToServer();
    }
    
    void OnDisable()
    {
        GameManager.Instance.StageEndedEventHandler -= SendScore;
    }

    public void ConnectButtonPressed()
    {
        ConnectToServer();
    }

    public void ConnectToServer()
    { 
        NetworkClient client = new NetworkClient();

        Logger.Log("Attempting client connection");
        
        Network.Connect("127.0.0.1", 25003);
        
        if (Network.isClient)
        {
            Logger.Log("Connected");
        }
        else
        {
            Logger.LogError("Not connected");
        }
    }

    void SendScore(ScoreData _scoreData)
    {
        Logger.Log("Attempting to send score for " + _scoreData.FirstName + " to the server");

        GetComponent<NetworkView>().RPC("RecieveScoreData", RPCMode.Server, _scoreData.FirstName, _scoreData.LastName, _scoreData.Email, _scoreData.Score);
    }

    [RPC]
    public void RecieveScoreData(string _firstName, string _lastName, string _email, int _score)
    {
        Debug.Log("Recieved data2" + _firstName + " " + _lastName + " " + _email + " " + _score);
    }

    void OnConnectedToServer()
    {
        Logger.Log("Connected to server");

        GetComponent<NetworkView>().RPC("LogMessage", RPCMode.Server, "Client connected");

    }

    void OnDisconnectedFromServer()
    {
        Logger.LogError("disconnected from server");
    }

    [RPC]
    void LogMessage(string msg)
    {
        Debug.Log(msg);
    }
}

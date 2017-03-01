using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.IO;
using System.Net;

public class SetServer : NetworkManager {

    public bool server = false;
    // Use this for initialization

    public NetworkClient myClient;
    public int Port;

    private string IP;

    private string LocalAddress;
    void OnEnable()
    {
        if (!server)
            GetIP();
        else
            FindLocalmachineAddress();

    }

    private void CreateDefaultFile()
    {
        Logger.Log("Creating new default address file");

        StreamWriter sw = new StreamWriter(File.Create("ServerAddress.txt"));
        sw.WriteLine("192.168.1.10");

        sw.Close();
        sw.Dispose();
    }

    public void GetIP()
    {
        try
        {
            if (File.Exists("ServerAddress.txt") == false)
            {
                CreateDefaultFile();
            }

            FileStream fs = (FileStream)File.OpenRead("ServerAddress.txt");

            StreamReader sr = new StreamReader(fs);
            string address = sr.ReadLine();

            if (string.IsNullOrEmpty(address))
                throw new System.Exception ("Address is invalid");

            IP = address;

            Logger.Log("Remote server IP = " + IP);
        }
        catch (System.Exception e)
        {
            Logger.LogError("Failed to read IP Address file Error: " + e.Message + " Stack: " + e.StackTrace);
        }

    }

    void FindLocalmachineAddress()
    {
        string strHostName = string.Empty;
        
        strHostName = System.Net.Dns.GetHostName();
        
        IPHostEntry ipEntry = Dns.GetHostEntry(strHostName);
        IPAddress[] addr = ipEntry.AddressList;

        for (int i = 0; i < addr.Length; i++)
        {
            
            Logger.LogDebug("IP Address {0}: {1} ", i, addr[i].ToString());
        }

        LocalAddress = addr[1].ToString();

    }

    void Start () {
        NetworkManager nm = GetComponent<NetworkManager>();
        nm.networkPort = Port;

        //return;
        if (server)
        {
            nm.StartServer();

            nm.networkPort = Port;
        }
        else
        {
            Logger.Log("Attempting client connection");

            nm.networkAddress = IP;

            StartCoroutine(MaintainConnectionRoutine());
        }
                
	}

    private bool connecting = false;

    IEnumerator MaintainConnectionRoutine()
    {
        
        while (true)
        {
            
            if (!IsClientConnected())
            {
                if (connecting == false)
                {
                    Logger.Log("Connecting to = " + networkAddress);
                    StartClient();
                    connecting = true;
                    yield return new WaitForSeconds(4);
                }
                else
                    yield return new WaitForSeconds(1);
            }
            else
                yield return new WaitForSeconds(9);

        }
    }

    public override void OnClientError(NetworkConnection conn, int errorCode)
    {
        base.OnClientError(conn, errorCode);
        Debug.Log("Failed to connecT?");
    }

    public override void OnServerConnect(NetworkConnection _con)
    {
        base.OnServerConnect(_con);

        ServerAdminPage s = ServerAdminPage.GetAdminPage();

        s.AddClient(_con.connectionId.ToString(), _con.address);

        Debug.Log("Client connected" + _con.connectionId + " ip " + _con.address);
    }

    public override void OnServerDisconnect(NetworkConnection _con)
    {
        base.OnServerDisconnect(_con);
        ServerAdminPage s = ServerAdminPage.GetAdminPage();

        s.RemoveClient(_con.connectionId.ToString(), _con.address);

        //Debug.Log("Client disconnected ");
        Debug.Log("Client disconnected " + _con.connectionId + " ip " + _con.address);
    }

    public override void OnClientConnect(NetworkConnection _con)
    {
        base.OnClientConnect(_con);
        Logger.Log("Client connected" + _con.connectionId + " ip " + _con.address);
    }

    public override void OnClientDisconnect(NetworkConnection _con)
    {
        if (connecting == true)
            Logger.LogError("Failed to connect to server " + LocalAddress);
        else
            Logger.Log("Client disconnected id " + _con.connectionId + " ip " + _con.address);

        connecting = false;
        base.OnClientDisconnect(_con);
    }

    void OnGUI()
    {
        if (!server)
        {
            if (!IsClientConnected())
            {
                string clientText = "No network connection";
                GUI.TextArea(new Rect(0, 0, 140, 20), clientText);
            }
            return;
        }

        //NetworkManager nm = GetComponent<NetworkManager>();
        
        //string text = "";
        //text = text + "\n" + " address: " + nm.networkAddress;
        //text = text + "\n" + " port: " + nm.networkPort;
        //text = text + "\n" + " Server bind address: " + nm.serverBindAddress;
        //text = text + "\n" + " Server bind ip: " + nm.serverBindToIP;
        
        //GUI.TextArea(new Rect(10, 100, 200, 80), text);

    }


}

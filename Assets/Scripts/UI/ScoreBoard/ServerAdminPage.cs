using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Net;
using System.Collections.Generic;

public class ServerAdminPage : MonoBehaviour {

    public string LocalAddress;

    public Text ServerAddressText;
    public Transform ClientViewPrefab;
    public Transform ClientViewBox;
    private Dictionary<string, Transform> ClientViewItems = new Dictionary<string, Transform>();
    

    private static ServerAdminPage Instance;
    void Awake()
    {
        Instance = this;
    }
        
    public static ServerAdminPage GetAdminPage()
    {
        return Instance;
    }

    void OnEnable()
    {
        
        FindLocalmachineAddress();

        
        ServerAddressText.text = "Server IP " + LocalAddress;
    }

    void FindLocalmachineAddress()
    {
        string strHostName = string.Empty;
        
        strHostName = System.Net.Dns.GetHostName();
        
        IPHostEntry ipEntry = Dns.GetHostEntry(strHostName);
        IPAddress[] addr = ipEntry.AddressList;

        LocalAddress = addr[1].ToString();
        Logger.Log("Server IP = " + LocalAddress);

    }

    public void AddClient(string _id, string _address)
    {
        Transform t = (Transform)Instantiate(ClientViewPrefab, ClientViewBox);

        ClientViewItem c =  t.GetComponent<ClientViewItem>();

        c.SetText(_id + " " + _address);

        ClientViewItems.Add(_id, t);

    }

    public void RemoveClient(string _id, string _address)
    {
        if (ClientViewItems.ContainsKey(_id) == false)
        {
            Logger.LogError("ServerAdminPage: key does not exist : " + _id);
            return;
        }

        Debug.Log("Destroying client element " + _id);

        Destroy(ClientViewItems[_id].gameObject);

        ClientViewItems.Remove(_id);
    }

}

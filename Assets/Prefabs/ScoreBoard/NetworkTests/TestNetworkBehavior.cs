using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class TestNetworkBehavior : NetworkBehaviour {

    public void SendMessage()
    {
        CmdDoPrint("from client " + name);
    }

	[Command]
    public void CmdDoPrint(string _msg)
    {
        Debug.Log(_msg + ":: Server print from" + name);
    }


    void Update()
    {
        if (!isLocalPlayer)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            Debug.Log("Client key pressed F");
            SendMessage();
        }

        var x = Input.GetAxis("Horizontal") * Time.deltaTime * 150.0f;
        var z = Input.GetAxis("Vertical") * Time.deltaTime * 3.0f;

        transform.Rotate(0, x, 0);
        transform.Translate(0, 0, z);
    }

}

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ClientViewItem : MonoBehaviour {

    public Text ClientInfoText;

	public void SetText(string _clientInfo)
    {
        ClientInfoText.text = _clientInfo;
    }
}

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class StartOfAppScreen : MonoBehaviour {

    public Text NameText;
	void OnEnable()
    {
        string name = GameManager.Instance.GetPlayerName();
        NameText.text = "Welcome " + name;
    }
}

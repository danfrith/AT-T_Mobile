using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class LoadingScreen : MonoBehaviour {

	void Start () {
		Logger.Log ("Loading");
		string LoadingLevel;

		if (MobileGameManager.Instance == null) {
			LoadingLevel = "Menus";
		} else {
			LoadingLevel = MobileGameManager.Instance.SelectedLevel;
		}
		Debug.Log ("Start loding" + LoadingLevel + ": level");

		SceneManager.LoadSceneAsync(LoadingLevel);
	}

}

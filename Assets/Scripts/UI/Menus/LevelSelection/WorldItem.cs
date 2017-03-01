using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class WorldItem : MonoBehaviour {

	Image Item;
	public Color SelectedColour;
	public Color UnselectedColour;
	public int World;
	void OnEnable()
	{
		Item = GetComponent<Image> ();
		Debug.Log ("MobileGameManager.Instance = " + MobileGameManager.Instance.ToString ());

		GameObject go = GameObject.Find("LevelSelect");
		LevelSelectScreen ls = null;
		if (go != null)
			ls = go.GetComponent<LevelSelectScreen>();

		ls.UIUpdatedEventHandler += new LevelSelectScreen.UIUpdatedEvent(UpdateColour);

	}

	void UpdateColour()
	{
		if (MobileGameManager.Instance.CurrentLevel == 0) {
			Item.color = SelectedColour;
		} else {
			
			LevelState appearance = MobileGameManager.Instance.GetWorldState (World*5);

			if (appearance == LevelState.Unselected) {
				Item.color = UnselectedColour;
			} else if (appearance == LevelState.Selected) {
				Item.color = SelectedColour;
			}		
		}		
	}
		
}

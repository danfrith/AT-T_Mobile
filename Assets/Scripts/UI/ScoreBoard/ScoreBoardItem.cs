using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScoreBoardItem : MonoBehaviour {

	public Text RankText;
	public Text ScoreText;
	public Text NameText;
    public Text SecondNameText;
    public Image Highlight;

    void OnEnable()
	{
		if (RankText == null)
			Debug.LogError ("RankText is missing for " + name);

		if (ScoreText == null)
			Debug.LogError ("ScoreText is missing for " + name);

		if (NameText == null)
			Debug.LogError ("NameText is missing for " + name);

        if (SecondNameText == null)
            Debug.LogError("SecondNameText is missing for " + name);
    }

	public void SetItem(int _index, int _score, string _name, string _secondName, bool _hasHighlight = false)
	{
        ScoreText.text = _score.ToString();

        NameText.text = _name;

        if (SecondNameText != null)
            SecondNameText.text = _secondName;

        if (_hasHighlight == true)
        {
            Highlight.enabled = true;
        }

        SetIndex (_index);
	}

	public void SetIndex(int _index)
	{
		if (_index < 0) {
			Debug.LogError ("Index out of bounds");
			RankText.text = "999.";
			return;
		}

        if (_index < 10)
            RankText.text = "0" + _index.ToString();
        else
            RankText.text = _index.ToString();// + ".";
	}

}

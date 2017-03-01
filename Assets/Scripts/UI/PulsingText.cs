using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PulsingText : MonoBehaviour {

	private Text mText;

	public float PulseTime = 1;
	public EaseType PulseType;

	public void OnEnable()
	{
		mText = GetComponent<Text> ();
		//SetText ("Test value");
	}

	public void SetText(string _text)
	{
		mText.text = _text;

		Color newCol = mText.color;

		newCol.a = 0;

		mText.color = newCol;

		// Trial run automatic pulsing
		PulseText ();
		Debug.Log ("Text pulsing");
	}

	public void PulseText()
	{
		StartCoroutine (PulseRoutine ());
	}

	IEnumerator PulseRoutine()
	{
		float t = 1;

		while (t > 0)
		{
			t -= Time.fixedDeltaTime/PulseTime;

			yield return new WaitForFixedUpdate();

			Color newCol = mText.color;

			newCol.a = Ease.GetValue(PulseType, 1-t);

			mText.color = newCol;
		}

	}

	public bool Test = false;
	void Update()
	{
		if (Test == true) {
			
			Test = false;

			SetText ("another value");

		}
	}
}

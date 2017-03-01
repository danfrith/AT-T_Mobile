using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ProgressBar : MonoBehaviour {

	public Image Bar;
	public Image Background;
	public EaseType MovementStyle;

	public float Progress;
	void OnEnable()
	{
		if (Bar == null)
			Logger.LogError ("Bar is missing in progress bar object " + name);

	}


	public void Init(float _progress)
	{
		Progress = _progress;
		mTargetFill = Progress;
        Bar.fillAmount = _progress;

		Background = GetComponent<Image> ();
    }

    public void SetValue(float _progress)
    {
        Progress = _progress;
        mTargetFill = Progress;
        StartCoroutine(AnimateBar());
    }

	public void SetColours(Color _forground, Color _Background)
	{
		Bar.color = _forground;
		Background.color = _Background;
	}

	/// <summary>
	/// Speed in seconds at which the image updates to the new position
	/// </summary>
	[Tooltip("Speed in seconds at which the image updates to the new position")]
	public float AnimateSpeed = 1;

	private bool mAnimating = false;

	IEnumerator AnimateBar()
	{
		mAnimating = true;
		while (Bar.fillAmount != mTargetFill)
		{
			//Debug.Log("Animate Bar");
			Bar.fillAmount += AnimateSpeed * Time.fixedDeltaTime;
			Bar.fillAmount = Mathf.Clamp(Bar.fillAmount, 0, mTargetFill);

			yield return new WaitForFixedUpdate();
		}
		mAnimating = false;
	}

	private float mTargetFill;
	private float mMax;


}

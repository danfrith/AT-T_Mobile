using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum ShieldSide
{
    LEFT,
    RIGHT,
	MOUSE,
}
public class ShieldTouchTrack : MonoBehaviour {

    public ShieldSide Side;

	public static ShieldTouchTrack LeftTracker;
	public static ShieldTouchTrack RightTracker;

    private int mCurrentFingerID;
    public ShieldBody sb;

	public bool LatchFinger = false;
	public float GetFingerID()
	{
		return mCurrentFingerID;
	}

	void OnEnable()
	{
		if (Side == ShieldSide.LEFT)
			LeftTracker = this;
		else if (Side == ShieldSide.RIGHT)
			RightTracker = this;
		
		
	}

    Touch TouchInfo;

	public ShieldTouchTrack GetOther()
	{
		if (this == LeftTracker)
			return RightTracker;
		else
			return LeftTracker;
	}

	void LatchTofinger(Touch _touch)
	{
		sb.StopFadeOut ();
		LatchFinger = true;
		mCurrentFingerID = _touch.fingerId;
		Vector2 posPercent = new Vector2(_touch.position.x / Screen.width, _touch.position.y / Screen.height);
		sb.SetPosition(posPercent);
	}

	void RemoveFinger(Touch _touch)
	{
		sb.StartFadeOut();

		LatchFinger = false;

		mCurrentFingerID = -1;

	}

	void ManageTouch (Touch _touch)
	{
		if (_touch.phase == TouchPhase.Began) {
			
			if (GetOther ().LatchFinger == true && GetOther().GetFingerID() != _touch.fingerId) {
				LatchTofinger (_touch);
			} else {
				if (LatchFinger == true) {
					// Already latched
				} else {
					Vector2 posPercent = new Vector2 (_touch.position.x / Screen.width, _touch.position.y / Screen.height);
					if (this == LeftTracker && posPercent.x < 0.5f) {
						LatchTofinger (_touch);
					} else if (this != LeftTracker && posPercent.x > 0.5f) {
						LatchTofinger (_touch);
					}
				}
			}

		} else if (_touch.phase == TouchPhase.Ended || _touch.phase == TouchPhase.Canceled) {
			
			if (_touch.fingerId == mCurrentFingerID)
				RemoveFinger (_touch);
			
		} else if (_touch.phase == TouchPhase.Moved && _touch.fingerId == mCurrentFingerID) {
			
			Vector2 posPercent = new Vector2(_touch.position.x / Screen.width, _touch.position.y / Screen.height);
			sb.SetPosition(posPercent);

		}

		/*
		 * New touch
		 * If no latched on other, pick the shield that is on the same side as the touch
		 * If there is a touch on the other shield, take this touch. latch finger
		 * 
		 * Continued touch
		 * Continue updating my shield
		 * 
		 * Touch end
		 * Un-latch finger
	

		
		*/

	}

	public Vector2 posPercent;
    void FixedUpdate()
    {
        

		if (Side == ShieldSide.MOUSE) {
			
			posPercent = new Vector2(Input.mousePosition.x / Screen.width, Input.mousePosition.y / Screen.height);
			sb.SetPosition (posPercent);
		} else {

			if (Input.touchCount < 1)
			{
				return;
			}


			for (int i = 0; i < Input.touchCount; i++) {
				ManageTouch (Input.GetTouch (i));	
			}
		}

    }

//    void OnGUI()
//    {
//        string clientText = "Pos = " + TouchInfo.position.ToString();
//        clientText = clientText + "\nPos = " + Screen.width + "," + Screen.height;
//		clientText = clientText + "\nMouse pos =" + posPercent.ToString();
//		clientText = clientText + "\nMouse =" + Input.mousePosition.ToString();
//
//        GUI.TextArea(new Rect(Screen.width-150, Screen.height-80, 150, 80), clientText);
//    }
}

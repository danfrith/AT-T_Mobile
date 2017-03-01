using UnityEngine;

[RequireComponent(typeof(GUITexture))]
public class Joystick : MonoBehaviour
{
	class Boundary
	{
		public Vector2 min = Vector2.zero;
		public Vector2 max = Vector2.zero;
	}

	private static Joystick[] joysticks;					// A static collection of all joysticks
	private static bool enumeratedJoysticks = false;
	private static float tapTimeDelta = 0.3f;				// Time allowed between taps

	public GUITexture Background;
	public bool touchPad;
	public Vector2 position = Vector2.zero;
	public Rect touchZone;
	public Vector2 BufferZone;// = Vector2.zero;						// Control when position is output
	public bool normalize = false; 							// Normalize output after the dead-zone?
	public int tapCount;

	private int lastFingerId = -1;								// Finger last used for this joystick
	private float tapTimeWindow;							// How much time there is left for a tap to occur
	private Vector2 fingerDownPos;
	//private float fingerDownTime;
	//private float firstDeltaTime = 0.5f;

	private GUITexture gui;
	private Rect defaultRect;								// Default position / extents of the joystick graphic
	private Rect scaledRect;								// This is the size of the graphic displayed on screen, scaled to the
	// area of the defaultRect space.
	private Boundary guiBoundary = new Boundary();			// Boundary for joystick graphic
	public Vector2 guiTouchOffset;						// Offset to apply to touch input
	public Vector2 scaledTouchOffset;						// Offset to apply to touch input
	public Vector2 guiCenter;							// Center of joystick
	public string path;
	public string path2;


	public Vector2 ScreenPosition;

	public static void DisableJoystick()
	{
		MainJoystick.Enabled = false;
		MainJoystick.position = Vector2.zero;
	}
	public static void EnableJoystick()
	{
		MainJoystick.Enabled = true;
		//MainJoystick.gui.enabled = true;
	}

	bool bStatic = false;
	Vector2 ScreenPos;


	public Vector2 GetScreenPos()
	{
		return ScreenPos;
	}

	//void OnGUI()
	//{
	//    string text = "Screen Pos + " + ScreenPos.x + " " + ScreenPos.y;
	//    GUI.Box(new Rect(300, 60, 300, 60), text);
	//    //if (bHasTouch)
	//    //{
	//    //    gui.enabled = true;
	//    //}
	//    //else
	//    //{
	//    //    gui.enabled = false;
	//    //}
	//}

	public static Joystick MainJoystick;

	void Start()
	{
		MainJoystick = this;
		gui = (GUITexture)GetComponent(typeof(GUITexture));
		BufferZone.x = 0.6f;
		BufferZone.y = 0.6f;

		gui.enabled = false;

		RepositionJoystick();

		// Option graphic stuff.
		GameObject go = GameObject.Find ("JoystickBg");

		if (go != null) {
			Background = GetComponent<GUITexture>();
		}

	}

	public bool Enabled = true;
	public Color FloatColor2;
	public Color FloatColor; 
	// Area in which it is valid to plop a joystick
	Rect JoystickArea = new Rect(0, 0, Screen.width / 2, Screen.height - 20);
	private void RepositionJoystick()
	{
		// This is used to check where the player has touched on the screen.
		defaultRect = gui.pixelInset;

		Vector3 pos = new Vector3(gui.pixelInset.x, gui.pixelInset.y, 0);
		pos.x /= Screen.width;
		pos.y /= Screen.height;
		Background.transform.position = new Vector3(pos.x, pos.y, 0);

		int size = 200;
		int halfSize = size/2;

		transform.position = Vector3.zero;

		if (touchPad)
		{
			// If a texture has been assigned, then use the rect ferom the gui as our touchZone
			if (gui.texture)
				touchZone = defaultRect;
		}
		else
		{
			guiTouchOffset.x = halfSize;
			guiTouchOffset.y = halfSize;

			scaledTouchOffset.x = halfSize;
			scaledTouchOffset.y = halfSize;

			// Cache the center of the GUI, since it doesn't change
			guiCenter.x = defaultRect.x + guiTouchOffset.x;
			guiCenter.y = defaultRect.y + guiTouchOffset.y;
			path2 = "c" + guiCenter.y.ToString() + " d" + defaultRect.ToString() + " g.y" + guiTouchOffset.y.ToString();
			// Let's build the GUI boundary, so we can clamp joystick movemen
		}
	}
	public Vector2 getGUICenter()
	{
		return guiCenter;
	}

	void Disable()
	{
		gameObject.active = false;
		//enumeratedJoysticks = false;	
	}

	private void ResetJoystick()
	{
		//gui.pixelInset = defaultRect;
		gui.pixelInset = defaultRect;
		lastFingerId = -1;
		position = Vector2.zero;
		fingerDownPos = Vector2.zero;
	}

	Rect Box = new Rect(0, 0, 200, 120);
	Vector2 diffDebug;

	float Rotation;
	Vector2 LastTouch;
	void OnGUI()
	{
		if (Enabled == false) return;
		GUI.BeginGroup(new Rect(0, 0, Screen.width, Screen.height));
		GUI.DrawTexture(new Rect(LastTouch.x, LastTouch.y, 5, 5), GetComponent<GUITexture>().texture, ScaleMode.StretchToFill, true, 0);
		//GUI.color = FloatColor2;
		GUI.color = Color.cyan;
		if (FreeJoystickEnabled)
		{
			FloatColor = new Color(GUI.color.r, GUI.color.g, GUI.color.b, timeSinceActive / fadeTime);
			GUI.color = FloatColor;
		}

		Vector2 point = new Vector2(guiCenter.x, Screen.height - guiCenter.y);
		GUIUtility.RotateAroundPivot(Rotation, point);
		Rect rect = new Rect(guiCenter.x - 100, Screen.height - (guiCenter.y + 100), 200, 200);
		GUI.DrawTexture(rect, GetComponent<GUITexture>().texture, ScaleMode.StretchToFill, true, 0);

		GUI.EndGroup();


		//string text = "Rotation:" + Rotation + "\n";
		//text = text + "Deadzone: " + BufferZone + "\n";
		//text = text + "deadzoned = " + hitDeadzone + "\n";
		//text = text + "diff = " + diffDebug.magnitude;
		//GUI.Box(Box, text);
	}
	bool hitDeadzone = false;
	private bool IsFingerDown()
	{
		return (lastFingerId != -1);
	}

	public void LatchedFinger(int fingerId)
	{
		// If another joystick has latched this finger, then we must release it
		if (lastFingerId == fingerId)
			ResetJoystick();
	}

	private bool bHasTouch;

	Vector3 pos = new Vector3(0.5f, 0.5f, 10f);
	private float timeSinceActive = 0f;
	private float fadeTime = 1.0f;
	private float latchTime = 0.1f;
	private float timeSinceTouched = 0;

	public bool FreeJoystickEnabled = true;
	void Update()
	{
//		if (State.TouchEnabled)
//		{   
//			Enabled = false;
//		}
//		else
//		{
//			Enabled = true;
//		}
		Enabled = true;

		if (Enabled == false)
		{
			gui.enabled = false;
			Background.color = new Color(Background.color.r, Background.color.g, Background.color.b, 0);
			return;
		}

		if (!gui.enabled)
		{
			if (timeSinceActive > 0)
			{
				timeSinceActive -= Time.deltaTime;
				timeSinceActive = Mathf.Max(timeSinceActive, 0);
			}

			if (!FreeJoystickEnabled)
			{
				Background.color = new Color(Background.color.r, Background.color.g, Background.color.b, timeSinceActive / fadeTime);
			}
			else
			{

				//Background.color = new Color(1, 0   , 0, 0.5f);
			}
		}

		int count = Input.touchCount;

		if (tapTimeWindow > 0)
			tapTimeWindow -= Time.deltaTime;
		else
			tapCount = 0;

		if (count == 0)
		{
			ResetJoystick();
		}
		else
		{
			for (int i = 0; i < count; i++)
			{
				Touch touch = Input.GetTouch(i);
				Vector2 guiTouchPos = touch.position -guiTouchOffset;

				bool shouldLatchFinger = false;
				if (JoystickArea.Contains(touch.position)) // Changed this to use a big area on the screen
				{
					shouldLatchFinger = true;
				}
				#region
				// Latch the finger if this is a new touch
				if (shouldLatchFinger && (lastFingerId == -1 || lastFingerId != touch.fingerId))
				{

					if (touchPad)
					{
						lastFingerId = touch.fingerId;
					}

					lastFingerId = touch.fingerId;

					// Accumulate taps if it is within the time window
					if (tapTimeWindow > 0)
						tapCount++;
					else
					{
						tapCount = 1;
						tapTimeWindow = tapTimeDelta;
					}

					// Tell other joysticks we've latched this finger
					//for (  j : Joystick in joysticks )
					//foreach (Joystick j in joysticks)
					//{
					//    if (j != this)
					//        j.LatchedFinger(touch.fingerId);
					//}
				}
				bHasTouch = false;
				if (lastFingerId == touch.fingerId)
				{
					ScreenPos.x = (touch.position.x - Background.pixelInset.width/2) / Screen.width;
					ScreenPos.y = (touch.position.y - Background.pixelInset.height/2) / Screen.height;
					if (touch.phase != TouchPhase.Ended)
					{
						bHasTouch = true;

					}
					else
					{
						gui.enabled = false;
						timeSinceTouched = 0;
						bHasTouch = false;
					}

					//
					//Background.position = new Vector3(0.5f, 0.5f, 0);
					// If we began this touch now, we move the joystick to the new position
					if (touch.phase == TouchPhase.Began)
					{
						// ... but only if the last use of the joystick was longer than a certain time.
						if (FreeJoystickEnabled)
						{
							Background.color = new Color(Background.color.r, Background.color.g, Background.color.b, 1);
							timeSinceActive = fadeTime;
							timeSinceTouched = 0;
							bHasTouch = true;
						}
						else
						{
							if (timeSinceActive <= 0)
							{
								timeSinceTouched += Time.deltaTime;
								if (timeSinceTouched >= latchTime)
								{
									//gui.enabled = true;
									gui.pixelInset = new Rect(touch.position.x - guiTouchOffset.x, touch.position.y - guiTouchOffset.y, 200, 200);

									LastTouch = touch.position;
									Vector3 pos = new Vector3(touch.position.x - guiTouchOffset.x, touch.position.y - guiTouchOffset.y, 0);
									Rotation = Mathf.Atan2(pos.y, pos.x) * 57.2f;
									Background.transform.position = new Vector3(pos.x, pos.y, 0);
									RepositionJoystick();
									Background.color = new Color(Background.color.r, Background.color.g, Background.color.b, 1);
									timeSinceActive = fadeTime;
									timeSinceTouched = 0;
									bHasTouch = true;
								}

							}
							else
							{
								//gui.enabled = true;
								bHasTouch = true;
							}
						}

					}
					else
					{
						if (FreeJoystickEnabled)
						{
							bHasTouch = true;
						}
						else
						{
							if (timeSinceActive <= 0) // If the joystick is currently unlatched
							{
								timeSinceTouched += Time.deltaTime;
								if (timeSinceTouched >= latchTime)
								{
									gui.pixelInset = new Rect(touch.position.x - guiTouchOffset.x, touch.position.y - guiTouchOffset.y, 200, 200);


									RepositionJoystick();
									Background.color = new Color(Background.color.r, Background.color.g, Background.color.b, 1);
									timeSinceActive = fadeTime;
									timeSinceTouched = 0;
									bHasTouch = true;
								}
							}
							else // we re-scure the latch by setting the timeSinceActive back to the maximum
							{
								Background.color = new Color(Background.color.r, Background.color.g, Background.color.b, 1);
								timeSinceActive = fadeTime;
								bHasTouch = true;
							}
						}
					}

					if (bHasTouch == true)
					{
						position.x = (gui.pixelInset.x + scaledTouchOffset.x - guiCenter.x) / scaledTouchOffset.x;
						position.y = (gui.pixelInset.y + scaledTouchOffset.y - guiCenter.y) / scaledTouchOffset.y;
						Rotation = ((Mathf.Atan2(position.y, position.x) * -57.2f) + 90) % 360;
					}

					// Override the tap count with what the iPhone SDK reports if it is greater
					// This is a workaround, since the iPhone SDK does not currently track taps
					// for multiple touches
					if (touch.tapCount > tapCount)
						tapCount = touch.tapCount;

					// Change the location of the joystick graphic to match where the touch is
					Rect r = gui.pixelInset;

					r.x -= guiTouchOffset.x;
					r.y -= guiTouchOffset.y;
					r.x = Mathf.Clamp(guiTouchPos.x, defaultRect.xMin - guiTouchOffset.x, defaultRect.xMax - guiTouchOffset.x);
					r.y = Mathf.Clamp(guiTouchPos.y, defaultRect.yMin - guiTouchOffset.x, defaultRect.yMax - guiTouchOffset.x);

					gui.pixelInset = r;


					if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
					{
						ResetJoystick();
					}
				}
				Vector2 touchPos;
				touchPos.x = (gui.pixelInset.x + scaledTouchOffset.x - guiCenter.x) / scaledTouchOffset.x;
				touchPos.y = (gui.pixelInset.y + scaledTouchOffset.y - guiCenter.y) / scaledTouchOffset.y;
				Rotation = ((Mathf.Atan2(touchPos.y, touchPos.x) * -57.2f) + 90) % 360;
				#endregion
			}
		}

		// Adjust for dead zone	
		var absoluteX = Mathf.Abs(position.x);
		var absoluteY = Mathf.Abs(position.y);
		hitDeadzone = false;
		Vector2 diff = position - BufferZone;
		diffDebug = diff;
		if (position.magnitude < 0.5f)
		{
			position.x = 0;
			position.y = 0;
			hitDeadzone = true;
		}
			
	}

}
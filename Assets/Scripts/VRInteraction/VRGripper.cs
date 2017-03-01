using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Valve.VR;

public enum HandAssignment
{
    None,
    Left,
    Right,
}

/// <summary>
/// VR gripper. Component goes on the controller. It is used to make physics joint connections with objects in the scene. Also handles haptic feedback along those connections
/// </summary>
public class VRGripper : MonoBehaviour {

	/// <summary>
	/// The current controller this component is attatched to.
	/// </summary>
	SteamVR_TrackedObject CurrentController;

	/// <summary>
	/// Optional audio source for playing surface hits
	/// </summary>
	AudioSource CurrentAudio; 

	/// <summary>
	/// List of controllers used by VRInteractable, can be accessed via the static method GetControllers
	/// </summary>
	private static List<SteamVR_TrackedObject> ControllerList = new List<SteamVR_TrackedObject>();

	/// <summary>
	/// List of controllers used by VRInteractable, can be accessed via the static method GetControllers
	/// </summary>
	private static List<VRGripper> Grippers = new List<VRGripper>();


	/// <summary>
	/// Gets the controllers currently managed by grippers
	/// </summary>
	/// <returns>List of SteamVR_TrackedObject </returns>
	public static List<SteamVR_TrackedObject> GetControllers()
	{
		return new List<SteamVR_TrackedObject> (ControllerList);
	}

	public static List<VRGripper> GetGrippers()
	{
		return new List<VRGripper> (Grippers);
	}

	public static VRGripper GetGripper(SteamVR_TrackedObject _controller)
	{
		foreach (VRGripper gripper in Grippers) {
			if (gripper.CurrentController == _controller)
				return gripper;
		}

		return null;
	}

	public static VRGripper GetGripper(Transform _object)
	{
		foreach (VRGripper gripper in Grippers) {
			if (gripper.transform == _object)
				return gripper;
		}

		return null;
	}


    public SteamVR_TrackedObject GetController()
    {
        return CurrentController;
    } 

	void OnEnable()
	{
		// Cache local components
		CurrentController = GetComponent<SteamVR_TrackedObject> ();
		CurrentAudio = GetComponent<AudioSource> ();

		// Add this controller to the static list
		if (ControllerList.Contains (CurrentController) == false)
			ControllerList.Add (CurrentController);

		if (Grippers.Contains (this) == false)
			Grippers.Add (this);
		

		if (DebugOrb == null)
			return;
		
		HmdQuad_t pRect = new HmdQuad_t();
		if (SteamVR_PlayArea.GetBounds (SteamVR_PlayArea.Size.Calibrated, ref pRect) == false) {
			Debug.Log ("-- Failed to get bounds");
		} else {
			Debug.Log ("-- gour bounds,Spwaning bounds");
			Transform t;
			t =(Transform) Instantiate (DebugOrb, new Vector3(pRect.vCorners0.v0, pRect.vCorners0.v1,pRect.vCorners0.v2) + CameraRig.transform.position, Quaternion.identity);
			t.parent = tParent;
			t.gameObject.SetActive (false);
			t= (Transform)Instantiate (DebugOrb, new Vector3(pRect.vCorners1.v0, pRect.vCorners1.v1,pRect.vCorners1.v2) + CameraRig.transform.position, Quaternion.identity);
			t.parent = tParent;
			t= (Transform)Instantiate (DebugOrb, new Vector3(pRect.vCorners2.v0, pRect.vCorners2.v1,pRect.vCorners2.v2) + CameraRig.transform.position, Quaternion.identity);
			t.parent = tParent;
			t= (Transform)Instantiate (DebugOrb, new Vector3(pRect.vCorners3.v0, pRect.vCorners3.v1,pRect.vCorners3.v2) + CameraRig.transform.position, Quaternion.identity);
			t.parent = tParent;
			Debug.Log ("-- bounds created");

		}

	}

	public Transform tParent;
	public Transform CameraRig;
	public Transform DebugOrb;
	void OnDisable()
	{
	
		// When the controller is disabled it need to be removed from the static list
		if (ControllerList.Contains (CurrentController) == true)
			ControllerList.Remove (CurrentController);

		// When the controller is disabled it need to be removed from the static list
		if (Grippers.Contains (this) == true)
			Grippers.Remove (this);
		
	}

	void OnDestroy()
	{
		if (ControllerList.Contains (CurrentController) == true)
			ControllerList.Remove (CurrentController);
		
		// When the controller is disabled it need to be removed from the static list
		if (Grippers.Contains (this) == true)
			Grippers.Remove (this);
	}

	/// <summary>
	/// Used to prevent multiple collision reactions
	/// </summary>
	private bool isColliding = false;

	/// <summary>
	/// Is this controller currently gripping something?
	/// </summary>
	private bool isGripping = false;

	/// <summary>
	/// How long the haptic pulse lasts
	/// Note this will likely be moved to another class in future releases
	/// </summary>
	public float VibrationLength = 0.1f;

	/// <summary>
	/// The haptic pulse strength.
	/// Note this will likely be moved to another class in future releases
	/// </summary>
	public float HapticPulseStrength = 0.5f;

	/// <summary>
	/// The haptic frame time.
	/// Note this will likely be moved to another class in future releases
	/// </summary>
	public float HapticFrameTime = 0.001f;


	void OnCollisionEnter(Collision _collision)
	{
		Debug.Log ("COlliding Dfsdf");
		// If we're not already holding something or colliding then play our response
		if (isColliding == false && isGripping == false) {
			
			//var device = SteamVR_Controller.Input ((int)CurrentController.index);
			//device.TriggerHapticPulse (HapticPulseStrength, Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad);
			StartCoroutine(LongVibration(HapticPulseStrength, VibrationLength));
			CurrentAudio.Play ();
			isColliding = true;
		}
	}

	// Ugly debug stuff
	void TestEnumeration()
	{
		//var device = SteamVR_Controller.Input ((int)CurrentController.index);
		//device.index
		 
	}


	/// <summary>
	/// Tracks if a haptic response is currently active
	/// </summary>
	bool isVibrating = false;

	//
	//

	/// <summary>
	/// Longs the vibration.
	/// </summary>
	/// <param name="_strength">Strength. strength is vibration strength from 0-1</param>
	/// <param name="_duration">Duration. length is how long the vibration should go for</param>
	IEnumerator LongVibration(float _strength, float _duration ) {

		// TODO: set vibration modes
		// Additive
		// Replace
		// Ignore

		if (isVibrating == true) // Already active so exit (will be updated to allow additive or raplce option in future versions
			yield break;
		
		isVibrating = true;

		_strength = Mathf.Clamp (_strength, 0, 1);
		var device = SteamVR_Controller.Input ((int)CurrentController.index); // Get the device 

		// The controller pulses for upto 3.999ms out of each 11ms window (90 fps). Which gives us our intensity.
		for(float i = 0; i < _duration; i += Time.fixedDeltaTime) { 
			device.TriggerHapticPulse ((ushort)(_strength*3999), Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad);
			yield return null;
		}

		// finished
		isVibrating = false;
	}

	IEnumerator LongVibrationEaseStrength(float _strengthMax, EaseType _easeType, float _duration ) {

		// TODO: set vibration modes
		// Additive
		// Replace
		// Ignore

		if (isVibrating == true) // Already active so exit (will be updated to allow additive or raplce option in future versions
			yield break;

		isVibrating = true;

		_strengthMax = Mathf.Clamp (_strengthMax, 0, 1);
		var device = SteamVR_Controller.Input ((int)CurrentController.index); // Get the device 

		// The controller pulses for upto 3.999ms out of each 11ms window (90 fps). Which gives us our intensity.
		for(float i = 0; i < _duration; i += Time.fixedDeltaTime) { 
			float value = 1 - (i / _duration);
			float str = Ease.GetValue (_easeType, value);
			//Debug.Log ("str = " + str.ToString() + " duration = " + i + " div = " + value.ToString() ) ;
			device.TriggerHapticPulse ((ushort)(_strengthMax*str*3999), Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad);
			yield return null;
		}

		// finished
		isVibrating = false;
	}

//	public bool test = false;
//	public EaseType TestType;
//	public float testDuration = 1;
//	public float testStrength = 1;
//	void Update()
//	{
//		if (test) {
//			test = false;
//			StartCoroutine (LongVibrationEaseStrength (testStrength, TestType, testDuration));
//		}
//	}

	/// <summary>
	/// Trigger a haptic pulse with given strength.
	/// </summary>
	/// <param name="_strength">Strength.</param>
	public void HapticPulse(float _strength)
	{
		
		var device = SteamVR_Controller.Input ((int)CurrentController.index);
		device.TriggerHapticPulse ((ushort)(_strength*3999), Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad);
	}

	/// <summary>
	/// Vibrates the controller at given strength for given duration
	/// </summary>
	/// <param name="_strength">Strength.</param>
	/// <param name="_duration">Duration.</param>
	public void HapticVibration(float _strength, float _duration)
	{
		StartCoroutine(LongVibration(_strength, _duration)); 
	}

	public void HapticVibrationEaseStrength(float _strength, EaseType _type, float _duration)
	{
		StartCoroutine(LongVibrationEaseStrength(_strength, _type,  _duration)); 
	}

	void OnCollisionExit(Collision _collision)
	{
		isColliding = false;
	}

}

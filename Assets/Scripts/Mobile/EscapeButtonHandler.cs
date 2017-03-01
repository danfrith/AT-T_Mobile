using UnityEngine;
using System.Collections;
using UnityEngine.Events;

[System.Serializable]
public class EscapeKeyEvent : UnityEvent<EscapeButtonHandler> {}

public class EscapeButtonHandler : MonoBehaviour {

	public EscapeKeyEvent ButtonListeners;

	//public Event 
	void Update()
	{
		if (Input.GetKeyDown (KeyCode.Escape)) {
			if (ButtonListeners != null) { // Trigger our callbacks
				ButtonListeners.Invoke (this);
			}
		}
	}

}

using UnityEngine;
using System.Collections;

public class DebugDisable : MonoBehaviour {

    void OnEnable()
    {
        if (MobileGameManager.Instance.GodMode == false)
        {
            gameObject.SetActive(false);
        }
    }
}

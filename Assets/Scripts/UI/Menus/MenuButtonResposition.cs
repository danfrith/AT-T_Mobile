using UnityEngine;
using System.Collections;

public class MenuButtonResposition : MonoBehaviour {

    public float Position = 1;
    public float Padding;
    public bool isContinueButton = false;
    private void OnEnable()
    {
        if (isContinueButton)
        {
            if (MobileGameManager.Instance.HasSaveGame() == false)
                gameObject.SetActive(false);
            else
                Debug.Log("has save game");
        }
        else
        {
            float x = Screen.width * 0.25f;

            if (MobileGameManager.Instance.HasSaveGame() == false)
                transform.position = new Vector3(x * (0.55f + Position) + Padding, transform.position.y, transform.position.z);
            else
                //transform.position = new Vector3(x * Position + Padding, transform.position.y, transform.position.z);

            Debug.Log("Transform .pos = " + transform.position);
            Debug.Log("Screen width = " + Screen.width);
        }
    }
}

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SetColourForWorld : MonoBehaviour {

    private void Awake()
    {
        SetColour();
    }

    void SetColour()
    {
        int world = (MobileGameManager.Instance.CurrentLevel-1) / 5;

        Color c = IngameCanvasColours.Instance.GetWorldColour(world);

        //Debug.Log("((((((((((((((( Setting colour on " + name);
        Text t = GetComponent<Text>();
        if (t != null)
            t.color = c;

        Image i = GetComponent<Image>();
        if (i != null)
            i.color = c;
    }
}

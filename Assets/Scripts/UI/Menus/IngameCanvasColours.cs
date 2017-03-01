using UnityEngine;
using System.Collections;

public class IngameCanvasColours : MonoBehaviour {

    public Color WorldOneColour;
    public Color WorldTwoColour;
    public Color WorldThreeColour;
    public Color WorldFourColour;

    public static IngameCanvasColours Instance;

    private void OnEnable()
    {
        Instance = this;
    }

    public Color GetWorldColour(int _world)
    {
        switch (_world)
        {
            case 0:
                return WorldOneColour;
                break;
            case 1:
                return WorldTwoColour;
                break;
            case 2:
                return WorldThreeColour;
                break;
            case 3:
                return WorldFourColour;
                break;
        }

        return WorldOneColour;
    }
}

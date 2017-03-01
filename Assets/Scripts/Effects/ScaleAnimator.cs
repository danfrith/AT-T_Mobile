using UnityEngine;
using System.Collections;

public class ScaleAnimator : MonoBehaviour {

    public Vector3 ScaleTo;
    //public EaseType MoveStyle;
    public float PingPongTime = 1;

    float ElapsedTime = 0;

    void OnEnable()
    {
        OriginalScale = transform.localScale;

        diff = ScaleTo - OriginalScale;
        diff = diff / PingPongTime;
    }

    bool isInit = true;
    Vector3 OriginalScale;
    Vector3 PingPongOffset;

    Vector3 diff;

    void Update()
    {
        if (isInit == false)
            return;

        
        ElapsedTime += Time.deltaTime;
        
        if (ElapsedTime > PingPongTime)
        {
            if (ElapsedTime > PingPongTime * 2)
                ElapsedTime = 0;
            else
            {
                PingPongOffset -= diff * Time.deltaTime;
            }
        }
        else
            PingPongOffset += diff * Time.deltaTime;

        
        transform.localScale = PingPongOffset + OriginalScale;
    }


}

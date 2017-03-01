using UnityEngine;
using System.Collections;

public class AIBase : MonoBehaviour {

    bool AIEnabled = true;

    AIBase GetBase()
    {
        return this;
    }

    void OnEnable()
    {
        if (AIEnabled)
        {

        }
    }
//
//    void Awake()
//    {
//        this.enabled = false;
//    }
//
//    void OnEnable()
//    {
//        this.enabled = false;
//    }
}

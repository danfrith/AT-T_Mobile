using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogoHolder : MonoBehaviour {
    static public LogoHolder _instance;
    public GameObject logoHolder;

    void Awake()
    {
        _instance = this;
    }

    // Use this for initialization
    
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}



    public void hide()
    {
        logoHolder.SetActive(false);
    }

    public void show()
    {
        logoHolder.SetActive(true);
    }
}

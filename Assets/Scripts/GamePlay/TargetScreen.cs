using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TargetScreen : MonoBehaviour {

    GameObject HMD;

    float startHeight;

    public int MeasurementCount = 5;
    public float Offset = 1;

    public GameObject myCam;
	void OnEnable()
    {
        startHeight = transform.position.y;

        //HMD = GameObject.Find("Camera (head)");
        HMD = Camera.main.gameObject;
        myCam = HMD;

        Measurements = new float[MeasurementCount];
        for (int i = 0; i < MeasurementCount; i++)
        {
            Measurements[i] = startHeight;
        }

        StartCoroutine(GetHeightMeasurement());

    }

    float[] Measurements;
    IEnumerator GetHeightMeasurement()
    {
        int count = 0;
        while (true)
        {
            yield return new WaitForSeconds(1);
            Measurements[count] = HMD.transform.position.y;
            SetHeight(GetAverageHeight());
        }
    }

    float GetAverageHeight()
    {
        float total = 0;
        for (int i = 0; i < MeasurementCount; i++)
        {
            total += Measurements[i];
        }

        return total / MeasurementCount;
    }

    void SetHeight(float _newValue)
    {

        if (_newValue < startHeight)
            _newValue = startHeight;

        transform.position = new Vector3(transform.position.x, _newValue + Offset, transform.position.z);

    }

}

using UnityEngine;
using System.Collections;

public class LightPulseFactory : MonoBehaviour {

    public Transform[] Path;

    public Transform LightPulsePrefab;

    public float SpawnRate;

	public Vector3 SpawnOffset;

    private float mElapsedTime = 0;

	public bool WaitForLevelStart = false;

    public void Update()
    {
		if (WaitForLevelStart == true && Stage.Instance.IsRunning == false)
			return;
		
        mElapsedTime += Time.deltaTime;

        if (mElapsedTime > SpawnRate)
        {
            SpawnLightPulse();
            mElapsedTime = 0;
        }
    }

    public void SpawnLightPulse()
    {
        if (LightPulsePrefab == null)
        {
            Logger.LogError("LightPulseFactory: prefab is missing for " + name);
            return;
        }

        Vector3 pos = transform.position;

        if (Path == null)
            pos = Path[0].position;

		Transform t = (Transform)Instantiate(LightPulsePrefab, pos + SpawnOffset, Quaternion.identity);
        t.parent = this.transform;

        PointToPoint p = t.gameObject.GetComponent<PointToPoint>();

        if (p != null)
            p.Points = Path;

		Person per = (Person)t.gameObject.GetComponent<Person>();

		if (per != null)
			InitializePerson (per, transform);

    }

	public void InitializePerson(Person _person, Transform _spawner)
	{
		PathList pl = _spawner.GetComponent<PathList> ();
		if (pl == null) {
			Logger.LogError ("Pathlist not available on " + _spawner.name);
			return;
		}
			
		//Debug.Log("Setting pth to " +  pl.Paths.Length);
		_person.SetPath (pl.Paths [Random.Range (0, pl.Paths.Length)], pl.SelectedPath);

	}

}

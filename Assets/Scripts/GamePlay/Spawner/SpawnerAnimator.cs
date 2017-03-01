using UnityEngine;
using System.Collections;

public class SpawnerAnimator : MonoBehaviour {

    public Transform SpawnEffectPrefab;

    public Transform SpawnLocation;

    public float VirusEffectDelay = 0.5f;
	public bool SpawnAsChild = true;
    public VirusBase DelegatedSpawn(Transform _virusPrototype, int _id, Transform _spawner)
    {
        Debug.Log("Delegated spawn");
        if (Stage.Instance.IsRunning == false) // Don't spawn anything if the level is not running.
            return null;

        if (SpawnLocation == null)
            SpawnLocation = transform;

        Transform t = (Transform)Instantiate(_virusPrototype, SpawnLocation.position, Quaternion.identity);

        t.name = t.name + _id.ToString();

		if (SpawnAsChild)
        	t.parent = this.transform;

        Debug.Log("Spawned " + _virusPrototype.name + " at " + _spawner.name + " pos " + SpawnLocation.position);

        VirusBase v = t.gameObject.GetComponent<VirusBase>();
        v.SeteffectDelay(VirusEffectDelay);

        StartAnimator(v);

        return v;
    }

	public Flash OtherObjectFlash;
    public void StartAnimator(VirusBase _virus)
    {
        Debug.Log("Starting animator " + name);

        if (SpawnEffectPrefab != null)
            Instantiate(SpawnEffectPrefab, transform.position, Quaternion.identity);

        Flash f = GetComponent<Flash>();
        if (f != null)
            f.StartFlash();
        else
            Debug.Log("flash is null");

		if (OtherObjectFlash != null)
			OtherObjectFlash.StartFlash ();

    }
}

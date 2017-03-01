using UnityEngine;
using System.Collections;

public class ProxySpawner : MonoBehaviour {

    public Transform SpawnedPrefab;
    public int Quantity;
    public float Delay;
    public float DelayBetweenSpawns;

    public Transform SpawnEffectPrefab;
    public Transform ProxySpawnEffectPrefab;
    private MobSpawner mSpawner;
    public Vector3 Offset;
    void OnEnable()
    {
        StartCoroutine(Init());
    }

    IEnumerator Init()
    {
        yield return new WaitForEndOfFrame();

        mSpawner = MobSpawner.Instance;
        //Debug.Log("ProxySpawner Spawner = " + mSpawner.name);

        InstantiateSpawnEffect();
        StartCoroutine(SpawnUnits());

    }

    IEnumerator SpawnUnits()
    {
        yield return new WaitForSeconds(Delay);

        while (Quantity >= 1)
        {
            Debug.Log("Spawning virus from " + name);
            InstantiateVirus();
            InstantiateProxySpawnEffect();
            Quantity -= 1;
            yield return new WaitForSeconds(DelayBetweenSpawns);
        }
    }

    void InstantiateProxySpawnEffect()
    {
        
        Person p = GetComponent<Person>();
        if (p != null)
        {
            if (ProxySpawnEffectPrefab)
                Instantiate(ProxySpawnEffectPrefab, transform.GetChild(0).position + Offset, Quaternion.identity);

            Debug.Log("Triggering person flash " + name);
            FadeInFadeOutDestroy f = GetComponent<FadeInFadeOutDestroy>();

            f.StartFlash();

        }
        else
        {
            if (ProxySpawnEffectPrefab)
                Instantiate(ProxySpawnEffectPrefab, transform.position, Quaternion.identity);
        }
    }

    void InstantiateSpawnEffect()
    {
        if (SpawnEffectPrefab)
        {
             Instantiate(SpawnEffectPrefab, transform.position, Quaternion.identity);
            
        }

    }

    Transform InstantiateVirus()
    {
        
        if (Stage.Instance.IsRunning == false) // Don't spawn anything if the level is not running.
            return null;

        Person p = GetComponent<Person>();
        Transform t;
        if (p != null)
        {
            t = (Transform)Instantiate(SpawnedPrefab, transform.GetChild(0).position + Offset, Quaternion.identity, mSpawner.transform);
        }
        else
        {
            t = (Transform)Instantiate(SpawnedPrefab, transform.position, Quaternion.identity, mSpawner.transform);
        }

        VirusBase en = (VirusBase)t.gameObject.GetComponent<VirusBase>();

        IgnoreColliders(t, transform); // Prevent the spawned items colliding with the spawner.

        // TODO: Possibly move these calls into VirusBase itself, in the case that we spawn viruses from an "effect"
        if (en != null)
            mSpawner.AddEnemy(en);

        return t;
    }

    public static void IgnoreColliders(Transform _objA, Transform _objB)
    {
        Collider[] colliders = _objA.gameObject.GetComponentsInChildren<Collider>();
        Collider[] collidersB = _objB.gameObject.GetComponentsInChildren<Collider>();
        foreach (Collider ColA in colliders)
        {
            foreach (Collider ColB in collidersB)
                Physics.IgnoreCollision(ColA, ColB);
        }

        Collider colA = _objA.GetComponent<Collider>();
        Collider colB = _objB.GetComponent<Collider>();
        if  (colA != null && colB != null)
            Physics.IgnoreCollision(colA, colB);
    }



}

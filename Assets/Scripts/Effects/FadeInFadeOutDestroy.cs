using UnityEngine;
using System.Collections;

public class FadeInFadeOutDestroy : MonoBehaviour {

    Material mMaterial;

    public float FadeDuration = 1;
    public float LifeSpan = 10;
    public EaseType EaseType;

	public SkinnedMeshRenderer mMeshRenderer;

    void OnEnable()
    {
        MeshRenderer mr = GetComponent<MeshRenderer>();
        if (mr == null)
        {
            Logger.LogError("FadeInFadeOutdestroy: MeshRenderer component is missing on " + name);
            return;
        }

        mMaterial = mr.material;

        if (( FadeDuration*2) > LifeSpan )
        {
            Logger.LogError("FadeInFadeOutDestroy: Lifespan is too short for " + name);
            return;
        }

        StartCoroutine(FadeIn());

    }

//	void Start()
//	{
//		if (mMeshRenderer == null)
//			return;
//
//		Transform t = transform.GetChild (0);
//		t.GetComponent<Animator>().CrossFade("walk", 0.1f, 0, Random.Range(0.0f, 1.0f));
//		t.localRotation = Quaternion.identity;
//
//		Debug.Log ("Set animation to walk");
//	}

    public float debugVal;
    public float t;

    IEnumerator FadeIn()
    {
        float duration = 0;
        Color c;

        while (duration < FadeDuration)
        {
            duration += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
            t = duration;
            c = mMaterial.color;
            c.a = Ease.GetValue(EaseType, duration / FadeDuration);
            mMaterial.color = c;

            if (mMeshRenderer != null)
				mMeshRenderer.material.color = c;
            debugVal = c.a;
        }

        mColourCached = mMaterial.color;

        StartCoroutine(LifeDuration());
    }

    public float Echo = 1;
    public float Attack = 0;
    public float Sustain = 0;
    public float Drop;

    public Color FlashColour;

    Color mColourCached;
    bool bAnimating = false;

    // Cached var
    Color CurrentColour;

    public void StartFlash()
    {
        if (!bAnimating)
            StartCoroutine(FlashAnimate());
        else
            Debug.Log("|||| Already animating ");
    }
    //public bool test = false;

    //void Update()
    //{
    //    if (test)
    //    {
    //        test = false;
    //        StartCoroutine(FlashAnimate());
    //    }
    //}

    IEnumerator FlashAnimate()
    {
        Echo = Echo < 1 ? 1 : Echo; // Sanity check echo value

        bAnimating = true;

        float Movement = 0;
        float echoing = 1;
        float EchoChange = 1 / Echo;

        // If the echo is above the threshold...keep flashing
        while (echoing > 0.2f)
        {

            Movement = 0;
            while (Movement < Attack * echoing) // Get brighter...
            {
                yield return new WaitForFixedUpdate();

                Movement += Time.fixedDeltaTime;
                CurrentColour = Color.Lerp(mColourCached, FlashColour, Movement / (Attack * echoing));
                
                CurrentColour.a = 1;
                //mMaterial.color = CurrentColour;
                
                if (mMeshRenderer != null)
                    mMeshRenderer.material.color = CurrentColour;

            }
            
            Movement = 0;
            while (Movement < Sustain * echoing) // Stay at thi brightness...
            {
                yield return new WaitForFixedUpdate();
                Movement += Time.fixedDeltaTime;
            }

            Movement = 0;
            while (Movement < Drop * echoing) // Return to original colour...
            {
                yield return new WaitForFixedUpdate();
                Movement += Time.fixedDeltaTime;
                CurrentColour = Color.Lerp(FlashColour, mColourCached, Movement / (Drop * echoing));
                CurrentColour.a = 1;
                
                if (mMeshRenderer != null)
                    mMeshRenderer.material.color = CurrentColour;
            }
            echoing -= EchoChange; // Update the echo value and repeat

        }

        if (mMeshRenderer != null)
            mMeshRenderer.material.color = mColourCached;
        bAnimating = false;
    }

    IEnumerator LifeDuration()
    {
        
        yield return new WaitForSeconds(LifeSpan);

        
        StartCoroutine(FadeOut());
    }

    IEnumerator FadeOut()
    {
        
        float duration = FadeDuration;
        Color c;

        while (duration > 0)
        {
            duration -= Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
            t = duration;
            c = mMaterial.color;
            c.a = Ease.GetValue(EaseType, duration / FadeDuration);
            mMaterial.color = c;
			if (mMeshRenderer != null)
				mMeshRenderer.material.color = c;
            debugVal = c.a;
        }

        Destroy(gameObject);

    }

}

using UnityEngine;
using System.Collections;

/// <summary>
/// Shield body.
/// Maps the thumb stick positions to a position in 3d space on a 2d plane. This plane should correspond exactly to the mobile's
/// viewport. 
/// </summary>
public class ShieldBody : MonoBehaviour {

	public Vector3 BottomLeft;
	public Vector3 TopRight;

	public Vector2 TestPos = Vector2.zero;

	private Vector2 CachedPos = Vector2.zero;

	void OnValidate()
	{
		if (CachedPos != TestPos) {
			CachedPos = TestPos;
			SetPosition (TestPos);
		}

        if (TestFadeOut)
        {
            TestFadeOut = false;
            StartFadeOut();
        }
        
    }
    public float OffsetZ = -2.88f;
	public float OffsetY = 0.52f;
	// Pos is a vect who's values are between 0 and 1.
	public void SetPosition(Vector2 _pos)
	{
		Vector2 Rect = TopRight - BottomLeft;
		Vector3 pos = new Vector3 (BottomLeft.x + Rect.x * _pos.x, BottomLeft.y + Rect.y * _pos.y, OffsetZ);
		pos.y += OffsetY;
		transform.localPosition = pos;
	}

    public void StartFadeOut()
    {
        StartCoroutine(FadeOutRoutine());
    }

    public void StopFadeOut()
    {
        isFadingOut = false;

        MeshRenderer.material.color = ShieldBodyColourCached;

        MeshRenderer.material.SetColor("_MKGlowColor", ShieldGlowColourCached);

        mCollider.enabled = true;
    }

    Renderer MeshRenderer;

    private Color ShieldBodyColourCached;
    private Color ShieldGlowColourCached;

    private Collider mCollider;
    void OnEnable()
    {
        MeshRenderer = GetComponent<Renderer>();

        ShieldBodyColourCached = MeshRenderer.material.color;
        ShieldGlowColourCached = MeshRenderer.material.GetColor("_MKGlowColor");

        mCollider = GetComponent<Collider>();
    }

    bool isFadingOut = false;
    public bool TestFadeOut = false;
    public EaseType FadeType;
    

    public float fadeDuration = 4;

    IEnumerator FadeOutRoutine()
    {
        if (isFadingOut)
            yield break;

        isFadingOut = true;

        float t = 0;

        Color TempColour = Color.green;
        Color Transparent = new Color(0, 0, 0, 0);
        //Debug.Log("Starting lerp " + ShieldBodyColourCached);
        while(t < fadeDuration)
        {
            yield return new WaitForFixedUpdate();

            if (!isFadingOut)
            {
                yield break;
            }

            t += Time.fixedDeltaTime;

            TempColour = Color.Lerp(ShieldBodyColourCached, Transparent, Ease.GetValue(FadeType, t / fadeDuration));

            //Debug.Log(t + " lerping " + TempColour.ToString());

            MeshRenderer.material.color = TempColour;

            TempColour = Color.Lerp(ShieldGlowColourCached, Transparent, Ease.GetValue(FadeType, t / fadeDuration));
            MeshRenderer.material.SetColor("_MKGlowColor", TempColour);

        }

        MeshRenderer.material.SetColor("_MKGlowColor", Transparent);

        mCollider.enabled = false;
        isFadingOut = false;
    }

    //IEnumerator FadeInRoutine()
    //{
    //    if (isFadingIn)
    //        yield break;

    //    Color CurrentBody = MeshRenderer.material.color;
    //    Color CurrentGlow = MeshRenderer.material.GetColor("_MKGlowColor");

    //    float t = 0;

    //    while (t < fadeDuration)
    //    {

    //    }
    //}
}

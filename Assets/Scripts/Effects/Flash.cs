using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public enum FlashTargets
{
    None, // Default option for instantiation
    MeshRenderer,
    SpriteRenderer,
    Image,
    Text,
}

/// <summary>
/// Causes the object to flash when hit. 
/// Requires shader with "_TintColor" or a sprite type rendrer
/// </summary>
public class Flash : MonoBehaviour
{

    /// <summary>
    /// Set which renderer to change
    /// </summary>
    private Renderer mRenderer;
    private SpriteRenderer mSpriteRenderer;
    private Image mImage;
    private Text mText;

    /// <summary>
    /// Flash colour
    /// </summary>
    [Tooltip("Flash colour")]
    public Color FlashColour;

    /// <summary>
    /// How quickly the object lights up        
    /// </summary>
    [Tooltip("How quickly the object lights up")]
    public float Attack;

    /// <summary>
    /// How long it stays lit
    /// </summary>
    [Tooltip("How long it stays lit")]
    public float Sustain;

    /// <summary>
    /// Number of repeats
    /// </summary>
    [Tooltip("Number of repeats")]
    public float Echo;

    /// <summary>
    /// How quickly it returns to its original colour
    /// </summary>
    [Tooltip("How quickly it returns to its original colour")]
    public float Drop;

    /// <summary>
    /// Shader variable to alter (not used for SpriteRenderers
    /// </summary>
    [Tooltip("Shader variable to alter (not used for SpriteRenderers")]
    public string ColourName = "";

    /// <summary>
    /// Set the target renderer to apply the flash effect to
    /// </summary>
    [Tooltip("Set the target renderer to apply the flash effect to")]
    public FlashTargets RendererType;


    /// <summary>
    /// Set the target renderer to apply the flash effect to
    /// </summary>
    [Tooltip("Makes this component flash continuously on a loop")]
    public bool LoopFlash = false;

    /// <summary>
    /// Set the target renderer to apply the flash effect to
    /// </summary>
    [Tooltip("Makes this component flash when it is enabled")]
    public bool FlashOnEnable = false;

	/// <summary>
	/// Sets the "colour" field to a float field, using the alpha channel of the selected colour for the float value
	/// </summary>
	[Tooltip("Makes this component flash when it is enabled")]
	public bool ColourAsFloat = false;

	public bool MultipleMaterials = false;
	public int MaterialIndex = 0;

    // Private cached vars
    private Color mColourCached;
    private Color CurrentColour;

	public Color GetCurrentColour()
	{
		return CurrentColour;
	}

    private bool bCached = false;
    private bool bAnimating = false;

    public event System.EventHandler FlashCompleteEvent;

    void OnValidate()
    {
        Validate();
    }

    /// <summary>
    /// Validates the flash component
    /// </summary>
    /// <returns>bool valid</returns>
    bool Validate()
    {
        if (RendererType == FlashTargets.Image)
            mImage = GetComponent<Image>();
        else if (RendererType == FlashTargets.SpriteRenderer)
            mSpriteRenderer = GetComponent<SpriteRenderer>();
        else if (RendererType == FlashTargets.Text)
            mText = GetComponent<Text>();
        else
            mRenderer = GetComponent<MeshRenderer>();

        // Find the slected renderer type
        if ((RendererType == FlashTargets.SpriteRenderer && mSpriteRenderer == null)
            || (RendererType == FlashTargets.MeshRenderer && mRenderer == null)
            || (RendererType == FlashTargets.Image && mImage == null)
            || (RendererType == FlashTargets.Text && mText == null)
            )
        {
            Debug.LogError(string.Format("The {0} component is missing on {1}!", RendererType.ToString(), name));
            return false;
        }

		MultipleMaterials = false;

		if (RendererType == FlashTargets.MeshRenderer) {
			
			if (mRenderer.sharedMaterials.Length > 1){
				MultipleMaterials = true;
			}
		}

        //if (RendererType == FlashTargets.None)
        //    Debug.LogError(string.Format("Flash target not set"));

        return true;
    }

    private bool bInit = false;

    void OnEnable()
    {
        bool validated = Validate();

        if (validated == true) // I have changes this to do it on a delay so objects can have their colour set before this component caches it.
            StartCoroutine(DelayedCacheColour());

        //mRenderer.material.SetColor("Main" , Color.black);
    }
	public void ReCacheColour()
	{
		CacheColour ();
	}

    void CacheColour()
    {
        // When re-enabling the object we check to see if there is a cached colour, if there is restore it
        // This is done incase the object is disabled while the coroutine is running
		if (bCached == false) {
			if (RendererType == FlashTargets.SpriteRenderer)
				mColourCached = mSpriteRenderer.color;
			else if (RendererType == FlashTargets.MeshRenderer) {
				CacheColourMeshRenderer ();
			} else if (RendererType == FlashTargets.Image)
				mColourCached = mImage.color;
			else if (RendererType == FlashTargets.Text)
				mColourCached = mText.color;
			else if (RendererType == FlashTargets.None)
				mColourCached = Color.white;
			else
				SetColour (CurrentColour);

			bInit = true;
			bCached = true;

			if (FlashOnEnable == true)
				StartCoroutine (FlashAnimate ());
		}
    }

	void CacheColourMeshRenderer()
	{
		
		// Getting value from a specific property on a material.
		if (!string.IsNullOrEmpty (ColourName)) {
			if (ColourAsFloat == true) {
				if (MultipleMaterials) // If a material is specified
					mColourCached.a = mRenderer.materials [MaterialIndex].GetFloat (ColourName);
				else
					mColourCached.a = mRenderer.material.GetFloat (ColourName);

			} else {
				if (MultipleMaterials) { // If a material is specified
					//Debug.Log ("Getting colour " + mRenderer.materials [MaterialIndex].name);
					mColourCached = mRenderer.materials [MaterialIndex].GetColor (ColourName);
				} else {
					//Debug.Log ("path 2 Getting colour " + mRenderer.material.name);
					mColourCached = mRenderer.material.GetColor (ColourName);
				}
			}

		} else {

			if (MultipleMaterials) { // If a material is specified
				//Debug.Log ("Getting colour " + mRenderer.materials [MaterialIndex].name);
				mColourCached = mRenderer.materials [MaterialIndex].color;
			} else {
				//Debug.Log ("path 2 Getting colour " + mRenderer.material.name);
				mColourCached = mRenderer.material.color;
			}

		}
		//Debug.Log ("Caching colour " + ColourName + " on object " + name +" colour "+  mColourCached);
	}

    IEnumerator DelayedCacheColour()
    {
        yield return new WaitForEndOfFrame();
        CacheColour();

    }

    /// <summary>
    /// Initializes the flash object
    /// </summary>
    public void Init()
    {
        OnEnable();
    }

    /// <summary>
    /// Starts the flash effect
    /// </summary>
    public void StartFlash()
    {
        if (bInit == true) 
            StartCoroutine(FlashAnimate());
    }

    /// <summary>
    /// Sets the flash colour
    /// </summary>
    /// <param name="_colour">Colour value</param>
    private void SetColour(Color _colour)
    {
        if (RendererType == FlashTargets.SpriteRenderer)
            mSpriteRenderer.color = _colour;
        else if (RendererType == FlashTargets.MeshRenderer)
		{
			SetColourMeshRenderer (_colour);
		}
        else if (RendererType == FlashTargets.Image)
            mImage.color = _colour;
        else if (RendererType == FlashTargets.Text)
            mText.color = _colour;
    }

	private void SetColourMeshRenderer(Color _colour)
	{
		//mRenderer.material.color = _colour;
		if (!string.IsNullOrEmpty (ColourName)) {
			if (ColourAsFloat == true) {

				if (MultipleMaterials) // If a material is specified
					mRenderer.materials [MaterialIndex].SetFloat (ColourName, _colour.a);
				else
					mRenderer.material.SetFloat (ColourName, _colour.a);
			
			} else {
				
				if (MultipleMaterials) // If a material is specified
					mRenderer.materials[MaterialIndex].SetColor (ColourName, _colour);
				else
					mRenderer.material.SetColor (ColourName, _colour);
				
			}
		} else {
			if (MultipleMaterials) // If a material is specified
				mRenderer.materials[MaterialIndex].color = _colour;
			else
				mRenderer.material.color = _colour;
		}
	}

    /// <summary>
    /// Flash animation coroutine
    /// </summary>
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

                SetColour(CurrentColour);
                
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

                SetColour(CurrentColour);
            }
            echoing -= EchoChange; // Update the echo value and repeat

        }

        //Debug.LogError("Flash complete event" + name);
        if (FlashCompleteEvent != null)
            FlashCompleteEvent(this, System.EventArgs.Empty);

        bAnimating = false;
    }

    /// <summary>
    /// Set to true to test the flash effect
    /// </summary>
    [Tooltip("Set to true to test the flash effect")]
    public bool test = false;

    
    void Update()
    {
        if (bInit == false) return;

        if (test == true)
        {
            test = false;
            StartCoroutine(FlashAnimate());
        }
        if (LoopFlash == true && bAnimating == false)
        {
            StartCoroutine(FlashAnimate());
        }

    }


}

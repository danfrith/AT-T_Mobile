using UnityEngine;
using System.Collections;

public class UVAnimator : MonoBehaviour {

    public SkinnedMeshRenderer Renderer;
    public MeshRenderer MeshRenderer;
    public Vector2 Rotation;

    public Vector2 PingPong;
    public float PingPongTime = 2;
    Vector2 PingPongOffset;
    float ElapsedTime = 0;
    Vector2 diff;
    Vector2 OriginalOffset;

    public bool isMKShader = false;

    bool isInit = true;
    void OnEnable()
    {
        if (Renderer == null && MeshRenderer == null)
        {
            Renderer = GetComponent<SkinnedMeshRenderer>();
            if (Renderer == null)
            {
                MeshRenderer = GetComponent<MeshRenderer>();
                if (MeshRenderer == null)
                    isInit = false;
            }
        }

        
		Init ();
    }

	void Init()
	{
		PingPongOffset = Vector2.one;

		if (Renderer != null) {
			diff = Renderer.material.mainTextureOffset = PingPong;
			OriginalOffset = Renderer.material.mainTextureOffset;
		} else if (MeshRenderer != null) {
			diff = MeshRenderer.material.mainTextureOffset = PingPong;
			OriginalOffset = MeshRenderer.material.mainTextureOffset;
		}
		else
			return;
		
		diff = diff / PingPongTime;


	}

//    void OnValidate()
//    {
//        PingPongOffset = Vector2.one;
//        diff = Renderer.material.mainTextureOffset = PingPong;
//        diff = diff / PingPongTime;
//
//    }

    Vector2 texOffset;
    
    void Update()
    {
        if (isInit == false)
            return;

        ElapsedTime += Time.deltaTime;

        

        if (ElapsedTime > PingPongTime)
        {
            if (ElapsedTime > PingPongTime*2)
                ElapsedTime = 0;
            else
            {
                PingPongOffset -= diff * Time.deltaTime;
            }
        }
        else
            PingPongOffset += diff * Time.deltaTime;

        //PingPongTime = 
        //PingPongOffset += PingPong * (PingPongTime 
        texOffset.x += Time.deltaTime * Rotation.x;
        texOffset.y += Time.deltaTime * Rotation.y;
        
        if (Renderer != null)
            Renderer.material.mainTextureOffset = OriginalOffset + PingPongOffset + texOffset;
        else
            MeshRenderer.material.mainTextureOffset = OriginalOffset + PingPongOffset + texOffset;

        if (isMKShader)
        {
            //Debug.Log("Setting mk offset");
            if (Renderer != null)
                Renderer.material.SetTextureOffset("_MKGlowTex", OriginalOffset + PingPongOffset + texOffset);
            else
                MeshRenderer.material.SetTextureOffset("_MKGlowTex", OriginalOffset + PingPongOffset + texOffset);
        }

    }
}

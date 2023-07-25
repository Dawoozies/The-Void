using RuntimeContainers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PixelDisplacement : MonoBehaviour
{
    public ComputeShader shader;
    int kernelIndex;
    public float updateTime;
    //when we "dissolve" a particle we push it to a texture
    Texture originalTexture;
    SpriteRenderer spriteRenderer;
    Material objectMaterial;
    public Material screenMaterial;
    [Range(0,1)]
    public float redCutoff;
    public RenderTexture originalEdited; //Just where we punch holes in the original texture
    int MaxWidth = 256;
    int MaxHeight = 256;
    public RenderTexture particles;
    public RenderTexture particlesAlt;
    bool particleUpdateAlternate;
    float time;
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        objectMaterial = spriteRenderer.material;
        originalTexture = spriteRenderer.sprite.texture;
        MaxWidth = Mathf.CeilToInt(spriteRenderer.sprite.rect.width/8);
        MaxHeight = Mathf.CeilToInt(spriteRenderer.sprite.rect.height/8);

        originalEdited = new RenderTexture(MaxWidth, MaxHeight, 24, RenderTextureFormat.ARGB32);
        originalEdited.enableRandomWrite = true;
        originalEdited.filterMode = FilterMode.Point;
        originalEdited.wrapMode = TextureWrapMode.Repeat;
        originalEdited.Create();

        MaxWidth = Screen.width;
        MaxHeight = Screen.height;
        particles = new RenderTexture(MaxWidth, MaxHeight, 24, RenderTextureFormat.ARGB32);
        particles.enableRandomWrite = true;
        particles.filterMode = FilterMode.Point;
        particles.wrapMode = TextureWrapMode.Repeat;
        particles.Create();
        particlesAlt = new RenderTexture(MaxWidth, MaxHeight, 24, RenderTextureFormat.ARGB32);
        particlesAlt.enableRandomWrite = true;
        particlesAlt.filterMode = FilterMode.Point;
        particlesAlt.wrapMode = TextureWrapMode.Repeat;
        particlesAlt.Create();

        shader.FindKernel("PixelDisplacement");
        shader.SetFloat("MaxWidth", MaxWidth);
        shader.SetFloat("MaxHeight", MaxWidth);
        shader.SetMatrix("CameraToWorld", Camera.main.cameraToWorldMatrix);

        objectMaterial.SetTexture("_EditedTexture", originalEdited);
        particleUpdateAlternate = false;
    }
    void Update()
    {
        if (time > 0)
            time -= Time.deltaTime;
        if(time <= 0)
        {
            time = updateTime;
            Render();
        }
    }
    private void Render()
    {
        shader.SetFloat("Time", Time.time);
        shader.SetTexture(kernelIndex, "originalEdited", originalEdited);
        shader.SetFloat("redCutoff", redCutoff);
        if(particleUpdateAlternate)
        {
            shader.SetTexture(kernelIndex, "particlesPrevious", particlesAlt);
            shader.SetTexture(kernelIndex, "particles", particles);
            shader.Dispatch(kernelIndex, MaxWidth, MaxHeight, 1);
            screenMaterial.SetTexture("_NewTexture", particles);
            particleUpdateAlternate = false;
        }
        else
        {
            shader.SetTexture(kernelIndex, "particlesPrevious", particles);
            shader.SetTexture(kernelIndex, "particles", particlesAlt);
            shader.Dispatch(kernelIndex, MaxWidth, MaxHeight, 1);
            screenMaterial.SetTexture("_NewTexture", particlesAlt);
            particleUpdateAlternate = true;
        }
        objectMaterial.SetTexture("_EditedTexture", originalEdited);
    }
}

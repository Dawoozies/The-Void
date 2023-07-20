using UnityEngine;
using UnityEngine.Windows;

public class SimpleComputeShaderExample : MonoBehaviour
{
    public ComputeShader startShader;
    public ComputeShader computeShader;
    public RenderTexture resultTexture;
    public RenderTexture resultTextureAlt;
    public Material blitMaterial;
    public Texture startTexture;
    int kernelIndex;
    bool alternateTexture;
    public int pixelWidth;
    public int pixelHeight;
    public Vector2 translationVector;
    public float maxTime = 1;
    float time; 
    private void Start()
    {
        int width = Screen.width;
        int height = Screen.height;

        // Create the Render Texture to store the result
        resultTexture = new RenderTexture(width, height, 24, RenderTextureFormat.ARGB32);
        resultTexture.enableRandomWrite = true;
        resultTexture.Create();

        resultTextureAlt = new RenderTexture(width, height, 24, RenderTextureFormat.ARGB32);
        resultTextureAlt.enableRandomWrite = true;
        resultTextureAlt.Create();

        //Graphics.Blit(startTexture, resultTexture);

        // Set the Compute Shader properties
        int startKernel = startShader.FindKernel("StartTextureSetup");
        startShader.SetTexture(startKernel, "StartTexture", startTexture);
        startShader.SetTexture(startKernel, "OutputTexture", resultTextureAlt);
        startShader.SetInt("ScreenWidth", width);
        startShader.SetInt("ScreenHeight", height);
        startShader.Dispatch(startKernel, Screen.width / 8, Screen.height / 8, 1);

        kernelIndex = computeShader.FindKernel("Life");
        //computeShader.SetTexture(kernelIndex, "resultTexture", resultTexture);
        computeShader.SetFloat("ScreenWidth", width);
        computeShader.SetFloat("ScreenHeight", height);
        computeShader.SetTexture(kernelIndex, "StartTexture", startTexture);
        computeShader.SetFloat("StartTextureWidth", startTexture.width);
        computeShader.SetFloat("StartTextureHeight", startTexture.height);
        computeShader.SetFloat("startTime", Time.time);
        // Create the blit material to display the result texture
        blitMaterial.SetTexture("_NewTexture", resultTexture);

        // Register the OnPostRender callback to draw the result texture after rendering
        alternateTexture = true;
    }
    //Obviously this will be on managed update
    private void Update()
    {
        /*int q = 100000000;
        while (q > 0)
        {
            float t = 12345678f / 98735462.0f;
            q--;
        }*/
        if (time > 0)
        {
            time -= Time.deltaTime;
        }
        if(time <= 0)
        {
            time = maxTime;
            Render();
        }

    }
    private void Render()
    {
        //computeShader.SetTexture(kernelIndex, "InputTexture", resultTexture);
        //computeShader.SetTexture(kernelIndex, "OutputTexture", resultTextureAlt);
        //Debug.Log($"currentTime = {Time.time}");
        //computeShader.SetFloat("currentTime", Time.time);
        //computeShader.SetInt("pixelWidth", pixelWidth);
        //computeShader.SetInt("pixelHeight", pixelHeight);
        //computeShader.SetFloat("translationVectorX", translationVector.x);
        //computeShader.SetFloat("translationVectorY", translationVector.y);
        //computeShader.Dispatch(kernelIndex, Screen.width / 8, Screen.height / 8, 1);
        //(resultTextureAlt, resultTexture) = (resultTexture, resultTextureAlt);
        computeShader.SetFloat("currentTime", Time.time);
        if(alternateTexture)
        {
            computeShader.SetTexture(kernelIndex, "InputTexture", resultTextureAlt);
            computeShader.SetTexture(kernelIndex, "OutputTexture", resultTexture);
            computeShader.Dispatch(kernelIndex, Screen.width / 8, Screen.height / 8, 1);
            alternateTexture = false;
        }
        else
        {
            computeShader.SetTexture(kernelIndex, "InputTexture", resultTexture);
            computeShader.SetTexture(kernelIndex, "OutputTexture", resultTextureAlt);
            computeShader.Dispatch(kernelIndex, Screen.width / 8, Screen.height / 8, 1);
            alternateTexture = true;
        }
    }
    private void OldUpdate()
    {
        if (alternateTexture)
        {
            computeShader.SetTexture(kernelIndex, "InputTexture", resultTexture);
            computeShader.SetTexture(kernelIndex, "OutputTexture", resultTextureAlt);
            computeShader.SetFloat("previousTime", Time.time);
            computeShader.Dispatch(kernelIndex, Screen.width / 8, Screen.height / 8, 1);

            alternateTexture = false;
        }
        else
        {
            computeShader.SetTexture(kernelIndex, "InputTexture", resultTextureAlt);
            computeShader.SetTexture(kernelIndex, "OutputTexture", resultTexture);
            computeShader.SetFloat("previousTime", Time.time);
            computeShader.Dispatch(kernelIndex, Screen.width / 8, Screen.height / 8, 1);

            alternateTexture = true;
        }
        computeShader.SetFloat("currentTime", Time.time);
    }
}

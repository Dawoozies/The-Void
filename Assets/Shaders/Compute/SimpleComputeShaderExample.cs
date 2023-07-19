using UnityEngine;
using UnityEngine.Windows;

public class SimpleComputeShaderExample : MonoBehaviour
{
    public ComputeShader computeShader;
    public RenderTexture resultTexture;
    public RenderTexture resultTextureAlt;
    public Material blitMaterial;
    public Texture startTexture;
    int kernelIndex;
    bool alternateTexture;
    public int pixelWidth;
    public int pixelHeight;
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

        Graphics.Blit(startTexture, resultTexture);

        // Set the Compute Shader properties
        kernelIndex = computeShader.FindKernel("DrawWhitePixel");
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
        computeShader.SetTexture(kernelIndex, "OutputTexture", resultTexture);
        Debug.Log($"currentTime = {Time.time}");
        computeShader.SetFloat("currentTime", Time.time);
        computeShader.SetInt("pixelWidth", pixelWidth);
        computeShader.SetInt("pixelHeight", pixelHeight);
        computeShader.Dispatch(kernelIndex, Screen.width / 8, Screen.height / 8, 1);
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

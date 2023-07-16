using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestGlitch : MonoBehaviour
{
    SpriteRenderer spriteRenderer;
    public float waveDisplacementFrequencyMax = 1f;
    public float waveDisplacementFrequencyMin = -1f;
    public float waveDisplacementFrequency = -0.1f;
    public float timeMax = 1f;
    float time;
    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        //Debug.LogError($"Material name = {spriteRenderer.material.name}");
    }
    private void Update()
    {
        if (time > 0)
        {
            time -= Time.deltaTime;
        }
        else
        {
            time = timeMax;
            waveDisplacementFrequency = Random.Range(waveDisplacementFrequencyMin, waveDisplacementFrequencyMax);
        }
        spriteRenderer.material.SetFloat("_WaveDisplacementFrequency", waveDisplacementFrequency);
    }
}

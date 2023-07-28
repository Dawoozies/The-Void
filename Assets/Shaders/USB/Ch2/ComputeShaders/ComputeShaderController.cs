using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComputeShaderController : MonoBehaviour
{
    public ComputeShader m_shader;
    public Texture m_tex;
    public RenderTexture m_mainTex;
    int m_texSize = 256;
    Renderer m_rend;

    [Range(0.0f, 0.5f)]
    public float m_radius = 0.5f;
    [Range(0.0f, 1.0f)]
    public float m_center = 0.5f;
    [Range(0.0f, 0.5f)]
    public float m_smooth = 0.01f;
    public Color m_mainColor = new Color();
    struct Circle
    {
        public float radius;
        public float center;
        public float smooth;
    }
    Circle[] m_circle;
    ComputeBuffer m_buffer;
    void Start()
    {
        CreateShaderTex();
        //m_shader.SetTexture(0, "Result", m_mainTex);
        //m_shader.SetTexture(0, "ColTex", m_tex);
        //m_rend.material.SetTexture("_BaseMap", m_mainTex);
        //m_shader.Dispatch(0, m_texSize / 8, m_texSize / 8, 1);
    }
    void CreateShaderTex()
    {
        m_mainTex = new RenderTexture(m_texSize, m_texSize, 0, RenderTextureFormat.ARGB32);
        m_mainTex.enableRandomWrite = true;
        m_mainTex.Create();

        m_rend = GetComponent<Renderer>();
        m_rend.enabled = true;
    }
    void SetShaderTex()
    {
        uint threadGroupSizeX;
        m_shader.GetKernelThreadGroupSizes(0, out threadGroupSizeX, out _, out _);
        int size = (int)threadGroupSizeX;
        m_circle = new Circle[size];

        for (int i = 0; i < size; i++)
        {
            Circle circle = m_circle[i];
            circle.radius = m_radius;
            circle.center = m_center;
            circle.smooth = m_smooth;
            m_circle[i] = circle;
        }

        int stride = 12; 
        //Stride is 12 because of how many bytes needed to store the struct Circle
        //1 dim float requires 4 bytes and Circle stores 3 different floats
        //so (1+1+1)*4
        m_buffer = new ComputeBuffer(size, stride, ComputeBufferType.Default);
        m_buffer.SetData(m_circle);
        m_shader.SetBuffer(0, "CircleBuffer", m_buffer);

        m_shader.SetTexture(0, "Result", m_mainTex);
        m_shader.SetVector("MainColor", m_mainColor);
        m_rend.material.SetTexture("_BaseMap", m_mainTex);

        m_shader.Dispatch(0, m_texSize, m_texSize, 1);
        m_buffer.Release();
    }
    void Update()
    {
        SetShaderTex();
    }
}

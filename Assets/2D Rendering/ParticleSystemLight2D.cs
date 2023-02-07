using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using System;
public class ParticleSystemLight2D : MonoBehaviour
{
    //Main Fields
    GameObject m_LightPrefab;
    ParticleSystem m_ParticleSystem;
    ParticleSystem.Particle[] m_Particles;

    //Light Pool Fields
    List<Light2D> lightPool = new List<Light2D>();

    //Light Multiplier Fields;
    public float intensityMultiplier = 1f;
    public float outerRadius = 30f;
    //Warning when working with ParticleSystem.Particle this object is not a reference type so storing it in a variable and passing it on will not work
    private void Awake()
    {
        m_ParticleSystem = GetComponent<ParticleSystem>();
        m_Particles = new ParticleSystem.Particle[m_ParticleSystem.main.maxParticles];
        m_LightPrefab = Resources.Load("Lights/ParticleLight_VoidAmbience") as GameObject;

        for (int i = 0; lightPool.Count < m_ParticleSystem.main.maxParticles; i++)
        {
            GameObject newPoolMember = Instantiate(m_LightPrefab, m_ParticleSystem.transform);
            newPoolMember.name = $"AmbientLightPoolMember{i}";
            Light2D newLightObject = newPoolMember.GetComponent<Light2D>();
            newLightObject.intensity = 0;
            newLightObject.color = Color.clear;

            lightPool.Add(newLightObject);
        }
    }

    void LateUpdate()
    {
        if (m_ParticleSystem.main.simulationSpace != ParticleSystemSimulationSpace.World)
            Debug.LogError("ParticleSystemLight2D Error: Particle System Simulation Space has not been set to world. Particle lights will not track properly.");

        int numOfParticlesAlive = m_ParticleSystem.GetParticles(m_Particles);

        if (numOfParticlesAlive <= 0 || lightPool.Count <= 0)
            return;

        for (int i = 0; i < numOfParticlesAlive; i++)
        {
            if (numOfParticlesAlive > lightPool.Count)
            {
                Debug.LogError("ParticleSystemLight2D Error: Light Pool is not big enough for the amount of particles currently present in the particle system");
                return;
            }

            lightPool[i].transform.position = m_Particles[i].position;
            lightPool[i].intensity = m_Particles[i].GetCurrentSize(m_ParticleSystem) * intensityMultiplier;
            lightPool[i].color = m_Particles[i].GetCurrentColor(m_ParticleSystem);
            lightPool[i].pointLightOuterRadius = outerRadius;
        }
    }

    private void OnDrawGizmos()
    {
        if (m_ParticleSystem == null)
            return;

        if (m_Particles == null)
            return;

        Gizmos.color = Color.magenta;

        int numOfParticlesAlive = m_ParticleSystem.GetParticles(m_Particles);
        if(numOfParticlesAlive > 0)
        {
            ParticleSystem.Particle debugParticle = m_Particles[0];
            Gizmos.DrawWireSphere(debugParticle.position, debugParticle.GetCurrentSize(m_ParticleSystem));
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExtensionMethods_Animator;
using LinearAlgebra;
public class FrameVelocityDataManager : MonoBehaviour
{
    // Start is called before the first frame update
    public string nameOfEntity;
    public bool showVelocityGizmos;
    Dictionary<string, FrameVelocityData> frameVelocityDataDictionary = new Dictionary<string, FrameVelocityData>();
    Animator animator;
    string clipName;
    string lastClipName;
    float currentVelocityMagnitude;
    Vector3 currentVelocity;
    Listener_FrameVelocityData[] listeners_FrameVelocityData;
    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        listeners_FrameVelocityData = GetComponentsInChildren<Listener_FrameVelocityData>();
        LoadData();
    }
    void LoadData()
    {
        FrameVelocityData[] allData = Resources.LoadAll<FrameVelocityData>("FrameVelocityData");
        if (nameOfEntity == null || nameOfEntity.Length <= 0)
        {
            Debug.LogError("FrameVelocityDataManager Error: Name of entity null or not set");
            return;
        }

        if (allData != null && allData.Length > 0)
        {
            foreach (FrameVelocityData data in allData)
            {
                if (data.clip.name.Contains(nameOfEntity))
                {
                    frameVelocityDataDictionary.Add(data.clip.name, data);
                }
            }
        }
    }
    void Update()
    {
        clipName = animator.GetCurrentAnimatorClipInfo(0)[0].clip.name;
        if (!frameVelocityDataDictionary.ContainsKey(clipName))
            return;
        int frame = animator.CurrentFrame();
        currentVelocity = frameVelocityDataDictionary[clipName].VelocityAtFrame(frame);
        currentVelocityMagnitude = frameVelocityDataDictionary[clipName].MagnitudeAtFrame(frame);
        HandleListeners_FrameVelocityData();
    }
    void HandleListeners_FrameVelocityData()
    {

    }
    private void OnDrawGizmos()
    {
        if (!showVelocityGizmos)
            return;
        ParametrisedLine line = new ParametrisedLine();
        line.pathStart = transform.position;
        line.pathEnd = transform.position + currentVelocity*currentVelocityMagnitude;
        Gizmos.color = Color.white;
        Gizmos.DrawLine(line.pathStart, line.pathEnd);
    }
}

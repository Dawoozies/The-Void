using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExtensionMethods_Animator;
using LinearAlgebra;
public class FrameVelocityDataManager : MonoBehaviour
{
    // Start is called before the first frame update
    public string nameOfEntity;
    Dictionary<string, FrameVelocityData> frameVelocityDataDictionary = new Dictionary<string, FrameVelocityData>();
    Animator animator;
    string clipName;
    int frame;
    FrameVelocityData currentData;
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
    void FixedUpdate()
    {
        if(clipName != animator.GetCurrentAnimatorClipInfo(0)[0].clip.name)
            NotifyListeners_FrameVelocityData();
        clipName = animator.GetCurrentAnimatorClipInfo(0)[0].clip.name;
        if (!frameVelocityDataDictionary.ContainsKey(clipName))
            return;
        frame = animator.CurrentFrame();
        currentData = frameVelocityDataDictionary[clipName];
        HandleListeners_FrameVelocityData();
    }
    void NotifyListeners_FrameVelocityData()
    {
        if (listeners_FrameVelocityData == null)
            return;
        for (int i = 0; i < listeners_FrameVelocityData.Length; i++)
        {
            listeners_FrameVelocityData[i].Notify_AnimationClipChanged();
        }
    }
    void HandleListeners_FrameVelocityData()
    {
        if (listeners_FrameVelocityData == null)
            return;
        List<VelocityComponent> velocityComponents = currentData.VelocityComponentsAtFrame(frame);
        if (velocityComponents == null || velocityComponents.Count <= 0)
            return;
        for (int i = 0; i < listeners_FrameVelocityData.Length; i++)
        {
            listeners_FrameVelocityData[i].Update_FrameVelocityData(frame, velocityComponents);
        }
    }
}

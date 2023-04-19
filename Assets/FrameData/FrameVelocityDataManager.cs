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
    Listener_FrameVelocityDataLStick[] listeners_FrameVelocityDataLStick;
    Listener_FrameVelocityDataRStick[] listeners_FrameVelocityDataRStick;
    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        listeners_FrameVelocityData = GetComponentsInChildren<Listener_FrameVelocityData>();
        listeners_FrameVelocityDataLStick = GetComponentsInChildren<Listener_FrameVelocityDataLStick>();
        listeners_FrameVelocityDataRStick = GetComponentsInChildren<Listener_FrameVelocityDataRStick>();
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
        clipName = animator.GetCurrentAnimatorClipInfo(0)[0].clip.name;
        if (!frameVelocityDataDictionary.ContainsKey(clipName))
            return;
        frame = animator.CurrentFrame();
        currentData = frameVelocityDataDictionary[clipName];
        HandleListeners_FrameVelocityData();
        HandleListeners_FrameVelocityDataLStick();
        HandleListeners_FrameVelocityDataRStick();
    }
    void HandleListeners_FrameVelocityData()
    {
        if (listeners_FrameVelocityData == null)
            return;
        Vector3 baseData = new Vector3(currentData.VelocityAtFrame(frame).x * animator.transform.localScale.x, currentData.VelocityAtFrame(frame).y, currentData.VelocityAtFrame(frame).z);
        for (int i = 0; i < listeners_FrameVelocityData.Length; i++)
        {
            listeners_FrameVelocityData[i].Update_FrameVelocityData(baseData, currentData.DragAtFrame(frame), currentData.velocityAdditive);
        }
    }
    void HandleListeners_FrameVelocityDataLStick()
    {
        if (listeners_FrameVelocityDataLStick == null)
            return;
        for (int i = 0; i < listeners_FrameVelocityDataLStick.Length; i++)
        {
            listeners_FrameVelocityDataLStick[i].Update_FrameVelocityDataLStick(currentData.LeftStickVelocityAtFrame(frame), currentData.leftStickVelocityAdditive);
        }
    }
    void HandleListeners_FrameVelocityDataRStick()
    {
        if (listeners_FrameVelocityDataRStick == null)
            return;
        for (int i = 0; i < listeners_FrameVelocityDataRStick.Length; i++)
        {
            listeners_FrameVelocityDataRStick[i].Update_FrameVelocityDataRStick(currentData.RightStickVelocityAtFrame(frame), currentData.rightStickVelocityAdditive);
        }
    }
}

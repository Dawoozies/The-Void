using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExtensionMethods_Animator;
public class SoundManager : MonoBehaviour
{
    public string nameOfEntity;
    Dictionary<string, FrameSoundData> frameSoundDataDictionary = new Dictionary<string, FrameSoundData>();
    Animator animator;
    AudioSource audioSource;
    string lastClipName;
    string clipName;
    int lastFrame;
    int frame;
    FrameSoundData frameSoundData;
    void Awake()
    {

        audioSource = GetComponentInChildren<AudioSource>();
        animator = GetComponentInChildren<Animator>();

        LoadData();
    }

    void LoadData()
    {
        FrameSoundData[] allData = Resources.LoadAll<FrameSoundData>("FrameSoundData");
        if(nameOfEntity == null || nameOfEntity.Length <= 0)
        {
            Debug.LogError("SoundManager Error: Name of entity null or not set");
            return;
        }

        if(allData != null && allData.Length > 0)
        {
            foreach (FrameSoundData data in allData)
            {
                if(data.clip.name.Contains(nameOfEntity))
                {
                    frameSoundDataDictionary.Add(data.clip.name, data);
                }
            }
        }
    }
    void Update()
    {
        clipName = animator.GetCurrentAnimatorClipInfo(0)[0].clip.name;
        if(lastClipName == null || lastClipName.Length == 0)
        {
            //Then this is the first clip data being loaded
            lastClipName = clipName;
        }
        if(lastClipName != null && lastClipName.Length > 0)
        {
            //Then last clip played was not null
            if(lastClipName != clipName)
            {
                //Then the current clip being played is different to the one we played last update
                lastFrame = -1;
            }
        }

        lastClipName = clipName;

        if(frameSoundDataDictionary.ContainsKey(clipName))
        {
            frame = animator.CurrentFrame();

            if (lastFrame == frame)
                return;

            //Debug.Log("How many times we get here?" + " Current Frame = " + frame + " Last Frame = " + lastFrame);
            frameSoundData = frameSoundDataDictionary[clipName];
            
            if (frameSoundData.dataList[frame].useCycleAudioClips)
            {
                //Then do cycle audio shit
                CycleAudio();
            }
            if (frameSoundData.dataList[frame].fixedAudioClips != null && frameSoundData.dataList[frame].fixedAudioClips.Count > 0)
            {
                //Then do fixed audio shit
                FixedAudio();
            }
            if (frameSoundData.dataList[frame].randomAudioClips != null && frameSoundData.dataList[frame].randomAudioClips.Count > 0)
            {
                //Then do random audio shit
                RandomAudio();
            }

            lastFrame = animator.CurrentFrame();
        }
    }

    void CycleAudio()
    {
        audioSource.PlayOneShot(frameSoundData.GetCycleAudioClip());
    }

    void FixedAudio()
    {
        //Just goes through the list and plays all the assigned clips
        for (int i = 0; i < frameSoundData.dataList[frame].fixedAudioClips.Count; i++)
        {
            audioSource.PlayOneShot(frameSoundData.dataList[frame].fixedAudioClips[i]);
        }
    }
    void RandomAudio()
    {
        audioSource.PlayOneShot(frameSoundData.GetRandomAudioClipAtFrame(frame));
    }
}

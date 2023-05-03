using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrameOverlapDataCache : MonoBehaviour
{
    public static FrameOverlapDataCache ins;
    Dictionary<AnimationClip, FrameOverlapData> frameOverlapDataDictionary = new Dictionary<AnimationClip, FrameOverlapData>();
    private void Awake()
    {
        ins = this;
        FrameOverlapData[] allData = Resources.LoadAll<FrameOverlapData>("FrameOverlapData");
        if (allData != null && allData.Length > 0)
        {
            foreach (FrameOverlapData data in allData)
            {
                if(!frameOverlapDataDictionary.ContainsKey(data.clip))
                {
                    frameOverlapDataDictionary.Add(data.clip, data);
                }
            }
        }
    }
    public FrameOverlapData GetOverlapData(AnimationClip clip)
    {
        if (!frameOverlapDataDictionary.ContainsKey(clip))
            return null;
        return frameOverlapDataDictionary[clip];
    }
}

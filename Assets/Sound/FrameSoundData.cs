using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class FrameSoundData : ScriptableObject
{
    public AnimationClip clip;
    public List<List<AudioClip>> dataList;

    public List<AudioClip> GetSoundData(int frame)
    {
        return dataList[frame];
    }
}

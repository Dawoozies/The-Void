using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrameSoundData : ScriptableObject
{
    public AnimationClip clip;
    public List<SoundList> dataList;
    int cycleIndex;
    public List<AudioClip> cycleAudioClips;
    int lastRandomIndex;
    public AudioClip GetCycleAudioClip()
    {
        if (cycleAudioClips == null)
            return null;
        if (cycleIndex >= cycleAudioClips.Count)
            cycleIndex = 0;

        AudioClip returnClip = cycleAudioClips[cycleIndex];
        cycleIndex++;
        return returnClip;
    }

    public AudioClip GetRandomAudioClipAtFrame(int frame)
    {
        if (dataList[frame].randomAudioClips == null)
            return null;
        if (dataList[frame].randomAudioClips.Count == 0)
            return null;
        if (dataList[frame].randomAudioClips.Count == 1)
            return dataList[frame].randomAudioClips[0];

        List<int> indexChoices = new List<int>();
        for (int i = 0; i < dataList[frame].randomAudioClips.Count - 1; i++)
        {
            if (i == lastRandomIndex)
                i++;

            indexChoices.Add(i);
        }

        int finalChoice = Random.Range(0, indexChoices.Count);
        lastRandomIndex = indexChoices[finalChoice];
        return dataList[frame].randomAudioClips[lastRandomIndex];
    }
}
[System.Serializable]
public class SoundList
{
    public int frame;
    public bool useCycleAudioClips;
    public List<AudioClip> fixedAudioClips;
    public List<AudioClip> randomAudioClips;

    public SoundList(int frame)
    {
        this.frame = frame;
    }
}
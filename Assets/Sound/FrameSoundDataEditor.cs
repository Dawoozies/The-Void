using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using AudioUtilities;
public class FrameSoundDataEditor : EditorWindow
{
    Transform selectedTransform;
    AnimationClip animationClip;
    AnimationClip newAnimationClip;
    int frame;
    float time;
    List<float> frameTimes;
    FrameSoundData currentSoundData;
    AudioClip soundToAdd;
    List<AudioClip> soundList;
    [MenuItem("Window/Frame Sound Data Editor")]
    static void Init()
    {
        FrameSoundDataEditor window = (FrameSoundDataEditor)EditorWindow.GetWindow(typeof(FrameSoundDataEditor));
        window.Show();
    }

    private void OnGUI()
    {
        GUILayout.Label("Frame Sound Data", EditorStyles.boldLabel);
        if (Selection.transforms.Length > 0)
        {
            selectedTransform = Selection.transforms[0];
            newAnimationClip = EditorGUILayout.ObjectField(newAnimationClip, typeof(AnimationClip), false) as AnimationClip;

            if (newAnimationClip == null)
                return;

            if (animationClip == null)
            {
                animationClip = newAnimationClip;
                GUILayout.Label("Select Animation Clip", EditorStyles.boldLabel);
                return;
            }

            if (newAnimationClip.name != animationClip.name)
            {
                //Then we have selected a new animation clip
                //Reset frame to 0
                frame = 0;
                animationClip = newAnimationClip;
            }

            AnimationMode.StartAnimationMode();

            EditorGUILayout.BeginVertical();

            frame = Mathf.FloorToInt(time * animationClip.frameRate);
            float totalFrames = (animationClip.length - 1) * animationClip.frameRate;
            GUILayout.Label("Current Frame = " + frame);
            GUILayout.Label("Total Frames = " + Mathf.RoundToInt(totalFrames));

            frameTimes = new List<float>();
            for (int i = 0; frameTimes.Count <= totalFrames; i++)
            {
                frameTimes.Add(i / animationClip.frameRate);
            }

            //FRAME SELECTION STUFF
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Previous Frame"))
            {
                frame--;
                if (frame < 0)
                    frame = 0;

                time = frameTimes[frame];
            }
            if (GUILayout.Button("Next Frame"))
            {
                if (frame < frameTimes.Count - 1)
                    frame++;

                time = frameTimes[frame];
            }
            GUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();

            string[] results = AssetDatabase.FindAssets(animationClip.name + "_SoundData");
            if (results.Length <= 0)
            {
                FrameSoundData frameSoundData = CreateInstance<FrameSoundData>();

                frameSoundData.clip = animationClip;

                if (frameSoundData.dataList == null)
                {
                    frameSoundData.dataList = new List<SoundList>();

                    for (int i = 0; i <= totalFrames; i++)
                    {
                        frameSoundData.dataList.Add(new SoundList(i));
                    }
                }

                if (!AssetDatabase.IsValidFolder("Assets/Resources/FrameSoundData"))
                {
                    AssetDatabase.CreateFolder("Assets/Resources", "FrameSoundData");
                }

                AssetDatabase.CreateAsset(frameSoundData, $"Assets/Resources/FrameSoundData/{animationClip.name}_SoundData.asset");
                AssetDatabase.SaveAssets();
            }

            if(results.Length > 0)
            {
                currentSoundData = AssetDatabase.LoadAssetAtPath<FrameSoundData>(AssetDatabase.GUIDToAssetPath(results[0]));
                soundToAdd = EditorGUILayout.ObjectField(soundToAdd, typeof(AudioClip), false) as AudioClip;

                if (GUILayout.Button("Add To Cycle"))
                {
                    if (currentSoundData.cycleAudioClips == null)
                        currentSoundData.cycleAudioClips = new List<AudioClip>();

                    currentSoundData.cycleAudioClips.Add(soundToAdd);
                    soundToAdd = null;

                    EditorUtility.SetDirty(currentSoundData);
                    Repaint();
                }
                if(GUILayout.Button("Add To Fixed"))
                {
                    if (currentSoundData.dataList[frame].fixedAudioClips == null)
                        currentSoundData.dataList[frame].fixedAudioClips = new List<AudioClip>();

                    currentSoundData.dataList[frame].fixedAudioClips.Add(soundToAdd);
                    soundToAdd = null;

                    EditorUtility.SetDirty(currentSoundData);
                    Repaint();
                }
                if(GUILayout.Button("Add To Random"))
                {
                    if (currentSoundData.dataList[frame].randomAudioClips == null)
                        currentSoundData.dataList[frame].randomAudioClips = new List<AudioClip>();

                    currentSoundData.dataList[frame].randomAudioClips.Add(soundToAdd);
                    soundToAdd = null;

                    EditorUtility.SetDirty(currentSoundData);
                    Repaint();
                }

                GUILayout.BeginVertical();
                if (currentSoundData.cycleAudioClips != null)
                {
                    if (currentSoundData.cycleAudioClips.Count > 0)
                    {
                        currentSoundData.dataList[frame].useCycleAudioClips = EditorGUILayout.Toggle($"Frame {frame} Use Cycle Clips", currentSoundData.dataList[frame].useCycleAudioClips);
                        GUILayout.Label("Cycle Audio Clips");
                        for (int i = 0; i < currentSoundData.cycleAudioClips.Count; i++)
                        {
                            GUILayout.BeginHorizontal();
                            currentSoundData.cycleAudioClips[i] = EditorGUILayout.ObjectField(currentSoundData.cycleAudioClips[i], typeof(AudioClip), false) as AudioClip;
                            if(GUILayout.Button("Remove"))
                            {
                                currentSoundData.cycleAudioClips.RemoveAt(i);

                                EditorUtility.SetDirty(currentSoundData);
                                Repaint();
                            }
                            GUILayout.EndHorizontal();
                        }
                    }
                }
                if (currentSoundData.dataList[frame].fixedAudioClips != null)
                {
                    if (currentSoundData.dataList[frame].fixedAudioClips.Count > 0)
                    {
                        GUILayout.Label("Fixed Audio Clips");
                        for (int i = 0; i < currentSoundData.dataList[frame].fixedAudioClips.Count; i++)
                        {
                            GUILayout.BeginHorizontal();
                            currentSoundData.dataList[frame].fixedAudioClips[i] = EditorGUILayout.ObjectField(currentSoundData.dataList[frame].fixedAudioClips[i], typeof(AudioClip), false) as AudioClip;
                            if(GUILayout.Button("Remove"))
                            {
                                currentSoundData.dataList[frame].fixedAudioClips.RemoveAt(i);

                                EditorUtility.SetDirty(currentSoundData);
                                Repaint();
                            }
                            GUILayout.EndHorizontal();
                        }
                    }
                }
                if (currentSoundData.dataList[frame].randomAudioClips != null)
                {
                    if (currentSoundData.dataList[frame].randomAudioClips.Count > 0)
                    {
                        GUILayout.Label("Random Audio Clips");
                        for (int i = 0; i < currentSoundData.dataList[frame].randomAudioClips.Count; i++)
                        {
                            GUILayout.BeginHorizontal();
                            currentSoundData.dataList[frame].randomAudioClips[i] = EditorGUILayout.ObjectField(currentSoundData.dataList[frame].randomAudioClips[i], typeof(AudioClip), false) as AudioClip;
                            if(GUILayout.Button("Remove"))
                            {
                                currentSoundData.dataList[frame].randomAudioClips.RemoveAt(i);

                                EditorUtility.SetDirty(currentSoundData);
                                Repaint();
                            }
                            GUILayout.EndHorizontal();
                        }
                    }
                }
                GUILayout.EndVertical();
            }
        }
    }
    private void OnDestroy()
    {
        if (AnimationMode.InAnimationMode())
            AnimationMode.StopAnimationMode();
    }
    private void Update()
    {
        if (selectedTransform == null)
            return;
        if (animationClip == null)
            return;

        if(!EditorApplication.isPlaying && AnimationMode.InAnimationMode())
        {
            AnimationMode.BeginSampling();
            AnimationMode.SampleAnimationClip(selectedTransform.gameObject, animationClip, time);
            AnimationMode.EndSampling();
            SceneView.RepaintAll();
        }
    }
}

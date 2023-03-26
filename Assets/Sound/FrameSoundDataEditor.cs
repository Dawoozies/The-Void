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
    [MenuItem("Window/Frame Sound Data Editor")]
    static void Init()
    {
        FrameSoundDataEditor window = (FrameSoundDataEditor)EditorWindow.GetWindow(typeof(FrameSoundDataEditor));
        window.Show();
    }

    private void OnGUI()
    {
        GUILayout.Label("Frame Sound Data", EditorStyles.boldLabel);
        if(Selection.transforms.Length > 0)
        {
            selectedTransform = Selection.transforms[0];
            newAnimationClip = EditorGUILayout.ObjectField(newAnimationClip, typeof(AnimationClip), false) as AnimationClip;

            if(newAnimationClip == null)
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

            frameTimes = new List<float>();
            for (int i = 0; frameTimes.Count <= totalFrames; i++)
            {
                frameTimes.Add(i / animationClip.frameRate);
            }

            //FRAME SELECTION STUFF
            GUILayout.BeginHorizontal();
            if(GUILayout.Button("Previous Frame"))
            {
                frame--;
                if (frame < 0)
                    frame = 0;

                time = frameTimes[frame];
            }
            if(GUILayout.Button("Next Frame"))
            {
                if (frame < frameTimes.Count - 1)
                    frame++;

                if (frame >= frameTimes.Count)
                    frame = frameTimes.Count - 1;

                time = frameTimes[frame];
            }
            GUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();

            string[] results = AssetDatabase.FindAssets(animationClip.name + "_SoundData");
            if(results.Length <= 0)
            {
                FrameSoundData frameSoundData = CreateInstance<FrameSoundData>();
                frameSoundData.clip = animationClip;
                frameSoundData.dataList = new List<List<AudioClip>>();

                for (int i = 0; frameSoundData.dataList.Count < frameTimes.Count; i++)
                {
                    frameSoundData.dataList.Add(new List<AudioClip>());
                }

                if(!AssetDatabase.IsValidFolder("Assets/Resources/FrameSoundData"))
                {
                    AssetDatabase.CreateFolder("Assets/Resources", "FrameSoundData");
                }

                AssetDatabase.CreateAsset(frameSoundData, $"Assets/Resources/FrameSoundData/{animationClip.name}_SoundData.asset");
                AssetDatabase.SaveAssets();

                return;
            }

            currentSoundData = AssetDatabase.LoadAssetAtPath<FrameSoundData>(AssetDatabase.GUIDToAssetPath(results[0]));

            if (currentSoundData.dataList == null)
                currentSoundData.dataList = new List<List<AudioClip>>();

            while(currentSoundData.dataList.Count < totalFrames)
            {
                currentSoundData.dataList.Add(new List<AudioClip>());
            }


            GUILayout.BeginHorizontal();
            soundToAdd = EditorGUILayout.ObjectField(soundToAdd, typeof(AudioClip), false) as AudioClip;
            if(GUILayout.Button("Add"))
            {
                currentSoundData.dataList[frame].Add(soundToAdd);
                soundToAdd = null;

                EditorUtility.SetDirty(currentSoundData);
                Repaint();
            }
            GUILayout.EndHorizontal();

            //We should add a button which will let us play all sound assigned to the current frame

            if (currentSoundData == null)
                return;
            if (currentSoundData.dataList == null)
                return;

            GUILayout.BeginVertical();
            for (int i = 0; i < currentSoundData.dataList[frame].Count; i++)
            {
                GUILayout.BeginHorizontal();
                currentSoundData.dataList[frame][i] = EditorGUILayout.ObjectField(currentSoundData.dataList[frame][i], typeof(AudioClip), false) as AudioClip;
                if (GUILayout.Button("Play"))
                    AudioPreview.Play(currentSoundData.dataList[frame][i]);
                if (GUILayout.Button("Remove"))
                {
                    currentSoundData.dataList[frame].RemoveAt(i);
                    EditorUtility.SetDirty(currentSoundData);
                    Repaint();
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndVertical();
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
public class FrameOverlapDataEditor : EditorWindow
{
    AnimationClip inputClip;
    AnimationClip clip;
    int frame;
    float time;
    (int, int) selectedComponentIndices;
    FrameOverlapData overlapData;
    Vector2 scrollPosition;
    [MenuItem("Window/Frame Overlap Data Editor")]
    static void Init()
    {
        FrameOverlapDataEditor window = (FrameOverlapDataEditor)GetWindow(typeof(FrameOverlapDataEditor));
        window.Show();
    }
    private void OnGUI()
    {
        GUILayout.Label("Frame Overlap Data", EditorStyles.boldLabel);
        if (Selection.transforms.Length == 0)
            return;

        inputClip = EditorGUILayout.ObjectField(inputClip, typeof(AnimationClip), false) as AnimationClip;
        if (inputClip == null)
            return;
        if(clip == null || inputClip.name != clip.name)
        {
            frame = 0;
            time = 0f;
            selectedComponentIndices = (-1, -1);
            clip = inputClip;
        }
        AnimationMode.StartAnimationMode();
        EditorGUILayout.BeginVertical();
        frame = Mathf.FloorToInt(time * clip.frameRate);
        int totalFrames = Mathf.RoundToInt(clip.length);
        GUILayout.Label("Current Frame = " + frame);
        GUILayout.Label("Total Frames = " + totalFrames);
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Previous Frame"))
        {
            frame--;
            if (frame < 0)
                frame = 0;

            time = frame;
        }
        if (GUILayout.Button("Next Frame"))
        {
            if (frame < Mathf.RoundToInt(clip.length - 1))
                frame++;

            time = frame;
        }
        GUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();

        string[] results = AssetDatabase.FindAssets(clip.name + "_OverlapData");
        if(results.Length <= 0)
        {
            FrameOverlapData frameOverlapData = CreateInstance<FrameOverlapData>();
            frameOverlapData.clip = clip;
            frameOverlapData.InitializeFrameData(totalFrames);
            if (!AssetDatabase.IsValidFolder("Assets/Resources/FrameOverlapData"))
                AssetDatabase.CreateFolder("Assets/Resources", "FrameOverlapData");
            AssetDatabase.CreateAsset(frameOverlapData, $"Assets/Resources/FrameOverlapData/{clip.name}_OverlapData.asset");
            AssetDatabase.SaveAssets();
        }
        if(results.Length > 0)
        {
            overlapData = AssetDatabase.LoadAssetAtPath<FrameOverlapData>(AssetDatabase.GUIDToAssetPath(results[0]));
            if (overlapData.overlapDataList == null || overlapData.overlapDataList.Count <= 0)
                overlapData.InitializeFrameData(totalFrames);
            if(GUILayout.Button("Add New Overlap Component"))
            {
                overlapData.OverlapComponentsAtFrame(frame).Add(new OverlapComponent());
                EditorUtility.SetDirty(overlapData);
                Repaint();
                return;
            }
            List<OverlapComponent> overlapComponentsAtFrame = overlapData.OverlapComponentsAtFrame(frame);
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            for (int overlapComponentIndex = 0; overlapComponentIndex < overlapComponentsAtFrame.Count; overlapComponentIndex++)
            {
                int i = overlapComponentIndex;
                GUILayout.BeginHorizontal();
                if (GUILayout.Button($"Frame {frame} Overlap Component {i}"))
                {
                    if(selectedComponentIndices.Item1 != i)
                    {
                        selectedComponentIndices.Item1 = i;
                    }
                    else
                    {
                        selectedComponentIndices.Item1 = -1;
                    }
                }
                if (GUILayout.Button($"Remove Component {i}"))
                {
                    overlapData.OverlapComponentsAtFrame(frame).RemoveAt(i);
                    EditorUtility.SetDirty(overlapData);
                    Repaint();
                }
                GUILayout.EndHorizontal();
            }
            EditorGUILayout.EndScrollView();
        }
    }
    private void OnDestroy()
    {
        if (AnimationMode.InAnimationMode())
            AnimationMode.StopAnimationMode();
    }
    private void Update()
    {
        if (Selection.transforms == null || Selection.transforms.Length == 0)
            return;
        if (clip == null)
            return;
        if(!EditorApplication.isPlaying && AnimationMode.InAnimationMode())
        {
            AnimationMode.BeginSampling();
            AnimationMode.SampleAnimationClip(Selection.transforms[0].gameObject, clip, time);
            AnimationMode.EndSampling();
            SceneView.RepaintAll();
        }
    }
}

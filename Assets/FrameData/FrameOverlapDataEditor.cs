using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using Geometry;
public class FrameOverlapDataEditor : EditorWindow
{
    AnimationClip inputClip;
    AnimationClip clip;
    int frame;
    float time;
    (int, int, int) selectedComponentIndices;
    FrameOverlapData overlapData;
    Vector2 scrollPositionOverlapComponents;
    Vector2 scrollPositionCircles;
    Vector2 scrollPositionParameterOptions;
    string inputOverlapComponentName;
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
            selectedComponentIndices = (-1, -1, -1);
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
            GUILayout.Label($"Overlap Data At Frame {frame}", EditorStyles.boldLabel);
            if(selectedComponentIndices.Item1 >= 0)
            {
                GUILayout.BeginHorizontal();
                inputOverlapComponentName = EditorGUILayout.TextField(inputOverlapComponentName);
                OverlapComponent overlapComponent = overlapData.OverlapComponentsAtFrame(frame)[selectedComponentIndices.Item1];
                if (GUILayout.Button($"Set Name Of OverlapData {selectedComponentIndices.Item1}"))
                {
                    overlapComponent.componentName = inputOverlapComponentName;
                    inputOverlapComponentName = "";
                    EditorUtility.SetDirty(overlapData);
                }
                GUILayout.EndHorizontal();
                overlapComponent.targetLayerMask = EditorGUILayout.LayerField("Target Layer Mask", overlapComponent.targetLayerMask);
                EditorUtility.SetDirty(overlapData);
                Repaint();
            }
            scrollPositionOverlapComponents = EditorGUILayout.BeginScrollView(scrollPositionOverlapComponents);
            for (int overlapComponentIndex = 0; overlapComponentIndex < overlapComponentsAtFrame.Count; overlapComponentIndex++)
            {
                int _i = overlapComponentIndex;
                GUILayout.BeginHorizontal();
                if (overlapData.OverlapComponentsAtFrame(frame)[_i].componentName.Length > 0)
                {
                    if(GUILayout.Button($"Frame {frame} {overlapData.OverlapComponentsAtFrame(frame)[_i].componentName}"))
                    {
                        if(selectedComponentIndices.Item1 != _i)
                        {
                            selectedComponentIndices = (_i, -1, -1);
                        }
                        else
                        {
                            selectedComponentIndices = (-1, -1, -1);
                        }
                    }
                }
                else
                {
                    if (GUILayout.Button($"Frame {frame} Overlap Component {_i}"))
                    {
                        if (selectedComponentIndices.Item1 != _i)
                        {
                            selectedComponentIndices = (_i, -1, -1);
                        }
                        else
                        {
                            selectedComponentIndices = (-1, -1, -1);
                        }
                    }
                }
                if (GUILayout.Button($"Remove Component {_i}"))
                {
                    overlapData.OverlapComponentsAtFrame(frame).RemoveAt(_i);
                    EditorUtility.SetDirty(overlapData);
                    EditorGUILayout.EndScrollView();
                    GUILayout.EndHorizontal();
                    Repaint();
                    selectedComponentIndices = (-1, -1, -1);
                }
                GUILayout.EndHorizontal();
            }
            EditorGUILayout.EndScrollView();
            if (overlapData.OverlapComponentsAtFrame(frame).Count != overlapComponentsAtFrame.Count)
                return;
            if (overlapComponentsAtFrame == null || overlapComponentsAtFrame.Count == 0)
                return;
            if (selectedComponentIndices.Item1 >= overlapComponentsAtFrame.Count)
                selectedComponentIndices = (-1, -1, -1);
            if (selectedComponentIndices.Item1 < 0)
                return;
            int i = selectedComponentIndices.Item1;
            List<Geometry.Circle> circlesAtIndex = overlapComponentsAtFrame[i].circles;
            if (GUILayout.Button($"Add New Circle To Overlap Component {i}"))
            {
                circlesAtIndex.Add(new Geometry.Circle());
                EditorUtility.SetDirty(overlapData);
                Repaint();
            }
            GUILayout.Label("Circle List", EditorStyles.boldLabel);
            scrollPositionCircles = EditorGUILayout.BeginScrollView(scrollPositionCircles);
            for (int circleIndex = 0; circleIndex < circlesAtIndex.Count; circleIndex++)
            {
                int _j = circleIndex;
                GUILayout.BeginHorizontal();
                if (GUILayout.Button($"Circle {_j}"))
                {
                    if (selectedComponentIndices.Item2 != _j)
                    {
                        selectedComponentIndices.Item2 = _j;
                    }
                    else
                    {
                        selectedComponentIndices.Item2 = -1;
                    }
                }
                if (GUILayout.Button($"Remove Circle {_j}"))
                {
                    overlapData.OverlapComponentsAtFrame(frame)[i].circles.RemoveAt(_j);
                    EditorUtility.SetDirty(overlapData);
                    EditorGUILayout.EndScrollView();
                    GUILayout.EndHorizontal();
                    Repaint();
                    selectedComponentIndices.Item2 = -1;
                }
                GUILayout.EndHorizontal();
            }
            if (selectedComponentIndices.Item2 >= 0)
            {
                //Display Circle Properties
                int j = selectedComponentIndices.Item2;
                Geometry.Circle circle = overlapData.OverlapComponentsAtFrame(frame)[i].circles[j];
                GUILayout.Label($"  Circle {j}", EditorStyles.miniBoldLabel);
                circle.center = EditorGUILayout.Vector3Field("Center", circle.center);
                circle.radius = EditorGUILayout.FloatField("Radius", circle.radius);
                EditorUtility.SetDirty(overlapData);
                Repaint();
            }
            EditorGUILayout.EndScrollView();

            List<ParameterComponent> parameterComponentsAtIndex = overlapComponentsAtFrame[i].parameterComponents;
            if(GUILayout.Button($"Add New Parameter Component To Overlap Component {i}"))
            {
                parameterComponentsAtIndex.Add(new ParameterComponent());
                EditorUtility.SetDirty(overlapData);
                Repaint();
            }
            GUILayout.Label("Parameter Component List: ", EditorStyles.boldLabel);
            scrollPositionParameterOptions = EditorGUILayout.BeginScrollView(scrollPositionParameterOptions);
            for (int parameterComponentIndex = 0; parameterComponentIndex < parameterComponentsAtIndex.Count; parameterComponentIndex++)
            {
                int _k = parameterComponentIndex;
                GUILayout.BeginHorizontal();
                if(GUILayout.Button($"Parameter Component {_k}"))
                {
                    if(selectedComponentIndices.Item3 != _k)
                    {
                        selectedComponentIndices.Item3 = _k;
                    }
                    else
                    {
                        selectedComponentIndices.Item3 = -1;
                    }
                }
                if (GUILayout.Button($"Remove Parameter Component {_k}"))
                {
                    overlapData.OverlapComponentsAtFrame(frame)[i].parameterComponents.RemoveAt(_k);
                    EditorUtility.SetDirty(overlapData);
                    EditorGUILayout.EndScrollView();
                    GUILayout.EndHorizontal();
                    Repaint();
                    selectedComponentIndices.Item3 = -1;
                }
                GUILayout.EndHorizontal();  
            }
            if (selectedComponentIndices.Item3 >= 0)
            {
                int k = selectedComponentIndices.Item3;
                ParameterComponent parameterComponent = overlapData.OverlapComponentsAtFrame(frame)[i].parameterComponents[k];
                GUILayout.Label($"  Parameter Component {k}", EditorStyles.miniBoldLabel);
                parameterComponent.parameterName = EditorGUILayout.TextField("Parameter Name", parameterComponent.parameterName);
                parameterComponent.parameterType = (AnimatorControllerParameterType)EditorGUILayout.EnumPopup("Parameter Type", parameterComponent.parameterType);
                parameterComponent.floatValue = EditorGUILayout.FloatField("Float Value", parameterComponent.floatValue);
                parameterComponent.integerValue = EditorGUILayout.IntField("Integer Value", parameterComponent.integerValue);
                parameterComponent.boolValue = EditorGUILayout.Toggle("Bool Value", parameterComponent.boolValue);
                parameterComponent.componentOptions = (ParameterComponentOptions)EditorGUILayout.EnumFlagsField("Component Options", parameterComponent.componentOptions);
                EditorUtility.SetDirty(overlapData);
                Repaint();
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

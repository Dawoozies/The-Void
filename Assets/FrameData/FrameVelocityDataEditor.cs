using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public class FrameVelocityDataEditor : EditorWindow
{
    Transform selectedTransform;
    AnimationClip newAnimationClip;
    AnimationClip animationClip;
    int frame;
    float time;
    FrameVelocityData currentFrameData;
    [MenuItem("Window/Frame Velocity Data Editor")]
    static void Init()
    {
        FrameVelocityDataEditor window = (FrameVelocityDataEditor)EditorWindow.GetWindow(typeof(FrameVelocityDataEditor));
        window.Show();
    }
    private void OnGUI()
    {
        GUILayout.Label("Frame Velocity Data", EditorStyles.boldLabel);
        if(Selection.transforms.Length > 0)
        {
            selectedTransform = Selection.transforms[0];
            newAnimationClip = EditorGUILayout.ObjectField(newAnimationClip, typeof(AnimationClip), false) as AnimationClip;
            if (newAnimationClip == null)
                return;
            if(animationClip == null)
            {
                animationClip = newAnimationClip;
                GUILayout.Label("Select Animation Clip", EditorStyles.boldLabel);
                return;
            }
            if(newAnimationClip.name != animationClip.name)
            {
                frame = 0;
                animationClip = newAnimationClip;
            }
            AnimationMode.StartAnimationMode();
            EditorGUILayout.BeginVertical();
            frame = Mathf.FloorToInt(time * animationClip.frameRate);
            float totalFrames = Mathf.RoundToInt(animationClip.length);
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
                if (frame < Mathf.RoundToInt(animationClip.length - 1))
                    frame++;

                time = frame;
            }
            GUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();

            string[] results = AssetDatabase.FindAssets(animationClip.name + "_VelocityData");
            if(results.Length <= 0)
            {
                FrameVelocityData frameVelocityData = CreateInstance<FrameVelocityData>();
                frameVelocityData.clip = animationClip;
                if(frameVelocityData.dataList == null)
                {
                    frameVelocityData.dataList = new List<Vector3>();
                    frameVelocityData.dataListSecondary = new List<float>();
                    for (int i = 0; i < totalFrames; i++)
                    {
                        frameVelocityData.dataList.Add(Vector3.zero);
                        frameVelocityData.dataListSecondary.Add(0f);
                    }
                }
                if (!AssetDatabase.IsValidFolder("Assets/Resources/FrameVelocityData"))
                    AssetDatabase.CreateFolder("Assets/Resources", "FrameVelocityData");
                AssetDatabase.CreateAsset(frameVelocityData, $"Assets/Resources/FrameVelocityData/{animationClip.name}_VelocityData.asset");
                AssetDatabase.SaveAssets();
            }
            if (results.Length > 0)
            {
                currentFrameData = AssetDatabase.LoadAssetAtPath<FrameVelocityData>(AssetDatabase.GUIDToAssetPath(results[0]));
                GUILayout.Label("NOTE: Z component is magnitude factor");
                currentFrameData.dataListSecondary[frame] = EditorGUILayout.FloatField("Drag/Secondary Data", currentFrameData.dataListSecondary[frame]);
                currentFrameData.dataList[frame] = EditorGUILayout.Vector3Field("Velocity", currentFrameData.dataList[frame]);
                if(GUILayout.Button("Duplicate Previous Frame"))
                {
                    if(frame > 0)
                    {
                        currentFrameData.dataList[frame] = currentFrameData.dataList[frame - 1];
                        currentFrameData.dataListSecondary[frame] = currentFrameData.dataListSecondary[frame - 1];
                    }
                }
                if (GUILayout.Button("Normalize"))
                {
                    Vector3 velocity = currentFrameData.VelocityAtFrame(frame).normalized;
                    currentFrameData.dataList[frame] = new Vector3(velocity.x, velocity.y, currentFrameData.MagnitudeAtFrame(frame));
                }
                if (GUILayout.Button("Set Zero Velocity"))
                    currentFrameData.dataList[frame] = Vector3.zero;
                EditorUtility.SetDirty(currentFrameData);
                Repaint();
            }
        }
    }
    private void OnFocus()
    {
        SceneView.duringSceneGui -= this.OnSceneGUI;
        SceneView.duringSceneGui += this.OnSceneGUI;
    }
    private void OnDestroy()
    {
        SceneView.duringSceneGui -= this.OnSceneGUI;
        if (AnimationMode.InAnimationMode())
            AnimationMode.StopAnimationMode();
    }
    private void Update()
    {
        if (selectedTransform == null)
            return;
        if (animationClip == null)
            return;
        if (!EditorApplication.isPlaying && AnimationMode.InAnimationMode())
        {
            AnimationMode.BeginSampling();
            AnimationMode.SampleAnimationClip(selectedTransform.gameObject, animationClip, time);
            AnimationMode.EndSampling();
            SceneView.RepaintAll();
        }
    }
    void OnSceneGUI(SceneView sceneView)
    {
        if (currentFrameData == null)
            return;
        EditorGUI.BeginChangeCheck();
        Vector3 oldVector = currentFrameData.VelocityAtFrame(frame);
        Vector3 newVector = Handles.FreeMoveHandle(oldVector, Quaternion.identity, 0.25f, Vector3.one * 0.1f, Handles.RectangleHandleCap);
        if(EditorGUI.EndChangeCheck())
        {
            currentFrameData.dataList[frame] = new Vector3(newVector.x, newVector.y, currentFrameData.MagnitudeAtFrame(frame));
            EditorUtility.SetDirty(currentFrameData);
            Repaint();
        }

        Handles.DrawDottedLine(Vector3.zero, currentFrameData.VelocityAtFrame(frame), 2f);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public class FrameVelocityDataEditor : EditorWindow
{
    Transform selectedTransform;
    AnimationClip newAnimationClip;
    AnimationClip animationClip;
    FrameVelocityData copiedVelocityData;
    int frame;
    float time;
    FrameVelocityData currentFrameData;
    Vector2 scrollPosition;
    int selectedComponentIndex;
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
                time = 0f;
                selectedComponentIndex = -1;
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
                frameVelocityData.InitializeFrameData(Mathf.RoundToInt(totalFrames));
                if (!AssetDatabase.IsValidFolder("Assets/Resources/FrameVelocityData"))
                    AssetDatabase.CreateFolder("Assets/Resources", "FrameVelocityData");
                AssetDatabase.CreateAsset(frameVelocityData, $"Assets/Resources/FrameVelocityData/{animationClip.name}_VelocityData.asset");
                AssetDatabase.SaveAssets();
            }
            if (results.Length > 0)
            {
                currentFrameData = AssetDatabase.LoadAssetAtPath<FrameVelocityData>(AssetDatabase.GUIDToAssetPath(results[0]));
                if(currentFrameData.velocityDataList == null || currentFrameData.velocityDataList.Count <= 0)
                    currentFrameData.InitializeFrameData(Mathf.RoundToInt(totalFrames));
                if (GUILayout.Button($"Copy Data From {currentFrameData.clip.name} To Clipboard"))
                {
                    copiedVelocityData = currentFrameData;
                }
                if(copiedVelocityData != null && copiedVelocityData.clip.name.Length > 0)
                {
                    if(GUILayout.Button($"Paste Data From {copiedVelocityData.clip.name} To {currentFrameData.clip.name}"))
                    {
                        currentFrameData.PasteFrameVelocityData(copiedVelocityData);
                        EditorUtility.SetDirty(currentFrameData);
                        Repaint();
                        return;
                    }
                }
                if(GUILayout.Button("Add New Velocity Component"))
                {
                    currentFrameData.VelocityComponentsAtFrame(frame).Add(new VelocityComponent());
                    EditorUtility.SetDirty(currentFrameData);
                    Repaint();
                    return;
                }
                List<VelocityComponent> velocityComponentsAtFrame = currentFrameData.VelocityComponentsAtFrame(frame);
                if (velocityComponentsAtFrame == null || velocityComponentsAtFrame.Count == 0)
                    return;
                scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
                for (int i = 0; i < velocityComponentsAtFrame.Count; i++)
                {
                    GUILayout.BeginHorizontal();
                    if(GUILayout.Button($"Frame {frame} Velocity Component {i}"))
                    {
                        if (selectedComponentIndex != i)
                        {
                            selectedComponentIndex = i;
                        }
                        else
                        {
                            selectedComponentIndex = -1;
                        }
                    }
                    if(GUILayout.Button($"Remove Component {i}"))
                    {
                        currentFrameData.VelocityComponentsAtFrame(frame).RemoveAt(i);
                        EditorUtility.SetDirty(currentFrameData);
                        Repaint();
                    }
                    GUILayout.EndHorizontal();
                }
                EditorGUILayout.EndScrollView();
                if (currentFrameData.VelocityComponentsAtFrame(frame).Count != velocityComponentsAtFrame.Count)
                    return;
                if (velocityComponentsAtFrame == null || velocityComponentsAtFrame.Count <= 0)
                    return;
                if (selectedComponentIndex >= velocityComponentsAtFrame.Count)
                    selectedComponentIndex = -1;
                if (selectedComponentIndex >= 0)
                {
                    int i = selectedComponentIndex;
                    velocityComponentsAtFrame[i].velocityBase = EditorGUILayout.Vector3Field("Velocity Base", velocityComponentsAtFrame[i].velocityBase);
                    velocityComponentsAtFrame[i].maxSpeed = EditorGUILayout.FloatField("Max Speed", velocityComponentsAtFrame[i].maxSpeed);
                    velocityComponentsAtFrame[i].multiplier = EditorGUILayout.FloatField("Multiplier", velocityComponentsAtFrame[i].multiplier);
                    velocityComponentsAtFrame[i].isImpulse = EditorGUILayout.Toggle("Is Impulse", velocityComponentsAtFrame[i].isImpulse);
                    velocityComponentsAtFrame[i].isGravitational = EditorGUILayout.Toggle("Is Gravitational", velocityComponentsAtFrame[i].isGravitational);
                    velocityComponentsAtFrame[i].isConstant = EditorGUILayout.Toggle("Is Constant", velocityComponentsAtFrame[i].isConstant);
                    velocityComponentsAtFrame[i].useLocalSpace = EditorGUILayout.Toggle("Use Local Space", velocityComponentsAtFrame[i].useLocalSpace);
                    velocityComponentsAtFrame[i].useLStick = EditorGUILayout.Toggle("Use L Stick", velocityComponentsAtFrame[i].useLStick);
                    velocityComponentsAtFrame[i].useRStick = EditorGUILayout.Toggle("Use R Stick", velocityComponentsAtFrame[i].useRStick);
                    velocityComponentsAtFrame[i].useGravity = EditorGUILayout.Toggle("Use Gravity", velocityComponentsAtFrame[i].useGravity);
                    velocityComponentsAtFrame[i].useTransformUp = EditorGUILayout.Toggle("Use Transform Up", velocityComponentsAtFrame[i].useTransformUp);
                    velocityComponentsAtFrame[i].useTransformRight = EditorGUILayout.Toggle("Use Transform Right", velocityComponentsAtFrame[i].useTransformRight);
                    velocityComponentsAtFrame[i].useTransformForward = EditorGUILayout.Toggle("Use Transform Forward", velocityComponentsAtFrame[i].useTransformForward);
                    velocityComponentsAtFrame[i].useVelocityDirection = EditorGUILayout.Toggle("Use Velocity Direction", velocityComponentsAtFrame[i].useVelocityDirection);
                    velocityComponentsAtFrame[i].useVelocity = EditorGUILayout.Toggle("Use Velocity", velocityComponentsAtFrame[i].useVelocity);
                    if(GUILayout.Button("Add Parameter Multiplier"))
                    {
                        velocityComponentsAtFrame[i].parameterMultipliers.Add("ParameterName");
                    }
                    if (velocityComponentsAtFrame[i].parameterMultipliers.Count > 0)
                    {
                        for(int j = 0; j < velocityComponentsAtFrame[i].parameterMultipliers.Count; j++)
                        {
                            GUILayout.BeginHorizontal();
                            velocityComponentsAtFrame[i].parameterMultipliers[j] = EditorGUILayout.TextField($"Parameter Multiplier {j} = ", velocityComponentsAtFrame[i].parameterMultipliers[j]);
                            if (GUILayout.Button("Remove"))
                            {
                                velocityComponentsAtFrame[i].parameterMultipliers.RemoveAt(j);
                                GUILayout.EndHorizontal();
                                break;
                            }
                            GUILayout.EndHorizontal();
                        }
                    }
                }
                if(GUILayout.Button($"Override All Frames With Frame {frame}"))
                {
                    currentFrameData.CopyFromFrameToAll(frame);
                }
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
        if (currentFrameData.VelocityComponentsAtFrame(frame) == null || currentFrameData.VelocityComponentsAtFrame(frame).Count <= 0)
            return;
        if (selectedComponentIndex >= currentFrameData.VelocityComponentsAtFrame(frame).Count)
            return;
        if (selectedComponentIndex == -1)
            return;
        
        EditorGUI.BeginChangeCheck();
        Vector3 oldVector = currentFrameData.VelocityComponentsAtFrame(frame)[selectedComponentIndex].velocityBase;
        Handles.color = new Color(1f, 0f, 0.216f, 1f);
        Vector3 newVector = Handles.FreeMoveHandle(oldVector, Quaternion.identity, 0.75f, Vector3.one * 0.1f, Handles.RectangleHandleCap);
        if(EditorGUI.EndChangeCheck())
        {
            currentFrameData.VelocityComponentsAtFrame(frame)[selectedComponentIndex].velocityBase = new Vector3(newVector.x, newVector.y, newVector.z);
            EditorUtility.SetDirty(currentFrameData);
            Repaint();
        }
        Handles.color = new Color(1f, 0f, 0.216f, 1f);
        Handles.DrawDottedLine(selectedTransform.position, selectedTransform.position + currentFrameData.VelocityComponentsAtFrame(frame)[selectedComponentIndex].velocityBase, 3f);
    }
}

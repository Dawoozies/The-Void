#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEditor.Animations;
using System;
using Unity.VisualScripting;

public class ControllerDataEditor : EditorWindow
{
    List<AnimatorController> allControllers;
    ControllerData[] allControllerData;
    Animator animator;
    AnimatorController controllerSelected;
    AnimatorControllerLayer layerSelected;
    AnimatorState stateSelected;
    AnimationClip clipSelected;
    int frame;
    Vector2 scrollPosition;

    ControllerData controllerDataSelected;
    DirectedCircleCollider[] allDirectedCircleColliders;
    DirectedCircleOverlap[] allDirectedCircleOverlaps;
    CircleSpriteMask[] allCircleSpriteMasks;
    DirectedPoint[] allDirectedPoints;
    DirectedCircleCollider directedCircleColliderSelected;
    DirectedCircleOverlap directedCircleOverlapSelected;
    CircleSpriteMask circleSpriteMaskSelected;
    DirectedPoint directedPointSelected;
    List<int> selectedCenters = new();
    List<int> selectedRadii = new();
    List<int> selectedUpDirections = new();
    List<int> selectedRightDirections = new();
    DataSelection DataSelection 
    {
        get 
        { 
            DataSelection selectionValue = DataSelection.None;
            if (directedCircleColliderSelected != null)
                selectionValue |= DataSelection.DirectedCircleCollider;
            if (directedCircleOverlapSelected != null)
                selectionValue |= DataSelection.DirectedCircleOverlap;
            if (circleSpriteMaskSelected != null)
                selectionValue |= DataSelection.CircleSpriteMask;
            if (directedPointSelected != null)
                selectionValue |= DataSelection.DirectedPoint;
            return selectionValue;
        }
    }
    [MenuItem("Window/ControllerDataEditor")]
    public static void ShowWindow()
    {
        ControllerDataEditor editorWindow = GetWindow<ControllerDataEditor>("ControllerDataEditor");
        editorWindow.Init();
    }
    void Init()
    {
        allControllers = new List<AnimatorController>();
        string[] guids = AssetDatabase.FindAssets("t:AnimatorController");
        for (int i = 0; i < guids.Length; i++)
        {
            allControllers.Add(AssetDatabase.LoadAssetAtPath<AnimatorController>(AssetDatabase.GUIDToAssetPath(guids[i])));
        }
        animator = new GameObject("EDITOR_ANIMATOR").AddComponent<Animator>();
        animator.gameObject.AddComponent<SpriteRenderer>();
        allControllerData = ResourcesUtility.LoadAllControllerDataInResources();
    }
    void ControllerSelection()
    {
        if (allControllers == null || allControllers.Count == 0)
            return;
        if(controllerSelected == null)
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("ANIMATOR CONTROLLER", EditorStyles.boldLabel);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            for (int i = 0; i < allControllers.Count; i++)
            {
                if (GUILayout.Button(allControllers[i].name))
                {
                    controllerSelected = allControllers[i];
                }
            }
        }
        else
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label($"CONTROLLER SELECTED: {controllerSelected.name}", EditorStyles.whiteLargeLabel);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }
    }
    void ControllerDataSelection()
    {
        if (controllerSelected == null)
            return;
        //Only in the case no controller data exists at all
        if(allControllerData == null || allControllerData.Length == 0)
        {
            //Create new controller data
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("No controller data exists yet. Create new?", EditorStyles.boldLabel);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            if (GUILayout.Button("CREATE NEW CONTROLLER DATA"))
            {
                ResourcesUtility.CreateNewControllerData(controllerSelected.name);
                allControllerData = ResourcesUtility.LoadAllControllerDataInResources();
            }
            return;
        }
        if(controllerDataSelected == null)
        {
            foreach (ControllerData controllerData in allControllerData)
            {
                if (controllerData.controllerName == controllerSelected.name)
                {
                    controllerDataSelected = controllerData;
                    return;
                }
            }

            //Create new controller data
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("No controller data exists yet. Create new?", EditorStyles.boldLabel);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            if (GUILayout.Button("CREATE NEW CONTROLLER DATA"))
            {
                ResourcesUtility.CreateNewControllerData(controllerSelected.name);
                allControllerData = ResourcesUtility.LoadAllControllerDataInResources();
            }
        }
    }
    void LayerSelection()
    {
        if (controllerSelected == null)
            return;
        if (controllerSelected.layers == null)
            return;
        if (controllerSelected.layers.Length == 0)
            return;
        if(layerSelected == null)
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("ANIMATOR CONTROLLER LAYER", EditorStyles.boldLabel);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            for (int i = 0; i < controllerSelected.layers.Length; i++)
            {
                if (GUILayout.Button(controllerSelected.layers[i].name))
                {
                    layerSelected = controllerSelected.layers[i];
                }
            }
        }
        else
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label($"LAYER SELECTED: {layerSelected.name}", EditorStyles.whiteLargeLabel);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }
    }
    void StateSelection()
    {
        if (layerSelected == null)
            return;
        if (layerSelected.stateMachine == null)
            return;
        if (layerSelected.stateMachine.states == null)
            return;
        if (layerSelected.stateMachine.states.Length == 0)
            return;
        if (stateSelected == null)
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("ANIMATOR STATE", EditorStyles.boldLabel);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            //During these button renders show if the states have data
            //if they do, show the data info when clicked
            for (int i = 0; i < layerSelected.stateMachine.states.Length; i++)
            {
                if (GUILayout.Button(layerSelected.stateMachine.states[i].state.name))
                {
                    stateSelected = layerSelected.stateMachine.states[i].state;
                }
            }
        }
        else
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label($"STATE SELECTED: {stateSelected.name}", EditorStyles.whiteLargeLabel);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }
    }
    void StateDataChoice()
    {
        if (stateSelected == null)
            return;
        if (DataSelection != DataSelection.None)
            return;
        if(GUILayout.Button("CHOOSE ANOTHER STATE"))
        {
            stateSelected = null;
            return;
        }
        allDirectedCircleColliders = controllerDataSelected.GetAllDirectedCircleColliderData(stateSelected.name);
        allDirectedCircleOverlaps = controllerDataSelected.GetAllDirectedCircleOverlapData(stateSelected.name);
        allCircleSpriteMasks = controllerDataSelected.GetAllCircleSpriteMaskData(stateSelected.name);
        allDirectedPoints = controllerDataSelected.GetAllDirectedPointData(stateSelected.name);
        if (GUILayout.Button("CREATE NEW DIRECTED CIRCLE COLLIDER DATA"))
        {
            DirectedCircleCollider.AddNew(controllerDataSelected, DirectedCircleCollider.CreateNew(stateSelected.name));
        }
        if (GUILayout.Button("CREATE NEW DIRECTED CIRCLE OVERLAP DATA"))
        {
            DirectedCircleOverlap.AddNew(controllerDataSelected, DirectedCircleOverlap.CreateNew(stateSelected.name));
        }
        if (GUILayout.Button("CREATE NEW CIRCLE SPRITE MASK DATA"))
        {
            CircleSpriteMask.AddNew(controllerDataSelected, CircleSpriteMask.CreateNew(stateSelected.name));
        }
        if(GUILayout.Button("CREATE NEW DIRECTED POINT DATA"))
        {
            DirectedPoint.AddNew(controllerDataSelected, DirectedPoint.CreateNew(stateSelected.name));
        }
        EditorUtility.SetDirty(controllerDataSelected);
    }
    void DirectedCircleColliderSelection()
    {
        if (stateSelected == null)
            return;
        if (allDirectedCircleColliders == null || allDirectedCircleColliders.Length == 0)
            return;
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.Label($"Directed Circle Collider Data", EditorStyles.whiteBoldLabel);
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        if (directedCircleColliderSelected == null)
        {
            for (int i = 0; i < allDirectedCircleColliders.Length; i++)
            {
                GUILayout.BeginHorizontal();
                if (GUILayout.Button(allDirectedCircleColliders[i].nickname))
                    directedCircleColliderSelected = allDirectedCircleColliders[i];
                if(GUILayout.Button("DELETE"))
                    DirectedCircleCollider.RemoveAt(controllerDataSelected, i);
                GUILayout.EndHorizontal();
            }
            return;
        }
        if (GUILayout.Button("CHOOSE ANOTHER DATA TYPE"))
        {
            directedCircleColliderSelected = null;
            ClearSelection();
            return;
        }
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.Label($"DIRECTED CIRCLE COLLIDER DATA SELECTED: {directedCircleColliderSelected.nickname}", EditorStyles.whiteLargeLabel);
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
    }
    void DirectedCircleOverlapSelection()
    {
        if (stateSelected == null)
            return;
        if (allDirectedCircleOverlaps == null || allDirectedCircleOverlaps.Length == 0)
            return;
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.Label($"Directed Circle Overlap Data", EditorStyles.whiteBoldLabel);
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        if (directedCircleOverlapSelected == null)
        {
            for (int i = 0; i < allDirectedCircleOverlaps.Length; i++)
            {
                GUILayout.BeginHorizontal();
                if (GUILayout.Button(allDirectedCircleOverlaps[i].nickname))
                    directedCircleOverlapSelected = allDirectedCircleOverlaps[i];
                if (GUILayout.Button("DELETE"))
                    DirectedCircleOverlap.RemoveAt(controllerDataSelected, i);
                GUILayout.EndHorizontal();
            }
            return;
        }
        if(GUILayout.Button("CHOOSE ANOTHER DATA TYPE"))
        {
            directedCircleOverlapSelected = null;
            ClearSelection();
            return;
        }
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.Label($"DIRECTED CIRCLE OVERLAP DATA SELECTED: {directedCircleOverlapSelected.nickname}", EditorStyles.whiteLargeLabel);
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
    }
    void CircleSpriteMaskSelection()
    {
        if (stateSelected == null)
            return;
        if (allCircleSpriteMasks == null || allCircleSpriteMasks.Length == 0)
            return;
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.Label($"Circle Sprite Mask Data", EditorStyles.whiteBoldLabel);
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        if (circleSpriteMaskSelected == null)
        {
            for (int i = 0; i < allCircleSpriteMasks.Length; i++)
            {
                GUILayout.BeginHorizontal();
                if (GUILayout.Button(allCircleSpriteMasks[i].nickname))
                    circleSpriteMaskSelected = allCircleSpriteMasks[i];
                if (GUILayout.Button("DELETE"))
                    CircleSpriteMask.RemoveAt(controllerDataSelected, i);
                GUILayout.EndHorizontal();
            }
            return;
        }
        if (GUILayout.Button("CHOOSE ANOTHER DATA TYPE"))
        {
            circleSpriteMaskSelected = null;
            ClearSelection();
            return;
        }
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.Label($"CIRCLE SPRITE MASK DATA SELECTED: {circleSpriteMaskSelected.nickname}", EditorStyles.whiteLargeLabel);
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
    }
    void DirectedPointSelection()
    {
        if (stateSelected == null)
            return;
        if (allDirectedPoints == null || allDirectedPoints.Length == 0)
            return;
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.Label($"Directed Point Data", EditorStyles.whiteBoldLabel);
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        if (directedPointSelected == null)
        {
            for (int i = 0; i < allDirectedPoints.Length; i++)
            {
                GUILayout.BeginHorizontal();
                if (GUILayout.Button(allDirectedPoints[i].nickname))
                    directedPointSelected = allDirectedPoints[i];
                if (GUILayout.Button("DELETE"))
                    DirectedPoint.RemoveAt(controllerDataSelected, i);
                GUILayout.EndHorizontal();
            }
            return;
        }
        if (GUILayout.Button("CHOOSE ANOTHER DATA TYPE"))
        {
            directedPointSelected = null;
            ClearSelection();
            return;
        }
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.Label($"DIRECTED POINT DATA SELECTED: {directedPointSelected.nickname}", EditorStyles.whiteLargeLabel);
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
    }
    void ClearSelection()
    {
        selectedCenters.Clear();
        selectedRadii.Clear();
        selectedUpDirections.Clear();
        selectedRightDirections.Clear();
    }
    private void OnGUI()
    {
        ControllerSelection();
        ControllerDataSelection();
        if (controllerDataSelected == null)
            return;
        scrollPosition = GUILayout.BeginScrollView(scrollPosition);
        LayerSelection();
        StateSelection();
        StateDataChoice();

        if(DataSelection.CheckComplementOfFlag(DataSelection.DirectedCircleCollider))
            DirectedCircleColliderSelection();
        if (DataSelection.CheckComplementOfFlag(DataSelection.DirectedCircleOverlap))
            DirectedCircleOverlapSelection();
        if(DataSelection.CheckComplementOfFlag(DataSelection.CircleSpriteMask))
            CircleSpriteMaskSelection();
        if(DataSelection.CheckComplementOfFlag(DataSelection.DirectedPoint))
            DirectedPointSelection();
        FrameSelection();
        
        if (directedCircleColliderSelected != null)
        {
            //Do all the editor GUI for this data type
            //pass through methods as ref please
            StateDataEditing.AssignedFramesLabel(directedCircleColliderSelected.assignedFrames);
            StateDataEditing.AssignedFramesEdit(frame, ref directedCircleColliderSelected.assignedFrames);
            StateDataEditing.MainDataEdit(ref directedCircleColliderSelected.nickname, ref directedCircleColliderSelected.stateName, ref directedCircleColliderSelected.color);
            StateDataEditing.ColliderDataEdit(ref directedCircleColliderSelected.isTrigger, ref directedCircleColliderSelected.collisionLayer);
            StateDataEditing.CenterSelection(directedCircleColliderSelected.nickname, ref directedCircleColliderSelected.centers, ref selectedCenters);
            StateDataEditing.RadiusSelection(directedCircleColliderSelected.nickname, ref directedCircleColliderSelected.radii, ref selectedRadii);
            StateDataEditing.UpDirectionSelection(directedCircleColliderSelected.nickname, ref directedCircleColliderSelected.upDirections, ref selectedUpDirections);
            StateDataEditing.RightDirectionSelection(directedCircleColliderSelected.nickname, ref directedCircleColliderSelected.rightDirections, ref selectedRightDirections);
        }
        if(directedCircleOverlapSelected != null)
        {
            //Do all the editor GUI for this data type
            //pass through methods as ref please
            StateDataEditing.AssignedFramesLabel(directedCircleOverlapSelected.assignedFrames);
            StateDataEditing.AssignedFramesEdit(frame, ref directedCircleOverlapSelected.assignedFrames);
            StateDataEditing.MainDataEdit(ref directedCircleOverlapSelected.nickname, ref directedCircleOverlapSelected.stateName, ref directedCircleOverlapSelected.color);
            StateDataEditing.OverlapDataEdit(ref directedCircleOverlapSelected.useNullResult, ref directedCircleOverlapSelected.targetLayers, ref directedCircleOverlapSelected.holdForNormalizedTime);
            StateDataEditing.CenterSelection(directedCircleOverlapSelected.nickname, ref directedCircleOverlapSelected.centers, ref selectedCenters);
            StateDataEditing.RadiusSelection(directedCircleOverlapSelected.nickname, ref directedCircleOverlapSelected.radii, ref selectedRadii);
            StateDataEditing.UpDirectionSelection(directedCircleOverlapSelected.nickname, ref directedCircleOverlapSelected.upDirections, ref selectedUpDirections);
            StateDataEditing.RightDirectionSelection(directedCircleOverlapSelected.nickname, ref directedCircleOverlapSelected.rightDirections, ref selectedRightDirections);
        }
        if(circleSpriteMaskSelected != null)
        {
            StateDataEditing.AssignedFramesLabel(circleSpriteMaskSelected.assignedFrames);
            StateDataEditing.AssignedFramesEdit(frame, ref circleSpriteMaskSelected.assignedFrames);
            StateDataEditing.MainDataEdit(ref circleSpriteMaskSelected.nickname, ref circleSpriteMaskSelected.stateName, ref circleSpriteMaskSelected.color);
            StateDataEditing.SpriteMaskDataEdit(ref circleSpriteMaskSelected.isCustomRangeActive, ref circleSpriteMaskSelected.sortingLayer, ref circleSpriteMaskSelected.sortingOrder);
            StateDataEditing.CenterSelection(circleSpriteMaskSelected.nickname, ref circleSpriteMaskSelected.centers, ref selectedCenters);
            StateDataEditing.RadiusSelection(circleSpriteMaskSelected.nickname, ref circleSpriteMaskSelected.radii, ref selectedRadii);
        }
        if(directedPointSelected != null)
        {
            StateDataEditing.AssignedFramesLabel(directedPointSelected.assignedFrames);
            StateDataEditing.AssignedFramesEdit(frame, ref directedPointSelected.assignedFrames);
            StateDataEditing.MainDataEdit(ref directedPointSelected.nickname, ref directedPointSelected.stateName, ref directedPointSelected.color);
            StateDataEditing.CenterSelection(directedPointSelected.nickname, ref directedPointSelected.centers, ref selectedCenters);
            StateDataEditing.UpDirectionSelection(directedPointSelected.nickname, ref directedPointSelected.upDirections, ref selectedUpDirections);
            StateDataEditing.RightDirectionSelection(directedPointSelected.nickname, ref directedPointSelected.rightDirections, ref selectedRightDirections);
        }
        GUILayout.EndScrollView();
        EditorUtility.SetDirty(controllerDataSelected);
    }
    private void OnEnable()
    {
        SceneView.duringSceneGui -= OnSceneGUI;
        SceneView.duringSceneGui += OnSceneGUI;
    }
    private void OnDestroy()
    {
        SceneView.duringSceneGui -= OnSceneGUI;
        if (controllerDataSelected != null)
        {
            EditorUtility.SetDirty(controllerDataSelected);
            AssetDatabase.SaveAssets();
        }
        if (animator != null)
            DestroyImmediate(animator.gameObject);
    }
    public void FrameSelection()
    {
        if (DataSelection == DataSelection.None)
        {
            frame = 0;
            return;
        }
        clipSelected = stateSelected.motion as AnimationClip;
        AnimationMode.StartAnimationMode();
        GUILayout.BeginHorizontal();
        if (GUILayout.Button(" < ") && frame > 0)
            frame--;
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.Label($"FRAME {frame}", EditorStyles.boldLabel);
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        if(GUILayout.Button(" > ") && frame < clipSelected.length)
            frame++;
        GUILayout.EndHorizontal();
    }
    private void Update()
    {
        if (clipSelected == null)
            return;
        if (!EditorApplication.isPlaying && AnimationMode.InAnimationMode())
        {
            AnimationMode.BeginSampling();
            AnimationMode.SampleAnimationClip(animator.gameObject, clipSelected, frame);
            AnimationMode.EndSampling();
            SceneView.RepaintAll();
        }
    }
    void OnSceneGUI(SceneView sceneView)
    {
        if(controllerDataSelected == null)
            return;
        if(directedCircleColliderSelected != null)
        {
            StateDataHandles.DrawCircles(animator.transform, directedCircleColliderSelected.centers, directedCircleColliderSelected.radii, selectedCenters, directedCircleColliderSelected.color);
            if(directedCircleColliderSelected.centers.Count > 0)
            {
                StateDataHandles.CenterHandle(animator.transform, ref directedCircleColliderSelected.centers, selectedCenters, directedCircleColliderSelected.color);
                StateDataHandles.RadiusHandle(animator.transform, directedCircleColliderSelected.centers, ref directedCircleColliderSelected.radii, selectedRadii, directedCircleColliderSelected.color);
                StateDataHandles.UpDirectionHandle(animator.transform, directedCircleColliderSelected.centers, ref directedCircleColliderSelected.upDirections, selectedUpDirections);
                StateDataHandles.RightDirectionHandle(animator.transform, directedCircleColliderSelected.centers, ref directedCircleColliderSelected.rightDirections, selectedRightDirections);
            }
        }
        if(directedCircleOverlapSelected != null)
        {
            StateDataHandles.DrawCircles(animator.transform, directedCircleOverlapSelected.centers, directedCircleOverlapSelected.radii, selectedCenters, directedCircleOverlapSelected.color);
            if(directedCircleOverlapSelected.centers.Count > 0)
            {
                StateDataHandles.CenterHandle(animator.transform, ref directedCircleOverlapSelected.centers, selectedCenters, directedCircleOverlapSelected.color);
                StateDataHandles.RadiusHandle(animator.transform, directedCircleOverlapSelected.centers, ref directedCircleOverlapSelected.radii, selectedRadii, directedCircleOverlapSelected.color);
                StateDataHandles.UpDirectionHandle(animator.transform, directedCircleOverlapSelected.centers, ref directedCircleOverlapSelected.upDirections, selectedUpDirections);
                StateDataHandles.RightDirectionHandle(animator.transform, directedCircleOverlapSelected.centers, ref directedCircleOverlapSelected.rightDirections, selectedRightDirections);
            }
        }
        if(circleSpriteMaskSelected != null)
        {
            StateDataHandles.DrawCircles(animator.transform, circleSpriteMaskSelected.centers, circleSpriteMaskSelected.radii, selectedCenters, circleSpriteMaskSelected.color);
            if(circleSpriteMaskSelected.centers.Count > 0)
            {
                StateDataHandles.CenterHandle(animator.transform, ref circleSpriteMaskSelected.centers, selectedCenters, circleSpriteMaskSelected.color);
                StateDataHandles.RadiusHandle(animator.transform, circleSpriteMaskSelected.centers, ref circleSpriteMaskSelected.radii, selectedRadii, circleSpriteMaskSelected.color);
            }
        }
        if(directedPointSelected != null)
        {
            if(directedPointSelected.centers.Count > 0)
            {
                StateDataHandles.PointHandle(animator.transform, ref directedPointSelected.centers, selectedCenters, directedPointSelected.color);
                StateDataHandles.UpDirectionHandle(animator.transform, directedPointSelected.centers, ref directedPointSelected.upDirections, selectedUpDirections);
                StateDataHandles.RightDirectionHandle(animator.transform, directedPointSelected.centers, ref directedPointSelected.rightDirections, selectedRightDirections);
            }
        }
    }
}
public static class StateDataEditing
{
    public static void AssignedFramesLabel(List<int> assignedFrames)
    {
        string assignedFramesLabel = string.Empty;
        if(assignedFrames == null || assignedFrames.Count == 0)
        {
            assignedFramesLabel += "NO FRAMES APPLIED";
        }
        for (int i = 0; i < assignedFrames.Count; i++)
        {
            if(i == 0)
            {
                assignedFramesLabel += $"APPLIED FRAMES: {assignedFrames[i]}";
            }
            else
            {
                assignedFramesLabel += $", {assignedFrames[i]}";
            }
        }
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.Label(assignedFramesLabel, EditorStyles.whiteLargeLabel);
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
    }
    public static void AssignedFramesEdit(int frame, ref List<int> assignedFrames)
    {
        if(assignedFrames == null || assignedFrames.Count == 0)
        {
            if(GUILayout.Button($"Apply To Frame {frame}"))
            {
                assignedFrames = new List<int>() { frame };
            }
            return;
        }
        if(assignedFrames.Contains(frame))
        {
            if(GUILayout.Button($"Remove From Frame {frame}"))
            {
                assignedFrames.Remove(frame);
                assignedFrames.Sort();
            }
        }
        else
        {
            if(GUILayout.Button($"Apply To Frame {frame}"))
            {
                assignedFrames.Add(frame);
                assignedFrames.Sort();
            }
        }
        if(GUILayout.Button("Clear All"))
        {
            assignedFrames.Clear();
        }
    }
    public static LayerMask LayerMaskField(string label, LayerMask layerMask)
    {
        LayerMask tempMask = EditorGUILayout.MaskField(label, UnityEditorInternal.InternalEditorUtility.LayerMaskToConcatenatedLayersMask(layerMask), UnityEditorInternal.InternalEditorUtility.layers);
        return UnityEditorInternal.InternalEditorUtility.ConcatenatedLayersMaskToLayerMask(tempMask);
    }
    public static void MainDataEdit(ref string nickname, ref string stateName, ref Color color)
    {
        nickname = EditorGUILayout.DelayedTextField("Nickname: ", nickname);
        stateName = EditorGUILayout.DelayedTextField("State Name: ", stateName);
        color = EditorGUILayout.ColorField("Color: ", color);
    }
    public static void ColliderDataEdit(ref bool isTrigger, ref int collisionLayer)
    {
        isTrigger = EditorGUILayout.Toggle("Is Trigger: ", isTrigger);
        collisionLayer = EditorGUILayout.LayerField("Collision Layer: ", collisionLayer);
    }
    public static void OverlapDataEdit(ref bool useNullResult, ref LayerMask targetLayers, ref float holdForNormalizedTime)
    {
        useNullResult = EditorGUILayout.Toggle("Use Null Result: ", useNullResult);
        targetLayers = LayerMaskField("Target Layers: ", targetLayers);
        holdForNormalizedTime = EditorGUILayout.FloatField("Hold For Normalized Time: ", holdForNormalizedTime);
    }
    public static void CenterSelection(string nickname, ref List<Vector2> centers, ref List<int> selection)
    {
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.Label($"{nickname} Centers", EditorStyles.whiteBoldLabel);
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        if (GUILayout.Button("Add Center"))
        {
            centers.Add(Vector2.zero);
        }
        for (int i = 0; i < centers.Count; i++)
        {
            GUILayout.BeginHorizontal();
            if (selection.Contains(i))
            {
                if (GUILayout.Button("[ - ]"))
                {
                    selection.Remove(i);
                }
            }
            if(!selection.Contains(i))
            {
                if(GUILayout.Button("[ + ]"))
                {
                    selection.Add(i);
                }
            }
            centers[i] = EditorGUILayout.Vector2Field($"{nickname} Center {i}", centers[i]);
            if(GUILayout.Button("Delete"))
            {
                //Going to need to check all indices which are currently selected
                //find out which ones are greater than the one we are deleting
                //and then subtract one from them
                List<Vector2> newCenters = new();
                List<int> newSelections = new();
                for (int j = 0; j < centers.Count; j++)
                {
                    if (j == i)
                        continue;
                    newCenters.Add(centers[j]);
                    if (selection.Contains(j))
                    {
                        if (j > i)
                            newSelections.Add(j - 1);
                        else
                            newSelections.Add(j);
                    }
                }
                centers = newCenters;
                selection = newSelections;
            }
            GUILayout.EndHorizontal();
        }
    }
    public static void RadiusSelection(string nickname, ref List<float> radii, ref List<int> selection)
    {
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.Label($"{nickname} Radii", EditorStyles.whiteBoldLabel);
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        if (GUILayout.Button("Add Radius"))
        {
            radii.Add(0);
        }
        for (int i = 0; i < radii.Count; i++)
        {
            GUILayout.BeginHorizontal();
            if (selection.Contains(i))
            {
                if (GUILayout.Button("[ - ]"))
                {
                    selection.Remove(i);
                }
            }
            if (!selection.Contains(i))
            {
                if (GUILayout.Button("[ + ]"))
                {
                    selection.Add(i);
                }
            }
            radii[i] = EditorGUILayout.FloatField($"{nickname} Radius {i}", radii[i]);
            if (GUILayout.Button("Delete"))
            {
                //Going to need to check all indices which are currently selected
                //find out which ones are greater than the one we are deleting
                //and then subtract one from them
                List<float> newRadii = new();
                List<int> newSelections = new();
                for (int j = 0; j < radii.Count; j++)
                {
                    if (j == i)
                        continue;
                    newRadii.Add(radii[j]);
                    if (selection.Contains(j))
                    {
                        if (j > i)
                            newSelections.Add(j - 1);
                        else
                            newSelections.Add(j);
                    }
                }
                radii = newRadii;
                selection = newSelections;
            }
            GUILayout.EndHorizontal();
        }
    }
    public static void UpDirectionSelection(string nickname, ref List<Vector2> upDirections, ref List<int> selection)
    {
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.Label($"{nickname} Up Directions", EditorStyles.whiteBoldLabel);
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        if (GUILayout.Button("Add Up Direction"))
        {
            upDirections.Add(Vector2.up);
        }
        for (int i = 0; i < upDirections.Count; i++)
        {
            GUILayout.BeginHorizontal();
            if (selection.Contains(i))
            {
                if (GUILayout.Button("[ - ]"))
                {
                    selection.Remove(i);
                }
            }
            if (!selection.Contains(i))
            {
                if (GUILayout.Button("[ + ]"))
                {
                    selection.Add(i);
                }
            }
            upDirections[i] = EditorGUILayout.Vector2Field($"{nickname} Up Direction {i}", upDirections[i]);
            if (GUILayout.Button("Delete"))
            {
                List<Vector2> newUpDirections = new List<Vector2>();
                List<int> newSelections = new List<int>();
                for (int j = 0; j < upDirections.Count; j++)
                {
                    if (j == i)
                        continue;
                    newUpDirections.Add(upDirections[j]);
                    if (selection.Contains(j))
                    {
                        if (j > i)
                            newSelections.Add(j - 1);
                        else
                            newSelections.Add(j);
                    }
                }
                upDirections = newUpDirections;
                selection = newSelections;
            }
            GUILayout.EndHorizontal();
        }
    }
    public static void RightDirectionSelection(string nickname, ref List<Vector2> rightDirections, ref List<int> selection)
    {
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.Label($"{nickname} Right Directions", EditorStyles.whiteBoldLabel);
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        if (GUILayout.Button("Add Right Direction"))
        {
            rightDirections.Add(Vector2.right);
        }
        for (int i = 0; i < rightDirections.Count; i++)
        {
            GUILayout.BeginHorizontal();
            if (selection.Contains(i))
            {
                if (GUILayout.Button("[ - ]"))
                {
                    selection.Remove(i);
                }
            }
            if (!selection.Contains(i))
            {
                if (GUILayout.Button("[ + ]"))
                {
                    selection.Add(i);
                }
            }
            rightDirections[i] = EditorGUILayout.Vector2Field($"{nickname} Right Direction {i}", rightDirections[i]);
            if (GUILayout.Button("Delete"))
            {
                List<Vector2> newRightDirections = new List<Vector2>();
                List<int> newSelections = new List<int>();
                for (int j = 0; j < rightDirections.Count; j++)
                {
                    if (j == i)
                        continue;
                    newRightDirections.Add(rightDirections[j]);
                    if (selection.Contains(j))
                    {
                        if (j > i)
                            newSelections.Add(j - 1);
                        else
                            newSelections.Add(j);
                    }
                }
                rightDirections = newRightDirections;
                selection = newSelections;
            }
            GUILayout.EndHorizontal();
        }
    }
    public static void SpriteMaskDataEdit(ref bool isCustomRangeActive, ref string sortingLayer, ref int sortingOrder)
    {
        isCustomRangeActive = EditorGUILayout.Toggle("Is Custom Range Active", isCustomRangeActive);
        sortingLayer = EditorGUILayout.DelayedTextField("Sorting Layer", sortingLayer);
        sortingOrder = EditorGUILayout.DelayedIntField("Sorting Order", sortingOrder);
    }
}
public static class StateDataHandles
{
    public static void DrawCircles(Transform parentObj, List<Vector2> centers, List<float> radii, List<int> selectedCenters, Color color)
    {
        for (int i = 0; i < centers.Count; i++)
        {
            if (i > radii.Count - 1)
                continue;
            Vector3 worldPos = parentObj.position + (Vector3)centers[i];
            Handles.color = color.WithAlpha(selectedCenters.Contains(i) ? 0.75f : 0.25f);
            Handles.DrawSolidDisc(worldPos, Vector3.forward, radii[i]);
            Handles.color = Color.black;
            Handles.DrawWireDisc(worldPos, Vector3.forward, radii[i], 4f);
            Handles.color = (Color.white - color).WithAlpha(selectedCenters.Contains(i) ? 0.75f : 0.25f);
            Handles.DrawDottedLine(worldPos, worldPos + Vector3.right * radii[i], 3f);
        }
    }
    public static void CenterHandle(Transform parentObj, ref List<Vector2> centers, List<int> selectedCenters, Color color)
    {
        if (centers == null || centers.Count == 0)
            return;
        if (selectedCenters == null || selectedCenters.Count == 0)
            return;
        Vector3 worldPos = parentObj.position + (Vector3)centers[selectedCenters[0]];
        EditorGUI.BeginChangeCheck();
        Handles.color = (Color.white - color).WithAlpha(1f);
        Vector3 oldSquarePos = worldPos;
        Vector3 newSquarePos = Handles.FreeMoveHandle(oldSquarePos, Quaternion.identity, 0.65f, Vector3.one*0.25f, Handles.RectangleHandleCap);
        if (EditorGUI.EndChangeCheck())
        {
            Vector3 delta = newSquarePos - oldSquarePos;
            for (int i = 0; i < selectedCenters.Count; i++)
            {
                centers[selectedCenters[i]] += (Vector2)delta;
            }
        }
    }
    public static void RadiusHandle(Transform parentObj, List<Vector2> centers, ref List<float> radii, List<int> selectedRadii, Color color)
    {
        if (radii == null || radii.Count == 0)
            return;
        if (selectedRadii == null || selectedRadii.Count == 0)
            return;
        for (int i = 0; i < selectedRadii.Count; i++)
        {
            if (selectedRadii[i] >= centers.Count)
                continue;
            Vector3 worldPos = parentObj.position + (Vector3)centers[selectedRadii[i]];
            EditorGUI.BeginChangeCheck();
            Handles.color = color.WithAlpha(1f);
            Vector3 oldArrowPos = worldPos + Vector3.right * radii[selectedRadii[i]];
            Vector3 newArrowPos = Handles.Slider(oldArrowPos, Vector3.right, 1f, Handles.ArrowHandleCap, 0.1f);
            if (EditorGUI.EndChangeCheck())
            {
                Vector3 delta = newArrowPos - oldArrowPos;
                radii[selectedRadii[i]] += delta.x;
                if (radii[selectedRadii[i]] < 0)
                    radii[selectedRadii[i]] = 0;
            }
        }
    }
    public static void UpDirectionHandle(Transform parentObj, List<Vector2> centers, ref List<Vector2> upDirections, List<int> selectedUpDirections)
    {
        if(upDirections == null || upDirections.Count == 0)
            return;
        if (selectedUpDirections == null || selectedUpDirections.Count == 0)
            return;
        for (int i = 0; i < selectedUpDirections.Count; i++)
        {
            if (selectedUpDirections[i] >= centers.Count)
                continue;
            Vector3 worldPos = parentObj.position + (Vector3)centers[selectedUpDirections[i]];
            EditorGUI.BeginChangeCheck();
            Handles.color = Handles.yAxisColor;
            Vector3 oldCirclePos = worldPos + (Vector3)upDirections[selectedUpDirections[i]];
            Vector3 newCirclePos = Handles.FreeMoveHandle(oldCirclePos, Quaternion.identity, 0.65f, Vector3.one * 0.25f, Handles.CircleHandleCap);
            Handles.DrawLine(worldPos, newCirclePos);
            if(EditorGUI.EndChangeCheck())
            {
                Vector3 delta = newCirclePos - oldCirclePos;
                upDirections[selectedUpDirections[i]] += (Vector2)delta;
                upDirections[selectedUpDirections[i]] = upDirections[selectedUpDirections[i]].normalized;
            }
        }
    }
    public static void RightDirectionHandle(Transform parentObj, List<Vector2> centers, ref List<Vector2> rightDirections, List<int> selectedRightDirections)
    {
        if (rightDirections == null || rightDirections.Count == 0)
            return;
        if (selectedRightDirections == null || selectedRightDirections.Count == 0)
            return;
        for (int i = 0; i < selectedRightDirections.Count; i++)
        {
            if (selectedRightDirections[i] >= centers.Count)
                continue;
            Vector3 worldPos = parentObj.position + (Vector3)centers[selectedRightDirections[i]];
            EditorGUI.BeginChangeCheck();
            Handles.color = Handles.xAxisColor;
            Vector3 oldCirclePos = worldPos + (Vector3)rightDirections[selectedRightDirections[i]];
            Vector3 newCirclePos = Handles.FreeMoveHandle(oldCirclePos, Quaternion.identity, 0.65f, Vector3.one * 0.25f, Handles.CircleHandleCap);
            Handles.DrawLine(worldPos, newCirclePos);
            if (EditorGUI.EndChangeCheck())
            {
                Vector3 delta = newCirclePos - oldCirclePos;
                rightDirections[selectedRightDirections[i]] += (Vector2)delta;
                rightDirections[selectedRightDirections[i]] = rightDirections[selectedRightDirections[i]].normalized;
            }
        }
    }
    public static void PointHandle(Transform parentObj, ref List<Vector2> centers, List<int> selectedCenters, Color color)
    {
        if (centers == null || centers.Count == 0)
            return;
        if (selectedCenters == null || selectedCenters.Count == 0)
            return;
        for (int i = 0; i < selectedCenters.Count; i++)
        {
            if (selectedCenters[i] >= centers.Count)
                continue;
            Vector3 worldPos = parentObj.position + (Vector3)centers[selectedCenters[i]];
            EditorGUI.BeginChangeCheck();
            Handles.color = color.WithAlpha(1f);
            Vector3 oldSquarePos = worldPos;
            Vector3 newSquarePos = Handles.FreeMoveHandle(oldSquarePos, Quaternion.identity, 0.65f, Vector3.one * 0.25f, Handles.RectangleHandleCap);
            Handles.DrawLine(worldPos - Vector3.right, worldPos + Vector3.right, 3f);
            Handles.DrawLine(worldPos - Vector3.up, worldPos + Vector3.up, 3f);
            if (EditorGUI.EndChangeCheck())
            {
                Vector3 delta = newSquarePos - oldSquarePos;
                centers[selectedCenters[i]] += (Vector2)delta;
            }
        }
    }
}
[Flags]
public enum DataSelection
{
    None = 0,
    DirectedCircleCollider = 1,
    DirectedCircleOverlap = 2,
    CircleSpriteMask = 4,
    DirectedPoint = 8,
}
#endif
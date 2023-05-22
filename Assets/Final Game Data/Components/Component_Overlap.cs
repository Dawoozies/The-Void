using System;
using System.Collections.Generic;
using UnityEngine;
using GeometryDefinitions;
using UnityEditor.Animations;
using UnityEditor;
using ExtensionMethods_AnimatorController;
using ComponentIO;
using System.Linq;
using ComponentEditorUI;

[Serializable]
public class Component_Overlap : ScriptableObject
{
    public int stateHash;
    public Component_Overlap_Data[] componentData;
    public void AddComponentData(Component_Overlap_Data data)
    {
        if (componentData == null)
        {
            componentData = new Component_Overlap_Data[1];
            componentData[0] = new Component_Overlap_Data();
            data.CopyToNew(componentData[0]);
            return;
        }

        Component_Overlap_Data[] resizedData = new Component_Overlap_Data[componentData.Length + 1];
        for (int i = 0; i < resizedData.Length; i++)
        {
            if (i < componentData.Length)
            {
                resizedData[i] = new Component_Overlap_Data();
                componentData[i].CopyToNew(resizedData[i]);
            }
            if (i == resizedData.Length - 1)
            {
                resizedData[i] = new Component_Overlap_Data();
                data.CopyToNew(resizedData[i]);
            }
        }
        componentData = resizedData;
        EditorUtility.SetDirty(this);
    }
    public Component_Overlap_Data[] DataWithFrame(int frame)
    {
        if (componentData == null || componentData.Length == 0)
            return null;
        List<Component_Overlap_Data> dataAtFrame = new List<Component_Overlap_Data>();
        for (int i = 0; i < componentData.Length; i++)
        {
            if (componentData[i].frames.Contains(frame))
                dataAtFrame.Add(componentData[i]);
        }
        return dataAtFrame.ToArray();
    }
    public void RemoveComponentData(Component_Overlap_Data dataToRemove)
    {
        if (componentData == null || componentData.Length == 0)
            return;
        if (!componentData.Contains(dataToRemove))
            return;
        Component_Overlap_Data[] resizedData = new Component_Overlap_Data[componentData.Length - 1];
        int shift = 0;
        for (int i = 0; i < resizedData.Length; i++)
        {
            if (componentData[i] == dataToRemove)
            {
                shift++;
            }
            resizedData[i] = new Component_Overlap_Data();
            componentData[i + shift].CopyToNew(resizedData[i]);
        }
        componentData = resizedData;
        EditorUtility.SetDirty(this);
    }
}
[Serializable]
public class Component_Overlap_Data
{
    public string nickname;
    public List<int> frames;
    public Color fillColor;
    public Color lineColor;
    public LayerMask layerMask;
    public List<Circle> circles;
    public List<Area> areas;
    public List<Point> points;
    public Component_Overlap_Data()
    {
        nickname = "Overlap_Data";
        frames = new List<int>();
        fillColor = Color.cyan;
        lineColor = Color.red;
        circles = new List<Circle>();
        areas = new List<Area>();
        points = new List<Point>();
    }
    public void CopyToNew(Component_Overlap_Data copy)
    {
        copy.nickname = nickname;
        copy.frames = new List<int>();
        for (int i = 0; i < frames.Count; i++)
        {
            copy.frames.Add(frames[i]);
        }
        copy.fillColor = fillColor;
        copy.lineColor = lineColor;
        copy.layerMask = layerMask;
        copy.circles = new List<Circle>();
        for (int i = 0; i < circles.Count; i++)
        {
            Circle copyCircle = new Circle();
            circles[i].CopyToNew(copyCircle);
            copy.circles.Add(copyCircle);
        }
        copy.areas = new List<Area>();
        for (int i = 0; i < areas.Count; i++)
        {
            Area copyArea = new Area();
            areas[i].CopyToNew(copyArea);
            copy.areas.Add(copyArea);
        }
        copy.points = new List<Point>();
        for (int i = 0; i < points.Count; i++)
        {
            Point copyPoint = new Point();
            points[i].CopyToNew(copyPoint);
            copy.points.Add(copyPoint);
        }
    }
    public string GetAppliedFramesLabel()
    {
        string appliedFramesLabel = "";
        if (frames == null || frames.Count == 0)
            return "NONE";
        for (int i = 0; i < frames.Count; i++)
        {
            if (i == 0)
            {
                appliedFramesLabel = appliedFramesLabel + frames[i];
            }
            else
            {
                appliedFramesLabel = appliedFramesLabel + ", " + frames[i];
            }
        }
        return appliedFramesLabel;
    }
}
public class Component_Overlap_CreateNew : EditorWindow
{
    GameObject animatorObject;
    Animator animator;
    SpriteRenderer spriteRenderer;
    //
    AnimatorController controller;
    AnimationClip clip;
    bool stateExistsInController;
    //int frame;
    string stateName;
    //
    Component_Overlap componentSelected;
    //
    Component_Overlap_Data newComponentData;
    //
    int geometryTab;
    SelectionList<Circle> selectedCircles;
    ListSelectionAndEdit<Circle> circleEdit;
    SelectionList<Area> selectedAreas;
    ListSelectionAndEdit<Area> areaEdit;
    SelectionList<Point> selectedPoints;
    ListSelectionAndEdit<Point> pointEdit;
    //
    FrameEdit frameEdit;
    [MenuItem("Window/Component Editors/Component_Overlap/CreateNew")]
    public static void ShowWindow()
    {
        Component_Overlap_CreateNew editorWindow = GetWindow<Component_Overlap_CreateNew>();
        editorWindow.Init();
    }
    void Init()
    {
        animatorObject = new GameObject("EDITOR_ANIMATOR");
        animator = animatorObject.AddComponent<Animator>();
        spriteRenderer = animatorObject.AddComponent<SpriteRenderer>();
        Selection.activeObject = animatorObject;
        frameEdit = new FrameEdit();
        circleEdit = new ListSelectionAndEdit<Circle>();
        areaEdit = new ListSelectionAndEdit<Area>();
        pointEdit = new ListSelectionAndEdit<Point>();
    }
    void ControllerAndStateSelection()
    {
        bool newControllerSelected = ControllerAndStateEdit.ControllerSelect(ref controller);
        if (newControllerSelected)
        {
            stateExistsInController = false;
            clip = null;
            frameEdit.frame = 0;
            componentSelected = null;
        }
        if (controller == null)
            return;
        animator.runtimeAnimatorController = controller;
        bool newStateSelected = ControllerAndStateEdit.StateSelect(ref stateName);
        if (newStateSelected)
        {
            stateExistsInController = controller.CheckStateIsInController(stateName);
            clip = controller.ClipFromStateHash(Animator.StringToHash(stateName));
            frameEdit.frame = 0;
            componentSelected = null;
        }
    }
    void TryLoadComponentAsset()
    {
        if (!stateExistsInController)
            return;
        if (componentSelected != null)
            return;
        componentSelected = Assets<Component_Overlap>.LoadExistingAsset(controller, stateName);
        if (componentSelected == null)
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label($"No {typeof(Component_Overlap).Name} exists for state {stateName}", EditorStyles.boldLabel);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            if (GUILayout.Button($"CREATE {controller.name}_{stateName}_{typeof(Component_Overlap).Name}.asset"))
            {
                componentSelected = Assets<Component_Overlap>.CreateNewAsset(controller, stateName);
                componentSelected.stateHash = Animator.StringToHash(stateName);
            }
        }
    }
    void FrameEdit()
    {
        frameEdit.FrameSelect(stateExistsInController, clip);
        if (componentSelected == null)
            return;
        if (newComponentData == null)
            return;
        frameEdit.AppliedFramesEdit(ref newComponentData.frames);
    }
    void DataEdit()
    {
        if (componentSelected == null)
            return;
        if (newComponentData == null)
        {
            newComponentData = new Component_Overlap_Data();
            selectedCircles = new SelectionList<Circle>();
            selectedAreas = new SelectionList<Area>();
            selectedPoints = new SelectionList<Point>();
        }
        if (newComponentData == null)
            return;
        newComponentData.nickname = EditorGUILayout.TextField("Nickname", newComponentData.nickname);
        newComponentData.fillColor = EditorGUILayout.ColorField("Fill Color", newComponentData.fillColor);
        newComponentData.lineColor = EditorGUILayout.ColorField("Line Color", newComponentData.lineColor);
        newComponentData.layerMask = EditorGUICustomField.LayerMaskField("LayerMask", newComponentData.layerMask);
        //Geometry Stuff
        geometryTab = GUILayout.Toolbar(geometryTab, new string[] { "Circle", "Area", "Point"});
        switch(geometryTab)
        {
            case 0:
                circleEdit.ListEdit(ref selectedCircles, ref newComponentData.circles);
                break;
            case 1:
                areaEdit.ListEdit(ref selectedAreas, ref newComponentData.areas);
                break;
            case 2:
                pointEdit.ListEdit(ref selectedPoints, ref newComponentData.points);
                break;
        }
    }
    void DataSave()
    {
        if (componentSelected == null)
            return;
        if (newComponentData == null)
            return;
        if (GUILayout.Button("SAVE COMPONENT DATA"))
        {
            componentSelected.AddComponentData(newComponentData);
        }
    }
    private void OnFocus()
    {
        SceneView.duringSceneGui -= OnSceneGUI;
        SceneView.duringSceneGui += OnSceneGUI;
    }
    private void OnDestroy()
    {
        SceneView.duringSceneGui -= OnSceneGUI;
        if (animatorObject != null)
            DestroyImmediate(animatorObject);
        if (AnimationMode.InAnimationMode())
            AnimationMode.StopAnimationMode();
    }
    private void OnGUI()
    {
        ControllerAndStateSelection();
        TryLoadComponentAsset();
        FrameEdit();
        DataEdit();
        DataSave();
    }
    void OnSceneGUI(SceneView sceneView)
    {
        if (newComponentData == null)
            return;
        DrawGeometry.DrawAreaList(animatorObject, ref selectedAreas, ref newComponentData.areas, newComponentData.fillColor, newComponentData.lineColor);
    }
    private void Update()
    {
        if (!stateExistsInController)
            return;
        if(!EditorApplication.isPlaying && AnimationMode.InAnimationMode())
        {
            if (clip == null)
                return;
            AnimationMode.BeginSampling();
            AnimationMode.SampleAnimationClip(animatorObject, clip, frameEdit.frame);
            AnimationMode.EndSampling();
            SceneView.RepaintAll();
        }
    }
}
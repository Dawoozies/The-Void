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
using GameManagement;
[Serializable]
public class Component_Overlap : ScriptableObject, IComponent
{
    public string controllerName;
    public int stateHash;
    public Component_Overlap_Data[] componentData;
    public void AddComponentData(Component_Overlap_Data data)
    {
        if (componentData == null)
        {
            componentData = new Component_Overlap_Data[1];
            componentData[0] = new Component_Overlap_Data();
            data.CopyToNew(componentData[0]);
            EditorUtility.SetDirty(this);
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
        //
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

    public (string, int) GetControllerNameAndStateHash()
    {
        return (controllerName, stateHash);
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
    public float holdTime;
    public bool useNullResult;
    public Color fillColor;
    public Color lineColor;
    public LayerMask layerMask;
    public List<Circle> circles;
    public List<Area> areas;
    public Component_Overlap_Data()
    {
        nickname = "Overlap_Data";
        frames = new List<int>();
        fillColor = Color.cyan;
        lineColor = Color.red;
        circles = new List<Circle>();
        areas = new List<Area>();
        holdTime = 1f;
        useNullResult = true;
    }
    public ContactFilter2D ContactFilter()
    {
        ContactFilter2D contactFilter2D = new ContactFilter2D();
        contactFilter2D.SetLayerMask(layerMask);
        contactFilter2D.useTriggers = true;
        return contactFilter2D;
    }
    public void CopyToNew(Component_Overlap_Data copy)
    {
        copy.nickname = nickname;
        copy.frames = new List<int>();
        copy.holdTime = holdTime;
        copy.useNullResult = useNullResult;
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
                componentSelected.controllerName = controller.name;
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
        }
        if (newComponentData == null)
            return;
        newComponentData.nickname = EditorGUILayout.TextField("Nickname", newComponentData.nickname);
        newComponentData.holdTime = EditorGUILayout.FloatField("Hold Time", newComponentData.holdTime);
        newComponentData.useNullResult = EditorGUILayout.Toggle("Use Null Result", newComponentData.useNullResult);
        if (newComponentData.holdTime < 0)
            newComponentData.holdTime = 0;
        if (newComponentData.holdTime > 1)
            newComponentData.holdTime = 1;
        newComponentData.fillColor = EditorGUILayout.ColorField("Fill Color", newComponentData.fillColor);
        newComponentData.lineColor = EditorGUILayout.ColorField("Line Color", newComponentData.lineColor);
        newComponentData.layerMask = EditorGUICustomField.LayerMaskField("LayerMask", newComponentData.layerMask);
        //Geometry Stuff
        geometryTab = GUILayout.Toolbar(geometryTab, new string[] { "Circle", "Area"});
        switch(geometryTab)
        {
            case 0:
                circleEdit.ListEdit(ref selectedCircles, ref newComponentData.circles);
                break;
            case 1:
                areaEdit.ListEdit(ref selectedAreas, ref newComponentData.areas);
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
        switch(geometryTab)
        {
            case 0:
                DrawGeometry.DrawCircleList(animatorObject, ref selectedCircles, ref newComponentData.circles, newComponentData.fillColor, newComponentData.lineColor);
                break;
            case 1:
                DrawGeometry.DrawAreaList(animatorObject, ref selectedAreas, ref newComponentData.areas, newComponentData.fillColor, newComponentData.lineColor);
                break;
        }
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
public class Component_Overlap_Edit : EditorWindow
{
    GameObject animatorObject;
    Animator animator;
    SpriteRenderer spriteRenderer;
    //
    AnimatorController controller;
    AnimationClip clip;
    bool stateExistsInController;
    FrameEdit frameEdit;
    string stateName;
    //
    Component_Overlap componentSelected;
    //
    Component_Overlap_Data[] dataAtFrame;
    Component_Overlap_Data dataSelected;
    //
    int geometryTab;
    SelectionList<Circle> selectedCircles;
    ListSelectionAndEdit<Circle> circleEdit;
    SelectionList<Area> selectedAreas;
    ListSelectionAndEdit<Area> areaEdit;
    //
    int dataFilterMode;
    bool deleteWarning;
    [MenuItem("Window/Component Editors/Component_Overlap/Edit")]
    public static void ShowWindow()
    {
        Component_Overlap_Edit editorWindow = GetWindow<Component_Overlap_Edit>();
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
                componentSelected.controllerName = controller.name;
                componentSelected.stateHash = Animator.StringToHash(stateName);
            }
        }
    }
    void FrameEdit()
    {
        frameEdit.FrameSelect(stateExistsInController, clip);
        if (componentSelected == null)
            return;
        if (dataSelected == null)
            return;
        frameEdit.AppliedFramesEdit(ref dataSelected.frames);
    }
    void DataDeleteChoice()
    {
        if (!deleteWarning)
            return;
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.Label("COMPONENT DATA WILL BE DELETED!", EditorStyles.boldLabel);
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.Label("CONTINUE?", EditorStyles.largeLabel);
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("YES"))
        {
            componentSelected.RemoveComponentData(dataSelected);
            dataSelected = null;
            deleteWarning = false;
        }
        if (GUILayout.Button("NO"))
        {
            deleteWarning = false;
        }
        GUILayout.EndHorizontal();
    }
    void ChangeData()
    {
        if (dataSelected == null)
            return;
        if (deleteWarning)
            return;
        if (GUILayout.Button("CHOOSE DIFFERENT DATA"))
        {
            if (dataSelected.frames == null || dataSelected.frames.Count == 0)
            {
                deleteWarning = true;
                return;
            }
            else
            {
                dataSelected = null;
            }
        }
    }
    void ComponentDataSelectAll()
    {
        if (componentSelected == null)
            return;
        if (componentSelected.componentData == null || componentSelected.componentData.Length == 0)
            return;
        if (dataSelected == null)
        {
            for (int i = 0; i < componentSelected.componentData.Length; i++)
            {
                Component_Overlap_Data data = componentSelected.componentData[i];
                if (GUILayout.Button($"Component_Data: {data.nickname} [ Applied Frames: {data.GetAppliedFramesLabel()} ]"))
                {
                    dataSelected = data;
                }
            }
        }
    }
    void ComponentDataSelectByFrame()
    {
        if (componentSelected == null)
            return;
        if (componentSelected.componentData == null || componentSelected.componentData.Length == 0)
            return;
        dataAtFrame = componentSelected.DataWithFrame(frameEdit.frame);
        if (dataAtFrame == null || dataAtFrame.Length == 0)
        {
            GUILayout.Label("NO DATA AT FRAME", EditorStyles.boldLabel);
            return;
        }
        if (dataSelected == null)
        {
            for (int i = 0; i < dataAtFrame.Length; i++)
            {
                Component_Overlap_Data data = dataAtFrame[i];
                if (GUILayout.Button($"Component_Data: {data.nickname} [ Applied Frames: {data.GetAppliedFramesLabel()} ]"))
                {
                    dataSelected = data;
                }
            }
        }
    }
    void ComponentDataEdit()
    {
        if (dataSelected == null)
            return;
        if (selectedCircles == null)
            selectedCircles = new SelectionList<Circle>();
        if (selectedAreas == null)
            selectedAreas = new SelectionList<Area>();
        dataSelected.nickname = EditorGUILayout.TextField("Nickname", dataSelected.nickname);
        dataSelected.holdTime = EditorGUILayout.FloatField("Hold Time", dataSelected.holdTime);
        dataSelected.useNullResult = EditorGUILayout.Toggle("Use Null Result", dataSelected.useNullResult);
        if (dataSelected.holdTime < 0)
            dataSelected.holdTime = 0;
        if (dataSelected.holdTime > 1)
            dataSelected.holdTime = 1;
        dataSelected.fillColor = EditorGUILayout.ColorField("Fill Color", dataSelected.fillColor);
        dataSelected.lineColor = EditorGUILayout.ColorField("Line Color", dataSelected.lineColor);
        dataSelected.layerMask = EditorGUICustomField.LayerMaskField("LayerMask", dataSelected.layerMask);
        //Geometry Stuff
        geometryTab = GUILayout.Toolbar(geometryTab, new string[] { "Circle", "Area" });
        switch (geometryTab)
        {
            case 0:
                circleEdit.ListEdit(ref selectedCircles, ref dataSelected.circles);
                break;
            case 1:
                areaEdit.ListEdit(ref selectedAreas, ref dataSelected.areas);
                break;
        }
        EditorUtility.SetDirty(componentSelected);
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
        DataDeleteChoice();
        ChangeData();
        if (deleteWarning)
            return;
        dataFilterMode = GUILayout.Toolbar(dataFilterMode, new string[] { "By Frame", "All"});
        switch (dataFilterMode)
        {
            case 0:
                ComponentDataSelectByFrame();
                break;
            case 1:
                ComponentDataSelectAll();
                break;
        }
        ComponentDataEdit();
    }
    void OnSceneGUI(SceneView sceneView)
    {
        if (dataSelected == null)
            return;
        switch (geometryTab)
        {
            case 0:
                DrawGeometry.DrawCircleList(animatorObject, ref selectedCircles, ref dataSelected.circles, dataSelected.fillColor, dataSelected.lineColor);
                break;
            case 1:
                DrawGeometry.DrawAreaList(animatorObject, ref selectedAreas, ref dataSelected.areas, dataSelected.fillColor, dataSelected.lineColor);
                break;
        }
    }
    private void Update()
    {
        if (!stateExistsInController)
            return;
        if (!EditorApplication.isPlaying && AnimationMode.InAnimationMode())
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
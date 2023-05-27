using System;
using System.Collections;
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
public class Component_CircleCollider2D : ScriptableObject, IComponent
{
    //Only one Component per state
    public string controllerName;
    public int stateHash;
    public Component_CircleCollider2D_Data[] componentData;
    public void AddComponentData(Component_CircleCollider2D_Data data)
    {
        if (componentData == null)
        {
            componentData = new Component_CircleCollider2D_Data[1];
            componentData[0] = new Component_CircleCollider2D_Data();
            data.CopyToNew(componentData[0]);
            EditorUtility.SetDirty(this);
            return;
        }
            
        Component_CircleCollider2D_Data[] resizedData = new Component_CircleCollider2D_Data[componentData.Length + 1];
        for (int i = 0; i < resizedData.Length; i++)
        {
            if(i < componentData.Length)
            {
                resizedData[i] = new Component_CircleCollider2D_Data();
                componentData[i].CopyToNew(resizedData[i]);
            }
            if (i == resizedData.Length - 1)
            {
                resizedData[i] = new Component_CircleCollider2D_Data();
                data.CopyToNew(resizedData[i]);
            }
        }
        componentData = resizedData;
        EditorUtility.SetDirty(this);
    }
    public Component_CircleCollider2D_Data[] DataWithFrame(int frame)
    {
        if (componentData == null || componentData.Length == 0)
            return null;
        List<Component_CircleCollider2D_Data> dataAtFrame = new List<Component_CircleCollider2D_Data>();
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

    public void RemoveComponentData(Component_CircleCollider2D_Data dataToRemove)
    {
        if (componentData == null || componentData.Length == 0)
            return;
        if (!componentData.Contains(dataToRemove))
            return;
        Component_CircleCollider2D_Data[] resizedData = new Component_CircleCollider2D_Data[componentData.Length - 1];
        int shift = 0;
        for (int i = 0; i < resizedData.Length; i++)
        {
            if (componentData[i] == dataToRemove)
            {
                shift++;
            }
            resizedData[i] = new Component_CircleCollider2D_Data();
            componentData[i+shift].CopyToNew(resizedData[i]);
        }
        componentData = resizedData;
        EditorUtility.SetDirty(this);
    }
}
[Serializable]
public class Component_CircleCollider2D_Data
{
    public string nickname;
    public List<int> frames; //Frames this data will be used
    public Color fillColor;
    public Color lineColor;
    public bool isTrigger;
    public int layer;
    public List<Circle> circles;
    public Component_CircleCollider2D_Data()
    {
        nickname = "CircleCollider2D_Data";
        frames = new List<int>();
        fillColor = Color.yellow;
        lineColor = Color.red;
        isTrigger = false;
        layer = 0;
        circles = new List<Circle>();
    }
    public void CopyToNew(Component_CircleCollider2D_Data copy)
    {
        copy.nickname = nickname;
        copy.frames = new List<int>();
        for (int i = 0; i < frames.Count; i++)
        {
            //Int is value type so no need to make new copy of that
            copy.frames.Add(frames[i]);
        }
        copy.fillColor = fillColor;
        copy.lineColor = lineColor;
        copy.isTrigger = isTrigger;
        copy.layer = layer;
        copy.circles = new List<Circle>();
        for (int i = 0; i < circles.Count; i++)
        {
            Circle copyCircle = new Circle();
            circles[i].CopyToNew(copyCircle);
            copy.circles.Add(copyCircle);
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
//EDITOR WINDOW FOR THIS COMPONENT
public class Component_CircleCollider2D_CreateNew : EditorWindow
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
    Component_CircleCollider2D componentSelected;
    //
    Component_CircleCollider2D_Data newComponentData;
    SelectionList<Circle> selectedCircles;
    ListSelectionAndEdit<Circle> circleEdit;
    //
    FrameEdit frameEdit;
    //3 Modes
    //Create New Mode - Have a set static component data which you change and then "Assign" to Components
    //  "Assign" just means create a value copy of the data and then push that to the state component
    //Edit Component Mode - Load All Components from a given state hash, then look through components and edit
    //  components or delete them. Works like current system we have which is editing existing data
    //Link/Copy/Reference Mode - This is specifically for Component Data sets which are shared among many 
    //  variant states, the states don't actually have their "own" data. A data set is created which is not
    //  linked to any specific state.
    [MenuItem("Window/Component Editors/Component_CircleCollider2D/CreateNew")]
    public static void ShowWindow()
    {
        Component_CircleCollider2D_CreateNew editorWindow = GetWindow<Component_CircleCollider2D_CreateNew>();
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
        componentSelected = Assets<Component_CircleCollider2D>.LoadExistingAsset(controller, stateName);
        if(componentSelected == null)
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label($"No {typeof(Component_CircleCollider2D).Name} exists for state {stateName}", EditorStyles.boldLabel);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            if(GUILayout.Button($"CREATE {controller.name}_{stateName}_{typeof(Component_CircleCollider2D).Name}.asset"))
            {
                componentSelected = Assets<Component_CircleCollider2D>.CreateNewAsset(controller, stateName);
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
        if(newComponentData == null)
        {
            newComponentData = new Component_CircleCollider2D_Data();
            selectedCircles = new SelectionList<Circle>();
        }
        if (newComponentData == null)
            return;
        newComponentData.nickname = EditorGUILayout.TextField("Nickname", newComponentData.nickname);
        newComponentData.fillColor = EditorGUILayout.ColorField("Fill Color", newComponentData.fillColor);
        newComponentData.lineColor = EditorGUILayout.ColorField("Line Color", newComponentData.lineColor);
        newComponentData.isTrigger = EditorGUILayout.Toggle("Is Trigger", newComponentData.isTrigger);
        newComponentData.layer = EditorGUILayout.LayerField("Layer", newComponentData.layer);
        //Circle Stuff
        circleEdit.ListEdit(ref selectedCircles, ref newComponentData.circles);
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
        DrawGeometry.DrawCircleList(animatorObject, ref selectedCircles, ref newComponentData.circles, newComponentData.fillColor, newComponentData.lineColor);
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
public class Component_CircleCollider2D_Edit : EditorWindow
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
    Component_CircleCollider2D componentSelected;
    //
    Component_CircleCollider2D_Data[] dataAtFrame;
    Component_CircleCollider2D_Data dataSelected;
    SelectionList<Circle> selectedCircles;
    ListSelectionAndEdit<Circle> circleEdit;
    //
    int dataFilterMode;//Basically ignore frame and show all component data
    bool deleteWarning;
    [MenuItem("Window/Component Editors/Component_CircleCollider2D/Edit")]
    public static void ShowWindow()
    {
        Component_CircleCollider2D_Edit editorWindow = GetWindow<Component_CircleCollider2D_Edit>();
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
        componentSelected = Assets<Component_CircleCollider2D>.LoadExistingAsset(controller, stateName);
        if (componentSelected == null)
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label($"No {typeof(Component_CircleCollider2D).Name} exists for state {stateName}", EditorStyles.boldLabel);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            if (GUILayout.Button($"CREATE {controller.name}_{stateName}_{typeof(Component_CircleCollider2D).Name}.asset"))
            {
                componentSelected = Assets<Component_CircleCollider2D>.CreateNewAsset(controller, stateName);
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
        if(GUILayout.Button("YES"))
        {
            componentSelected.RemoveComponentData(dataSelected);
            dataSelected = null;
            deleteWarning = false;
        }
        if(GUILayout.Button("NO"))
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
        if(GUILayout.Button("CHOOSE DIFFERENT DATA"))
        {
            if(dataSelected.frames == null || dataSelected.frames.Count == 0)
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
                Component_CircleCollider2D_Data data = componentSelected.componentData[i];
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
                Component_CircleCollider2D_Data data = dataAtFrame[i];
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
        dataSelected.nickname = EditorGUILayout.TextField("Nickname", dataSelected.nickname);
        dataSelected.fillColor = EditorGUILayout.ColorField("Fill Color", dataSelected.fillColor);
        dataSelected.lineColor = EditorGUILayout.ColorField("Line Color", dataSelected.lineColor);
        dataSelected.isTrigger = EditorGUILayout.Toggle("Is Trigger", dataSelected.isTrigger);
        dataSelected.layer = EditorGUILayout.LayerField("Layer", dataSelected.layer);
        //CIRCLE STUFF
        circleEdit.ListEdit(ref selectedCircles, ref dataSelected.circles);
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
        deleteWarning = false;
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
        dataFilterMode = GUILayout.Toolbar(dataFilterMode, new string[] { "By Frame", "All" });
        switch(dataFilterMode)
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
        DrawGeometry.DrawCircleList(animatorObject, ref selectedCircles, ref dataSelected.circles, dataSelected.fillColor, dataSelected.lineColor);
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
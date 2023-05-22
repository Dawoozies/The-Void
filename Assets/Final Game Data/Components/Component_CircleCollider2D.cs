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
using ExtensionMethods_Color;

[Serializable]
public class Component_CircleCollider2D : ScriptableObject
{
    //Only one Component per state
    public int stateHash;
    public Component_CircleCollider2D_Data[] componentData;
    public void AddComponentData(Component_CircleCollider2D_Data data)
    {
        if (componentData == null)
        {
            componentData = new Component_CircleCollider2D_Data[1];
            componentData[0] = new Component_CircleCollider2D_Data();
            data.CopyToNew(componentData[0]);
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
    int frame;
    string stateName;
    //
    Component_CircleCollider2D componentSelected;
    //
    Component_CircleCollider2D_Data newComponentData;
    SelectionList<Circle> selectedCircles;
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
        editorWindow.CreateAnimatorObject();
    }
    void CreateAnimatorObject()
    {
        animatorObject = new GameObject("EDITOR_ANIMATOR");
        animator = animatorObject.AddComponent<Animator>();
        spriteRenderer = animatorObject.AddComponent<SpriteRenderer>();
        Selection.activeObject = animatorObject;
    }
    void AnimatorControllerSelection()
    {
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.Label("CONTROLLER", EditorStyles.largeLabel);
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        AnimatorController fieldInput = (AnimatorController)EditorGUILayout.ObjectField(controller, typeof(AnimatorController), false, GUILayout.ExpandWidth(true));
        if (fieldInput != controller)
        {
            controller = fieldInput;
            stateExistsInController = false;
            clip = null;
            stateExistsInController = false;
            frame = 0;
            componentSelected = null;
        }
        if (controller == null)
            return;
        animator.runtimeAnimatorController = controller as RuntimeAnimatorController;
    }
    void StateSelect()
    {
        if (controller == null)
            return;
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.Label("STATE", EditorStyles.largeLabel);
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        string fieldInput = EditorGUILayout.TextField(stateName);
        if (fieldInput != stateName)
        {
            stateName = fieldInput;
            stateExistsInController = controller.CheckStateIsInController(stateName);
            clip = controller.ClipFromStateHash(Animator.StringToHash(stateName));
            frame = 0;
            componentSelected = null;
        }
    }
    void FrameSelect()
    {
        if (!stateExistsInController)
            return;
        AnimationMode.StartAnimationMode();
        GUILayout.BeginHorizontal();
        if(GUILayout.Button(" < ") && frame > 0)
        {
            frame--;
        }
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.Label($"FRAME {frame}", EditorStyles.boldLabel);
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        if(GUILayout.Button(" > ") && frame < clip.length)
        {
            frame++;
        }
        GUILayout.EndHorizontal();
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
                componentSelected.stateHash = Animator.StringToHash(stateName);
            }
        }
    }
    void FrameEdit()
    {
        if (componentSelected == null)
            return;
        if (newComponentData == null)
            return;
        if(newComponentData.frames == null || newComponentData.frames.Count == 0)
        {
            if (GUILayout.Button($"APPLY TO FRAME {frame}"))
            {
                newComponentData.frames = new List<int>() { frame };
            }
            return;
        }
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.Label($"APPLIED TO FRAMES: {newComponentData.GetAppliedFramesLabel()}", EditorStyles.boldLabel);
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        if (newComponentData.frames.Contains(frame))
        {
            if(GUILayout.Button($"REMOVE FROM FRAME {frame}"))
            {
                newComponentData.frames.Remove(frame);
            }
        }
        else
        {
            if(GUILayout.Button($"APPLY TO FRAME {frame}"))
            {
                newComponentData.frames.Add(frame);
            }
        }
        if(GUILayout.Button("CLEAR ALL APPLIED FRAMES"))
        {
            newComponentData.frames.Clear();
        }
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
        GUILayout.Label($"CIRCLE SELECTION MODE {selectedCircles.selectionMode.ToString().ToUpper()}", EditorStyles.boldLabel);
        GUILayout.BeginHorizontal();
        foreach (GeometrySelectionMode mode in Enum.GetValues(typeof(GeometrySelectionMode)))
        {
            if(GUILayout.Button(mode.ToString()))
            {
                selectedCircles.selectionMode = mode;
            }
        }
        if(GUILayout.Button("All"))
        {
            selectedCircles.selectionMode = GeometrySelectionMode.Multiple;
            foreach (Circle circle in newComponentData.circles)
            {
                if (selectedCircles.Contains(circle))
                    continue;
                selectedCircles.Add(circle);
            }
        }
        GUILayout.EndHorizontal();
        if (GUILayout.Button("ADD CIRCLE"))
        {
            newComponentData.circles.Add(new Circle());
        }
        if (newComponentData.circles.Count == 0)
            return;
        for (int i = 0; i < newComponentData.circles.Count; i++)
        {
            Circle circle = newComponentData.circles[i];
            string selectedLabel = (selectedCircles.Contains(circle)) ? "[S] " : "" ;
            GUILayout.BeginHorizontal();
            if (GUILayout.Button($"{selectedLabel}Circle {i}"))
            {
                if(selectedCircles.Contains(circle))
                {
                    selectedCircles.Remove(circle);
                }
                else
                {
                    selectedCircles.Add(circle);
                }
            }
            if (GUILayout.Button($"REMOVE {i}"))
            {
                if (selectedCircles != null && selectedCircles.Contains(circle))
                    selectedCircles.Remove(circle);
                newComponentData.circles.Remove(circle);
                GUILayout.EndHorizontal();
                return;
            }
            GUILayout.EndHorizontal();
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
        AnimatorControllerSelection();
        StateSelect();
        FrameSelect();
        TryLoadComponentAsset();
        FrameEdit();
        DataEdit();
        DataSave();
    }
    void OnSceneGUI(SceneView sceneView)
    {
        if (newComponentData == null || newComponentData.circles == null || newComponentData.circles.Count == 0)
            return;
        for (int i = 0; i < newComponentData.circles.Count; i++)
        {
            Circle circle = newComponentData.circles[i];
            DrawCircle(circle, selectedCircles.Contains(circle));
        }
        if (selectedCircles == null || selectedCircles.selectedItems == null || selectedCircles.selectedItems.Count == 0)
            return;
        DrawHandleForSelection(selectedCircles.selectedItems[0]);
    }
    void DrawCircle(Circle circle, bool isSelected)
    {
        Vector3 worldPos = animatorObject.transform.position + circle.center;
        float radius = circle.radius;
        float alpha = 0.35f;
        if (isSelected)
            alpha = 1f;
        Handles.color = newComponentData.fillColor.WithTransparency(alpha);
        Handles.DrawSolidDisc(worldPos, Vector3.forward, radius);
        Handles.color = Color.black;
        Handles.DrawWireDisc(worldPos, Vector3.forward, radius, 4f);
        Handles.color = newComponentData.lineColor.WithTransparency(alpha);
        Handles.DrawDottedLine(worldPos, worldPos + Vector3.right * radius, 3f);
    }
    void DrawHandleForSelection(Circle circle)
    {
        Vector3 worldPos = animatorObject.transform.position + circle.center;
        float radius = circle.radius;
        EditorGUI.BeginChangeCheck();
        Handles.color = newComponentData.lineColor;
        Vector3 oldArrowPos = worldPos + Vector3.right * radius;
        Vector3 newArrowPos = Handles.Slider(oldArrowPos, Vector3.right, 0.75f, Handles.ArrowHandleCap, 0.1f);
        Handles.color = Color.white;
        Vector3 oldSquarePos = worldPos;
        Vector3 newSquarePos = Handles.FreeMoveHandle(oldSquarePos, Quaternion.identity, 0.35f, Vector3.one * 0.1f, Handles.RectangleHandleCap);
        if(EditorGUI.EndChangeCheck())
        {
            foreach (Circle selectionCircle in selectedCircles.selectedItems)
            {
                selectionCircle.radius += newArrowPos.x - oldArrowPos.x;
                if (selectionCircle.radius < 0)
                    selectionCircle.radius = 0;
                selectionCircle.center += newSquarePos - oldSquarePos;
            }
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
            AnimationMode.SampleAnimationClip(animatorObject, clip, frame);
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
    int frame;
    string stateName;
    //
    bool useCurrentFrameChoice = true;
    //
    Component_CircleCollider2D componentSelected;
    //
    Component_CircleCollider2D_Data[] dataAtFrame;
    Component_CircleCollider2D_Data dataSelected;
    SelectionList<Circle> selectedCircles;
    //
    bool deleteWarning;
    [MenuItem("Window/Component Editors/Component_CircleCollider2D/Edit")]
    public static void ShowWindow()
    {
        Component_CircleCollider2D_Edit editorWindow = GetWindow<Component_CircleCollider2D_Edit>();
        editorWindow.CreateAnimatorObject();
    }
    void CreateAnimatorObject()
    {
        animatorObject = new GameObject("EDITOR_ANIMATOR");
        animator = animatorObject.AddComponent<Animator>();
        spriteRenderer = animatorObject.AddComponent<SpriteRenderer>();
        Selection.activeObject = animatorObject;
    }
    void AnimatorControllerSelection()
    {
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.Label("CONTROLLER", EditorStyles.largeLabel);
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        AnimatorController fieldInput = (AnimatorController)EditorGUILayout.ObjectField(controller, typeof(AnimatorController), false, GUILayout.ExpandWidth(true));
        if (fieldInput != controller)
        {
            controller = fieldInput;
            stateExistsInController = false;
            clip = null;
            stateExistsInController = false;
            frame = 0;
            componentSelected = null;
        }
        if (controller == null)
            return;
        animator.runtimeAnimatorController = controller as RuntimeAnimatorController;
    }
    void StateSelect()
    {
        if (controller == null)
            return;
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.Label("STATE", EditorStyles.largeLabel);
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        string fieldInput = EditorGUILayout.TextField(stateName);
        if (fieldInput != stateName)
        {
            stateName = fieldInput;
            stateExistsInController = controller.CheckStateIsInController(stateName);
            clip = controller.ClipFromStateHash(Animator.StringToHash(stateName));
            frame = 0;
            componentSelected = null;
        }
    }
    void FrameSelect()
    {
        if (!stateExistsInController)
            return;
        AnimationMode.StartAnimationMode();
        GUILayout.BeginHorizontal();
        if (GUILayout.Button(" < ") && frame > 0)
        {
            frame--;
        }
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.Label($"FRAME {frame}", EditorStyles.boldLabel);
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        if (GUILayout.Button(" > ") && frame < clip.length)
        {
            frame++;
        }
        GUILayout.EndHorizontal();
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
                componentSelected.stateHash = Animator.StringToHash(stateName);
            }
        }
    }
    void FrameEdit()
    {
        if (componentSelected == null)
            return;
        if (dataSelected == null)
            return;
        if (dataSelected.frames == null || dataSelected.frames.Count == 0)
        {
            if (GUILayout.Button($"APPLY TO FRAME {frame}"))
            {
                dataSelected.frames = new List<int>() { frame };
            }
            return;
        }
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.Label($"APPLIED TO FRAMES: {dataSelected.GetAppliedFramesLabel()}", EditorStyles.boldLabel);
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        if (dataSelected.frames.Contains(frame))
        {
            if (GUILayout.Button($"REMOVE FROM FRAME {frame}"))
            {
                dataSelected.frames.Remove(frame);
            }
        }
        else
        {
            if (GUILayout.Button($"APPLY TO FRAME {frame}"))
            {
                dataSelected.frames.Add(frame);
            }
        }
        if (GUILayout.Button("CLEAR ALL APPLIED FRAMES"))
        {
            dataSelected.frames.Clear();
        }
    }
    void ComponentDataSelect()
    {
        if (componentSelected == null)
            return;
        if (componentSelected.componentData == null || componentSelected.componentData.Length == 0)
            return;
        if (dataSelected != null)
        {
            if (deleteWarning)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("COMPONENT DATA WILL BE DELETED! CONTINUE?");
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
                return;
            }
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
        dataAtFrame = componentSelected.DataWithFrame(frame);
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
        GUILayout.Label($"CIRCLE SELECTION MODE {selectedCircles.selectionMode.ToString().ToUpper()}", EditorStyles.boldLabel);
        GUILayout.BeginHorizontal();
        foreach (GeometrySelectionMode mode in Enum.GetValues(typeof(GeometrySelectionMode)))
        {
            if (GUILayout.Button(mode.ToString()))
            {
                selectedCircles.selectionMode = mode;
            }
        }
        if (GUILayout.Button("All"))
        {
            selectedCircles.selectionMode = GeometrySelectionMode.Multiple;
            foreach (Circle circle in dataSelected.circles)
            {
                if (selectedCircles.Contains(circle))
                    continue;
                selectedCircles.Add(circle);
            }
        }
        GUILayout.EndHorizontal();
        if (GUILayout.Button("ADD CIRCLE"))
        {
            dataSelected.circles.Add(new Circle());
        }
        if (dataSelected.circles.Count == 0)
            return;
        for (int i = 0; i < dataSelected.circles.Count; i++)
        {
            Circle circle = dataSelected.circles[i];
            string selectedLabel = (selectedCircles.Contains(circle)) ? "[S] " : "";
            GUILayout.BeginHorizontal();
            if (GUILayout.Button($"{selectedLabel}Circle {i}"))
            {
                if (selectedCircles.Contains(circle))
                {
                    selectedCircles.Remove(circle);
                }
                else
                {
                    selectedCircles.Add(circle);
                }
            }
            if (GUILayout.Button($"REMOVE {i}"))
            {
                if (selectedCircles != null && selectedCircles.Contains(circle))
                    selectedCircles.Remove(circle);
                dataSelected.circles.Remove(circle);
                GUILayout.EndHorizontal();
                return;
            }
            GUILayout.EndHorizontal();
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
        deleteWarning = false;
    }
    private void OnGUI()
    {
        AnimatorControllerSelection();
        StateSelect();
        FrameSelect();
        TryLoadComponentAsset();
        ComponentDataSelect();
        FrameEdit();
        ComponentDataEdit();
    }
    void OnSceneGUI(SceneView sceneView)
    {
        if (dataSelected == null || dataSelected == null || dataSelected.circles.Count == 0)
            return;
        for (int i = 0; i < dataSelected.circles.Count; i++)
        {
            Circle circle = dataSelected.circles[i];
            DrawCircle(circle, selectedCircles.Contains(circle));
        }
        if (selectedCircles == null || selectedCircles.selectedItems == null || selectedCircles.selectedItems.Count == 0)
            return;
        DrawHandleForSelection(selectedCircles.selectedItems[0]);
    }
    void DrawCircle(Circle circle, bool isSelected)
    {
        Vector3 worldPos = animatorObject.transform.position + circle.center;
        float radius = circle.radius;
        float alpha = 0.35f;
        if (isSelected)
            alpha = 1f;
        Handles.color = dataSelected.fillColor.WithTransparency(alpha);
        Handles.DrawSolidDisc(worldPos, Vector3.forward, radius);
        Handles.color = Color.black;
        Handles.DrawWireDisc(worldPos, Vector3.forward, radius, 4f);
        Handles.color = dataSelected.lineColor.WithTransparency(alpha);
        Handles.DrawDottedLine(worldPos, worldPos + Vector3.right * radius, 3f);
    }
    void DrawHandleForSelection(Circle circle)
    {
        Vector3 worldPos = animatorObject.transform.position + circle.center;
        float radius = circle.radius;
        EditorGUI.BeginChangeCheck();
        Handles.color = dataSelected.lineColor;
        Vector3 oldArrowPos = worldPos + Vector3.right * radius;
        Vector3 newArrowPos = Handles.Slider(oldArrowPos, Vector3.right, 0.75f, Handles.ArrowHandleCap, 0.1f);
        Handles.color = Color.white;
        Vector3 oldSquarePos = worldPos;
        Vector3 newSquarePos = Handles.FreeMoveHandle(oldSquarePos, Quaternion.identity, 0.35f, Vector3.one * 0.1f, Handles.RectangleHandleCap);
        if (EditorGUI.EndChangeCheck())
        {
            foreach (Circle selectionCircle in selectedCircles.selectedItems)
            {
                selectionCircle.radius += newArrowPos.x - oldArrowPos.x;
                if (selectionCircle.radius < 0)
                    selectionCircle.radius = 0;
                selectionCircle.center += newSquarePos - oldSquarePos;
            }
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
            AnimationMode.SampleAnimationClip(animatorObject, clip, frame);
            AnimationMode.EndSampling();
            SceneView.RepaintAll();
        }
    }
}
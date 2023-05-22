using UnityEditor;
using UnityEngine;
using UnityEditor.Animations;
using OLD.GameData.StateData;
using System;
using System.Linq;
public class AnimatorWindow : EditorWindow
{
    private AnimationClip clip;
    private AnimatorController controller;
    private GameObject animatorObject;
    private Animator animator;
    private string stateName;
    private string inputStateName;
    private int stateHash;
    private bool stateExistsInController;
    private int frame;
    private StateData stateData;
    [MenuItem("Window/Animator Window")]
    public static void ShowWindow()
    {
        AnimatorWindow window = GetWindow<AnimatorWindow>("Animator Window");
        window.CreateAnimatorObject();
    }

    private void OnGUI()
    {
        AnimatorControllerSelection();
        if (controller == null)
            return;
        StateSelection();
        if (!stateExistsInController)
            return;
        FrameSelection();
        if (stateData == null)
        {
            StateDataCreateNew();
            return;
        }
        ComponentAdd();
        DrawComponents();
    }
    void DrawComponents()
    {
        stateData.Draw_StateDataAtFrame(frame);
    }
    void ComponentAdd()
    {
        stateData.Draw_ComponentAdd(frame);
    }
    void StateDataCreateNew()
    {
        if(GUILayout.Button("CREATE NEW STATE DATA"))
        {
            string assignedName = $"{controller.name}_{stateName}_{typeof(StateData).Name}";
            stateData = Create<StateData>.New(assignedName);
        }
            
    }
    void FrameSelection()
    {
        if (!stateExistsInController)
            return;
        AnimationMode.StartAnimationMode();
        EditorGUILayout.BeginHorizontal();
        if(GUILayout.Button(" < ") && frame > 0)
        {
            frame--;
        }
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.Label($"TIME {frame}", EditorStyles.boldLabel);
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        if (GUILayout.Button(" > ") && frame < clip.length)
        {
            frame++;
        }
        EditorGUILayout.EndHorizontal();
    }
    void StateSelection()
    {
        GUILayout.Label("STATE", EditorStyles.largeLabel);
        inputStateName = EditorGUILayout.TextField(inputStateName);
        if(inputStateName != stateName)
        {
            CheckStateIsInController(inputStateName);
            stateName = inputStateName;
            if(stateExistsInController)
                stateData = Load<StateData>.LoadExisting(controller, stateName);
        }
    }
    void CheckStateIsInController(string stateName)
    {
        if (controller == null)
            return;
        foreach (AnimatorControllerLayer layer in controller.layers)
        {
            AnimatorStateMachine stateMachine = layer.stateMachine;
            foreach (ChildAnimatorState state in stateMachine.states)
            {
                AnimatorState animatorState = state.state;
                if (Animator.StringToHash(stateName) == animatorState.nameHash)
                {
                    stateExistsInController = true;
                    stateHash = Animator.StringToHash(stateName);
                    return;
                }
            }
        }

        stateExistsInController = false;
        return;
    }
    void AnimatorControllerSelection()
    {
        GUILayout.Label("CONTROLLER", EditorStyles.largeLabel);
        AnimatorController inputController = (AnimatorController)EditorGUILayout.ObjectField(controller, typeof(AnimatorController), false, GUILayout.ExpandWidth(true));
        if(inputController != controller)
        {
            stateExistsInController = false;
            stateData = null;
            stateName = "";
            inputStateName = "";
            clip = null;
            frame = 0;
            controller = inputController;
        }
        if (controller == null)
            return;
        animator = animatorObject.GetComponent<Animator>();
        animator.runtimeAnimatorController = controller as RuntimeAnimatorController;
    }
    private void CreateAnimatorObject()
    {
        animatorObject = new GameObject("EDITOR_ANIMATOR");
        animatorObject.AddComponent<Animator>();
        animatorObject.AddComponent<SpriteRenderer>();
        Selection.activeObject = animatorObject;
    }
    private void OnDestroy()
    {
        GameObject editorAnimatorObject = GameObject.Find("EDITOR_ANIMATOR");
        if (editorAnimatorObject != null)
        {
            DestroyImmediate(editorAnimatorObject);
        }
        AnimationMode.StopAnimationMode();
        if (stateData.data == null)
            Delete<StateData>.Asset(stateData);
    }
    private void Update()
    {
        if (controller == null)
            return;
        if (animator == null)
            return;
        if (stateExistsInController)
        {
            if(!EditorApplication.isPlaying && AnimationMode.InAnimationMode())
            {
                clip = AnimatorStateUtilities.GetAnimationClipFromStateHash(controller, stateHash);
                AnimationMode.BeginSampling();
                AnimationMode.SampleAnimationClip(animatorObject, clip, frame);
                AnimationMode.EndSampling();
                SceneView.RepaintAll();
            }
        }
    }
}
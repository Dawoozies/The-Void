using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEditor.Animations;
using GameData.StateData;
using GameData.StateData.IO;
using GameData.StateData.VisualElements;
using Geometry;
using System.Collections.Generic;
public class StateDataEditor : EditorWindow
{
    private AnimatorController animatorController;
    private string stateTarget;
    private VisualElement root;
    private bool stateExistsInController;
    private Circle circle = new Circle();

    CircleCollider2DStateData circleCollider2DStateData;
    ObjectField circleCollider2DStateDataField;
    Button addCircleCollider2DStateDataButton;
    [MenuItem("Window/StateDataEditor")]
    public static void ShowExample()
    {
        StateDataEditor wnd = GetWindow<StateDataEditor>();
        wnd.titleContent = new GUIContent("StateDataEditor");
    }

    public void CreateGUI()
    {
        stateExistsInController = false;
        // Each editor window contains a root VisualElement object
        root = rootVisualElement;

        var controllerField = new ObjectField("Animator Controller");
        controllerField.objectType = typeof(AnimatorController);
        root.Add(controllerField);

        var stringField = new TextField("State");
        root.Add(stringField);

        var hashDisplay = new Label();
        hashDisplay.name = "HashDisplay";
        root.Add(hashDisplay);

        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/NewGameDataSystem/StateDataEditor.uxml");
        VisualElement labelFromUXML = visualTree.Instantiate();
        root.Add(labelFromUXML);

        circleCollider2DStateDataField = new ObjectField("CircleCollider2DStateData");
        circleCollider2DStateDataField.objectType = typeof(CircleCollider2DStateData);
        circleCollider2DStateDataField.RegisterValueChangedCallback(evt =>
        {
            circleCollider2DStateData = evt.newValue as CircleCollider2DStateData;
            if (circleCollider2DStateData == null)
                return;
            var objectVisualElement = new CircleCollider2DStateDataVisualElement(circleCollider2DStateData);
            root.Add(objectVisualElement);
        });

        addCircleCollider2DStateDataButton = new Button(() =>
        {
            circleCollider2DStateData = StateDataIO.CreateInstanceAndAsset<CircleCollider2DStateData>("NewCircleCollider2DStateData");
            root.Remove(addCircleCollider2DStateDataButton);
            root.Add(circleCollider2DStateDataField);
        });
        addCircleCollider2DStateDataButton.name = "AddCircleCollider2DStateData";
        addCircleCollider2DStateDataButton.text = "Create CircleCollider2DStateData Component";
    }
    void CheckStateIsInController(string stateName)
    {
        if (animatorController == null)
            return;
        foreach (AnimatorControllerLayer layer in animatorController.layers)
        {
            AnimatorStateMachine stateMachine = layer.stateMachine;
            foreach (ChildAnimatorState state in stateMachine.states)
            {
                AnimatorState animatorState = state.state;
                if(Animator.StringToHash(stateName) == animatorState.nameHash)
                {
                    stateExistsInController = true;
                    Debug.Log($"{stateName} is in {animatorController.name} controller");
                    return;
                }
            }
        }

        stateExistsInController = false;
        return;
    }
    private void OnGUI()
    {
        if(circleCollider2DStateData == null)
        {
            List<Button> button = root.Query<Button>("AddCircleCollider2DStateData").ToList();
            if (button == null || button.Count == 0)
                root.Add(addCircleCollider2DStateDataButton);
        }
    }
}
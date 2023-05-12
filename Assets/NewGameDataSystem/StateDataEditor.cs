using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEditor.Animations;
using System;
using System.Linq.Expressions;
using AnimatorStateData;
public class StateDataEditor : EditorWindow
{
    private AnimatorController animatorController;
    private string stateTarget;
    private VisualElement root;
    private bool stateExistsInController;
    [MenuItem("Window/UI Toolkit/StateDataEditor")]
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
        // Import UXML
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/NewGameDataSystem/StateDataEditor.uxml");
        VisualElement labelFromUXML = visualTree.Instantiate();
        root.Add(labelFromUXML);

        // A stylesheet can be added to a VisualElement.
        // The style will be applied to the VisualElement and all of its children.
        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/NewGameDataSystem/StateDataEditor.uss");
        VisualElement labelWithStyle = new Label("Hello World! With Style");
        labelWithStyle.styleSheets.Add(styleSheet);
        root.Add(labelWithStyle);

        controllerField.RegisterValueChangedCallback(evt => animatorController = evt.newValue as AnimatorController);
        stringField.RegisterValueChangedCallback(evt => CheckStateIsInController(evt.newValue));
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
        if (animatorController == null)
            return;
        Label hashDisplay = root.Query<Label>("HashDisplay");
        hashDisplay.text = Animator.StringToHash(stateTarget).ToString();
        StateData expression = new StateData();
        //Expression<Func<StateDataType>> expression = () => null;
        Type expType = expression.GetType();
        Debug.Log($"expression isSerializable = {expType.IsSerializable}");
    }
}
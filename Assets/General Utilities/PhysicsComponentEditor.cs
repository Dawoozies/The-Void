using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using PopupWindow = UnityEditor.PopupWindow;

public class PhysicsComponentEditor : EditorWindow
{
    TensorOptions newTensorOptions;
    Button theButton;
    [MenuItem("Window/Custom Editors/PhysicsComponentEditor")]
    public static void Init()
    {
        EditorWindow window = EditorWindow.CreateInstance<PhysicsComponentEditor>();
        window.Show();
    }

    public void CreateGUI()
    {
        var visualTreeAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/General Utilities/PhysicsComponentEditor.uxml");
        visualTreeAsset.CloneTree(rootVisualElement);
        theButton = rootVisualElement.Q<Button>();
        theButton.clicked += Setup;
    }

    public void Setup()
    {
        newTensorOptions = new TensorOptions();
        PopupWindow.Show(theButton.worldBound, newTensorOptions);
    }
}
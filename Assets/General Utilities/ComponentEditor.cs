using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;


public class ComponentEditor : EditorWindow
{
    [SerializeField]
    private int clickCount = 0;

    [MenuItem("Window/UI Toolkit/ComponentEditor")]
    public static void ShowExample()
    {
        ComponentEditor wnd = GetWindow<ComponentEditor>();
        wnd.titleContent = new GUIContent("ComponentEditor");
    }

    public void CreateGUI()
    {
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;

        

        // Import UXML
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/General Utilities/ComponentEditor.uxml");
        VisualElement labelFromUXML = visualTree.Instantiate();
        root.Add(labelFromUXML);
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using System;
public class TensorOptions : PopupWindowContent
{

    public override void OnGUI(Rect rect)
    {
        //Cum town
        //Sniper monkey
    }
    public override void OnOpen()
    {   
        var visualTreeAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/General Utilities/TensorOptions.uxml");
        visualTreeAsset.CloneTree(editorWindow.rootVisualElement);
        var buttons = editorWindow.rootVisualElement.Query<Button>();
        buttons.ForEach(RegisterHandler);
    }
    private void RegisterHandler(Button button)
    {
        button.RegisterCallback<ClickEvent>(PrintClickMessage);
    }
    private void PrintClickMessage(ClickEvent evt)
    {
        Button button = evt.currentTarget as Button;
        Debug.Log($"{button.name} was pressed.");
    }
    public override void OnClose()
    {

    }
}

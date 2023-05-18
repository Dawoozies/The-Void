using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using GameData.EditorWindow;
using Geometry;
using System;
using GameData.StateData;
public class ButtonListTesting : EditorWindow
{
    private ButtonList<Circle> buttonList;
    [MenuItem("Window/ButtonListTesting")]
    public static void Init()
    {
        ButtonListTesting window = GetWindow<ButtonListTesting>("ButtonListTesting");
        window.Show();
    }

    private void OnEnable()
    {
        var items = new List<Circle> { new Circle(Vector3.zero, 1f), new Circle(Vector3.zero, 2f), new Circle(Vector3.zero, 3f) };
        buttonList = new ButtonList<Circle>(items);
        buttonList.onButtonPress += ((circle) =>
        {
            foreach (var item in buttonList.items)
            {
                if (item == circle)
                {
                    item.SubscribeGUI();
                    continue;
                }
                item.UnsubscribeGUI();
            }
            SceneView.RepaintAll();
        });
        buttonList.onAddButtonPress += (() =>
        {
            return new Circle();
        });
    }
    private void OnDisable()
    {
        Debug.Log("OnDisable");
        buttonList.UnsubscribeAll();
        SceneView.RepaintAll();
    }

    private void OnGUI()
    {
        buttonList.Draw();
    }
}
public class CircleCollider2DEditorTesting : EditorWindow
{
    CircleCollider2DStateData inputData;
    ButtonList<Circle> buttonList;
    IndexList<string> indexList;
    [MenuItem("Window/CircleCollider2DEditorTesting")]
    public static void Init()
    {
        CircleCollider2DEditorTesting window = GetWindow<CircleCollider2DEditorTesting>("CircleCollider2DEditorTesting");
        window.Show();
    }
    private void OnEnable()
    {
        var items = new List<string>() { "First Object", "Second Object", "Third Object"};
        if(indexList == null)
        {
            indexList = new IndexList<string>(items);
        }
    }
    private void OnGUI()
    {
        if (indexList != null)
            indexList.Draw();
        OnGUIOLD();
    }
    private void OnGUIOLD()
    {
        CircleCollider2DStateData newInputData = (CircleCollider2DStateData)EditorGUILayout.ObjectField(inputData, typeof(CircleCollider2DStateData), false);
        if (newInputData != inputData)
        {
            if (buttonList == null)
            {
                buttonList = new ButtonList<Circle>(newInputData.circles);
                buttonList.onButtonPress += ((circle) =>
                {
                    foreach (var item in buttonList.items)
                    {
                        if (item == circle)
                        {
                            item.SubscribeGUI();
                            continue;
                        }
                        item.UnsubscribeGUI();
                    }
                    SceneView.RepaintAll();
                });
                buttonList.onAddButtonPress += (() =>
                {
                    return new Circle();
                });
            }
            else
            {
                buttonList.ChangeList(newInputData.circles);
            }
            inputData = newInputData;
        }
        if (inputData == null)
            return;
        buttonList.Draw();
    }   
}
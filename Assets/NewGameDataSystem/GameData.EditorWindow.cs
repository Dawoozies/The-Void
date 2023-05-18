using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;
using UnityEditor.Animations;
namespace GameData.EditorWindow
{
    public class AnimatorControllerStateEditor
    {
        public AnimatorController animatorController;
        public string stateName;
        int stateHash;
        public AnimatorControllerStateEditor()
        {

        }
    }
    public class IndexList<T>
    {
        public List<T> items;
        public int index;
        public Action<T> onNext;
        public Action<T> onPrevious;
        public Action<T> onElement;
        public IndexList(List<T> items)
        {
            this.items = items;
        }
        public void ChangeList(List<T> newItems)
        {
            this.items = newItems;
        }
        public void Draw()
        {
            if (items == null)
                return;
            string typeName = typeof(T).Name;
            GUILayout.Label($"List Of {typeName}");
            
            GUILayout.BeginHorizontal();
            if (GUILayout.Button(" < "))
            {
                if (index > 0)
                {
                    index--;
                    onPrevious?.Invoke(items[index]);
                }
            }
            if (GUILayout.Button($"Index {index}"))
            {

            }
            if(GUILayout.Button(" > "))
            {
                if(index < items.Count)
                {
                    index++;
                    onNext?.Invoke(items[index]);
                }
            }
            GUILayout.EndHorizontal();
        }
    }

    public class ButtonList<T>
    {
        public List<T> items;
        public Action<T> onButtonPress;
        public Func<T> onAddButtonPress;
        public Func<T, T> itemOut;
        public ButtonList(List<T> items)
        {
            this.items = items;
        }
        public void ChangeList(List<T> newItems)
        {
            this.items = newItems;
        }
        public void Draw()
        {
            if (items == null)
                return;
            string typeName = typeof(T).Name;
            GUILayout.Label($"List Of {typeName}");
            for (int i = 0; i < items.Count; i++)
            {
                GUILayout.BeginHorizontal();
                if(GUILayout.Button(typeName + " " + i))
                {
                    onButtonPress?.Invoke(items[i]);
                    itemOut?.Invoke(items[i]);
                }
                if(GUILayout.Button("Remove"))
                {
                    items.RemoveAt(i);
                    GUILayout.EndHorizontal();
                    return;
                }
                GUILayout.EndHorizontal();
            }
            if (GUILayout.Button($"Add New {typeName}"))
            {
                items.Add(onAddButtonPress.Invoke());
            }
        }
        public void UnsubscribeAll()
        {
            foreach (var item in items)
            {
                SceneGUI sceneGUI = item as SceneGUI;
                sceneGUI.UnsubscribeGUI();
            }
        }
        public void SubscribeAll()
        {
            foreach (var item in items)
            {
                SceneGUI sceneGUI = item as SceneGUI;
                sceneGUI.SubscribeGUI();
            }
        }
    }
    public interface SceneGUI
    {
        public void OnSceneGUI(SceneView sceneView);
        public void SubscribeGUI();
        public void UnsubscribeGUI();
    }
}
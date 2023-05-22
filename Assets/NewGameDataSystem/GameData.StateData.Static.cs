using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OLD.GameData.StateData;
using UnityEditor;
using UnityEditor.Animations;
using System.Linq;
using System;
namespace OLD.GameData.StateData
{
    [Serializable]
    public class Components
    {
        public int frame;
        public List<ScriptableObject> components;
        public Components ()
        {
            components = new List<ScriptableObject>() { };
        }
        public Components(ScriptableObject component)
        {
            components = new List<ScriptableObject>() { component };
        }
        public Components(int frame, ScriptableObject component)
        {
            this.frame = frame;
            components = new List<ScriptableObject>() { component };
        }
        public void Add(ScriptableObject component)
        {
            components.Add(component);
        }
        public List<ScriptableObject> GetScriptableObjects()
        {
            if (components == null)
                return null;
            return components;
        }
    }
    [Serializable]
    public class StateComponentDictionary
    {
        public List<Components> values;
        public StateComponentDictionary()
        {
            values = new List<Components>() { };
        }
        public Components ValueWithKey(int key)
        {
            if (values == null || values.Count == 0)
                return null; //No values at all
            Components valueReference = null;
            values.ForEach((v) => 
            {
                if (v.frame == key)
                    valueReference = v;
            });
            return valueReference;
        }
        public bool ContainsKey(int key)
        {
            Components valueReference = ValueWithKey(key);
            if (valueReference == null)
                return false;
            return true;
        }
        public void Add(int key, ScriptableObject component)
        {
            Debug.Log("Running Add");
            Components valueReference = ValueWithKey(key);
            if (valueReference != null)
            {
                Debug.Log("valueReference != null");
                valueReference.Add(component);
                return;
            }
            Debug.Log("valueReference == null");
            valueReference = new Components(key, component);
            values.Add(valueReference);
        }
        public int ValueCount()
        {
            if (values == null)
                return -1;
            return values.Count;
        }
    }
    public static class ComponentTypes
    {
        public static Type[] All = { 
            typeof(CircleCollider2DStateData), 
            typeof(OverlapStateData)
        };
    }
    public interface ScriptableObjectInitialization
    {
        public void Initialize();
    }
    public interface StateDataCast<T>
    {
        public T GetCastedData();
    }
    public interface Draw
    {
        public void Draw();
    }
    public static class Create<T> where T : ScriptableObject
    {
        public static T New(AnimatorController controller, string stateName)
        {
            T newObject = ScriptableObject.CreateInstance<T>();
            ScriptableObjectInitialization initialization = newObject as ScriptableObjectInitialization;
            if (initialization != null)
                initialization.Initialize();
            string baseAssetPath = $"Assets/Resources/{controller.name}_{stateName}_{typeof(T).Name}.asset";
            string uniqueAssetPath = AssetDatabase.GenerateUniqueAssetPath(baseAssetPath);
            AssetDatabase.CreateAsset(newObject, uniqueAssetPath);
            AssetDatabase.SaveAssets();
            return newObject;
        }
        public static T New(string assignedName)
        {
            T newComponent = ScriptableObject.CreateInstance<T>();
            ScriptableObjectInitialization initialization = newComponent as ScriptableObjectInitialization;
            if (initialization != null)
                initialization.Initialize();
            string baseAssetPath = $"Assets/Resources/{assignedName}.asset";
            string uniqueAssetPath = AssetDatabase.GenerateUniqueAssetPath(baseAssetPath);
            AssetDatabase.CreateAsset(newComponent, uniqueAssetPath);
            AssetDatabase.SaveAssets();
            return newComponent;
        }
    }
    public static class Draw<T> where T : ScriptableObject
    {
        public static void NewComponentOptions(StateData stateData, int frame)
        {
            CreateComponentButton(stateData, frame);
            LoadComponentButton(stateData, frame);
        }
        public static void CreateComponentButton(StateData stateData, int frame)
        {
            if (GUILayout.Button($"CREATE NEW {typeof(T).Name}"))
            {
                stateData.AddNewComponentToFrame(frame, Create<T>.New($"Component_{typeof(T).Name}"));
                EditorUtility.SetDirty(stateData);
            }
        }
        public static ScriptableObject loadedObject;
        public static void LoadComponentButton(StateData stateData, int frame)
        {
            GUILayout.BeginHorizontal();
            loadedObject = (ScriptableObject)EditorGUILayout.ObjectField(loadedObject, typeof(ScriptableObject), false, GUILayout.ExpandWidth(true));
            if(loadedObject != null)
            {
                if(GUILayout.Button("LOAD"))
                {

                }
            }
            GUILayout.EndHorizontal();
        }
    }
    public static class Load<T> where T : ScriptableObject
    {
        public static T LoadExisting(AnimatorController controller, string stateName)
        {
            string[] guids = AssetDatabase.FindAssets($"t:{typeof(T).Name}")
                .Where(guid =>
                {
                    string path = AssetDatabase.GUIDToAssetPath(guid);
                    string fileName = System.IO.Path.GetFileNameWithoutExtension(path);
                    return fileName.Contains($"{controller.name}_{stateName}");
                }).ToArray();
            if (guids.Length == 0)
                return null;
            string guid = guids[0];
            string path = AssetDatabase.GUIDToAssetPath(guid);
            return AssetDatabase.LoadAssetAtPath<T>(path);
        }
    }
    public static class Delete<T> where T : ScriptableObject
    {
        public static void Asset(T asset)
        {
            string assetPath = $"Assets/Resources/{asset.name}.asset";
            Debug.Log("Delete at " + assetPath);
            AssetDatabase.DeleteAsset(assetPath);
        }
    }
}

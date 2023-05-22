using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
[Serializable]
public class NewStateData : ScriptableObject
{
    public int stateHash;
    public List<ScriptableObject> components;
}
public static class Assets
{
    public static NewStateData CreateNewAsset(AnimatorController controller, string stateName)
    {
        NewStateData newInstance = ScriptableObject.CreateInstance<NewStateData>();
        newInstance.stateHash = Animator.StringToHash(stateName);
        newInstance.components = new List<ScriptableObject>();
        string uniqueAssetPath = AssetDatabase.GenerateUniqueAssetPath($"Assets/Resources/{controller.name}_{stateName}.asset");
        AssetDatabase.CreateAsset(newInstance, uniqueAssetPath);
        AssetDatabase.SaveAssets();
        return newInstance;
    }
    public static NewStateData LoadExistingAsset(AnimatorController controller, string stateName)
    {
        string[] guids = AssetDatabase.FindAssets($"{controller.name}_{stateName}");
        if (guids.Length == 0)
            return null;
        return AssetDatabase.LoadAssetAtPath<NewStateData>(AssetDatabase.GUIDToAssetPath(guids[0]));
    }
}
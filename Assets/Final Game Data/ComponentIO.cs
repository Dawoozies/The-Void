using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using System.Linq;
using System.IO;
namespace ComponentIO
{
    public static class Assets<T> where T : ScriptableObject
    {
        public static T CreateNewAsset(AnimatorController controller, string stateName)
        {
            var instance = ScriptableObject.CreateInstance<T>();
            string uniqueAssetPath = AssetDatabase.GenerateUniqueAssetPath(
                $"Assets/Resources/{controller.name}_{stateName}_{typeof(T).Name}.asset"
                );
            AssetDatabase.CreateAsset(instance, uniqueAssetPath);
            AssetDatabase.SaveAssets();
            return instance;
        }
        //ONLY EDITOR MODE
        public static T LoadExistingAsset(AnimatorController controller, string stateName)
        {
            string[] guids = AssetDatabase.FindAssets($"{controller.name}_{stateName}_{typeof(T).Name}");
            if (guids.Length == 0)
                return null;
            return AssetDatabase.LoadAssetAtPath<T>(AssetDatabase.GUIDToAssetPath(guids[0]));
        }
    }
    public static class Load<T> where T : ScriptableObject
    {
        public static T[] AllComponents()
        {
            return Resources.LoadAll("",(typeof(T))) as T[];
        }
    }
}
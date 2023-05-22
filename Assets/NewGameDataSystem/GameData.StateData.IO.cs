using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEditor;
namespace OLD.GameData.StateData.IO
{
    public static class StateDataIO
    {
        public static T CreateInstanceAndAsset<T>(string dataId) where T : ScriptableObject
        {
            T newInstance = ScriptableObject.CreateInstance<T>();
            AssetDatabase.CreateAsset(newInstance, $"Assets/Resources/{dataId}.asset");
            AssetDatabase.SaveAssets();
            //StateDataCache.Add(GetAssetKey<T>(dataId), newInstance);
            return newInstance;
        }
        public static T LoadAsset<T>(string dataId) where T : ScriptableObject
        {
            string assetKey = GetAssetKey<T>(dataId);
            if(StateDataCache.TryGet<T>(assetKey, out T asset))
                return asset;
            string[] guids = AssetDatabase.FindAssets($"t:{typeof(T).Name}")
                .Where(guid =>
                {
                    string path = AssetDatabase.GUIDToAssetPath(guid);
                    string fileName = System.IO.Path.GetFileNameWithoutExtension(path);
                    return fileName.Contains(dataId);
                }).ToArray();
            if (guids.Length == 0)
                return null;
            string guid = guids[0];
            string path = AssetDatabase.GUIDToAssetPath(guid);
            asset = AssetDatabase.LoadAssetAtPath<T>(path);
            return asset;
        }
        private static string GetAssetKey<T>(string dataId)
        {
            return $"{dataId}_{typeof(T).Name}";
        }
    }
    public static class StateDataCache
    {
        private static Dictionary<string, ScriptableObject> cache = new Dictionary<string, ScriptableObject>();
        public static void Add(string assetKey, ScriptableObject asset)
        {
            cache.Add(assetKey, asset);
        }
        public static bool TryGet<T>(string assetKey, out T asset) where T : ScriptableObject
        {
            if(cache.TryGetValue(assetKey, out ScriptableObject cachedAsset))
            {
                asset = cachedAsset as T;
                return asset != null;
            }
            asset = null;
            return false;
        }
    }
}
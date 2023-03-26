using UnityEngine;
using System;
using System.Reflection;
using UnityEditor;
namespace AudioUtilities
{
    public static class AudioPreview
    {
        public static void Play(AudioClip clip, int startSample = 0, bool loop = false)
        {
            Assembly assembly = typeof(AudioImporter).Assembly;
            Type utilityClass = assembly.GetType("UnityEditor.AudioUtil");
            MethodInfo method = utilityClass.GetMethod
            (
                "PlayPreviewClip",
                BindingFlags.Static | BindingFlags.Public,
                null,
                new Type[] { typeof(AudioClip), typeof(int), typeof(bool) },
                null
            );

            method.Invoke
            (
                null,
                new object[] { clip, startSample, loop}
            );
        }

        public static void StopAllClips()
        {
            Assembly assembly = typeof(AudioImporter).Assembly;
            Type utilityClass = assembly.GetType("UnityEditor.AudioUtil");
            MethodInfo method = utilityClass.GetMethod
            (
                "StopAllPreviewClips",
                BindingFlags.Static | BindingFlags.Public,
                null,
                new Type[] { },
                null
            );

            method.Invoke
            (
                null,
                new object[] { }
            );
        }
    }
}

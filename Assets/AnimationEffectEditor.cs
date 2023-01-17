using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
public class AnimationEffectEditor : EditorWindow
{
    //Entity Variables
    Transform entity;
    Animator entityAnimator;
    AnimationClip entityClip;
    List<float> entityFrameTimes;
    int entityFrame = 0;
    float entityTime = 0f;

    //Effect Variables
    Animator effectAnimator;
    AnimationClip effectClip;
    [MenuItem("Window/Animation Effect Editor")]
    static void Init()
    {
        AnimationEffectEditor window = (AnimationEffectEditor)EditorWindow.GetWindow(typeof(AnimationEffectEditor));
        window.Show();
    }

    private void OnGUI()
    {
        GUILayout.Label("Animation FX Editor", EditorStyles.boldLabel);

        if(Selection.transforms.Length <= 0)
        {
            GUILayout.Label("Select Entity", EditorStyles.boldLabel);
            return;
        }

        if(Selection.transforms.Length > 0)
        {
            entity = Selection.transforms[0];
            GUILayout.Label("Entity: " + entity.name, EditorStyles.label);

            entityAnimator = entity.GetComponent<Animator>();

            AnimationMode.StartAnimationMode();

            entityClip = EditorGUILayout.ObjectField(entityClip, typeof(AnimationClip), false) as AnimationClip;

            EditorGUILayout.BeginVertical();
            if(entityClip != null)
            {
                entityFrame = Mathf.FloorToInt(entityTime * entityClip.frameRate);
                float entityTotalFrames = (entityClip.length - 1) * entityClip.frameRate;
                GUILayout.Label("Entity Total Frames: " + (entityTotalFrames + 1));
                GUILayout.Label("Entity Current Frame: " + entityFrame);
                GUILayout.Label("Frame 0 is the first frame");

                entityFrameTimes = new List<float>();
                for (int i = 0; entityFrameTimes.Count <= entityTotalFrames; i++)
                {
                    entityFrameTimes.Add(i / entityClip.frameRate);
                }
                GUILayout.BeginHorizontal();
                if(GUILayout.Button("Previous Frame"))
                {
                    entityFrame--;
                    if(entityFrame <= 0)
                    {
                        entityFrame = 0;
                    }
                    entityTime = entityFrameTimes[entityFrame];
                }
                if(GUILayout.Button("Next Frame"))
                {
                    if(entityFrame < entityFrameTimes.Count - 1)
                    {
                        entityFrame++;
                    }
                    entityTime = entityFrameTimes[entityFrame];
                }
                GUILayout.EndHorizontal();
            }
        }
    }
}

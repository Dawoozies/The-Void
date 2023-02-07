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

    //Frame Effect Data
    AnimationEffect currentEffectData;
    EffectData effectToEdit;

    //Temporary Animator Setup
    Transform effectTransform;
    Animator effectAnimator;
    float effectTime;

    //Object Field Stuff
    AnimationClip effectClipToAdd;
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

            entityClip = EditorGUILayout.ObjectField(entityClip, typeof(AnimationClip), false) as AnimationClip;

            if(entityClip == null)
            {
                GUILayout.Label("Select Main Animation Clip", EditorStyles.boldLabel);
                return;
            }

            AnimationMode.StartAnimationMode();
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
                EditorGUILayout.EndVertical();

                string[] results = AssetDatabase.FindAssets(entityClip.name + "_EffectData");
                if (results.Length <= 0)// be careful of when results = 0
                {
                    //Debug.Log("Creating new Effect Data asset");
                    AnimationEffect animationEffect = ScriptableObject.CreateInstance<AnimationEffect>();
                    animationEffect.clip = entityClip;
                    animationEffect.effectsList = new List<EffectList>();

                    for (int i = 0; animationEffect.effectsList.Count < entityFrameTimes.Count; i++)
                    {
                        animationEffect.effectsList.Add(new EffectList());
                    }

                    if (!AssetDatabase.IsValidFolder("Assets/Resources/AnimationEffectData"))
                    {
                        AssetDatabase.CreateFolder("Assets/Resources", "AnimationEffectData");
                    }

                    AssetDatabase.CreateAsset(animationEffect, $"Assets/Resources/AnimationEffectData/{entityClip.name}_EffectData.asset");
                    AssetDatabase.SaveAssets();
                }

                if(results.Length > 0)
                {
                    //Debug.Log("AnimationEffectData found");
                    currentEffectData = AssetDatabase.LoadAssetAtPath<AnimationEffect>(AssetDatabase.GUIDToAssetPath(results[0]));
                    GUILayout.Label("Effect Data Asset Path: " + AssetDatabase.GUIDToAssetPath(results[0]), EditorStyles.boldLabel);

                    //Make sure to keep on the lookout for if the effectsList length is ever not equal to totalFrames

                    GUILayout.BeginHorizontal();

                    effectClipToAdd = EditorGUILayout.ObjectField(effectClipToAdd, typeof(AnimationClip), false) as AnimationClip;
                    if (GUILayout.Button("Add"))
                    {
                        currentEffectData.effectsList[entityFrame].effects.Add(new EffectData(effectClipToAdd));
                        effectClipToAdd = null;

                        EditorUtility.SetDirty(currentEffectData);
                        Repaint();
                    }
                    GUILayout.EndHorizontal();
                    
                    if (currentEffectData.effectsList[entityFrame].effects.Count > 0)
                    {
                        //Then there are effects on this frame
                        GUILayout.BeginVertical();
                        GUILayout.Label("Effect List: ", EditorStyles.boldLabel);
                        for (int i = 0; i < currentEffectData.effectsList[entityFrame].effects.Count; i++)
                        {
                            GUILayout.BeginHorizontal();
                            currentEffectData.effectsList[entityFrame].effects[i].clip = EditorGUILayout.ObjectField(currentEffectData.effectsList[entityFrame].effects[i].clip, typeof(AnimationClip), false) as AnimationClip;
                            if(GUILayout.Button("Edit"))
                            {
                                effectToEdit = currentEffectData.effectsList[entityFrame].effects[i];

                                if (effectTransform != null)
                                    effectTransform.localPosition = effectToEdit.position;
                            }
                            if(GUILayout.Button("Remove"))
                            {
                                currentEffectData.effectsList[entityFrame].effects.RemoveAt(i);

                                EditorUtility.SetDirty(currentEffectData);
                                Repaint();
                            }
                            GUILayout.EndHorizontal();
                        }
                        GUILayout.EndVertical();
                    }
                }
            }
        }

        if(effectToEdit != null)
        {
            if(effectTransform != null)
            {
                GUILayout.BeginHorizontal();
                if(GUILayout.Button("Previous Frame"))
                {
                    if(effectTime > 0)
                    {
                        effectTime -= 1 / effectToEdit.clip.frameRate;
                    }

                    if(effectTime < 0)
                    {
                        effectTime = 0;
                    }
                }

                if(GUILayout.Button("Next Frame"))
                {
                    if(effectTime < effectToEdit.clip.length)
                    {
                        effectTime += 1 / effectToEdit.clip.frameRate;
                    }

                    if(effectTime > effectToEdit.clip.length)
                    {
                        effectTime = effectToEdit.clip.length;
                    }
                }    
                GUILayout.EndHorizontal();
            }
        }
    }

    private void OnFocus()
    {
        SceneView.duringSceneGui -= this.OnSceneGUI;

        SceneView.duringSceneGui += this.OnSceneGUI;
    }

    private void OnDestroy()
    {
        SceneView.duringSceneGui -= this.OnSceneGUI;

        if (AnimationMode.InAnimationMode())
        {
            AnimationMode.StopAnimationMode();
        }

        if (effectTransform != null)
            DestroyImmediate(effectTransform.gameObject);
    }

    void OnSceneGUI (SceneView sceneView)
    {
        if(effectToEdit == null || effectTransform == null)
            return;

        Vector3 newPosition = Handles.PositionHandle(effectTransform.position, Quaternion.identity);

        if(EditorGUI.EndChangeCheck())
        {
            effectTransform.position = newPosition;
            effectToEdit.position = newPosition;
            EditorUtility.SetDirty(currentEffectData);
            Repaint();
        }
    }

    void Update()
    {
        //Debug.Log("Update has been called");
        // can these variables be null when it reaches here? UCAN
        if (entity == null)// nice guard claues instead of nested if statements <3
            return;
        if (entityClip == null)
            return;

        // Animate the GameObject
        if (!EditorApplication.isPlaying && AnimationMode.InAnimationMode())
        {
            AnimationMode.BeginSampling();
            AnimationMode.SampleAnimationClip(entity.gameObject, entityClip, entityTime);

            if (effectToEdit != null)
            {
                //Debug.Log("Effect To Edit is not null");
                if (effectTransform == null)
                {
                    effectTransform = new GameObject("Effect Transform").transform;
                    effectTransform.parent = entity;
                    effectTransform.localPosition = effectToEdit.position;
                    Animator effectAnimator = effectTransform.gameObject.AddComponent(typeof(Animator)) as Animator;
                    SpriteRenderer spriteRenderer = effectTransform.gameObject.AddComponent(typeof(SpriteRenderer)) as SpriteRenderer;
                    spriteRenderer.sortingOrder = 1000;
                }

                if (effectTransform != null)
                {
                    //Debug.Log("We should be animating the effect");
                    AnimationMode.SampleAnimationClip(effectTransform.gameObject, effectToEdit.clip, effectTime);

                    //if(effectTime < effectToEdit.clip.length)
                    //{
                    //    effectTime += Time.deltaTime/(effectToEdit.clip.frameRate);
                    //}
                    //else
                    //{
                    //    effectTime = 0f;
                    //}
                }
            }

            AnimationMode.EndSampling();
            SceneView.RepaintAll();
        }
    }
}

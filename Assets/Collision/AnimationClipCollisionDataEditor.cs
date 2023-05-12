using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using OldData;
public class AnimationClipCollisionDataEditor : EditorWindow
{
    float time = 0f;

    Transform entityToEdit;
    AnimationClip animationClip;
    List<float> currentFrameTimes;
    int currentFrame = 0;

    AnimationClipCollisionData currentClipData;
    FrameCollisionData currentFrameHitboxData;
    FrameCollisionData currentFrameHurtboxData;
    FrameCollisionData currentFrameGroundboxData;

    [MenuItem("Window/Animation Clip Collision Editor")]
    static void Init()
    {
        // Get existing open window or if none, make a new one:
        AnimationClipCollisionDataEditor window = (AnimationClipCollisionDataEditor)EditorWindow.GetWindow(typeof(AnimationClipCollisionDataEditor));
        window.Show();
    }

    void OnGUI()
    {
        GUILayout.Label("Hitbox/Hurtbox Editor", EditorStyles.boldLabel);

        if(Selection.transforms.Length > 0)
        {
            entityToEdit = Selection.transforms[0];
            GUILayout.Label("Entity: " + entityToEdit.name, EditorStyles.label);

            AnimationMode.StartAnimationMode();

            animationClip = EditorGUILayout.ObjectField(animationClip, typeof(AnimationClip), false) as AnimationClip;

            EditorGUILayout.BeginVertical();

            if (animationClip != null)
            {
                

                float clipLength = animationClip.length;
                float frameRate = animationClip.frameRate;

                //time = EditorGUILayout.Slider(time, 0f, animationClip.length - 1);


                currentFrame = Mathf.FloorToInt(time * animationClip.frameRate);


                //IF YOU DONT HAVE THE SAMPLES PROPERTY SET TO 1 IN YOUR ANIMATION CLIP
                //YOU WILL SOMETIMES GET TOTALFRAMES BEING 1 HIGHER THAN IT SHOULD BE
                float totalFrames = (animationClip.length - 1) * animationClip.frameRate;
                
                GUILayout.Label("Total Frames: " + (totalFrames + 1));
                GUILayout.Label("Current Frame: " + currentFrame);
                GUILayout.Label("Frame 0 is the first frame");

                currentFrameTimes = new List<float>();
                for (int i = 0; currentFrameTimes.Count <= totalFrames; i++)
                {
                    currentFrameTimes.Add(i / animationClip.frameRate);
                }

                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Previous Frame"))
                {
                    currentFrame--;

                    if(currentFrame <= 0)
                    {
                        currentFrame = 0;
                    }

                    time = currentFrameTimes[currentFrame];
                }
                if(GUILayout.Button("Next Frame"))
                {
                    if(currentFrame < currentFrameTimes.Count - 1)
                    {
                        currentFrame++;
                    }

                    time = currentFrameTimes[currentFrame];
                }
                GUILayout.EndHorizontal();
                string[] results = AssetDatabase.FindAssets(animationClip.name + "_CollisionData");
                if (results.Length <= 0)// be careful of when results = 0
                {
                    Debug.Log("Creating new asset");
                    AnimationClipCollisionData collisionDataAsset = ScriptableObject.CreateInstance<AnimationClipCollisionData>();
                    collisionDataAsset.animationClip = animationClip;
                    collisionDataAsset.hitboxes = new List<FrameCollisionData>();
                    collisionDataAsset.hurtboxes = new List<FrameCollisionData>();
                    collisionDataAsset.groundboxes = new List<FrameCollisionData>();

                    for (int i = 0; i < currentFrameTimes.Count; i++)
                    {
                        FrameCollisionData newFrameHitboxData = new FrameCollisionData(i);
                        FrameCollisionData newFrameHurtboxData = new FrameCollisionData(i);
                        FrameCollisionData newFrameGroundboxData = new FrameCollisionData(i);
                        collisionDataAsset.hitboxes.Add(newFrameHitboxData);
                        collisionDataAsset.hurtboxes.Add(newFrameHurtboxData);
                        collisionDataAsset.groundboxes.Add(newFrameGroundboxData);
                    }

                    currentClipData = collisionDataAsset;

                    if(!AssetDatabase.IsValidFolder("Assets/Resources/AnimationClipCollisionData"))
                    {
                        AssetDatabase.CreateFolder("Assets/Resources", "AnimationClipCollisionData");
                    }

                    AssetDatabase.CreateAsset(collisionDataAsset, $"Assets/Resources/AnimationClipCollisionData/{animationClip.name}_CollisionData.asset");
                    AssetDatabase.SaveAssets();
                }

                if(results.Length > 0)
                {
                    Debug.Log("Asset already found");
                    currentClipData = AssetDatabase.LoadAssetAtPath<AnimationClipCollisionData>(AssetDatabase.GUIDToAssetPath(results[0]));
                    GUILayout.Label("Collision Data Asset Path: " + AssetDatabase.GUIDToAssetPath(results[0]), EditorStyles.label);

                    if(currentClipData.hitboxes.Count < currentFrameTimes.Count)
                    {
                        Debug.Log("Generating new hitboxes");
                        for (int i = 0; i < currentFrameTimes.Count; i++)
                        {
                            FrameCollisionData newFrameCollisionData = new FrameCollisionData(i);
                            currentClipData.hitboxes.Add(newFrameCollisionData);
                        }
                    }

                    if(currentClipData.hurtboxes.Count < currentFrameTimes.Count)
                    {
                        for (int i = 0; i < currentFrameTimes.Count; i++)
                        {
                            FrameCollisionData newFrameCollisionData = new FrameCollisionData(i);
                            currentClipData.hurtboxes.Add(newFrameCollisionData);
                        }
                    }

                    if(currentClipData.groundboxes.Count < currentFrameTimes.Count)
                    {
                        for (int i = 0; i < currentFrameTimes.Count; i++)
                        {
                            FrameCollisionData newFrameCollisionData = new FrameCollisionData(i);
                            currentClipData.groundboxes.Add(newFrameCollisionData);
                        }
                    }
                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button("Duplicate Previous Frame Data"))
                    {
                        if(currentFrame > 0)
                        {
                            //Set the currentFrame hitbox circles to a fresh list of circles
                            //Then we add the circles from the previous frame to this new one
                            //Being careful that we don't set a reference to the previous frame's list
                            if (currentClipData.hitboxes[currentFrame - 1].circles.Count > 0)
                            {
                                currentClipData.hitboxes[currentFrame].circles = new List<Circle>();
                                for (int i = 0; i < currentClipData.hitboxes[currentFrame - 1].circles.Count; i++)
                                {
                                    Vector2 newCenter = currentClipData.hitboxes[currentFrame - 1].circles[i].center;
                                    float newRadius = currentClipData.hitboxes[currentFrame - 1].circles[i].radius;
                                    Circle newCircle = new Circle(newCenter, newRadius);

                                    currentClipData.hitboxes[currentFrame].circles.Add(newCircle);
                                }

                                EditorUtility.SetDirty(currentClipData);
                                Repaint();
                            }

                            if (currentClipData.hurtboxes[currentFrame - 1].circles.Count > 0)
                            {
                               currentClipData.hurtboxes[currentFrame].circles = new List<Circle>();
                               for (int i = 0; i < currentClipData.hurtboxes[currentFrame - 1].circles.Count; i++)
                               {
                                    Vector2 newCenter = currentClipData.hurtboxes[currentFrame - 1].circles[i].center;
                                    float newRadius = currentClipData.hurtboxes[currentFrame - 1].circles[i].radius;
                                    Circle newCircle = new Circle(newCenter, newRadius);

                                    currentClipData.hurtboxes[currentFrame].circles.Add(newCircle);
                               }

                                EditorUtility.SetDirty(currentClipData);
                                Repaint();
                            }

                            if (currentClipData.groundboxes[currentFrame - 1].circles.Count > 0)
                            {
                                currentClipData.groundboxes[currentFrame].circles = new List<Circle>();
                                for (int i = 0; i < currentClipData.groundboxes[currentFrame - 1].circles.Count; i++)
                                {
                                    Vector2 newCenter = currentClipData.groundboxes[currentFrame - 1].circles[i].center;
                                    float newRadius = currentClipData.groundboxes[currentFrame - 1].circles[i].radius;
                                    Circle newCircle = new Circle(newCenter, newRadius);

                                    currentClipData.groundboxes[currentFrame].circles.Add(newCircle);
                                }

                                EditorUtility.SetDirty(currentClipData);
                                Repaint();
                            }
                        }
                    }
                    if (GUILayout.Button("Duplicate Next Frame Data"))
                    {
                        if (currentFrame < currentFrameTimes.Count - 1)
                        {
                            //Set the currentFrame hitbox circles to a fresh list of circles
                            //Then we add the circles from the previous frame to this new one
                            //Being careful that we don't set a reference to the previous frame's list
                            if (currentClipData.hitboxes[currentFrame + 1].circles.Count > 0)
                            {
                                currentClipData.hitboxes[currentFrame].circles = new List<Circle>();
                                for (int i = 0; i < currentClipData.hitboxes[currentFrame + 1].circles.Count; i++)
                                {
                                    Vector2 newCenter = currentClipData.hitboxes[currentFrame + 1].circles[i].center;
                                    float newRadius = currentClipData.hitboxes[currentFrame + 1].circles[i].radius;
                                    Circle newCircle = new Circle(newCenter, newRadius);

                                    currentClipData.hitboxes[currentFrame].circles.Add(newCircle);
                                }

                                EditorUtility.SetDirty(currentClipData);
                                Repaint();
                            }

                            if (currentClipData.hurtboxes[currentFrame + 1].circles.Count > 0)
                            {
                                currentClipData.hurtboxes[currentFrame].circles = new List<Circle>();
                                for (int i = 0; i < currentClipData.hurtboxes[currentFrame + 1].circles.Count; i++)
                                {
                                    Vector2 newCenter = currentClipData.hurtboxes[currentFrame + 1].circles[i].center;
                                    float newRadius = currentClipData.hurtboxes[currentFrame + 1].circles[i].radius;
                                    Circle newCircle = new Circle(newCenter, newRadius);

                                    currentClipData.hurtboxes[currentFrame].circles.Add(newCircle);
                                }

                                EditorUtility.SetDirty(currentClipData);
                                Repaint();
                            }

                            if (currentClipData.groundboxes[currentFrame + 1].circles.Count > 0)
                            {
                                currentClipData.groundboxes[currentFrame].circles = new List<Circle>();
                                for (int i = 0; i < currentClipData.groundboxes[currentFrame + 1].circles.Count; i++)
                                {
                                    Vector2 newCenter = currentClipData.groundboxes[currentFrame + 1].circles[i].center;
                                    float newRadius = currentClipData.groundboxes[currentFrame + 1].circles[i].radius;
                                    Circle newCircle = new Circle(newCenter, newRadius);

                                    currentClipData.groundboxes[currentFrame].circles.Add(newCircle);
                                }

                                EditorUtility.SetDirty(currentClipData);
                                Repaint();
                            }
                        }
                    }
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button("Duplicate Previous Groundbox Data"))
                    {
                        if (currentFrame > 0)
                        {
                            //Set the currentFrame hitbox circles to a fresh list of circles
                            //Then we add the circles from the previous frame to this new one
                            //Being careful that we don't set a reference to the previous frame's list

                            if (currentClipData.groundboxes[currentFrame - 1].circles.Count > 0)
                            {
                                currentClipData.groundboxes[currentFrame].circles = new List<Circle>();
                                for (int i = 0; i < currentClipData.groundboxes[currentFrame - 1].circles.Count; i++)
                                {
                                    Vector2 newCenter = currentClipData.groundboxes[currentFrame - 1].circles[i].center;
                                    float newRadius = currentClipData.groundboxes[currentFrame - 1].circles[i].radius;
                                    Circle newCircle = new Circle(newCenter, newRadius);

                                    currentClipData.groundboxes[currentFrame].circles.Add(newCircle);
                                }

                                EditorUtility.SetDirty(currentClipData);
                                Repaint();
                            }
                        }
                    }
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button("Add Hitbox Circle"))
                    {
                        currentClipData.hitboxes[currentFrame].circles.Add(new Circle());
                    }

                    if(GUILayout.Button("Remove Hitbox Circle"))
                    {
                        if (currentClipData.hitboxes[currentFrame].circles.Count > 0)
                        {
                            currentClipData.hitboxes[currentFrame].circles.RemoveAt(currentClipData.hitboxes[currentFrame].circles.Count - 1);
                        }
                    }
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    if(GUILayout.Button("Add Hurtbox Circle"))
                    {
                        currentClipData.hurtboxes[currentFrame].circles.Add(new Circle());
                    }

                    if(GUILayout.Button("Remove Hurtbox Circle"))
                    {
                        if (currentClipData.hurtboxes[currentFrame].circles.Count > 0)
                        {
                            currentClipData.hurtboxes[currentFrame].circles.RemoveAt(currentClipData.hurtboxes[currentFrame].circles.Count - 1);
                        }
                    }
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button("Add Groundbox Circle"))
                    {
                        currentClipData.groundboxes[currentFrame].circles.Add(new Circle());
                    }

                    if (GUILayout.Button("Remove Groundbox Circle"))
                    {
                        if (currentClipData.groundboxes[currentFrame].circles.Count > 0)
                        {
                            currentClipData.groundboxes[currentFrame].circles.RemoveAt(currentClipData.groundboxes[currentFrame].circles.Count - 1);
                        }
                    }
                    GUILayout.EndHorizontal();
                }
            }
            else
            {
                GUILayout.Label("Please Select Animation Clip");
            }

            EditorGUILayout.EndVertical();
        }
        else
        {
            GUILayout.Label("Please Select Entity");
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
            
    }

    void OnSceneGUI(SceneView sceneView)
    {
        if(currentClipData != null)
        {
            //Create all the handles for changing/editing the hitbox circles
            if (currentClipData.hitboxes.Count > 0)
            {
                currentFrameHitboxData = currentClipData.hitboxes[currentFrame];
                if(currentFrameHitboxData.circles != null)
                {
                    if(currentFrameHitboxData.circles.Count > 0)
                    {
                        for (int i = 0; i < currentFrameHitboxData.circles.Count; i++)
                        {
                            EditorGUI.BeginChangeCheck();
                       
                            float oldRadius = currentFrameHitboxData.circles[i].radius;
                            Vector3 oldCenter = new Vector3(currentFrameHitboxData.circles[i].center.x, currentFrameHitboxData.circles[i].center.y, 0f);

                            float newRadius = Handles.RadiusHandle(Quaternion.identity, oldCenter, oldRadius);
                            Vector3 newCenter = Handles.FreeMoveHandle(oldCenter, Quaternion.identity, oldRadius - 0.01f, Vector3.one * 0.1f, Handles.CircleHandleCap);

                            if(EditorGUI.EndChangeCheck())
                            {
                                currentFrameHitboxData.circles[i].center = new Vector2(newCenter.x, newCenter.y);
                                currentFrameHitboxData.circles[i].radius = newRadius;
                                EditorUtility.SetDirty(currentClipData);
                                Repaint();
                            }

                            Handles.color = new Color(1f, 0.92f, 0.016f, 0.75f);
                            Handles.DrawSolidDisc(new Vector3(currentFrameHitboxData.circles[i].center.x, currentFrameHitboxData.circles[i].center.y, 0f), Vector3.forward, currentFrameHitboxData.circles[i].radius); ;

                            GUIStyle indexStyle = new GUIStyle();
                            indexStyle.fontSize = 12;
                            indexStyle.alignment = TextAnchor.MiddleCenter;
                            indexStyle.normal.textColor = Color.black;
                            Handles.Label(new Vector3(currentFrameHitboxData.circles[i].center.x, currentFrameHitboxData.circles[i].center.y, 0f), "HIT" + i.ToString(), indexStyle);
                        }
                    }
                }
            }

            //Create all the handles for changing/editing the hurtbox circles
            if(currentClipData.hurtboxes.Count > 0)
            {
                currentFrameHurtboxData = currentClipData.hurtboxes[currentFrame];
                if(currentFrameHurtboxData.circles != null)
                {
                    if(currentFrameHurtboxData.circles.Count > 0)
                    {
                        for (int i = 0; i < currentFrameHurtboxData.circles.Count; i++)
                        {
                            EditorGUI.BeginChangeCheck();

                            float oldRadius = currentFrameHurtboxData.circles[i].radius;
                            Vector3 oldCenter = new Vector3(currentFrameHurtboxData.circles[i].center.x, currentFrameHurtboxData.circles[i].center.y, 0f);

                            float newRadius = Handles.RadiusHandle(Quaternion.identity, oldCenter, oldRadius);
                            Vector3 newCenter = Handles.FreeMoveHandle(oldCenter, Quaternion.identity, oldRadius - 0.01f, Vector3.one * 0.1f, Handles.CircleHandleCap);

                            if(EditorGUI.EndChangeCheck())
                            {
                                currentFrameHurtboxData.circles[i].center = new Vector2(newCenter.x, newCenter.y);
                                currentFrameHurtboxData.circles[i].radius = newRadius;
                                EditorUtility.SetDirty(currentClipData);
                                Repaint();
                            }

                            Handles.color = new Color(1f, 0f, 0f, 0.75f);
                            Handles.DrawSolidDisc(new Vector3(currentFrameHurtboxData.circles[i].center.x, currentFrameHurtboxData.circles[i].center.y, 0f), Vector3.forward, currentFrameHurtboxData.circles[i].radius); ;

                            GUIStyle indexStyle = new GUIStyle();
                            indexStyle.fontSize = 12;
                            indexStyle.alignment = TextAnchor.MiddleCenter;
                            indexStyle.normal.textColor = Color.black;
                            Handles.Label(new Vector3(currentFrameHurtboxData.circles[i].center.x, currentFrameHurtboxData.circles[i].center.y, 0f), "HURT" + i.ToString(), indexStyle);
                        }
                    }
                }
            }

            //Create all the handles for changing/editing the groundbox circles
            if (currentClipData.groundboxes.Count > 0)
            {
                currentFrameGroundboxData = currentClipData.groundboxes[currentFrame];
                if (currentFrameGroundboxData.circles != null)
                {
                    if (currentFrameGroundboxData.circles.Count > 0)
                    {
                        for (int i = 0; i < currentFrameGroundboxData.circles.Count; i++)
                        {
                            EditorGUI.BeginChangeCheck();

                            float oldRadius = currentFrameGroundboxData.circles[i].radius;
                            Vector3 oldCenter = new Vector3(currentFrameGroundboxData.circles[i].center.x, currentFrameGroundboxData.circles[i].center.y, 0f);

                            float newRadius = Handles.RadiusHandle(Quaternion.identity, oldCenter, oldRadius);
                            Vector3 newCenter = Handles.FreeMoveHandle(oldCenter, Quaternion.identity, oldRadius - 0.01f, Vector3.one * 0.1f, Handles.CircleHandleCap);

                            if (EditorGUI.EndChangeCheck())
                            {
                                currentFrameGroundboxData.circles[i].center = new Vector2(newCenter.x, newCenter.y);
                                currentFrameGroundboxData.circles[i].radius = newRadius;
                                EditorUtility.SetDirty(currentClipData);
                                Repaint();
                            }

                            Handles.color = new Color(0f, 1f, 0f, 0.75f);
                            Handles.DrawSolidDisc(new Vector3(currentFrameGroundboxData.circles[i].center.x, currentFrameGroundboxData.circles[i].center.y, 0f), Vector3.forward, currentFrameGroundboxData.circles[i].radius); ;

                            GUIStyle indexStyle = new GUIStyle();
                            indexStyle.fontSize = 12;
                            indexStyle.alignment = TextAnchor.MiddleCenter;
                            indexStyle.normal.textColor = Color.black;
                            Handles.Label(new Vector3(currentFrameGroundboxData.circles[i].center.x, currentFrameGroundboxData.circles[i].center.y, 0f), "GRND" + i.ToString(), indexStyle);
                        }
                    }
                }
            }
        }
    }

    void Update()
    {
        // can these variables be null when it reaches here? UCAN
        if (entityToEdit == null)// nice guard claues instead of nested if statements <3
            return;
        if (animationClip == null)
            return;

        // Animate the GameObject
        if (!EditorApplication.isPlaying && AnimationMode.InAnimationMode())
        {
            AnimationMode.BeginSampling();
            AnimationMode.SampleAnimationClip(entityToEdit.gameObject, animationClip, time);
            AnimationMode.EndSampling();

            SceneView.RepaintAll();
        }
    }
}
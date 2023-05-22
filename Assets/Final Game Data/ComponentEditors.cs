using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using ExtensionMethods_AnimatorController;
using GeometryDefinitions;
namespace ComponentEditors
{
    public class Component_CircleCollider2D_EditorWindow : EditorWindow
    {
        GameObject animatorObject;
        Animator animator;
        SpriteRenderer spriteRenderer;
        //
        AnimatorController controller;
        AnimationClip clip;
        bool stateExistsInController;
        int frame;
        string stateName;
        //
        NewStateData stateDataToEdit;
        Component_CircleCollider2D componentSelected;
        Circle circleSelected;
        [MenuItem("Window/Component_CircleCollider2D_EditorWindow")]
        public static void ShowWindow()
        {
            Component_CircleCollider2D_EditorWindow editorWindow = GetWindow<Component_CircleCollider2D_EditorWindow>("Component_CircleCollider2D_EditorWindow");
            editorWindow.CreateAnimatorObject();
        }
        void CreateAnimatorObject()
        {
            animatorObject = new GameObject("EDITOR_ANIMATOR");
            animator = animatorObject.AddComponent<Animator>();
            spriteRenderer = animatorObject.AddComponent<SpriteRenderer>();
            Selection.activeObject = animatorObject;
        }
        void AnimatorControllerSelect()
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("CONTROLLER", EditorStyles.largeLabel);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            AnimatorController fieldInput = (AnimatorController)EditorGUILayout.ObjectField(controller, typeof(AnimatorController), false, GUILayout.ExpandWidth(true));
            if(fieldInput != controller)
            {
                controller = fieldInput;
                stateExistsInController = false;
                clip = null;
                stateExistsInController = false;
                frame = 0;
            }
            if (controller == null)
                return;
            animator.runtimeAnimatorController = controller as RuntimeAnimatorController;
        }
        void StateSelect()
        {
            if (controller == null)
                return;
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("STATE", EditorStyles.largeLabel);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            string fieldInput = EditorGUILayout.TextField(stateName);
            if(fieldInput != stateName)
            {
                stateName = fieldInput;
                stateExistsInController = controller.CheckStateIsInController(stateName);
                clip = controller.ClipFromStateHash(Animator.StringToHash(stateName));
                frame = 0;
            }
        }
        void FrameSelect()
        {
            if (!stateExistsInController)
                return;
            AnimationMode.StartAnimationMode();
            GUILayout.BeginHorizontal();
            if(GUILayout.Button(" < ") && frame > 0)
            {
                frame--;
            }
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label($"FRAME {frame}", EditorStyles.boldLabel);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            if(GUILayout.Button(" > ") && frame < clip.length)
            {
                frame++;
            }
            GUILayout.EndHorizontal();
        }
        void TryLoadStateDataAsset()
        {
            if (!stateExistsInController)
                return;
            NewStateData loadedData = Assets.LoadExistingAsset(controller, stateName);
            if(loadedData == null)
            {
                if(GUILayout.Button("CREATE NEW STATE DATA"))
                {
                    Assets.CreateNewAsset(controller, stateName);
                }
            }
            if(loadedData != null)
            {
                if(loadedData != stateDataToEdit)
                {
                    stateDataToEdit = loadedData;
                }
            }
        }
        private void OnFocus()
        {
            SceneView.duringSceneGui -= OnSceneGUI;
            SceneView.duringSceneGui += OnSceneGUI;
        }
        private void OnDestroy()
        {
            SceneView.duringSceneGui -= OnSceneGUI;
            if (animatorObject != null)
                DestroyImmediate(animatorObject);
            if (AnimationMode.InAnimationMode())
                AnimationMode.StopAnimationMode();
        }
        private void OnGUI()
        {
            AnimatorControllerSelect();
            StateSelect();
            FrameSelect();
            TryLoadStateDataAsset();
        }
        void OnSceneGUI(SceneView sceneView)
        {
            if (stateDataToEdit == null)
                return;
            if (stateDataToEdit.components == null || stateDataToEdit.components.Count == 0)
                return;
        }
        private void Update()
        {
            if (!stateExistsInController)
                return;
            if(!EditorApplication.isPlaying && AnimationMode.InAnimationMode())
            {
                if (clip == null)
                    return;
                AnimationMode.BeginSampling();
                AnimationMode.SampleAnimationClip(animatorObject, clip, frame);
                AnimationMode.EndSampling();
                SceneView.RepaintAll();
            }
        }
    }
}
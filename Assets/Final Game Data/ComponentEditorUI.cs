using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GeometryDefinitions;
using UnityEditor.Animations;
using UnityEditor;
using ExtensionMethods_AnimatorController;
using ComponentIO;
using System.Linq;
using ExtensionMethods_Color;
namespace ComponentEditorUI
{
    public static class ControllerAndStateEdit
    {
        public static bool ControllerSelect(ref AnimatorController controller)
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("ANIMATOR CONTROLLER", EditorStyles.largeLabel);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            AnimatorController fieldInput = (AnimatorController)EditorGUILayout.ObjectField(controller, typeof(AnimatorController), false, GUILayout.ExpandWidth(true));
            if(fieldInput != controller)
            {
                controller = fieldInput;
                return true;
            }
            return false;
        }
        public static bool StateSelect(ref string stateName)
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("STATE", EditorStyles.largeLabel);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            string fieldInput = EditorGUILayout.TextField(stateName);
            if(fieldInput != stateName)
            {
                stateName = fieldInput;
                return true;
            }
            return false;
        }
    }
    public class FrameEdit
    {
        public int frame;
        public FrameEdit()
        {
            frame = 0;
        }
        public void FrameSelect(bool stateExistsInController, AnimationClip clip)
        {
            if (!stateExistsInController)
                return;
            if (clip == null)
                return;
            AnimationMode.StartAnimationMode();
            GUILayout.BeginHorizontal();
            if (GUILayout.Button(" < ") && frame > 0)
            {
                frame--;
            }
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label($"FRAME {frame}", EditorStyles.boldLabel);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            if (GUILayout.Button(" > ") && frame < clip.length)
            {
                frame++;
            }
            GUILayout.EndHorizontal();
        }
        string GetAppliedFramesLabel(List<int> frames)
        {
            string appliedFramesLabel = "";
            if (frames == null || frames.Count == 0)
                return "NONE";
            for (int i = 0; i < frames.Count; i++)
            {
                if (i == 0)
                {
                    appliedFramesLabel = appliedFramesLabel + frames[i];
                }
                else
                {
                    appliedFramesLabel = appliedFramesLabel + ", " + frames[i];
                }
            }
            return appliedFramesLabel;
        }
        public void AppliedFramesEdit(ref List<int> frames)
        {
            if (frames == null || frames.Count == 0)
            {
                if (GUILayout.Button($"APPLY TO FRAME {frame}"))
                {
                    frames = new List<int>() { frame };
                }
                return;
            }
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label($"APPLIED TO FRAMES: {GetAppliedFramesLabel(frames)}", EditorStyles.boldLabel);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            if (frames.Contains(frame))
            {
                if (GUILayout.Button($"REMOVE FROM FRAME {frame}"))
                {
                    frames.Remove(frame);
                }
            }
            else
            {
                if (GUILayout.Button($"APPLY TO FRAME {frame}"))
                {
                    frames.Add(frame);
                }
            }
            if (GUILayout.Button("CLEAR ALL APPLIED FRAMES"))
            {
                frames.Clear();
            }
        }
    }
    public class ListSelectionAndEdit<T> where T : new()
    {
        public void ListEdit(ref SelectionList<T> selectedList, ref List<T> mainList)
        {
            GUILayout.Label($"{typeof(T).Name.ToUpper()} SELECTION MODE {selectedList.selectionMode.ToString().ToUpper()}", EditorStyles.boldLabel);
            GUILayout.BeginHorizontal();
            foreach (GeometrySelectionMode mode in Enum.GetValues(typeof(GeometrySelectionMode)))
            {
                if (GUILayout.Button(mode.ToString()))
                {
                    selectedList.selectionMode = mode;
                }
            }
            if (GUILayout.Button("All"))
            {
                selectedList.selectionMode = GeometrySelectionMode.Multiple;
                foreach (T item in mainList)
                {
                    if (selectedList.Contains(item))
                        continue;
                    selectedList.Add(item);
                }
            }
            GUILayout.EndHorizontal();
            if (GUILayout.Button($"ADD {typeof(T).Name.ToUpper()}"))
            {
                mainList.Add(new T());
            }
            if (mainList.Count == 0)
                return;
            for (int i = 0; i < mainList.Count; i++)
            {
                T item = mainList[i];
                string selectedLabel = (selectedList.Contains(item)) ? "[S] " : "";
                GUILayout.BeginHorizontal();
                if (GUILayout.Button($"{selectedLabel}{typeof(T).Name.ToUpper()} {i}"))
                {
                    if (selectedList.Contains(item))
                    {
                        selectedList.Remove(item);
                    }
                    else
                    {
                        selectedList.Add(item);
                    }
                }
                if (GUILayout.Button($"REMOVE {i}"))
                {
                    if (selectedList != null && selectedList.Contains(item))
                        selectedList.Remove(item);
                    mainList.Remove(item);
                    GUILayout.EndHorizontal();
                    return;
                }
                GUILayout.EndHorizontal();
            }
        }
    }
    public static class EditorGUICustomField
    {
        public static LayerMask LayerMaskField(string label, LayerMask layerMask)
        {
            LayerMask tempMask = EditorGUILayout.MaskField(label, UnityEditorInternal.InternalEditorUtility.LayerMaskToConcatenatedLayersMask(layerMask), UnityEditorInternal.InternalEditorUtility.layers);
            return UnityEditorInternal.InternalEditorUtility.ConcatenatedLayersMaskToLayerMask(tempMask);
        }
    }
    public static class DrawGeometry
    {
        public static void DrawCircleList(GameObject parentObject, ref SelectionList<Circle> selectedList, ref List<Circle> mainList, Color fillColor, Color lineColor)
        {
            if (mainList == null || mainList.Count == 0)
                return;
            for (int i = 0; i < mainList.Count; i++)
            {
                Circle circle = mainList[i];
                DrawCircle(parentObject, circle, selectedList.Contains(circle), fillColor, lineColor);
            }
            if (selectedList == null || selectedList.selectedItems == null || selectedList.selectedItems.Count == 0)
                return;
            DrawHandleForSelectionCircle(parentObject, ref selectedList, selectedList.selectedItems[0], fillColor, lineColor);
        }
        public static void DrawCircle(GameObject parentObject, Circle circle, bool isSelected, Color fillColor, Color lineColor)
        {
            Vector3 worldPos = parentObject.transform.position + circle.center;
            float radius = circle.radius;
            float alpha = 0.35f;
            if (isSelected)
                alpha = 1f;
            Handles.color = fillColor.WithTransparency(alpha);
            Handles.DrawSolidDisc(worldPos, Vector3.forward, radius);
            Handles.color = Color.black;
            Handles.DrawWireDisc(worldPos, Vector3.forward, radius, 4f);
            Handles.color = lineColor.WithTransparency(alpha);
            Handles.DrawDottedLine(worldPos, worldPos + Vector3.right * radius, 3f);
        }
        public static void DrawHandleForSelectionCircle(GameObject parentObject, ref SelectionList<Circle> selectedList, Circle circle, Color fillColor, Color lineColor)
        {
            Vector3 worldPos = parentObject.transform.position + circle.center;
            float radius = circle.radius;
            EditorGUI.BeginChangeCheck();
            //Square render first
            Handles.color = (Color.white - fillColor).WithTransparency(1f);
            Vector3 oldSquarePos = worldPos;
            Vector3 newSquarePos = Handles.FreeMoveHandle(oldSquarePos, Quaternion.identity, 0.65f, Vector3.one * 0.25f, Handles.RectangleHandleCap);
            //Arrow render last
            Handles.color = lineColor;
            Vector3 oldArrowPos = worldPos + Vector3.right * radius;
            Vector3 newArrowPos = Handles.Slider(oldArrowPos, Vector3.right, 1f, Handles.ArrowHandleCap, 0.1f);
            if (EditorGUI.EndChangeCheck())
            {
                foreach (Circle selectionCircle in selectedList.selectedItems)
                {
                    selectionCircle.radius += newArrowPos.x - oldArrowPos.x;
                    if (selectionCircle.radius < 0)
                        selectionCircle.radius = 0;
                    selectionCircle.center += newSquarePos - oldSquarePos;
                }
            }
        }
        public static void DrawAreaList(GameObject parentObject, ref SelectionList<Area> selectedList, ref List<Area> mainList, Color fillColor, Color lineColor)
        {
            if (mainList == null || mainList.Count == 0)
                return;
            for (int i = 0; i < mainList.Count; i++)
            {
                Area area = mainList[i];
                DrawArea(parentObject, area, selectedList.Contains(area), fillColor, lineColor);
            }
            if (selectedList == null || selectedList.selectedItems == null || selectedList.selectedItems.Count == 0)
                return;
            DrawHandleForSelectionArea(parentObject, ref selectedList, selectedList.selectedItems[0], fillColor, lineColor);
        }
        public static void DrawArea(GameObject parentObject, Area area, bool isSelected, Color fillColor, Color lineColor)
        {
            Vector3 worldPos = parentObject.transform.position + area.center;
            Vector3 extentX = Vector3.right * area.width/2;
            Vector3 extentY = Vector3.up * area.height/2;
            Vector3[] verts = new Vector3[]
            {
                worldPos - extentX - extentY,
                worldPos + extentX - extentY,
                worldPos + extentX + extentY,
                worldPos - extentX + extentY
            };
            float alpha = 0.15f;
            if (isSelected)
                alpha = 0.5f;
            Handles.DrawSolidRectangleWithOutline(verts, fillColor.WithTransparency(alpha), lineColor.WithTransparency(alpha));
        }
        public static void DrawHandleForSelectionArea(GameObject parentObject, ref SelectionList<Area> selectedList, Area area, Color fillColor, Color lineColor)
        {
            Vector3 worldPos = parentObject.transform.position + area.center;
            EditorGUI.BeginChangeCheck();
            //Square render first
            Handles.color = (Color.white - fillColor).WithTransparency(1f);
            Vector3 oldSquarePos = worldPos;
            Vector3 newSquarePos = Handles.FreeMoveHandle(oldSquarePos, Quaternion.identity, 0.25f, Vector3.one * 0.1f, Handles.RectangleHandleCap);
            Handles.color = lineColor;
            Vector3 oldWidthArrowPos = worldPos + Vector3.right * area.width/2;
            Vector3 newWidthArrowPos = Handles.Slider(oldWidthArrowPos, Vector3.right, 1f, Handles.ArrowHandleCap, 0.1f);
            Handles.color = lineColor;
            Vector3 oldHeightArrowPos = worldPos + Vector3.up * area.height / 2;
            Vector3 newHeightArrowPos = Handles.Slider(oldHeightArrowPos, Vector3.up, 1f, Handles.ArrowHandleCap, 0.1f);
            if (EditorGUI.EndChangeCheck())
            {
                foreach (Area selectionArea in selectedList.selectedItems)
                {
                    selectionArea.width += newWidthArrowPos.x - oldWidthArrowPos.x;
                    if (selectionArea.width < 0)
                        selectionArea.width = 0;
                    selectionArea.height += newHeightArrowPos.y - oldHeightArrowPos.y;
                    if (selectionArea.height < 0)
                        selectionArea.height = 0;
                    selectionArea.center += newSquarePos - oldSquarePos;
                }
            }
        }
    }
}
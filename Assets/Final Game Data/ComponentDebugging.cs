using ExtensionMethods_Color;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GeometryDefinitions;
using Mono.Cecil.Cil;
using UnityEngine.Analytics;

namespace GameManagement
{
    public class ComponentDebugging : MonoBehaviour
    {
        public static ComponentDebugging ins;
        [Range(0f, 1f)]
        public float colliderDebugAlpha;
        [Range(0f, 1f)]
        public float overlapDebugAlpha;
        public List<Image> circleDebugObjects;
        public CircleCollider2D[] circleColliders;
        public List<Image> overlapCircleDebugObjects;
        public List<Image> overlapAreaDebugObjects;
        public List<(RuntimeSceneObject, Component_Overlap_Data, float)> overlapsActive;
        private void Awake()
        {
            ins = this;
        }
        public void ManagedUpdate()
        {
            for (int i = 0; i < circleDebugObjects.Count; i++)
            {
                if (i == circleColliders.Length)
                    break;
                if (!circleColliders[i].gameObject.activeSelf)
                    continue;
                circleDebugObjects[i].rectTransform.anchoredPosition = circleColliders[i].transform.position;
            }
            DrawOverlaps();
            StopDrawInactiveOverlap();
        }
        public void DrawCircleCollider(int i, Color fillColor)
        {
            Image image = circleDebugObjects[i];
            image.color = fillColor.WithTransparency(colliderDebugAlpha);
            image.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, circleColliders[i].radius*2);
            image.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, circleColliders[i].radius*2);
        }
        public void StopDrawCircleCollider(int i)
        {
            circleDebugObjects[i].color = Color.clear;
        }
        void StopDrawInactiveOverlap()
        {
            if(overlapsActive == null || overlapsActive.Count == 0)
            {
                for (int i = 0; i < overlapCircleDebugObjects.Count; i++)
                {
                    if (!overlapCircleDebugObjects[i].gameObject.activeSelf)
                        continue;
                    overlapCircleDebugObjects[i].gameObject.SetActive(false);
                }
                for (int i = 0; i < overlapAreaDebugObjects.Count; i++)
                {
                    if (!overlapAreaDebugObjects[i].gameObject.activeSelf)
                        continue;
                    overlapAreaDebugObjects[i].gameObject.SetActive(false);
                }
                return;
            }
        }
        public void DrawOverlaps()
        {
            if (overlapsActive == null || overlapsActive.Count == 0)
                return;
            int assignedCircles = 0;
            int assignedAreas = 0;
            for (int i = 0; i < overlapsActive.Count; i++)
            {
                RuntimeSceneObject obj = overlapsActive[i].Item1;
                Component_Overlap_Data componentData = overlapsActive[i].Item2;
                float heldTime = overlapsActive[i].Item3;
                DrawOverlapCircles(obj, componentData.circles, componentData.fillColor, ref assignedCircles);
                DrawOverlapAreas(obj, componentData.areas, componentData.fillColor, ref assignedAreas);
            }
            for (int i = assignedCircles; i < overlapCircleDebugObjects.Count; i++)
            {
                if (!overlapCircleDebugObjects[i].gameObject.activeSelf)
                    continue;
                overlapCircleDebugObjects[i].gameObject.SetActive(false);
            }
            for (int i = assignedAreas; i < overlapAreaDebugObjects.Count; i++)
            {
                if (!overlapAreaDebugObjects[i].gameObject.activeSelf)
                    continue;
                overlapAreaDebugObjects[i].gameObject.SetActive(false);
            }
        }
        void DrawOverlapCircles(RuntimeSceneObject obj, List<Circle> circles, Color fillColor, ref int assignedCircles)
        {
            if (circles == null || circles.Count == 0)
                return;
            if (overlapCircleDebugObjects == null || overlapCircleDebugObjects.Count - assignedCircles < circles.Count)
            {
                Debug.LogError("Not enough overlapCircleDebugObjects! Cancelling debug.");
                return;
            }
            for (int i = 0; i < circles.Count; i++)
            {
                Circle circle = circles[i];
                Vector3 worldPos = obj.LocalPosFromTransform(circle.center);
                if(!overlapCircleDebugObjects[assignedCircles].gameObject.activeSelf)
                    overlapCircleDebugObjects[assignedCircles].gameObject.SetActive(true);
                Image image = overlapCircleDebugObjects[assignedCircles];
                image.color = fillColor.WithTransparency(overlapDebugAlpha);
                image.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, circle.radius * 2);
                image.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, circle.radius * 2);
                image.rectTransform.anchoredPosition = worldPos;
                assignedCircles++;
            }
        }
        void DrawOverlapAreas(RuntimeSceneObject obj, List<Area> areas, Color fillColor, ref int assignedAreas)
        {
            if (areas == null || areas.Count == 0)
                return;
            if(overlapAreaDebugObjects == null || overlapAreaDebugObjects.Count - assignedAreas < areas.Count)
            {
                Debug.LogError("Not enough overlapAreaDebugObjects! Cancelling debug.");
                return;
            }
            for (int i = 0; i < areas.Count; i++)
            {
                Area area = areas[i];
                Vector3 worldPos = obj.LocalPosFromTransform(area.center);
                if (!overlapAreaDebugObjects[assignedAreas].gameObject.activeSelf)
                    overlapAreaDebugObjects[assignedAreas].gameObject.SetActive(true);
                Image image = overlapAreaDebugObjects[assignedAreas];
                image.color = fillColor.WithTransparency(overlapDebugAlpha);
                image.transform.right = obj.transform.right;
                image.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, area.width);
                image.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, area.height);
                image.rectTransform.anchoredPosition = worldPos;
                assignedAreas++;
            }
        }
    }
}
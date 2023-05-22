using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OLD.Geometry;
using System;
using OLD.GameData.EditorWindow;
using UnityEditor;

namespace OLD.GameData.StateData
{
    [Serializable]
    public class CircleCollider2DCollection : Draw
    {
        public bool isTrigger;
        public int layer;
        public List<Circle> circles;
        private ButtonList<Circle> buttonList;
        public CircleCollider2DCollection()
        {
            isTrigger = false;
            layer = 0;
            circles = new List<Circle>();
        }
        public void Draw()
        {
            if(buttonList == null)
            {
                buttonList = new ButtonList<Circle>(circles);
                buttonList.onButtonPress += (circleToDraw) =>
                {
                    buttonList.UnsubscribeAll();
                    SceneGUI sceneGUI = circleToDraw as SceneGUI;
                    if (sceneGUI != null)
                        sceneGUI.SubscribeGUI();
                };
                buttonList.onAddButtonPress += () =>
                {
                    return new Circle();
                };
                buttonList.onRemoveButtonPress += (circleToRemove) =>
                {
                    circleToRemove.UnsubscribeGUI();
                    circles.Remove(circleToRemove);
                };
            }
            buttonList.Draw();
        }
    }
}
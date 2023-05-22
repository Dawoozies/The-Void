using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OLD.Geometry;
using System;
using ExtensionMethods_List;
using UnityEditor;
using OLD.GameData.EditorWindow;
using UnityEngine.UIElements;

namespace OLD.GameData.StateData
{
    [Serializable]
    public class CircleCollider2DStateData : ScriptableObject, ScriptableObjectInitialization, StateDataCast<CircleCollider2DStateData>, Draw
    {
        public List<CircleCollider2DCollection> circleCollider2DCollection;
        private ButtonList<CircleCollider2DCollection> buttonList;
        private CircleCollider2DCollection collectionToDraw;
        public void Initialize()
        {
            circleCollider2DCollection = new List<CircleCollider2DCollection>() { new CircleCollider2DCollection()};
        }
        public CircleCollider2DStateData GetCastedData()
        {
            return this;
        }
        public void Draw()
        {
            if(buttonList == null)
            {
                buttonList = new ButtonList<CircleCollider2DCollection>(circleCollider2DCollection);
                buttonList.onButtonPress += (collection) =>
                {
                    if (collectionToDraw != null && collectionToDraw == collection)
                    {
                        collectionToDraw = null;
                        return;
                    }
                    collectionToDraw = collection;
                };
                buttonList.onAddButtonPress += () =>
                {
                    return new CircleCollider2DCollection();
                };
            }
            buttonList.Draw();
            if (collectionToDraw != null)
                collectionToDraw.Draw();
            EditorUtility.SetDirty(this);
        }
    }
}
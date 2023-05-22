using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using OLD.Geometry;
using ExtensionMethods_List;
using OLD.GameData.StateData.IO;
using System.Linq;
using System.Reflection;
namespace OLD.GameData.StateData
{
    [Serializable]
    public class StateData : ScriptableObject, ScriptableObjectInitialization
    {
        public int stateHash;
        public StateComponentDictionary data;
        public void Initialize()
        {
            this.stateHash = 0;
            data = new StateComponentDictionary();
        }
        public Components ComponentsAtFrame(int frame)
        {
            if (!data.ContainsKey(frame))
                return null;
            return data.ValueWithKey(frame);
        }
        public void AddNewComponentToFrame(int frame, ScriptableObject component)
        {
            data.Add(frame, component);
            Debug.Log("Running AddNewComponentToFrame");
        }
        public Type[] ComponentsNotAddedAtFrame(int frame)
        {
            if (!data.ContainsKey(frame))
                return ComponentTypes.All;
            return ComponentTypes.All.Except(data.ValueWithKey(frame).components
                .Select(c => c.GetType())
                ).ToArray();
        }
        public void Draw_ComponentAdd(int frame)
        {
            ComponentsNotAddedAtFrame(frame)
                .Select(c => typeof(Draw<>).MakeGenericType(c))
                .Select(drawType => drawType.GetMethod("NewComponentOptions"))
                .Where(drawMethod => drawMethod != null)
                .ToList()
                .ForEach(drawMethod => drawMethod.Invoke(null, new object[] { this, frame }));
        }
        public void Draw_StateDataAtFrame(int frame)
        {
            Components componentsAtFrame = ComponentsAtFrame(frame);
            if (componentsAtFrame == null)
                return;
            componentsAtFrame.GetScriptableObjects()
                .ForEach(scriptableObject => 
                {
                    Draw draw = scriptableObject as Draw;
                    if (draw != null)
                        draw.Draw();
                });
        }
    }
}
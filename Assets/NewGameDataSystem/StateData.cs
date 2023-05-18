using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Geometry;
using ExtensionMethods_List;
using GameData.StateData.IO;
using System.Linq;
namespace GameData.StateData
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
            if(!data.ContainsKey(frame))
            {
                ComponentTypes.All
                    .Select(c => typeof(Draw<>).MakeGenericType(c))
                    .Select(drawType => drawType.GetMethod("CreateComponentButton"))
                    .Where(drawMethod => drawMethod != null)
                    .ToList()
                    .ForEach(drawMethod => drawMethod.Invoke(null, new object[] { this, frame}));
            }
        }
    }
}
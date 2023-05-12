using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using GameData.BlackboardManager;
namespace Math.BoardStructure
{
    //A place for storing on the fly data
    //When needed, if not already there, something we actively care about is added to the blackboard
    //That object is labelled and it's type is stored. A simple retrieval of the object when required
    //Objects are categorised by type not by context of use
    public class Blackboard<T>
    {
        Dictionary<string, T> _boardData;
        //Constructor
        public Blackboard ()
        {
            _boardData = new Dictionary<string, T>();
        }
        public bool IsOnBoard(string variableName)
        {
            return _boardData.ContainsKey(variableName);
        }
        public void WriteToBoard(string variableName, T variable)
        {
            _boardData.Add(variableName, variable);
        }
        public T ReadFromBoard(string variableName)
        {
            if(!IsOnBoard(variableName))
            {
                Debug.LogError($"Trying to get {variableName} from a Blackboard but this variable hasn't been written yet");
                return default(T);
            }

            return _boardData[variableName];
        }
    }
    public static class ObjectFind
    {
        public static GameObject GameObjectFind(string gameObjectName) { return GameObject.Find(gameObjectName); }
        public static Rigidbody2D Rigidbody2DFind(string gameObjectName) { return GameObject.Find(gameObjectName).GetComponent<Rigidbody2D>(); }
        public static Animator AnimatorFind(string gameObjectName) { return GameObject.Find(gameObjectName).GetComponent<Animator>(); }
    }
}
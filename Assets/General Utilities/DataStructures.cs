using UnityEngine;
using System.Collections.Generic;
using System;
namespace DataStructures
{
    //A timed list will return and remove elements after a set time
    public class TimedList<T>
    {
        readonly List<(T, float)> _list;

        public void Add(T element, float timerLength)
        {
            if(timerLength <= 0){ Debug.LogError("Trying to add a TimedList element with removal time less than or equal to 0."); return; }

            _list.Add((element, timerLength));
        }

        public List<T> Update(float timeStep)
        {
            List<T> timedOutElements = new List<T>();

            for (int i = 0; i < _list.Count; i++)
            {
                if (_list[i].Item2 <= 0)
                { timedOutElements.Add(_list[i].Item1); _list.RemoveAt(i); continue; }
                
                _list[i] = (_list[i].Item1, _list[i].Item2 - timeStep);
            }

            return timedOutElements;
        }
    }
}
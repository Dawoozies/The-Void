using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class DataCache<T>
{
    private Dictionary<string, T> dataDictionary;
    public void Initialize(Animator entityAnimator, Rigidbody2D entityRigidbody)
    {
        dataDictionary = new Dictionary<string, T>();
        T[] loadedData = Resources.LoadAll(typeof(T).Name) as T[];
        if(loadedData?.Length > 0)
        {

        }
    }
}

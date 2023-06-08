using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager ins;
    public RuntimeAnimatorController[] controllers;
    public Dictionary<string, ControllerData> allControllerData;
    public Dictionary<string, RuntimeObject> allObjects;
    private void Awake()
    {
        ins = this;
        Setup_ControllerData();
    }
    void Setup_ControllerData()
    {
        ControllerData[] loadedControllerData = ResourcesUtility.LoadAllControllerDataInResources();
        allControllerData = new Dictionary<string, ControllerData>();
        int dataAssigned = 0;
        foreach (ControllerData controllerData in loadedControllerData)
        {
            if (allControllerData.ContainsValue(controllerData))
                continue;
            for (int i = 0; i < controllers.Length; i++)
            {
                if (allControllerData.ContainsKey(controllers[i].name))
                    continue;
                if (controllers[i].name == controllerData.controllerName)
                {
                    Debug.Log($"allControllerData Caching: {controllers[i].name} Controller Data");
                    allControllerData.Add(controllers[i].name, controllerData);
                    dataAssigned++;
                }
                if(dataAssigned == controllers.Length)
                {
                    Debug.Log("All controller data assigned");
                    return;
                }
            }
        }
    }
    private void Start()
    {
        
    }
}

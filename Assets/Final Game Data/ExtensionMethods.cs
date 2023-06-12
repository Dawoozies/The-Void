using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RuntimeObjects;
public static class ExtensionMethods
{
    public static bool HasFlag(this RuntimeObjectStructure obj, RuntimeObjectStructure checkFlag)
    {
        return (obj & checkFlag) == checkFlag;
    }
#if UNITY_EDITOR
    public static bool CheckComplementOfFlag(this DataSelection obj, DataSelection basisToIgnore)
    {
        Debug.Log("Check complement of flag");
        foreach (DataSelection enumValue in Enum.GetValues(typeof(DataSelection)))
        {
            if (enumValue == DataSelection.None)
                continue;
            if (enumValue == basisToIgnore)
                continue;
            if ((obj & enumValue) == enumValue)
                return false;
        }
        Debug.Log("Return true");
        return true;
    }
#endif
}
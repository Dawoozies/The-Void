using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GeometryDefinitions;

[Serializable]
public class Component_Overlap : ScriptableObject
{
    //Only one component per state
    public int stateHash;
    public Component_Overlap_Data[] componentData;
}
[Serializable]
public class Component_Overlap_Data
{
    public string nickname;
    public int frame;
    public Color fillColor;
    public Color lineColor;
    public List<Circle> circles;
    public List<Box> boxes;
    public List<Area> areas;
    public Component_Overlap_Data()
    {
        nickname = "Overlap_Data";
        frame = -1;
        fillColor = Color.cyan;
        lineColor = Color.red;
        circles = new List<Circle>();
        boxes = new List<Box>();
        areas = new List<Area>();
    }
}
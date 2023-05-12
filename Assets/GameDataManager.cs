using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Math.BoardStructure;
using System;
public class GameDataManager : MonoBehaviour
{
    public static GameDataManager ins;
    private void Awake()
    {
        ins = this;
    }
    public Vector3 WorldGravityDirection;
}
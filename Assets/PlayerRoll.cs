using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRoll : MonoBehaviour
{
    bool rollInput => BasicInput.ins.InputRoll;
    Animator animator;
    void Start()
    {
        animator = GetComponentInChildren<Animator>();
    }
    void Update()
    {
        
    }
}

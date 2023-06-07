using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;

public class TEST : MonoBehaviour
{
    Animator animator;
    public RuntimeAnimatorController controller;
    public RuntimeAnimatorController controllerTwo;
    public bool useControllerOne = true;
    void Awake()
    {
        animator = gameObject.AddComponent<Animator>();
        controller = Resources.Load("TEST") as RuntimeAnimatorController;
        controllerTwo = Resources.Load("TEST 1") as RuntimeAnimatorController;
        
    }

    // Update is called once per frame
    void Update()
    {
        if (controller == null)
            return;
        if(controllerTwo == null)
            return;
        if (useControllerOne)
        {
            animator.runtimeAnimatorController = controller;
        }
        else
        {
            animator.runtimeAnimatorController = controllerTwo;
        }
    }
}

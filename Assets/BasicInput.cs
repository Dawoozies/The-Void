using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicInput : MonoBehaviour
{
    public static BasicInput ins;
    private void Awake()
    {
        ins = this;
    }

    public string ControllerType = "PS4";

    public int InputLDirection = 1;
    public float InputLHorizontal;
    public float InputLVertical;
    public bool InputJump;

    //Hold Times
    public float LeftHeldTime;
    public float RightHeldTime;


    private void Update()
    {
        //Direction held times
        if(Input.GetAxis("L_Horizontal") > 0)
        {
            //Direction to Right
            RightHeldTime += Time.deltaTime;
            LeftHeldTime = 0;
        }
        else if(Input.GetAxis("L_Horizontal") < 0)
        {
            //Direction to Left
            LeftHeldTime += Time.deltaTime;
            RightHeldTime = 0;
        }
        else
        {
            //No Direction
            LeftHeldTime = 0;
            RightHeldTime = 0;
        }

        InputLHorizontal = Input.GetAxis("L_Horizontal");
        InputLVertical = Input.GetAxis("L_Vertical");

        InputJump = JumpInputCheck();

        if(InputLHorizontal > 0)
        {
            InputLDirection = 1;
        }
        if(InputLHorizontal < 0)
        {
            InputLDirection = -1;
        }
    }

    bool JumpInputCheck()
    {
        return Input.GetButton($"Jump{ControllerType}");
    }
}

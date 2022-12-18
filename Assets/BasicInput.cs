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
    public int InputLDirection = 1;
    public float InputLHorizontal;
    public float InputLVertical;


    private void Update()
    {
        InputLHorizontal = Input.GetAxis("L_Horizontal");
        InputLVertical = Input.GetAxis("L_Vertical");

        if(InputLHorizontal > 0)
        {
            InputLDirection = 1;
        }
        if(InputLHorizontal < 0)
        {
            InputLDirection = -1;
        }
    }
}

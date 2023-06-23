using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Direction
{
    static Vector2 R_Input => InputManager.ins.R_Input;
    public static int R_Direction;
    public static int Compute8WayDirection()
    {
        float xCoordinate = R_Input.x;
        float yCoordinate = R_Input.y;
        float angle = Mathf.Atan2(yCoordinate, xCoordinate)*Mathf.Rad2Deg;
        if(-22.5f < angle && angle < 22.5f)
        {
            //Horizontal + Right
            return 1;
        }
        if(22.5f <= angle && angle <= 67.5f)
        {
            //Diag Up + Right
            return 2;
        }
        if(67.5f < angle && angle < 112.5f)
        {
            //Up
            return 3;
        }
        if(112.5f <= angle && angle <= 157.5f)
        {
            //Diag Up + Left
            return 4;
        }
        if(157.5f < angle || angle < -157.5f)
        {
            //Horizontal + Left
            return 5;
        }
        if(-157.5f <= angle && angle <= -112.5f)
        {
            //Diag Down + Left
            return 6;
        }
        if(-112.5f < angle && angle < -67.5f)
        {
            //Down
            return 7;
        }
        if(-67.5f <= angle && angle <= -22.5f)
        {
            //Diag Down + Right
            return 8;
        }

        Debug.LogError("DirectionManager Error: Something went wrong, none of the 8 directions were returned!");
        return -1;
    }
}
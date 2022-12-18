using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    public Transform target;
    public Vector3 attackPositionOffset = new Vector3(0f,5f,0f);
    public Vector3 targetPosition;
    public bool getTargetPosition;
    public bool moveToAttackPosition;
    Vector3 velocity;
    public float smoothTime;
    public float aggression = 1f;
    public bool AllowTargeting;
    public float MoveCooldown = 5f;
    public float TimeTillNextMove = 0f;
    void Start()
    {
        TimeTillNextMove = MoveCooldown;
    }
    void Update()
    {
        if(AllowTargeting)
        {
            if(TimeTillNextMove > 0)
            {
                TimeTillNextMove -= Time.deltaTime;
            }
            else
            {
                getTargetPosition = true;
                moveToAttackPosition = true;
                AllowTargeting = false;
                TimeTillNextMove = MoveCooldown/aggression;
            }
        }

        if(getTargetPosition)
        {
            targetPosition = target.position;
            getTargetPosition = false;
        }

        if(moveToAttackPosition)
        {
            if (Vector3.Distance(transform.position, targetPosition + attackPositionOffset) > 0.05f)
            {
                //Then we should be moving to attackPosition
                transform.position = Vector3.SmoothDamp(transform.position, targetPosition + attackPositionOffset, ref velocity, smoothTime / aggression);
            }
            else
            {
                moveToAttackPosition = false;
                velocity = Vector3.zero;

                //Putting this here causes a loop
                AllowTargeting = true;
            }
        }
    }
}

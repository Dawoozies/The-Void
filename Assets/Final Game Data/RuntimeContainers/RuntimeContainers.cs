using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RuntimeObjects;
using System;

namespace RuntimeContainers
{
    public class DirectedCircleColliderContainer
    {
        public RuntimeObject obj;
        public string colliderNickname;
        public CircleCollider2D collider;
        public Vector2 up;
        public Vector2 right;
    }
    //Maybe could make player inventory a runtime container
    //Or maybe just make an actual inventory for the player lmao
    public class PlayerDamageContainer
    {
        public string stateName; //Animation state which dealt this damage
        public float percentage; //How much percentage damage to apply
        public Vector2 up; //"Up" direction for damage
        public Vector2 right; //"Right" direction for damage
        public bool killDamage;
        public PlayerDamageContainer(string stateName, float percentage, Vector2 up, Vector2 right, bool killDamage)
        {
            this.stateName = stateName;
            this.percentage = percentage;
            this.up = up;
            this.right = right;
            this.killDamage = killDamage;
        }
    }
    public class TimedAction
    {
        public float timeMax;
        public float timeLeft;
        public Action onTimerEnd; //So we can subscribe methods on runtimeobjects
        public bool isActive = false;
        public bool loop = false;
        public TimedAction(float timeMax, bool loop)
        {
            this.timeMax = timeMax;
            timeLeft = timeMax;
            this.loop = loop;
        }
        public TimedAction(float timeMax, bool loop, Action onTimerEnd)
        {
            this.timeMax = timeMax;
            timeLeft = timeMax;
            this.loop = loop;
            this.onTimerEnd += onTimerEnd;
        }
        public void Update(RuntimeObject obj, float tickDelta)
        {
            if(isActive || loop)
                timeLeft -= tickDelta;

            if(timeLeft < 0)
            {
                if(loop)
                {
                    timeLeft = timeMax;
                    isActive = true;
                }
                else
                {
                    timeLeft = 0;
                    isActive = false;
                }
                onTimerEnd?.Invoke();
            }
        }
        public void Activate()
        {
            timeLeft = timeMax;
            isActive = true;
        }
    }
    public class BoundedRandomFloat
    {
        //When you access this, it will spit out random number between defined bounds
        public float upperBound;
        public float lowerBound;
        public float value;
        public BoundedRandomFloat(float upperBound, float lowerBound, float baseValue)
        {
            this.upperBound = upperBound;
            this.lowerBound = lowerBound;
            value = baseValue;
        }

        public void ReRollValue()
        {
            value = UnityEngine.Random.Range(lowerBound, upperBound);
        }
    }
}
//Hitting a DirectedCircleCollider / RuntimeObject
//Hitting a non RuntimeObject
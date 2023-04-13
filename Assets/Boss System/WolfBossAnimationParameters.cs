using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WolfBossAnimationParameters : MonoBehaviour
{
    public Animator WolfHeadAnimator;
    public bool swipe;
    public bool swipeStart;
    void Update()
    {
        if (WolfHeadAnimator == null)
            return;

        WolfHeadAnimator.SetBool("SwipeStart", swipeStart);
        WolfHeadAnimator.SetBool("Swipe", swipe);
        
    }
}

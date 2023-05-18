using UnityEngine;
using UnityEditor.Animations;
using UnityEditor;
public static class AnimatorStateUtilities
{
    public static AnimationClip GetAnimationClipFromStateHash(AnimatorController controller, int stateHash)
    {
        if (controller == null)
            return null;
        foreach (AnimatorControllerLayer layer in controller.layers)
        {
            AnimatorStateMachine stateMachine = layer.stateMachine;
            foreach (ChildAnimatorState state in stateMachine.states)
            {
                AnimatorState animatorState = state.state;
                if (stateHash == animatorState.nameHash)
                {
                    if(animatorState.motion is AnimationClip animationClip)
                        return animatorState.motion as AnimationClip;
                }
            }
        }
        return null;
    }
}

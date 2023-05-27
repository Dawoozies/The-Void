using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Animations;
namespace ExtensionMethods_AnimatorController
{
    public static class Extensions
    {
        public static bool CheckStateIsInController(this AnimatorController controller, string stateName)
        {
            if (controller == null)
                return false;
            if (stateName == null || stateName.Length == 0)
                return false;
            foreach (AnimatorControllerLayer layer in controller.layers)
            {
                AnimatorStateMachine stateMachine = layer.stateMachine;
                foreach (ChildAnimatorState state in stateMachine.states)
                {
                    AnimatorState animatorState = state.state;
                    if(Animator.StringToHash(stateName) == animatorState.nameHash)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        public static bool CheckStateIsInController(this AnimatorController controller, int stateHash)
        {
            if (controller == null)
                return false;
            foreach (AnimatorControllerLayer layer in controller.layers)
            {
                AnimatorStateMachine stateMachine = layer.stateMachine;
                foreach (ChildAnimatorState state in stateMachine.states)
                {
                    AnimatorState animatorState = state.state;
                    if(stateHash == animatorState.nameHash)
                        return true;
                }
            }
            return false;
        }
        public static AnimationClip ClipFromStateHash(this AnimatorController controller, int stateHash)
        {
            if (controller == null)
                return null;
            foreach (AnimatorControllerLayer layer in controller.layers)
            {
                AnimatorStateMachine stateMachine = layer.stateMachine;
                foreach (ChildAnimatorState state in stateMachine.states)
                {
                    AnimatorState animatorState = state.state;
                    if(stateHash == animatorState.nameHash)
                    {
                        if (animatorState.motion is AnimationClip)
                            return animatorState.motion as AnimationClip;
                    }
                }
            }
            return null;
        }
    }
}
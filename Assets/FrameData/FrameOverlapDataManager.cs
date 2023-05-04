using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExtensionMethods_Animator;
public class FrameOverlapDataManager : MonoBehaviour
{
    Animator animator;
    public bool showGizmos;
    public float gizmosTransparency = 1f;
    public List<Collider2D> result;
    public AnimationClip clip;
    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    private void Update()
    {
        if (animator == null)
            return;

        if (FrameOverlapDataCache.ins.GetOverlapData(animator.GetCurrentAnimatorClipInfo(0)[0].clip) == null)
            return;
        clip = animator.GetCurrentAnimatorClipInfo(0)[0].clip;
        FrameOverlapData overlapData = FrameOverlapDataCache.ins.GetOverlapData(animator.GetCurrentAnimatorClipInfo(0)[0].clip);
        OverlapData currentFrameData = overlapData.overlapDataList[animator.CurrentFrame()];
        foreach (OverlapComponent component in currentFrameData.overlapComponents)
        {
            if((component.overlapComponentType & OverlapComponentType.Collider) == OverlapComponentType.Collider)
            {
                CircleCollider2DPoolManager.ins.UpdatePool(transform, component);
            }
            if ((component.overlapComponentType & OverlapComponentType.Cast) == OverlapComponentType.Cast)
            {
                result = component.CastCircles(transform);
                if (component.parameterComponents == null || component.parameterComponents.Count == 0)
                    continue;
                foreach (ParameterComponent parameterComponent in component.parameterComponents)
                {
                    parameterComponent.InvokeComponentAction(animator, result);
                }
            }
        }
    }
    private void OnDrawGizmos()
    {
        if (!showGizmos)
            return;
        if (animator == null)
            return;

        if (FrameOverlapDataCache.ins.GetOverlapData(animator.GetCurrentAnimatorClipInfo(0)[0].clip) == null)
            return;
        FrameOverlapData overlapData = FrameOverlapDataCache.ins.GetOverlapData(animator.GetCurrentAnimatorClipInfo(0)[0].clip);
        OverlapData currentFrameData = overlapData.overlapDataList[animator.CurrentFrame()];
        foreach (OverlapComponent component in currentFrameData.overlapComponents)
        {
            List<Geometry.Circle> circles = component.circles;
            if (circles == null || circles.Count == 0)
                continue;
            Gizmos.color = new Color(component.circleColor.r, component.circleColor.g, component.circleColor.b, gizmosTransparency);
            foreach (Geometry.Circle circle in circles)
            {
                Gizmos.DrawSphere(component.GetCircleWorldPosition(transform, circle), circle.radius);
            }
        }
    }
}

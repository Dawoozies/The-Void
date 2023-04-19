using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExtensionMethods_Animator;
public class GroundboxManager : MonoBehaviour
{
    CollisionManager collisionManager;
    Animator animator;
    public LayerMask layerMask;
    public bool showGizmos = false;
    private List<FrameCollisionData> groundboxData;
    Listener_GroundboxOverlap[] listeners_GroundboxOverlap;
    void Start()
    {
        collisionManager = GetComponent<CollisionManager>();
        animator = GetComponentInChildren<Animator>();
        listeners_GroundboxOverlap = GetComponentsInChildren<Listener_GroundboxOverlap>();
    }
    void FixedUpdate()
    {
        string clipName = animator.GetCurrentAnimatorClipInfo(0)[0].clip.name;
        groundboxData = collisionManager.GetGroundboxData(clipName);
        if (groundboxData == null || groundboxData.Count == 0)
            return;
        int frame = animator.CurrentFrame();
        List<Circle> castCircles = groundboxData[frame].circles;
        if (castCircles == null || castCircles.Count == 0)
            return;
        bool hitResult = false;
        for (int i = 0; i < castCircles.Count; i++)
        {
            Vector3 circleWorldPos = transform.position + new Vector3(
                castCircles[i].center.x * animator.transform.localScale.x,
                castCircles[i].center.y * animator.transform.localScale.y,
                0f);
            float circleRadius = castCircles[i].radius;
            Collider2D[] overlappedColliders = Physics2D.OverlapCircleAll(circleWorldPos, circleRadius, layerMask);
            hitResult = !(overlappedColliders == null) && !(overlappedColliders.Length == 0);
            if (hitResult)
                break;
        }
        HandleListeners_GroundboxOverlap(hitResult);
    }
    void HandleListeners_GroundboxOverlap(bool hitResult)
    {
        if (listeners_GroundboxOverlap == null || listeners_GroundboxOverlap.Length == 0)
            return;
        for (int i = 0; i < listeners_GroundboxOverlap.Length; i++)
        {
            listeners_GroundboxOverlap[i].Update_GroundboxOverlap(hitResult);
        }
    }
    private void OnDrawGizmos()
    {
        if (!showGizmos)
            return;
        if (groundboxData == null || groundboxData.Count == 0)
            return;
        int frame = animator.CurrentFrame();
        List<Circle> castCircles = groundboxData[frame].circles;
        if (castCircles == null || castCircles.Count == 0)
            return;
        for (int i = 0; i < castCircles.Count; i++)
        {
            Vector3 circleWorldPos = transform.position + new Vector3(
                castCircles[i].center.x * animator.transform.localScale.x,
                castCircles[i].center.y * animator.transform.localScale.y,
                0f);
            float circleRadius = castCircles[i].radius;
            Gizmos.color = new Color(0, 1, 0, 0.65f);
            Gizmos.DrawSphere(circleWorldPos, circleRadius);
        }
    }
}

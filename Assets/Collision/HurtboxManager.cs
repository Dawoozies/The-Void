using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExtensionMethods_Animator;
using ExtensionMethods_List;
public class HurtboxManager : MonoBehaviour
{
    CollisionManager collisionManager;
    Animator animator;

    AnimatorClipInfo currentClipInfo;
    List<FrameCollisionData> hurtboxes = new List<FrameCollisionData>();

    int frame;

    List<HurtboxOverlap> lastHurtboxOverlap = new List<HurtboxOverlap>();
    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        collisionManager = GetComponent<CollisionManager>();

        if (animator == null)
        {
            Debug.LogError("Hurtbox Manager Error: No Animator component found");
            return;
        }

        if (currentClipInfo.clip == null)
        {
            currentClipInfo = animator.GetCurrentAnimatorClipInfo(0)[0];

            hurtboxes = collisionManager.GetHurtboxData(currentClipInfo.clip.name);
        }
    }
    void Update()
    {
        if(currentClipInfo.clip != null)
        {
            if(currentClipInfo.clip.name != animator.GetCurrentAnimatorClipInfo(0)[0].clip.name)
            {
                currentClipInfo = animator.GetCurrentAnimatorClipInfo(0)[0];

                hurtboxes = collisionManager.GetHurtboxData(currentClipInfo.clip.name);
            }
        }

        currentClipInfo = animator.GetCurrentAnimatorClipInfo(0)[0];

        frame = animator.CurrentFrame();

        CastHurtboxes();
    }

    void CastHurtboxes()
    {
        if (hurtboxes == null)
            return;

        if (hurtboxes.Count > 0 && hurtboxes[frame].circles.Count > 0)
        {
            List<Collider2D> collidersHit = new List<Collider2D>();
            List<HurtboxOverlap> overlaps = new List<HurtboxOverlap>();
            for (int i = 0; i < hurtboxes[frame].circles.Count; i++)
            {
                Vector3 circlePos = transform.position + new Vector3(
                        hurtboxes[frame].circles[i].center.x * animator.transform.localScale.x,
                        hurtboxes[frame].circles[i].center.y,
                        0f);

                float circleRadius = hurtboxes[frame].circles[i].radius;

                //Grabs all colliders overlapping with current circle
                Collider2D[] colliders = Physics2D.OverlapCircleAll(circlePos, circleRadius);

                foreach (Collider2D collider in colliders)
                {
                    //We only want to package up some overlap data for a collider we haven't dealt with
                    if(!collidersHit.Contains(collider))
                    {
                        //Package the overlap data here
                        HurtboxOverlap hurtboxOverlap = new HurtboxOverlap();
                        hurtboxOverlap.clip = animator.GetCurrentAnimatorClipInfo(0)[0].clip;
                        hurtboxOverlap.SetStateData(animator.GetCurrentAnimatorStateInfo(0));
                        hurtboxOverlap.collider = collider;
                        hurtboxOverlap.SetHurtboxData(circlePos, circleRadius);

                        //Now we've finished packaging the overlap data we add it to the overlap list
                        overlaps.Add(hurtboxOverlap);
                        //Add this to last cast return so we know not to do any more with this collider
                        collidersHit.Add(collider);
                    }
                }
            }

            if(!lastHurtboxOverlap.OverlapEquals(overlaps))
            {
                lastHurtboxOverlap = overlaps;
                HandleListeners_ColliderOverlaps();
            }
        }
    }

    Listener_ColliderOverlap[] listener_ColliderOverlaps;
    void HandleListeners_ColliderOverlaps()
    {
        listener_ColliderOverlaps = GetComponentsInChildren<Listener_ColliderOverlap>();

        if (listener_ColliderOverlaps == null)
            return;

        for (int i = 0; i < listener_ColliderOverlaps.Length; i++)
        {
            listener_ColliderOverlaps[i].Update_ColliderOverlap(lastHurtboxOverlap);
        }
    }

    public bool showHurtboxGizmos = false;
    private void OnDrawGizmos()
    {
        if (hurtboxes == null)
            return;

        if(showHurtboxGizmos)
        {
            if(hurtboxes.Count > 0)
            {
                for (int i = 0; i < hurtboxes[frame].circles.Count; i++)
                {
                    Gizmos.color = new Color(1, 0, 0, 0.65f);
                    Vector3 circlePos = transform.position + new Vector3(
                        hurtboxes[frame].circles[i].center.x * animator.transform.localScale.x,
                        hurtboxes[frame].circles[i].center.y,
                        0f);

                    Gizmos.DrawSphere(circlePos, hurtboxes[frame].circles[i].radius);
                }
            }
        }
    }
}

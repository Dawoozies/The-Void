using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExtensionMethods_Animator;
using OldData;
public class GroundCheck : MonoBehaviour
{
    Animator animator;
    CollisionManager collisionManager;

    //Have to run checks for when animator changes clip/state
    //Have to always get frame of current animation clip/state

    AnimatorClipInfo currentClipInfo;

    List<FrameCollisionData> groundboxes = new List<FrameCollisionData>();

    int frame;

    public LayerMask layerMask;
    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        collisionManager = GetComponent<CollisionManager>();

        if (currentClipInfo.clip == null)
        {
            currentClipInfo = animator.GetCurrentAnimatorClipInfo(0)[0];

            groundboxes = collisionManager.GetGroundboxData(currentClipInfo.clip.name);

            //Debug.Log("Initialising Groundboxes for " + gameObject.name);
        }
    }
    void Update()
    {
        if(currentClipInfo.clip != null)
        {
            //Then we need to check if the clip has changed since last frame
            //We need to do this check so we can refresh the frame data
            if(currentClipInfo.clip.name != animator.GetCurrentAnimatorClipInfo(0)[0].clip.name)
            {
                currentClipInfo = animator.GetCurrentAnimatorClipInfo(0)[0];
                //Then the animator has switched to another animation state (Ideally also changing the clip)
                groundboxes = collisionManager.GetGroundboxData(currentClipInfo.clip.name);
                //Debug.Log("Animator has changed clips for " + gameObject.name + " setting new groundbox data, new animation clip is: " + animator.GetCurrentAnimatorClipInfo(0)[0].clip.name);
            }
        }

        currentClipInfo = animator.GetCurrentAnimatorClipInfo(0)[0];

        frame = animator.CurrentFrame();

        //Debug.Log("Groundboxes Frame: " + groundboxes[frame].frame + "Current Frame: " + frame);
        if(groundboxes.Count > 0)
        {
            List<Circle> groundboxCircles = groundboxes[frame].circles;
            List<Collider2D> hitColliders = new List<Collider2D>();
            for (int i = 0; i < groundboxCircles.Count; i++)
            {
                Vector3 circlePos = transform.position + new Vector3(groundboxCircles[i].center.x * animator.transform.localScale.x, groundboxCircles[i].center.y, 0f);
                float circleRadius = groundboxCircles[i].radius;

                Collider2D hitCollider = Physics2D.OverlapCircle(circlePos, circleRadius, layerMask);
                
                if(hitCollider != null && Physics2D.Raycast(circlePos, Vector2.down, groundboxCircles[i].radius, layerMask))
                {
                    hitColliders.Add(hitCollider);
                }
            }

            if(hitColliders.Count > 0)
            {
                animator.SetBool("Grounded", true);
            }
            else
            {
                animator.SetBool("Grounded", false);
            }
        }
    }
    public bool showGroundboxGizmos = false;
    private void OnDrawGizmos()
    {
        if(showGroundboxGizmos)
        {
            if (groundboxes.Count > 0)
            {
                //Then we draw all the groundbox circles as wireframe spheres
                for (int i = 0; i < groundboxes[frame].circles.Count; i++)
                {
                    Gizmos.color = new Color(0, 1, 0, 0.65f);
                    Vector3 circlePos = transform.position + new Vector3(groundboxes[frame].circles[i].center.x * animator.transform.localScale.x, groundboxes[frame].circles[i].center.y, 0f);
                    Gizmos.DrawSphere(circlePos, groundboxes[frame].circles[i].radius);
                }
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Geometry;
public class Collider2DPool
{
    public bool initialized;
    public int size;
    public List<CircleCollider2D> colliders;
    public Collider2DPool ()
    {
        initialized = false;
        size = 30;
        colliders = new List<CircleCollider2D>();
    }
    public void InitializePool(Transform parent)
    {
        GameObject poolParent = new GameObject($"POOL:{parent.name}");
        poolParent.transform.parent = parent;
        for (int i = 0; i < size; i++)
        {
            GameObject newPoolObject = new GameObject($"PoolMember{i}");
            newPoolObject.transform.parent = poolParent.transform;
            CircleCollider2D collider = newPoolObject.AddComponent(typeof(CircleCollider2D)) as CircleCollider2D;
            colliders.Add(collider);
        }
        initialized = true;
    }
    public void UpdateColliders(Transform parent, List<Geometry.Circle> circles, bool isTrigger)
    {
        for (int i = 0; i < colliders.Count; i++)
        {
            if (i < circles.Count)
            {
                colliders[i].gameObject.SetActive(true);
                colliders[i].isTrigger = isTrigger;
                colliders[i].transform.position = new Vector3
                    (
                        parent.transform.position.x + circles[i].center.x * parent.transform.localScale.x,
                        parent.transform.position.y + circles[i].center.y * parent.transform.localScale.y,
                        parent.transform.position.z + circles[i].center.z * parent.transform.localScale.z
                    );
                colliders[i].radius = circles[i].radius;
            }
            else
            {
                colliders[i].gameObject.SetActive(false);
            }

        }
    }
}

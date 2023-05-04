using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Geometry;
public class CircleCollider2DPoolManager : MonoBehaviour
{
    public static CircleCollider2DPoolManager ins;
    private void Awake()
    {
        ins = this;
    }
    public Dictionary<Transform, Collider2DPool> pools = new Dictionary<Transform, Collider2DPool>();
    public void UpdatePool(Transform parent, List<Geometry.Circle> circles, bool isTrigger)
    {
        if(!pools.ContainsKey(parent))
        {
            Collider2DPool newPool = new Collider2DPool();
            newPool.InitializePool(parent);
            pools.Add(parent, newPool);
        }
        if(pools.ContainsKey(parent))
        {
            pools[parent].UpdateColliders(parent, circles, isTrigger);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteTestStuff : MonoBehaviour
{
    public Vector3 transformCenterPosition;
    SpriteRenderer spriteRenderer;
    Sprite currentSprite;
    public Vector2 pivot;
    public float pixelsPerUnit;
    public Bounds bounds;
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        transformCenterPosition = transform.position;
        currentSprite = spriteRenderer.sprite;
        pivot = currentSprite.pivot;
        pixelsPerUnit = currentSprite.pixelsPerUnit;
        bounds = currentSprite.bounds;
    }
}

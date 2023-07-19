using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[ExecuteInEditMode]
public class PixelDisplacement : MonoBehaviour
{
    SpriteRenderer spriteRenderer;
    //Should calculate the pixels we are removing here
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    void Update()
    {
        
    }
}
public class Pixel
{
    public Vector2 uv;
    public Color color;
}

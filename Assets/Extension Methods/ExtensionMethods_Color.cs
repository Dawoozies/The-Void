using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ExtensionMethods_Color
{
    public static class Extensions
    {
        public static Color WithTransparency(this Color color, float a)
        {
            return new Color(color.r, color.g, color.b, Mathf.Clamp(a, 0f, 1f));
        }
    }
}
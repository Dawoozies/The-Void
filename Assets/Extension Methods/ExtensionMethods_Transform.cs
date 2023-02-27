using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ExtensionMethods_Transform
{
    public static class Extensions
    {
        public static int ActiveChildCount(this Transform t)
        {
            int k = 0;

            for (int i = 0; i < t.childCount; i++)
            {
                if (t.GetChild(i).gameObject.activeSelf)
                k++;
            }

            return k;
        }
    }
}

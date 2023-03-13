using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ExtensionMethods_LayerMask
{
    public static class Extensions
    {
        public static bool Contains(this LayerMask layerMask, int layer)
        {
            if((layerMask & (1 << layer)) != 0)
                return true;

            return false;
        }
    }
}

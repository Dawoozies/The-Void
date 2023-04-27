using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ExtensionMethods_Bool
{
    public static class Extensions
    {
        public static float DefinedValue(this bool x, float falseValue, float trueValue)
        {
            if (x)
                return trueValue;

            return falseValue;
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RuntimeObjects;
namespace StateHandlers.HangedFrame
{
    public static class Handler
    {
        public static void Update(RuntimeObject obj, float tickDelta)
        {
            RuntimeObjects.HangedFrame hangedFrame = obj as RuntimeObjects.HangedFrame;
        }
    }
}
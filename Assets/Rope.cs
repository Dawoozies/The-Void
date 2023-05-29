using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace GameManagement
{
    public class Rope : MonoBehaviour
    {
        public LineRenderer lineRenderer;
        public float segmentDistance = 1f;
        public Transform pointA;
        public Transform pointB;
        public Vector3 A => pointA.position;
        public Vector3 B => pointB.position;
        public Vector3 dirToB => (B - A).normalized;
        public float totalDistance => Vector3.Distance(A, B);
        public float initialT = 1;
        void Update()
        {
            int segments = 0;
            for (float lineLength = 0; lineLength < totalDistance; lineLength += segmentDistance)
            {
                segments++;
            }
            lineRenderer.positionCount = segments + 1;
            for (int i = 0; i < lineRenderer.positionCount; i++)
            {
                //Vector3 chainInitialPos = segmentDistance*Vector3.Dot(A, pointA.right)*pointA.right + segmentDistance*Vector3.Dot(A, pointA.up)*pointA.up;
                Vector3 chainInitialPos = A + (float)i * segmentDistance * dirToB;
                Vector3 chainPos = chainInitialPos;
                float yDesiredDisplacement = 0;
                //displace by dist from middle
                float index = i;
                float middlePos = (float)lineRenderer.positionCount / 2;
                float endPos = (float)lineRenderer.positionCount;
                float distToMiddle = Mathf.Abs(i - middlePos);
                float percentageToMiddle = 0;
                if (index <= middlePos)
                    percentageToMiddle = index / middlePos;
                if (index > middlePos)
                    percentageToMiddle = index / middlePos - index / endPos;
                yDesiredDisplacement = percentageToMiddle;
                chainPos = chainPos + yDesiredDisplacement*GameManagement.ins.gravityDirection;
                lineRenderer.SetPosition(i, chainPos);
            }
            Debug.Log($"Segments = {segments}");
        }
    }

}
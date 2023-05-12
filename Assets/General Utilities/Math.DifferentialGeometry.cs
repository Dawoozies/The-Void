using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LinearAlgebra;
using ExtensionMethods_LayerMask;
namespace Math.Functions
{
    //Will be useful for defining various Vector field -like transformations for velocity components
    //I.e. all velocity components are actually tensors!
    //This is also reflected in construction of velocity components, i.e. compositions of tensors
    public static class VectorFields
    {
        private static ParametrisedLine parametrisedLine;
        public static Vector3 Identity(Vector3 inputVector) 
        { 
            return inputVector;
        }
        public static Vector3 PathDirection(Vector3 pathStart, Vector3 pathEnd) 
        {
            if (parametrisedLine == null)
                parametrisedLine = new ParametrisedLine();
            parametrisedLine.pathStart = pathStart;
            parametrisedLine.pathEnd = pathEnd;
            return parametrisedLine.direction;
        }
        public static Vector3 PathDirectionObjectToObject(GameObject startObject, GameObject endObject)
        {
            return PathDirection(startObject.transform.position, endObject.transform.position);
        }
        public static void RaycastReflectVelocity(Rigidbody2D rb, LayerMask raycastTargetLayers)
        {
            RaycastHit2D hit = Physics2D.Raycast(rb.transform.position, rb.velocity.normalized, rb.velocity.magnitude, raycastTargetLayers);
            if (hit.collider == null)
                return;
            Vector3 reflectionVelocity = Vector3.Reflect(rb.velocity, hit.normal);
            rb.MovePosition(hit.point);
            rb.velocity = reflectionVelocity;
        }
    }
}
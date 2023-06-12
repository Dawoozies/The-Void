using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RuntimeObjects
{
    public class Player : RuntimeObject
    {
        //before we do this we have to finish

        public Player(string id) : base(id)
        {
            managedStart += ManagedStart;
            managedUpdate += ManagedUpdate;
        }
        public void ManagedStart()
        {

        }
        public void ManagedUpdate(float tickDelta)
        {

        }
    }
}
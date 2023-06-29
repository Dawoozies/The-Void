using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RuntimeObjects
{
    public class Mantis : RuntimeObject
    {
        public MantisLegs legs;
        public MantisTorso torso;
        public Mantis(string id) : base(id)
        {
            managedStart += ManagedStart;
            legs = new MantisLegs("MantisLegs");
            GameManager.ins.allRuntimeObjects.Add("MantisLegs", legs);
            RuntimeAnimator.CreateAndAttach(legs, GameManager.ins.allControllers["MantisLegs"]);
            RuntimeDirectedCircleColliders.CreateAndAttach(legs);
            RuntimeDirectedCircleOverlaps.CreateAndAttach(legs);
            RuntimeDirectedPoints.CreateAndAttach(legs);
            torso = new MantisTorso("MantisTorso");
            GameManager.ins.allRuntimeObjects.Add("MantisTorso", torso);
            RuntimeAnimator.CreateAndAttach(torso, GameManager.ins.allControllers["MantisTorso"]);
            RuntimeDirectedCircleColliders.CreateAndAttach(torso);
            RuntimeDirectedCircleOverlaps.CreateAndAttach(torso);
            RuntimeDirectedPoints.CreateAndAttach(torso);
        }
        public void ManagedStart()
        {
            managedUpdate += RuntimeAnimator.Update;
            managedUpdate += RuntimeDirectedCircleOverlaps.Update; 
        }
    }
    public class MantisLegs : RuntimeObject
    {
        public MantisLegs(string id) : base(id)
        {

        }
        public void ManagedStart()
        {

        }
    }
    public class MantisTorso : RuntimeObject
    {
        public MantisTorso(string id) : base(id)
        {

        }
        public void ManagedStart()
        {

        }
    }
}
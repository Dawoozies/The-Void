using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RuntimeObjects
{
    //This is the full handler of the hallway boss
    //I know you're tempted to clean the code
    //But don't abstract away everything, this game's code is already going to be a huge mess
    //we don't need it to be insanely hard to understand too
    public class Hallway : RuntimeObject
    {
        //references to frames here
        //should be a list of frames
        public Hallway() : base("Hallway")
        {
            RuntimeAnimator.CreateAndAttach(this, GameManager.ins.allControllers["Hallway"]);
            RuntimeDirectedCircleColliders.CreateAndAttach(this);
            RuntimeDirectedCircleOverlaps.CreateAndAttach(this);
            RuntimeDirectedPoints.CreateAndAttach(this);
        }
    }
    //Trapped soul is the base object type for the hallway main entities
    public class TrappedSoul : RuntimeObject
    {
        public TrappedSoul(string id) : base(id)
        {
            GameManager.ins.allRuntimeObjects.Add(this);
            RuntimeAnimator.CreateAndAttach(this, GameManager.ins.allControllers[id]);
            RuntimeDirectedCircleColliders.CreateAndAttach(this);
            RuntimeDirectedCircleOverlaps.CreateAndAttach(this);
            RuntimeDirectedPoints.CreateAndAttach(this);
        }
    }
}
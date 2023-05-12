using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Math.BoardStructure;
namespace GameData.BlackboardManager
{
    public class GameBlackboardManager : MonoBehaviour
    {
        public static GameBlackboardManager ins;
        public Blackboard<GameObject> Blackboard_GameObject;
        public Blackboard<Rigidbody2D> Blackboard_Rigidbody2D;
        public Blackboard<Animator> Blackboard_Animator;
        private void Awake()
        {
            ins = this;
            //Construct blackboards here in case we want to possibly read off old blackboard data
            Blackboard_GameObject = new Blackboard<GameObject>();
            Blackboard_Rigidbody2D = new Blackboard<Rigidbody2D>();
            Blackboard_Animator = new Blackboard<Animator>();
        }
    }
}
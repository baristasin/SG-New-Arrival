using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Game.Scripts.BehaviourTree
{
    public enum NodeState
    {
        RUNNING,
        SUCCESS,
        FAILURE
    }

    [System.Serializable]
    public abstract class Node
    {
        public NodeState NodeState => _nodeState;

        protected NodeState _nodeState;

        public abstract NodeState Evaluate();
    }
}
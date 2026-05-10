using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Scripts.BehaviourTree
{
    public class Sequence : Node
    {
        protected List<Node> _nodes = new List<Node>();

        public Sequence(List<Node> nodes)
        {
            _nodes = nodes;
        }

        public override NodeState Evaluate()
        {
            foreach (var node in _nodes)
            {
                switch (node.Evaluate())
                {
                    case NodeState.RUNNING:
                        _nodeState = NodeState.RUNNING;
                        return _nodeState;
                    case NodeState.FAILURE:
                        _nodeState = NodeState.FAILURE;
                        return _nodeState;
                }
            }

            _nodeState = NodeState.SUCCESS;
            return _nodeState;
        }
    }
}
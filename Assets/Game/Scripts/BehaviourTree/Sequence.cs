using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Game.Scripts.BehaviourTree
{
    public class Sequence : Node
    {
        protected List<Node> _nodes = new List<Node>();

        protected int _currentNodeIndex;
        public Sequence(List<Node> nodes)
        {
            _nodes = nodes;
        }

        public override NodeState Evaluate()
        {
            if (_currentNodeIndex < _nodes.Count)
            {
                _nodeState = _nodes[_currentNodeIndex].Evaluate();

                if (_nodeState == NodeState.RUNNING)
                {
                    _currentNodeIndex = 0;
                    return NodeState.RUNNING;
                }

                else if (_nodeState == NodeState.FAILURE)
                {
                    _currentNodeIndex = 0;
                    return NodeState.FAILURE;
                }

                else // SUCCESS
                {
                    _currentNodeIndex++;
                    if (_currentNodeIndex < _nodes.Count)
                        return NodeState.RUNNING;
                    else
                    {
                        _currentNodeIndex = 0;
                        return NodeState.SUCCESS;
                    }
                }
            }
            return NodeState.SUCCESS;
        }
    }
}
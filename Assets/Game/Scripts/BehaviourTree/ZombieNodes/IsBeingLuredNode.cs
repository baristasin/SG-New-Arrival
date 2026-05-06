using UnityEngine;

namespace Game.Scripts.BehaviourTree.ZombieNodes
{
    public class IsBeingLuredNode : ZombieBaseNode
    {
        public override NodeState Evaluate()
        {
            return ZombieController.ZombiePerceptionModule.IsCloseEnoughToBeingLured() ? NodeState.SUCCESS : NodeState.FAILURE;
        }
    }
}
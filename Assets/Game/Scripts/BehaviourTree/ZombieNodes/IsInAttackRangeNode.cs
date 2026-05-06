using Game.Scripts.BehaviourTree.ZombieNodes;
using UnityEngine;

namespace Game.Scripts.BehaviourTree.ZombieNodes
{
    public class IsInAttackRangeNode : ZombieBaseNode
    {
        public override NodeState Evaluate()
        {
            return ZombieController.ZombiePerceptionModule.IsCloseEnoughToAttack() ? NodeState.SUCCESS : NodeState.FAILURE;
        }
    }
}
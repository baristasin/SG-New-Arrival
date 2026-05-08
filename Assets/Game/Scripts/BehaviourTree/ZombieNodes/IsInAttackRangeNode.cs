using Game.Scripts.BehaviourTree.ZombieNodes;
using UnityEngine;

namespace Game.Scripts.BehaviourTree.ZombieNodes
{
    public class IsInAttackRangeNode : ZombieBaseNode
    {
        public override NodeState Evaluate()
        {
            bool isCloseEnoughToAttack = ZombieController.ZombieMovementModule.IsCloseEnoughToAttack();
            if(isCloseEnoughToAttack) Debug.Log("CloseEnoughToAttack");
            return isCloseEnoughToAttack ? NodeState.SUCCESS : NodeState.FAILURE;
        }
    }
}
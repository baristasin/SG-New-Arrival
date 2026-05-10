using Game.Scripts.BehaviourTree.ZombieNodes;
using UnityEngine;

namespace Game.Scripts.BehaviourTree.ZombieNodes
{
    public class IsInAttackRangeNode : ZombieBaseNode
    {
        public override NodeState Evaluate()
        {
            bool isCloseEnoughToAttack = ZombieController.ZombieMovementModule.IsCloseEnoughToAttack();
            bool isAttacking = ZombieController.ZombieCombatModule.IsAttacking;
            if(isCloseEnoughToAttack && !isAttacking) Debug.Log("CloseEnoughToAttack");
            return isCloseEnoughToAttack ? NodeState.SUCCESS : NodeState.FAILURE;
        }
    }
}
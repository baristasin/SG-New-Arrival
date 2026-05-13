using Game.Scripts.BehaviourTree.ZombieNodes;
using UnityEngine;

namespace Game.Scripts.BehaviourTree.ZombieNodes
{
    public class WaitAndProceedAttackNode : ZombieBaseNode
    {
        public override NodeState Evaluate()
        {
            if (!ZombieController.ZombieCombatModule.IsAttacking)
            {
                // Debug.Log("is Attacking false, so go to TryToAttackNode");
                return NodeState.SUCCESS;
            }
            else
            {
                // Debug.Log("Updating Attack");
                ZombieController.ZombieCombatModule.UpdateAttack();
                return NodeState.RUNNING;
            }
        }
    }
}
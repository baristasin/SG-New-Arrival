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
                return NodeState.SUCCESS;
            }
            else
            {
                ZombieController.ZombieCombatModule.UpdateAttack();
                return NodeState.RUNNING;
            }
        }
    }
}
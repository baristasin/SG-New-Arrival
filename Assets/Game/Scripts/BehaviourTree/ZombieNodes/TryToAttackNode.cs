using UnityEngine;

namespace Game.Scripts.BehaviourTree.ZombieNodes
{
    public class TryToAttackNode : ZombieBaseNode
    {
        public override NodeState Evaluate()
        {
            // Debug.Log("TryingToAttack");
            ZombieController.ZombieCombatModule.TryToAttack();
            return NodeState.RUNNING;
        }
    }
}
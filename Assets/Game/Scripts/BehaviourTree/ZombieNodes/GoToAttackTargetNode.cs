using UnityEngine;

namespace Game.Scripts.BehaviourTree.ZombieNodes
{
    public class GoToAttackTargetNode : ZombieBaseNode
    {
        public override NodeState Evaluate()
        {
            ZombieController.ZombieCombatModule.SetIsAttacking(false);
            ZombieController.ZombieCombatModule.ResetAttackTimer();
            ZombieController.ZombieMovementModule.GoToPosition();
            return ZombieController.ZombieMovementModule.HasArrived ? NodeState.SUCCESS : NodeState.RUNNING;
        }
    }
}
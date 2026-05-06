using UnityEngine;

namespace Game.Scripts.BehaviourTree.ZombieNodes
{
    public class GoToAttackTargetNode : ZombieBaseNode
    {
        public override NodeState Evaluate()
        {
            ZombieController.ZombieMovementModule.GoToPosition();
            return NodeState.RUNNING;
        }
    }
}

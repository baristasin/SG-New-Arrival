using UnityEngine;

namespace Game.Scripts.BehaviourTree.ZombieNodes{
    public class CheckHealthNode : ZombieBaseNode
    {
        public override NodeState Evaluate()
        {
            return ZombieController.ZombieHealthModule.IsInCriticalHealth() ? NodeState.SUCCESS : NodeState.FAILURE;
        }
    }
}
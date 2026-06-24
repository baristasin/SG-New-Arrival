using UnityEngine;

namespace Game.Scripts.BehaviourTree.ZombieNodes{
    public class CheckHealthNode : ZombieBaseNode
    {
        public override NodeState Evaluate()
        {
            bool isInCriticalHealth = ZombieController.ZombieHealthModule.IsInCriticalHealth();
            return isInCriticalHealth ? NodeState.FAILURE : NodeState.FAILURE;
        }
    }
}
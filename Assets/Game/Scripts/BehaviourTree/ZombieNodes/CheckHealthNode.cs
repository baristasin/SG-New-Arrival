using UnityEngine;

namespace Game.Scripts.BehaviourTree.ZombieNodes{
    public class CheckHealthNode : ZombieBaseNode
    {
        public override NodeState Evaluate()
        {
            bool isInCriticalHealth = ZombieController.ZombieHealthModule.IsInCriticalHealth();
            if(isInCriticalHealth) Debug.Log("Critical Health");
            return isInCriticalHealth ? NodeState.SUCCESS : NodeState.FAILURE;
        }
    }
}
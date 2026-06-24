using UnityEngine;

namespace Game.Scripts.BehaviourTree.ZombieNodes
{
    public class IsBeingLuredNode : ZombieBaseNode
    {
        public override NodeState Evaluate()
        {
            bool isBeingLured = ZombieController.ZombieMovementModule.IsCloseEnoughToBeingLured();
            if (isBeingLured)
            {
                ZombieController.ZombiePerceptionModule.SetTargetTransformToPlayer();
            }
            else
            {
                ZombieController.ZombiePerceptionModule.SetTargetTransformToBuildingHitPoint();
            }
            
            return isBeingLured ? NodeState.SUCCESS : NodeState.FAILURE;
        }
    }
}
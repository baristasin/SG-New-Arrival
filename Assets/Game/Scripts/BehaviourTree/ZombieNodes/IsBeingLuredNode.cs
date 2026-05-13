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
                // Debug.Log("Lured");
                ZombieController.ZombiePerceptionModule.SetTargetTransformToPlayer();
            }
            else
            {
                // Debug.Log("Not lured");
                ZombieController.ZombiePerceptionModule.SetTargetTransformToBuildingHitPoint();
            }
            
            return isBeingLured ? NodeState.SUCCESS : NodeState.FAILURE;
        }
    }
}
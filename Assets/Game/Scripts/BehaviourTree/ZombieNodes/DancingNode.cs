using Game.Scripts.Utilities;
using Game.Scripts.ZombieModules;

namespace Game.Scripts.BehaviourTree.ZombieNodes
{
    public class DancingNode : ZombieBaseNode
    {
        public override NodeState Evaluate()
        {
            if (!ZombieState.IsDancing) return NodeState.FAILURE;

            ZombieController.ZombieMovementModule.StopMovement();
            ZombieController.ZombieAnimationModule.Play(ZombieAnimState.Dance);
            return NodeState.RUNNING;
        }
    }
}

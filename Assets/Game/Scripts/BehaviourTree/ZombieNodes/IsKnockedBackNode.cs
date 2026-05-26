namespace Game.Scripts.BehaviourTree.ZombieNodes
{
    // Freezes the tree while the zombie is being knocked back (the movement module flies it and
    // disables the NavMeshAgent, so no other node should issue movement/attack commands).
    public class IsKnockedBackNode : ZombieBaseNode
    {
        public override NodeState Evaluate()
        {
            return ZombieController.ZombieMovementModule.IsKnockedBack
                ? NodeState.RUNNING
                : NodeState.FAILURE;
        }
    }
}

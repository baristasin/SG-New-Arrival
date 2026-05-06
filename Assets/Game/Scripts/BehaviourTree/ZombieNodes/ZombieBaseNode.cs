using Game.Scripts.ZombieModules;
using UnityEngine;

namespace Game.Scripts.BehaviourTree.ZombieNodes
{
    public abstract class ZombieBaseNode : Node
    {
        public ZombieController ZombieController { get; private set; }
        public void Initialize(ZombieController zombieController)
        {
            ZombieController = zombieController;
        }

        public override abstract NodeState Evaluate();
    }
}
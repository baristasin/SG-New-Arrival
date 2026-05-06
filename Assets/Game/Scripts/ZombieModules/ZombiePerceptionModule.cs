using UnityEngine;

namespace Game.Scripts.ZombieModules
{
    public class ZombiePerceptionModule : ZombieBaseModule
    {
        public override void Initialize(ZombieController zombieController)
        {
            base.Initialize(zombieController);
        }

        public bool IsCloseEnoughToBeingLured()
        {
            return true;
        }

        public bool IsCloseEnoughToAttack()
        {
            return true;
        }
    }
}
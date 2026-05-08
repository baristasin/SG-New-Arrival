using Game.Scripts.Utilities;
using UnityEngine;

namespace Game.Scripts.ZombieModules
{
    public enum ZombieAttackTarget
    {
        Player,
        Building
    }
    
    public class ZombiePerceptionModule : ZombieBaseModule
    {
        public ZombieAttackTarget ZombieAttackTarget => _zombieAttackTarget;
        
        private ZombieAttackTarget _zombieAttackTarget;
        
        public void SetTargetTransformToPlayer()
        {
            _zombieAttackTarget = ZombieAttackTarget.Player;
        }

        public void SetTargetTransformToBuildingHitPoint()
        {
            _zombieAttackTarget = ZombieAttackTarget.Building;
        }
    }
}
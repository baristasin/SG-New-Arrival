using UnityEngine;

namespace Game.Scripts.ZombieModules
{
    public class ZombieController : MonoBehaviour
    {
        public Transform PlayerTransform { get; set; }
        public Vector3 BuildingAttackingPosition { get; set; }
        
        public ZombieActionModule ZombieActionModule => _zombieActionModule;
        public ZombieAnimationModule ZombieAnimationModule => _zombieAnimationModule;
        public ZombieCombatModule ZombieCombatModule => _zombieCombatModule;
        public ZombieHealthModule ZombieHealthModule => _zombieHealthModule;
        public ZombiePerceptionModule ZombiePerceptionModule => _zombiePerceptionModule;
        public ZombieMovementModule ZombieMovementModule => _zombieMovementModule;

        [SerializeField] private ZombieActionModule _zombieActionModule;
        [SerializeField] private ZombieAnimationModule _zombieAnimationModule;
        [SerializeField] private ZombieCombatModule _zombieCombatModule;
        [SerializeField] private ZombieHealthModule _zombieHealthModule;
        [SerializeField] private ZombiePerceptionModule _zombiePerceptionModule;
        [SerializeField] private ZombieMovementModule _zombieMovementModule;

        public void Initialize()
        {
            _zombieActionModule.Initialize(this);
            _zombieAnimationModule.Initialize(this);
            _zombieCombatModule.Initialize(this);
            _zombieHealthModule.Initialize(this);
            _zombiePerceptionModule.Initialize(this);
            _zombieMovementModule.Initialize(this);

            // top node activate
        }
    }
}
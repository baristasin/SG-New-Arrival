using Game.Scripts.Utilities;
using UnityEngine;

namespace Game.Scripts.ZombieModules
{
    public class ZombieCombatModule : ZombieBaseModule
    {
        public bool IsAttacking { get; private set; }

        [SerializeField] private int _attackDamage = 1;

        private float _attackEndTime;
        private float _attackDuration = 2f;

        public void TryToAttack()
        {
            IsAttacking = true;
            ResetAttackTimer();
            
            ZombieController.ZombieMovementModule.Stop();
            ZombieController.ZombieAnimationModule.Play(ZombieAnimState.Attack);
        }

        public void UpdateAttack()
        {
            if (Time.time < _attackEndTime) return;

            var target = ZombieController.ZombiePerceptionModule.ZombieAttackTarget;
            AttackProcessor.Submit(target, _attackDamage, ZombieController.transform.position.x, ZombieController.transform.position.z);
            IsAttacking = false;
        }

        public void ResetAttackTimer()
        {
            _attackEndTime = Time.time + _attackDuration;
        }
    }
}
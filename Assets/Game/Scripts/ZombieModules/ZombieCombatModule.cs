using Game.Scripts.Utilities;
using UnityEngine;

namespace Game.Scripts.ZombieModules
{
    public class ZombieCombatModule : ZombieBaseModule
    {
        public bool IsAttacking { get; private set; }

        [SerializeField] private int _attackDamage = 10;
        private float _attackTimer;
        private float _attackDuration;

        public void TryToAttack()
        {
            IsAttacking = true;
            _attackTimer = 0f;
            _attackDuration = ZombieController.ZombieAnimationModule.GetCurrentClipLength();
            ZombieController.ZombieMovementModule.Stop();
            ZombieController.ZombieAnimationModule.Play(ZombieAnimState.Attack);
        }

        public void UpdateAttack()
        {
            _attackTimer += Time.deltaTime;
            if (_attackTimer < _attackDuration) return;

            var target = ZombieController.ZombiePerceptionModule.ZombieAttackTarget;

            AttackProcessor.Submit(target, _attackDamage, ZombieController.transform.position.x,ZombieController.transform.position.z);
            IsAttacking = false;
        }
    }
}
using FMODUnity;
using Game.Scripts.Utilities;
using UnityEngine;

namespace Game.Scripts.ZombieModules
{
    public class ZombieCombatModule : ZombieBaseModule
    {
        public bool IsAttacking { get; private set; }

        [SerializeField] private int _attackDamage = 1;
        [Tooltip("FMOD event played at the zombie's position the moment a Building attack lands.")]
        [SerializeField] private EventReference _wallHitEvent;

        private float _attackEndTime;
        private float _attackDuration = 2f;

        public void TryToAttack()
        {
            IsAttacking = true;
            ZombieController.ZombieMovementModule.Stop();
            ZombieController.ZombieAnimationModule.Play(ZombieAnimState.Attack);
            _attackDuration = ZombieController.ZombieAnimationModule.GetCurrentClipLength() * 0.5f;
            ResetAttackTimer();
        }

        public void UpdateAttack()
        {
            ZombieController.ZombieMovementModule.FaceTarget();

            if (Time.time < _attackEndTime) return;

            var target = ZombieController.ZombiePerceptionModule.ZombieAttackTarget;
            AttackProcessor.Submit(target, _attackDamage, ZombieController.transform.position.x, ZombieController.transform.position.z);

            if (target == ZombieAttackTarget.Building && !_wallHitEvent.IsNull)
                AudioManager.Instance.PlayOneShot(_wallHitEvent, ZombieController.transform.position);

            IsAttacking = false;
        }

        public void ResetAttackTimer()
        {
            _attackEndTime = Time.time + _attackDuration;
        }

        public void SetIsAttacking(bool value)
        {
            IsAttacking = value;
        }
    }
}
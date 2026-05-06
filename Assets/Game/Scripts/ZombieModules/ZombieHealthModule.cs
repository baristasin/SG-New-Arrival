using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Scripts.ZombieModules
{
    public class ZombieHealthModule : ZombieBaseModule
    {
        public event Action<int> OnHealthChanged;
        [SerializeField] private int _startingZombieHealth;
        [SerializeField] private int _criticalZombieHealth;
        private int _zombieHealth;

        public override void Initialize(ZombieController zombieController)
        {
            base.Initialize(zombieController);

            _zombieHealth = _startingZombieHealth;
        }

        public bool IsInCriticalHealth()
        {
            return _zombieHealth <= _criticalZombieHealth;
        }

        public void GetHit(int damage)
        {
            _zombieHealth -= damage;

            OnHealthChanged?.Invoke(_zombieHealth);

            if(_zombieHealth <= 0)
            {
                Debug.Log("Dead");
            }            
        }

        public float GetHealthPercentage() => (float)_zombieHealth / _startingZombieHealth;
    }
}
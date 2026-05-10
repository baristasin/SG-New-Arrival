using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Scripts.ZombieModules
{
    public class ZombieHealthModule : ZombieBaseModule
    {
        [SerializeField] private int _startingZombieHealth;
        [SerializeField] private int _criticalZombieHealth;
        [SerializeField] private Renderer _renderer;

        private int _zombieHealth;
        private Color _originalColor;
        private Coroutine _flashCoroutine;

        public override void Initialize(ZombieController zombieController)
        {
            base.Initialize(zombieController);
            _zombieHealth = _startingZombieHealth;

            if (_flashCoroutine != null)
            {
                ZombieController.StopCoroutine(_flashCoroutine);
                _flashCoroutine = null;
            }

            if (_originalColor == default)
                _originalColor = _renderer.material.color;
            else
                _renderer.material.color = _originalColor;
        }

        public bool IsInCriticalHealth()
        {
            return _zombieHealth <= _criticalZombieHealth;
        }

        [SerializeField] private float _knockbackDistance = 0.3f;

        public void GetHit(int damage, Vector3 knockDir = default)
        {
            _zombieHealth -= damage;

            if (knockDir != default)
                ZombieController.ZombieMovementModule.Knockback(knockDir, _knockbackDistance);

            if (_flashCoroutine != null)
                ZombieController.StopCoroutine(_flashCoroutine);
            _flashCoroutine = ZombieController.StartCoroutine(FlashRed());

            ZombieHealthBarPool.Show(ZombieController.transform, GetHealthPercentage());

            if (_zombieHealth <= 0)
                ZombieController.ZombieDead();
        }

        private IEnumerator FlashRed()
        {
            _renderer.material.color = Color.red;
            yield return new WaitForSeconds(0.15f);
            _renderer.material.color = _originalColor;
            _flashCoroutine = null;
        }

        public float GetHealthPercentage() => (float)_zombieHealth / _startingZombieHealth;
    }
}
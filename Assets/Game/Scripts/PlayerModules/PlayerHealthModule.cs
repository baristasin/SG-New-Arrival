using System;
using System.Collections;
using UnityEngine;

namespace Game.Scripts.PlayerModules
{
    public class PlayerHealthModule : MonoBehaviour
    {
        [SerializeField] private Renderer _renderer;
        [SerializeField] private int _playerStartingHealth;

        public event Action<int> OnHealthChanged;

        private Color _originalColor;
        private Coroutine _flashCoroutine;
        private int _playerCurrentHealth;

        private void Awake()
        {
            _originalColor = _renderer.material.color;
            _playerCurrentHealth = _playerStartingHealth;
        }

        public void TakeDamage(int damage)
        {
            _playerCurrentHealth -= damage;
            OnHealthChanged?.Invoke(_playerCurrentHealth);

            if (_flashCoroutine != null) return;
            _flashCoroutine = StartCoroutine(FlashRed());
        }

        private IEnumerator FlashRed()
        {
            _renderer.material.color = Color.red;
            yield return new WaitForSeconds(0.15f);
            _renderer.material.color = _originalColor;
            _flashCoroutine = null;
        }
    }
}
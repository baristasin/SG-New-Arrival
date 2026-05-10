using System;
using System.Collections;
using UnityEngine;

namespace Game.Scripts.BuildingModules
{
    public class BuildingHealthModule : MonoBehaviour
    {
        [SerializeField] private Renderer _renderer;
        [SerializeField] private int _buildingStartingHealth;

        public event Action<int> OnHealthChanged;

        private Color _originalColor;
        private Coroutine _flashCoroutine;
        private int _buildingCurrentHealth;

        private void Awake()
        {
            _originalColor = _renderer.material.color;
            _buildingCurrentHealth = _buildingStartingHealth;
        }

        public void TakeDamage(int damage)
        {
            _buildingCurrentHealth -= damage;
            OnHealthChanged?.Invoke(_buildingCurrentHealth);

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
